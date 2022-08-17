using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class ObjectDrawer : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<ObjectDrawer>();

            DrawDistanceManagerField = members.GetSourceFieldOf(nameof(DrawDistanceManager));

            SetRouteMethod = members.GetSourceMethodOf(nameof(SetRoute));
        }

        private ObjectDrawer(object src) : base(src)
        {
        }

        private static FieldInfo DrawDistanceManagerField;
        /// <summary>
        /// ストラクチャーを描画する範囲を算出するための機能を提供する <see cref="ClassWrappers.DrawDistanceManager"/> を取得します。
        /// </summary>
        public DrawDistanceManager DrawDistanceManager => ClassWrappers.DrawDistanceManager.FromSource(DrawDistanceManagerField.GetValue(Src));

        private static MethodInfo SetRouteMethod;
        public void SetRoute(Route route) => SetRouteMethod.Invoke(Src, new object[] { route.Src });
    }
}
