using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal abstract class TypeMemberNameSetBase
    {
        public string WrapperTypeName { get; }
        public string OriginalTypeName { get; }

        public TypeMemberNameSetBase Parent { get; private set; } = null;
        public List<TypeMemberNameSetBase> Children { get; }

        public TypeMemberNameSetBase(string wrapperTypeName, string originalTypeName, List<TypeMemberNameSetBase> children)
        {
            WrapperTypeName = wrapperTypeName;
            OriginalTypeName = originalTypeName;

            Children = children;
            foreach (TypeMemberNameSetBase child in Children) child.Parent = this;
        }

        public TypeMemberNameSetBase(string wrapperTypeName, string originalTypeName) : this(wrapperTypeName, originalTypeName, new List<TypeMemberNameSetBase>())
        {
        }

        public override string ToString() => WrapperTypeName;


        public class Member : IComparable<Member>
        {
            public string WrapperName { get; }
            public string OriginalName { get; }

            public bool IsOriginalPrivate { get; }
            public bool IsOriginalStatic { get; }

            public bool IsWrapperPrivate { get; }
            public bool IsWrapperStatic { get; }

            protected Member(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
            {
                WrapperName = wrapperName;
                OriginalName = originalName;

                IsOriginalStatic = isOriginalStatic;
                IsOriginalPrivate = isOriginalPrivate;

                IsWrapperStatic = isWrapperStatic;
                IsWrapperPrivate = isWrapperPrivate;
            }

            public int CompareTo(Member other) => WrapperName.CompareTo(other.WrapperName);
        }

        public class PropertyAccessor : Member
        {
            public PropertyAccessor(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : base(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public override string ToString() => $"Property: {WrapperName}";
        }

        public class Field : Member
        {
            public Field(string wrapperPropertyName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : base(wrapperPropertyName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public override string ToString() => $"Field: {WrapperName}";
        }

        public class Method : Member
        {
            public IEnumerable<TypeInfoBase> WrapperParamNames { get; set; }

            public Method(string wrapperName, string originalName, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
                : this(wrapperName, originalName, null, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate)
            {
            }

            public Method(string wrapperName, string originalName, IEnumerable<TypeInfoBase> wrapperParamNames, bool isOriginalStatic = false, bool isOriginalPrivate = false, bool isWrapperStatic = false, bool isWrapperPrivate = false)
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
