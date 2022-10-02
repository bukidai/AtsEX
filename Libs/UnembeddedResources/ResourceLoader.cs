using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnembeddedResources
{
    /// <summary>
    /// <see cref="ResourceStringHolderAttribute"/> を使用して複数のリソースを簡単に読み込むための機能を提供します。
    /// </summary>
    public static class ResourceLoader
    {
        /// <summary>
        /// 指定したインスタンス内の <see cref="ResourceStringHolderAttribute"/> が関連付けられたメンバーに、そのメンバーと同名の <see cref="Resource{T}"/> を一括で読み込み、設定します。
        /// </summary>
        /// <param name="targetInstance">読み込む対象のインスタンス。</param>
        /// <param name="throwExceptionIfHasUnusedResources">使用されていないリソースを検出した場合に例外をスローするか。</param>
        public static void LoadAndSetAll(dynamic targetInstance, bool throwExceptionIfHasUnusedResources = true)
        {
            if (targetInstance is null) throw new ArgumentNullException(nameof(targetInstance));
            
            LoadAndSetAll(targetInstance.GetType(), targetInstance, true, throwExceptionIfHasUnusedResources);
        }

        /// <summary>
        /// 指定した型内の <see cref="ResourceStringHolderAttribute"/> が関連付けられた静的メンバーに、そのメンバーと同名の <see cref="Resource{T}"/> を一括で読み込み、設定します。
        /// </summary>
        /// <param name="targetType">読み込む対象の型。</param>
        /// <param name="throwExceptionIfHasUnusedResources">使用されていないリソースを検出した場合に例外をスローするか。</param>
        public static void LoadAndSetAll(Type targetType, bool throwExceptionIfHasUnusedResources = true) => LoadAndSetAll(targetType, null, false, throwExceptionIfHasUnusedResources);

        private static void LoadAndSetAll(Type targetType, dynamic targetInstance, bool referenceInstanceMembers, bool throwExceptionIfHasUnusedResources)
        {
            ConcurrentDictionary<string, ResourceLocalizer> localizers = new ConcurrentDictionary<string, ResourceLocalizer>();
            ConcurrentDictionary<string, int> usedResourceCounts = new ConcurrentDictionary<string, int>();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod;
            if (referenceInstanceMembers) bindingFlags |= BindingFlags.Instance;

            IEnumerable<MemberInfo> members = targetType.GetMembers(bindingFlags);
            members.AsParallel().ForAll(member =>
            {
                ResourceStringHolderAttribute attribute = member.GetCustomAttribute<ResourceStringHolderAttribute>();
                if (attribute is null) return;

                ResourceLocalizer localizer = localizers.GetOrAdd(attribute.LocalizerMemberName, key =>
                {
                    MemberInfo result = members.FirstOrDefault(x => x.Name == key);
                    if (result is null) throw new KeyNotFoundException($"The member named '{key}' is not found in the class or struct '{targetType}'.");

                    switch (result)
                    {
                        case FieldInfo field:
                            return (ResourceLocalizer)field.GetValue(targetInstance);

                        case PropertyInfo property:
                            return (ResourceLocalizer)property.GetValue(targetInstance);

                        default:
                            throw new InvalidOperationException($"Cannot get a value from the member '{key}'; it is not a field or a property.");
                    }
                });

                usedResourceCounts.AddOrUpdate(attribute.LocalizerMemberName, 1, (_, count) => count + 1);

                Resource<string> resource = localizer.GetString(member.Name);

                switch (member)
                {
                    case FieldInfo field:
                        field.SetValue(targetInstance, resource);
                        break;

                    case PropertyInfo property:
                        property.SetValue(targetInstance, resource);
                        break;

                    default:
                        throw new InvalidOperationException($"Cannot set a value to the member '{member}'; it is not a field or a property.");
                }
            });

            if (throwExceptionIfHasUnusedResources)
            {
                if (localizers.Any(pair => pair.Value.Resources.Count != usedResourceCounts[pair.Key]))
                {
                    throw new InvalidOperationException($"Some resources are not referenced.");
                }
            }
        }
    }
}
