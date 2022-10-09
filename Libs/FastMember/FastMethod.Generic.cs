using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace FastMember
{
    public partial class FastMethod
    {
        private class Generic : FastMethod
        {
            private class ResourceSet
            {
                private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<Generic>(@"FastMember\FastMethod");

                [ResourceStringHolder(nameof(Localizer))] public Resource<string> InstanceTypeMismatch { get; private set; }

                public ResourceSet()
                {
                    ResourceLoader.LoadAndSetAll(this);
                }
            }

            private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

            private readonly ConcurrentDictionary<object, Type[]> GenericTypeArgumentCache = new ConcurrentDictionary<object, Type[]>();
            private readonly ConcurrentDictionary<Type[], FastMethod> MethodCache = new ConcurrentDictionary<Type[], FastMethod>();

            public override MethodInfo Source { get; }

            public Generic(MethodInfo source)
            {
                Source = source;
            }

            public override object Invoke(object instance, object[] args)
            {
                if (instance is null) throw new NotImplementedException();

                Type[] genericTypeArguments = GenericTypeArgumentCache.GetOrAdd(instance, _ =>
                {
                    Type type = instance.GetType();
                    while (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == Source.DeclaringType)
                    {
                        if (type == typeof(object)) throw new ArgumentException(Resources.Value.InstanceTypeMismatch.Value);
                        type = type.BaseType;
                    }

                    Type[] result = type.GenericTypeArguments;
                    return result;
                });

                FastMethod method = MethodCache.GetOrAdd(genericTypeArguments, _ =>
                {
                    MethodInfo constructedMethod = Source.MakeGenericMethod(genericTypeArguments);
                    FastMethod result = Create(constructedMethod);

                    return result;
                });

                return method.Invoke(instance, args);
            }
        }
    }
}
