using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Panels.Native
{
    /// <summary>
    /// ATS プラグインによって制御可能な運転台パネルの状態量 (例えば「ats12」など、subjectKey が「ats」から始まる状態量) を AtsEX プラグインから操作するための機能を提供します。
    /// </summary>
    public interface IAtsPanelValueSet
    {
        /// <summary>
        /// インデックスと型を指定して、AtsEX から使用する状態量を登録します。
        /// </summary>
        /// <typeparam name="TValue">状態量の型。</typeparam>
        /// <param name="index">状態量のインデックス。例えばこの値が 12 なら、subjectKey「ats12」の状態量を表します。</param>
        /// <param name="valueSerializer"><typeparamref name="TValue"/> 型の状態量を BVE 本体へ渡す整数値に変換するシリアライザ。</param>
        /// <param name="initialValue">状態量の初期値。</param>
        /// <returns>登録した状態量を AtsEX から操作するためのオブジェクト。</returns>
        IAtsPanelValue<TValue> Register<TValue>(int index, Converter<TValue, int> valueSerializer, TValue initialValue = default);

        /// <summary>
        /// インデックスを指定して、AtsEX から使用する <see langword="bool"/> 型の状態量を登録します。
        /// </summary>
        /// <param name="index">状態量のインデックス。例えばこの値が 12 なら、subjectKey「ats12」の状態量を表します。</param>
        /// <param name="initialValue">状態量の初期値。</param>
        /// <returns>登録した状態量を AtsEX から操作するためのオブジェクト。</returns>
        IAtsPanelValue<bool> RegisterBoolean(int index, bool initialValue = default);

        /// <summary>
        /// インデックスを指定して、AtsEX から使用する <see cref="int"/> 型の状態量を登録します。
        /// </summary>
        /// <param name="index">状態量のインデックス。例えばこの値が 12 なら、subjectKey「ats12」の状態量を表します。</param>
        /// <param name="initialValue">状態量の初期値。</param>
        /// <returns>登録した状態量を AtsEX から操作するためのオブジェクト。</returns>
        IAtsPanelValue<int> RegisterInt32(int index, int initialValue = default);
    }
}
