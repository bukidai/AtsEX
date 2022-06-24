using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// マップ上に設置されたすべてのオブジェクトの基本クラスを表します。
    /// </summary>
    /// <remarks>
    /// ストラクチャーに限らず、停車場や信号機、地上子など、ステートメントによって設置したすべてのオブジェクトを表します。
    /// </remarks>
    /// <seealso cref="MapObjectList"/>
    /// <seealso cref="LocatableMapObject"/>
    /// <seealso cref="PutBetweenStructure"/>
    /// <seealso cref="Station"/>
    /// <seealso cref="Structure"/>
    public class MapObjectBase : ClassWrapperBase, IComparable
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<MapObjectBase>();

            Constructor = members.GetSourceConstructor();

            LocationGetMethod = members.GetSourcePropertyGetterOf(nameof(Location));
            LocationSetMethod = members.GetSourcePropertySetterOf(nameof(Location));
        }

        protected MapObjectBase(object src) : base(src)
        {
        }

        private static ConstructorInfo Constructor;
        protected MapObjectBase(double location) : this(Constructor.Invoke(new object[] { location }))
        {
        }

        private static MethodInfo LocationGetMethod;
        private static MethodInfo LocationSetMethod;
        /// <summary>
        /// 設置された距離程を取得・設定します。
        /// </summary>
        /// <remarks>
        /// マップ オブジェクトの種類によっては、この数値を変更しても BVE に反映されない場合があります。
        /// </remarks>
        public double Location
        {
            get => LocationGetMethod.Invoke(Src, null);
            set => LocationSetMethod.Invoke(Src, new object[] { value });
        }

        public int CompareTo(object obj)
        {
            if (obj is MapObjectBase mapObject)
            {
                return (Src as IComparable).CompareTo(mapObject.Src);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
