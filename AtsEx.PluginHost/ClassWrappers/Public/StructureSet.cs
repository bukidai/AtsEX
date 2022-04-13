﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class StructureSet : ClassWrapper
    {
        static StructureSet()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<StructureSet>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            RepeatedGetMethod = members.GetSourcePropertyGetterOf(nameof(Repeated));

            PutGetMethod = members.GetSourcePropertyGetterOf(nameof(Put));

            PutBetweenGetMethod = members.GetSourcePropertyGetterOf(nameof(PutBetween));

            SignalsGetMethod = members.GetSourcePropertyGetterOf(nameof(Signals));
        }

        private StructureSet(object src) : base(src)
        {
        }

        public static StructureSet FromSource(object src) => new StructureSet(src);

        private static MethodInfo DrawLimitLocationGetMethod;
        private static MethodInfo DrawLimitLocationSetMethod;
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo RepeatedGetMethod;
        private static readonly Func<object, MapObjectList> RepeatedParserToWrapper = src => src is null ? null : MapObjectList.FromSource(src);
        public WrappedSortedList<string, MapObjectList> Repeated
        {
            get
            {
                IDictionary dictionarySrc = RepeatedGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, MapObjectList>(dictionarySrc, RepeatedParserToWrapper);
            }
        }

        private static MethodInfo PutGetMethod;
        public SingleStructureList Put
        {
            get => SingleStructureList.FromSource(PutGetMethod.Invoke(Src, null));
        }

        private static MethodInfo PutBetweenGetMethod;
        public SingleStructureList PutBetween
        {
            get => SingleStructureList.FromSource(PutBetweenGetMethod.Invoke(Src, null));
        }

        private static MethodInfo SignalsGetMethod;
        public SingleStructureList Signals
        {
            get => SingleStructureList.FromSource(SignalsGetMethod.Invoke(Src, null));
        }
    }
}
