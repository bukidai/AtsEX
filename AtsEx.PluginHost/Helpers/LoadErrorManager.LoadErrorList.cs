using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    public static partial class LoadErrorManager
    {
        /// <summary>
        /// <see cref="LoadErrorList"/> のリストを表します。
        /// </summary>
        public class LoadErrorList : IList<LoadError>
        {
            protected LoadingProgressForm Form;
            protected Form FormSource;

            /// <summary>
            /// <see cref="LoadErrorList"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            public LoadErrorList()
            {
                Form = InstanceStore.BveHacker.LoadingProgressForm;
                FormSource = InstanceStore.BveHacker.LoadingProgressFormSource;
            }


            protected static ListViewItem ToListViewItem(LoadError loadError, int index)
            {
                ListViewItem item = new ListViewItem(new string[] {
                    index.ToString(),
                    loadError.Text,
                    loadError.SenderFileName,
                    loadError.LineIndex > 0 ? loadError.LineIndex.ToString() : string.Empty,
                    loadError.CharIndex > 0 ? loadError.CharIndex.ToString() : string.Empty,
                });
                return item;
            }

            protected static LoadError ToLoadError(ListViewItem item)
            {
                string text = item.SubItems[1].Text;
                string senderFileName = item.SubItems[2].Text;

                string lineIndexText = item.SubItems[3].Text;
                int lineIndex = 0;
                int.TryParse(lineIndexText, out lineIndex);

                string charIndexText = item.SubItems[4].Text;
                int charIndex = 0;
                int.TryParse(charIndexText, out charIndex);

                LoadError loadError = new LoadError(text, senderFileName, lineIndex, charIndex);
                return loadError;
            }

            [UnderConstruction]
            protected void Update()
            {
                Form.ErrorCount = Count;

                if (Count == 0)
                {
                    Form.IsErrorCritical = false;
                    // TODO: タイトルの書きかえ
                }

                FormSource.ClientSize = new Size(800, Count == 0 ? Form.Panel.Height : 500);
                FormSource.Update();

                Form.ErrorListView.Update();
            }


            public LoadError this[int index]
            {
                get => ToLoadError(Form.ErrorListView.Items[index]);
                set => Form.ErrorListView.Items[index] = ToListViewItem(value, index + 1);
            }

            public int Count => Form.ErrorListView.Items.Count;

            public bool IsReadOnly => false;

            public void Add(LoadError item)
            {
                Form.ErrorListView.Items.Add(ToListViewItem(item, Count + 1));
                Update();
            }

            public void Clear()
            {
                Form.ErrorListView.Items.Clear();
                Update();
            }

            public bool Contains(LoadError item) => IndexOf(item) >= 0;

            public void CopyTo(LoadError[] array, int arrayIndex)
            {
                LoadError[] items = new LoadError[Count];
                for (int i = 0; i < Count; i++)
                {
                    LoadError item = ToLoadError(Form.ErrorListView.Items[i]);
                    items[i] = item;
                }

                items.CopyTo(array, arrayIndex);
            }

            public IEnumerator<LoadError> GetEnumerator() => new Enumerator(Form);

            public int IndexOf(LoadError item)
            {
                for (int i = 0; i < Form.ErrorListView.Items.Count; i++)
                {
                    ListViewItem listViewItem = Form.ErrorListView.Items[i];

                    string text = listViewItem.SubItems[1].Text;
                    string senderFileName = listViewItem.SubItems[2].Text;
                    string lineIndexText = listViewItem.SubItems[3].Text;
                    string charIndexText = listViewItem.SubItems[4].Text;

                    if (item.Text != text || item.SenderFileName != senderFileName) continue;

                    if (item.LineIndex > 0)
                    {
                        if (item.LineIndex.ToString() != lineIndexText) continue;
                    }
                    else
                    {
                        if (lineIndexText != string.Empty) continue;
                    }

                    if (item.CharIndex > 0)
                    {
                        if (item.CharIndex.ToString() != charIndexText) continue;
                    }
                    else
                    {
                        if (charIndexText != string.Empty) continue;
                    }

                    return i;
                }

                return -1;
            }

            public void Insert(int index, LoadError item)
            {
                ListViewItem listViewItem = ToListViewItem(item, index);
                Form.ErrorListView.Items.Insert(index, listViewItem);

                for (int i = index + 1; i < Form.ErrorListView.Items.Count; i++)
                {
                    Form.ErrorListView.Items[i].SubItems[0].Text = (i + 1).ToString();
                }

                Update();
            }

            public bool Remove(LoadError item)
            {
                int targetIndex = IndexOf(item);
                if (targetIndex == -1)
                {
                    return false;
                }
                else
                {
                    RemoveAt(targetIndex);
                    return true;
                }
            }

            public void RemoveAt(int index)
            {
                Form.ErrorListView.Items.RemoveAt(index);
                Update();
            }

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(Form);


            protected class Enumerator : IEnumerator<LoadError>
            {
                protected IEnumerator ListViewItemEnumerator;

                public Enumerator(LoadingProgressForm form)
                {
                    ListViewItemEnumerator = form.ErrorListView.Items.GetEnumerator();
                }


                public LoadError Current
                {
                    get
                    {
                        ListViewItem source = ListViewItemEnumerator.Current as ListViewItem;
                        LoadError item = ToLoadError(source);
                        return item;
                    }
                }

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }

                public bool MoveNext() => ListViewItemEnumerator.MoveNext();

                public void Reset() => ListViewItemEnumerator.Reset();
            }
        }
    }
}
