using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 他列車の情報を提供します。
    /// </summary>
    public class TrainInfo : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<TrainInfo>();

            StructuresGetMethod = members.GetSourcePropertyGetterOf(nameof(Structures));

            TrackKeyGetMethod = members.GetSourcePropertyGetterOf(nameof(TrackKey));
            TrackKeySetMethod = members.GetSourcePropertySetterOf(nameof(TrackKey));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="TrainInfo"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected TrainInfo(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="TrainInfo"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static TrainInfo FromSource(object src) => src is null ? null : new TrainInfo(src);

        private static FastMethod StructuresGetMethod;
        /// <summary>
        /// この他列車に紐づけるストラクチャーの一覧を取得・設定します。
        /// </summary>
        public WrappedList<Structure> Structures => WrappedList<Structure>.FromSource(StructuresGetMethod.Invoke(Src, null));

        /*
        private static MethodInfo SoundsGetMethod;
        /// <summary>
        /// この他列車に紐づける音源の一覧を取得・設定します。
        /// </summary>
        public WrappedList<Sound3DObject> Sounds => WrappedList<Sound3DObject>.FromSource(SoundsGetMethod.Invoke(Src, null));
        */

        private static FastMethod TrackKeyGetMethod;
        private static FastMethod TrackKeySetMethod;
        /// <summary>
        /// この他列車が走行する軌道を取得・設定します。
        /// </summary>
        public string TrackKey
        {
            get => (string)TrackKeyGetMethod.Invoke(Src, null);
            set => TrackKeySetMethod.Invoke(Src, new object[] { value });
        }
    }
}
