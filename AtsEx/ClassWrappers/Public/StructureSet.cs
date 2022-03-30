using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal sealed class StructureSet : ClassWrapper, IStructureSet
    {
        static StructureSet()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<PluginHost.ClassWrappers.IStructureSet>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            RepeatedGetMethod = members.GetSourcePropertyGetterOf(nameof(Repeated));

            PutGetMethod = members.GetSourcePropertyGetterOf(nameof(Put));

            PutBetweenGetMethod = members.GetSourcePropertyGetterOf(nameof(PutBetween));

            SignalsGetMethod = members.GetSourcePropertyGetterOf(nameof(Signals));
        }

        public StructureSet(object src) : base(src)
        {
        }

        private static MethodInfo DrawLimitLocationGetMethod;
        private static MethodInfo DrawLimitLocationSetMethod;
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo RepeatedGetMethod;
        private static readonly Func<object, IStructureList> RepeatedParserToWrapper = src => src is null ? null : new StructureList(src);
        public IWrappedSortedList<string, IStructureList> Repeated
        {
            get
            {
                IDictionary dictionarySrc = RepeatedGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, IStructureList>(dictionarySrc, RepeatedParserToWrapper);
            }
        }

        private static MethodInfo PutGetMethod;
        public ISingleStructureList Put
        {
            get => new SingleStructureList(PutGetMethod.Invoke(Src, null));
        }

        private static MethodInfo PutBetweenGetMethod;
        public ISingleStructureList PutBetween
        {
            get => new SingleStructureList(PutBetweenGetMethod.Invoke(Src, null));
        }

        private static MethodInfo SignalsGetMethod;
        public ISingleStructureList Signals
        {
            get => new SingleStructureList(SignalsGetMethod.Invoke(Src, null));
        }
    }
}
