using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// Repeater ステートメントを使用して独自に定義する構文を表します。
    /// </summary>
    /// <remarks>
    /// <see cref="IHeader"/> と異なり、シナリオの読込が完了して <see cref="BveHacker.ScenarioCreated"/> イベントが発生するまでは取得できませんが、
    /// マップ内の変数を埋め込んだり、定義した距離程と関連付けたりすることが可能です。
    /// </remarks>
    public interface IStatement
    {
        /// <summary>
        /// このステートメントの名前を表す識別子を取得します。
        /// </summary>
        /// <remarks>
        /// 使用する 1 番目のストラクチャーのキー (structureKey1) が該当します。
        /// </remarks>
        Identifier Name { get; }

        /// <summary>
        /// このステートメントの宣言の付加情報を表す識別子の一覧を取得します。
        /// </summary>
        /// <remarks>
        /// 使用する 2 番目以降のストラクチャーのキー (structureKey2、structureKey3、…、structureKeyN) が該当します。
        /// </remarks>
        Identifier[] AdditionalDeclaration { get; }

        /// <summary>
        /// このヘッダーの引数のテキストを取得します。
        /// </summary>
        /// <remarks>
        /// 連続ストラクチャー名が該当します。
        /// </remarks>
        string Argument { get; }

        /// <summary>
        /// このステートメントが設置されている範囲の始端の距離程 [m] を取得します。
        /// </summary>
        double From { get; }

        /// <summary>
        /// このステートメントが設置されている範囲の終端の距離程 [m] を取得します。
        /// </summary>
        double To { get; }

        /// <summary>
        /// このステートメントの定義に使用されている連続ストラクチャーを取得します。
        /// </summary>
        RepeatedStructure DefinedStructure { get; }

        /// <summary>
        /// このステートメント上へ自列車が進入した瞬間に発生します。
        /// </summary>
        event EventHandler<PassedEventArgs> Entered;

        /// <summary>
        /// このステートメント上へ先行列車が進入した瞬間に発生します。
        /// </summary>
        event EventHandler<PassedEventArgs> PreTrainEntered;

        /// <summary>
        /// このステートメント上へ他列車が進入した瞬間に発生します。
        /// </summary>
        event EventHandler<TrainPassedEventArgs> TrainEntered;

        /// <summary>
        /// このステートメント上から自列車が退出した瞬間に発生します。
        /// </summary>
        event EventHandler<PassedEventArgs> Exited;

        /// <summary>
        /// このステートメント上から先行列車が退出した瞬間に発生します。
        /// </summary>
        event EventHandler<PassedEventArgs> PreTrainExited;

        /// <summary>
        /// このステートメント上から他列車が退出した瞬間に発生します。
        /// </summary>
        event EventHandler<TrainPassedEventArgs> TrainExited;

    }
}
