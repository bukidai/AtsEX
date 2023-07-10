using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Utilities;
using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal sealed partial class HeaderSet : IHeaderSet, IEnumerable<Header>
    {
        private readonly IDictionary<Identifier, IReadOnlyList<Header>> Headers;

        public IReadOnlyList<Header> PrivateHeaders { get; }

        public HeaderSet(IDictionary<Identifier, IReadOnlyList<Header>> headers, IReadOnlyList<Header> privateHeaders)
        {
            Headers = headers;
            PrivateHeaders = privateHeaders;
        }

        public IReadOnlyList<IHeader> GetAll(Identifier identifier) => Headers.TryGetValue(identifier, out IReadOnlyList<Header> result) ? result : new List<Header>();

        public IEnumerator<Header> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<Header>
        {
            private readonly HeaderSet Source;

            private bool AreHeadersEnumerated = false;

            private readonly IEnumerator<Header> HeadersEnumerator;
            private readonly IEnumerator<Header> PrivateHeadersEnumerator;

            private IEnumerator<Header> CurrentEnumerator => AreHeadersEnumerated ? PrivateHeadersEnumerator : HeadersEnumerator;

            public Header Current => CurrentEnumerator.Current;
            object IEnumerator.Current => Current;

            public Enumerator(HeaderSet source)
            {
                Source = source;

                HeadersEnumerator = new EnumerableInDictionaryEnumerator<Identifier, Header>(Source.Headers);
                PrivateHeadersEnumerator = Source.PrivateHeaders.GetEnumerator();

                Reset();
            }

            public void Dispose()
            {
                HeadersEnumerator.Dispose();
                PrivateHeadersEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (AreHeadersEnumerated)
                {
                    return PrivateHeadersEnumerator.MoveNext();
                }
                else
                {
                    bool isEnd = !HeadersEnumerator.MoveNext();
                    if (isEnd)
                    {
                        AreHeadersEnumerated = true;
                        return MoveNext();
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                HeadersEnumerator.Reset();
                PrivateHeadersEnumerator.Reset();

                AreHeadersEnumerated = false;
            }
        }
    }
}
