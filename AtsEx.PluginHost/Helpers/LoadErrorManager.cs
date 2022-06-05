using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    /// <summary>
    /// シナリオ読込時のエラーを編集するための機能を提供します。
    /// </summary>
    public static partial class LoadErrorManager
    {
        private static IApp App;
        private static IBveHacker BveHacker;

        [InitializeHelper]
        private static void Initialize(IApp app, IBveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;

            LoadErrorList.Initialize(App, BveHacker);

            Errors = new LoadErrorList();
        }


        /// <summary>
        /// エラーの一覧を取得します。
        /// </summary>
        public static LoadErrorList Errors { get; private set; }


        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        /// <param name="lineIndex">エラーの発生元となる行番号。</param>
        /// <param name="charIndex">エラーの発生元となる列番号。</param>
        public static void Throw(string text, string senderFileName, int lineIndex, int charIndex)
        {
            BveHacker.LoadingProgressForm.ThrowError(text, senderFileName, lineIndex, charIndex);
        }

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        /// <param name="lineIndex">エラーの発生元となる行番号。</param>
        public static void Throw(string text, string senderFileName, int lineIndex) => Throw(text, senderFileName, lineIndex, 0);

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        public static void Throw(string text, string senderFileName) => Throw(text, senderFileName, 0);

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        public static void Throw(string text) => Throw(text, "");

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="error">スローするエラー。</param>
        public static void Throw(LoadError error)
        {
            BveHacker.LoadingProgressForm.ThrowError(error);
        }
    }
}
