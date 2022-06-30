using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 「シナリオを読み込んでいます...」フォームを表します。
    /// </summary>
    public sealed class LoadingProgressForm : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<LoadingProgressForm>();

            IsErrorCriticalField = members.GetSourceFieldOf(nameof(IsErrorCritical));
            ErrorCountField = members.GetSourceFieldOf(nameof(ErrorCount));
            PanelField = members.GetSourceFieldOf(nameof(Panel));
            ErrorListViewField = members.GetSourceFieldOf(nameof(ErrorListView));

            ThrowErrorMethod1 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
            ThrowErrorMethod2 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(LoadError) });
            ThrowErrorsMethod = members.GetSourceMethodOf(nameof(ThrowErrors), new Type[] { typeof(IEnumerable<LoadError>) });
        }

        private LoadingProgressForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="LoadingProgressForm"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static LoadingProgressForm FromSource(object src)
        {
            if (src is null) return null;
            return new LoadingProgressForm(src);
        }


        private static FieldInfo IsErrorCriticalField;
        /// <summary>
        /// 読み込みの強制継続が不可能なエラーが発生しているかどうかを取得・設定します。
        /// </summary>
        public bool IsErrorCritical
        {
            get => IsErrorCriticalField.GetValue(Src);
            set => IsErrorCriticalField.SetValue(Src, value);
        }

        private static FieldInfo ErrorCountField;
        internal int ErrorCount
        {
            get => ErrorCountField.GetValue(Src);
            set => ErrorCountField.SetValue(Src, value);
        }

        private static FieldInfo PanelField;
        internal Panel Panel => PanelField.GetValue(Src);

        private static FieldInfo ErrorListViewField;
        internal ListView ErrorListView => ErrorListViewField.GetValue(Src);


        private static MethodInfo ThrowErrorMethod1;
        /// <summary>
        /// エラーをエラー一覧に追加します。
        /// </summary>
        /// <param name="text">エラーの内容を表すテキスト。</param>
        /// <param name="senderFileName">エラーの発生元となるファイルのファイル名。</param>
        /// <param name="lineIndex">エラーの発生元となる行番号。</param>
        /// <param name="charIndex">エラーの発生元となる列番号。</param>
        /// <remarks>
        /// 通常は <see cref="LoadErrorManager.Throw(string, string, int, int)"/> メソッド、<see cref="LoadErrorManager.Throw(string, string, int)"/> メソッド、
        /// <see cref="LoadErrorManager.Throw(string, string)"/> メソッド、<see cref="LoadErrorManager.Throw(string)"/> メソッドを使用してください。
        /// </remarks>
        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
            => ThrowErrorMethod1.Invoke(Src, new object[] { text, senderFileName, lineIndex, charIndex });

        private static MethodInfo ThrowErrorMethod2;
        /// <summary>
        /// エラーをエラー一覧に追加します。
        /// </summary>
        /// <param name="error">追加するエラー。</param>
        /// <remarks>
        /// 通常は <see cref="LoadErrorManager.Throw(LoadError)"/> メソッドを使用してください。
        /// </remarks>
        public void ThrowError(LoadError error) => ThrowErrorMethod2.Invoke(Src, new object[] { error.Src });

        private static MethodInfo ThrowErrorsMethod;
        /// <summary>
        /// 複数のエラーをエラー一覧に追加します。
        /// </summary>
        /// <param name="errors">追加するエラー。</param>
        /// <remarks>
        /// 通常は <see cref="LoadErrorManager.Throw(LoadError)"/> メソッドを使用してください。
        /// </remarks>
        public void ThrowErrors(IEnumerable<LoadError> errors) => ThrowErrorsMethod.Invoke(Src, new object[] { errors.Select(error => error.Src) });
    }
}
