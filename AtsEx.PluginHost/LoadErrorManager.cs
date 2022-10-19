using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// シナリオ読込時のエラーを編集するための機能を提供します。
    /// </summary>
    public sealed partial class LoadErrorManager
    {
        private readonly LoadingProgressForm LoadingProgressForm;

        internal LoadErrorManager(LoadingProgressForm loadingProgressForm)
        {
            LoadingProgressForm = loadingProgressForm;
            Errors = new LoadErrorList(LoadingProgressForm);
        }

        /// <summary>
        /// エラーの一覧を取得します。
        /// </summary>
        public LoadErrorList Errors { get; private set; }

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        /// <param name="lineIndex">エラーの発生元となる行番号。</param>
        /// <param name="charIndex">エラーの発生元となる列番号。</param>
        public void Throw(string text, string senderFileName, int lineIndex, int charIndex)
        {
            LoadingProgressForm.ThrowError(text, senderFileName, lineIndex, charIndex);
        }

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        /// <param name="lineIndex">エラーの発生元となる行番号。</param>
        public void Throw(string text, string senderFileName, int lineIndex) => Throw(text, senderFileName, lineIndex, 0);

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        public void Throw(string text, string senderFileName) => Throw(text, senderFileName, 0);

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        public void Throw(string text) => Throw(text, "");

        /// <summary>
        /// エラーをスローします。
        /// </summary>
        /// <param name="error">スローするエラー。</param>
        public void Throw(LoadError error)
        {
            LoadingProgressForm.ThrowError(error);
        }
    }
}
