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
    /// 自車両に関する情報にアクセスするための機能を提供します。
    /// </summary>
    public sealed class Vehicle : ClassWrapper
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<Vehicle>();
        }

        private Vehicle(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Vehicle"/> クラスのインスタンス。</returns>
        public static Vehicle FromSource(object src)
        {
            if (src is null) return null;
            return new Vehicle(src);
        }
    }
}
