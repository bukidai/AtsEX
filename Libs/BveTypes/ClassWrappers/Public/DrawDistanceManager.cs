using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// ストラクチャーの描画範囲を算出するための機能を提供します。
    /// </summary>
    public class DrawDistanceManager : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<DrawDistanceManager>();

            FrontDrawDistanceGetMethod = members.GetSourcePropertyGetterOf(nameof(FrontDrawDistance));

            BackDrawDistanceGetMethod = members.GetSourcePropertyGetterOf(nameof(BackDrawDistance));

            DrawDistanceGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawDistance));

            DrawDistanceObjectsGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawDistanceObjects));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="DrawDistanceManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected DrawDistanceManager(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="DrawDistanceManager"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static DrawDistanceManager FromSource(object src) => src is null ? null : new DrawDistanceManager(src);

        private static FastMethod FrontDrawDistanceGetMethod;
        /// <summary>
        /// 前方の描画距離 [m] を取得します。
        /// </summary>
        /// <remarks>
        /// 前方を向いている場合は <see cref="DrawDistance"/> プロパティの値、後方を向いている場合は必要最低限の距離となります。
        /// </remarks>
        public double FrontDrawDistance => FrontDrawDistanceGetMethod.Invoke(Src, null);

        private static FastMethod BackDrawDistanceGetMethod;
        /// <summary>
        /// 後方の描画距離 [m] を取得します。
        /// </summary>
        /// <remarks>
        /// 前方を向いている場合は必要最低限の距離、後方を向いている場合は <see cref="DrawDistance"/> プロパティの値となります。
        /// </remarks>
        public double BackDrawDistance => BackDrawDistanceGetMethod.Invoke(Src, null);

        private static FastMethod DrawDistanceGetMethod;
        /// <summary>
        /// マップファイル内での最長描画距離指定とユーザー設定から算出された描画距離 [m] を取得します。
        /// </summary>
        public double DrawDistance => DrawDistanceGetMethod.Invoke(Src, null);

        private static FastMethod DrawDistanceObjectsGetMethod;
        /// <summary>
        /// マップファイルで DrawDistance.Change ステートメントにより設置された、最長描画距離を指定するためのオブジェクトを取得します。
        /// </summary>
        public MapFunctionList DrawDistanceObjects => MapFunctionList.FromSource(DrawDistanceObjectsGetMethod.Invoke(Src, null));
    }
}
