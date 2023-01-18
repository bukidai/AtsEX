using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers.Extensions
{
    internal class WrappedEnumerator<TWrapper> : IEnumerator<TWrapper>, IEnumerator
    {
        private readonly IEnumerator Src;
        private readonly ITwoWayConverter<object, TWrapper> Converter;

        public WrappedEnumerator(IEnumerator src, ITwoWayConverter<object, TWrapper> converter)
        {
            Src = src;
            Converter = converter;
        }

        public TWrapper Current => Converter.Convert(Src.Current);

        object IEnumerator.Current => Current;

        public bool MoveNext() => Src.MoveNext();

        public void Reset() => Src.Reset();

        public void Dispose()
        {
            if (Src is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
