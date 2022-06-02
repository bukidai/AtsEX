using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// ランダムに 1 つの項目が選ばれる <see cref="BveFile"/> のリストを表します。
    /// </summary>
    public sealed class RandomFileList : ClassWrapper, IList<BveFile>
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<RandomFileList>();

            SelectedFileGetMethod = members.GetSourcePropertyGetterOf(nameof(SelectedFile));
            SelectedFileSetMethod = members.GetSourcePropertySetterOf(nameof(SelectedFile));
        }

        private RandomFileList(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="RandomFileList"/> クラスのインスタンス。</returns>
        public static RandomFileList FromSource(object src)
        {
            if (src is null) return null;
            return new RandomFileList(src);
        }

        private static MethodInfo SelectedFileGetMethod;
        private static MethodInfo SelectedFileSetMethod;
        /// <summary>
        /// ランダムに選ばれた項目を取得・設定します。
        /// </summary>
        public BveFile SelectedFile
        {
            get => BveFile.FromSource(SelectedFileGetMethod.Invoke(Src, null));
            set => SelectedFileGetMethod.Invoke(Src, new object[] { value.Src });
        }

        // 以下IList<IBveFile>の実装

        public BveFile this[int index] { get => Src[index]; set => Src[index] = value; }
        public int Count => Src.Count;
        public bool IsReadOnly => Src.IsReadOnly;

        public void Add(BveFile item) => Src.Add(item);
        public void Clear() => Src.Clear();
        public bool Contains(BveFile item) => Src.Contains(item);
        public void CopyTo(BveFile[] array, int arrayIndex) => Src.CopyTo(array, arrayIndex);
        public IEnumerator<BveFile> GetEnumerator() => Src.GetEnumerator();
        public int IndexOf(BveFile item) => Src.IndexOf(item);
        public void Insert(int index, BveFile item) => Src.Insert(index, item);
        public bool Remove(BveFile item) => Src.Remove(item);
        public void RemoveAt(int index) => Src.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => Src.GetEnumerator();
    }
}
