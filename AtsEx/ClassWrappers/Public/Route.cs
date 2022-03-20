﻿using System;
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
    internal sealed class Route : ClassWrapper, PluginHost.ClassWrappers.IRoute
    {
        static Route()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<PluginHost.ClassWrappers.IRoute>();
        }

        public Route(object src) : base(src)
        {
        }
    }
}
