using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    /// <summary>
    /// 拡張地上子上を列車が通過したときに発生するイベントのデータを提供します。
    /// </summary>
    public class PassedEventArgs : EventArgs
    {
        /// <summary>
        /// 通過した列車の進行方向を取得します。
        /// </summary>
        public Direction Direction { get; }

        protected readonly IReadOnlyDictionary<string, dynamic> ScriptVariables;

        public PassedEventArgs(Direction direction)
        {
            Direction = direction;
        }

        public PassedEventArgs(PassedEventArgs original, IReadOnlyDictionary<string, dynamic> scriptVariables)
        {
            Direction = original.Direction;
            ScriptVariables = scriptVariables;
        }

        /// <summary>
        /// 拡張地上子スクリプト内で設定された変数の値を取得します。
        /// </summary>
        /// <typeparam name="T">変数の型。</typeparam>
        /// <param name="name">変数名。</param>
        /// <returns>指定した変数の値。</returns>
        /// <exception cref="InvalidOperationException">拡張地上子スクリプト内など、変数が未設定の状態で呼び出そうとしました。</exception>
        public T GetScriptVariable<T>(string name) => ScriptVariables is null ? throw new InvalidOperationException() : (T)ScriptVariables[name];
    }
}
