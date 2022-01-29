using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class RandomFileList : ClassWrapper, IRandomFileList
    {
        [UnderConstruction]
        private Assembly _assembly;

        public RandomFileList(Assembly assembly, object src) : base(src)
        {
            _assembly = assembly;

            SelectedFileGetMethod = GetMethod("a");
            SelectedFileSetMethod = GetMethod("a", assembly, "b5");
        }

        protected MethodInfo SelectedFileGetMethod;
        protected MethodInfo SelectedFileSetMethod;
        public IBveFile SelectedFile
        {
            get => new BveFile(_assembly, SelectedFileGetMethod.Invoke(Src, null));
            set => SelectedFileGetMethod.Invoke(Src, new object[] { value.Src });
        }

        // 以下IList<IBveFile>の実装

        public IBveFile this[int index] { get => Src[index]; set => Src[index] = value; }
        public int Count => Src.Count;
        public bool IsReadOnly => Src.IsReadOnly;

        public void Add(IBveFile item) => Src.Add(item);
        public void Clear() => Src.Clear();
        public bool Contains(IBveFile item) => Src.Contains(item);
        public void CopyTo(IBveFile[] array, int arrayIndex) => Src.CopyTo(array, arrayIndex);
        public IEnumerator<IBveFile> GetEnumerator() => Src.GetEnumerator();
        public int IndexOf(IBveFile item) => Src.IndexOf(item);
        public void Insert(int index, IBveFile item) => Src.Insert(index, item);
        public bool Remove(IBveFile item) => Src.Remove(item);
        public void RemoveAt(int index) => Src.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => Src.GetEnumerator();
    }
}
