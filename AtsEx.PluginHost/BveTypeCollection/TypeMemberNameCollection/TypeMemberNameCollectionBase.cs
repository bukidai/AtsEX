using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal abstract class TypeMemberNameCollectionBase
    {
        public string WrapperTypeName { get; }
        public string OriginalTypeName { get; }

        public TypeMemberNameCollectionBase Parent { get; private set; } = null;
        public IEnumerable<TypeMemberNameCollectionBase> Children { get; }

        public TypeMemberNameCollectionBase(string wrapperTypeName, string originalTypeName, IEnumerable<TypeMemberNameCollectionBase> children)
        {
            WrapperTypeName = wrapperTypeName;
            OriginalTypeName = originalTypeName;

            Children = children.ToArray();
            foreach (TypeMemberNameCollectionBase child in Children) child.Parent = this;
        }

        public TypeMemberNameCollectionBase(string wrapperTypeName, string originalTypeName) : this(wrapperTypeName, originalTypeName, Enumerable.Empty<TypeMemberNameCollectionBase>())
        {
        }

        public override string ToString() => WrapperTypeName;


        public class MemberInfo : IComparable<MemberInfo>
        {
            public string WrapperName { get; }
            public string OriginalName { get; }

            public bool IsOriginalPrivate { get; }
            public bool IsOriginalStatic { get; }

            public bool IsWrapperPrivate { get; }
            public bool IsWrapperStatic { get; }

            protected MemberInfo(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
            {
                WrapperName = wrapperName;
                OriginalName = originalName;

                IsOriginalStatic = isOriginalStatic;
                IsOriginalPrivate = isOriginalPrivate;

                IsWrapperStatic = isWrapperStatic;
                IsWrapperPrivate = isWrapperPrivate;
            }

            public int CompareTo(MemberInfo other) => WrapperName.CompareTo(other.WrapperName);
        }

        public class PropertyAccessorInfo : MemberInfo
        {
            public PropertyAccessorInfo(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : base(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public override string ToString() => $"Property: {WrapperName}";
        }

        public class FieldInfo : MemberInfo
        {
            public FieldInfo(string wrapperPropertyName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : base(wrapperPropertyName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public override string ToString() => $"Field: {WrapperName}";
        }

        public class MethodInfo : MemberInfo
        {
            public IEnumerable<TypeInfoBase> WrapperParamNames { get; set; }

            public MethodInfo(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : this(wrapperName, originalName, null, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public MethodInfo(string wrapperName, string originalName, IEnumerable<TypeInfoBase> wrapperParamNames, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : base(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
                WrapperParamNames = wrapperParamNames;
            }

            public override string ToString() => $"Method: {WrapperName}";
        }

        public abstract class TypeInfoBase
        {
            public abstract override string ToString();
        }

        public class TypeInfo : TypeInfoBase
        {
            public string TypeName { get; }

            public TypeInfo(string typeName)
            {
                TypeName = typeName;
            }

            public override string ToString() => TypeName;
        }

        public class GenericTypeInfo : TypeInfo
        {
            public IEnumerable<TypeInfoBase> TypeParams { get; }

            public GenericTypeInfo(string typeName, IEnumerable<TypeInfoBase> typeParams) : base(typeName)
            {
                TypeParams = typeParams;
            }

            public override string ToString() => $"{TypeName}`{TypeParams.Count()}[{string.Join(",", TypeParams.Select(p => p.ToString()))}]";
        }

        public class ArrayTypeInfo : TypeInfoBase
        {
            public TypeInfoBase BaseType { get; }
            public int DimensionCount { get; }

            public ArrayTypeInfo(TypeInfoBase baseType, int dimensionCount)
            {
                BaseType = baseType;
                DimensionCount = dimensionCount;
            }

            public override string ToString() => $"{BaseType}[{new string(',', DimensionCount - 1)}]";
        }
    }
}
