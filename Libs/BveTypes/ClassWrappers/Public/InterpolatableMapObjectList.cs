using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 2 点間の値の線形補間値を計算可能な <see cref="MapObjectList"/> を表します。
    /// </summary>
    public class InterpolatableMapObjectList : MapObjectList
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<InterpolatableMapObjectList>();

            GetValueAtMethod = members.GetSourceMethodOf(nameof(GetValueAt));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="InterpolatableMapObjectList"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected InterpolatableMapObjectList(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="InterpolatableMapObjectList"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new InterpolatableMapObjectList FromSource(object src) => src is null ? null : new InterpolatableMapObjectList((IList)src);

        private static FastMethod GetValueAtMethod;
        /// <summary>
        /// 指定した距離程における線形補間値を取得します。
        /// </summary>
        public double GetValueAt(double location)
            => (double)GetValueAtMethod.Invoke(Src, new object[] { location });
    }
}
