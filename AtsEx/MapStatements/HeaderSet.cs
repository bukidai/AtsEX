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
        private readonly IDictionary<Identifier, IEnumerable<Header>> Headers;

        public IEnumerable<Header> NoMapPluginHeaders { get; }

        public HeaderSet(IDictionary<Identifier, IEnumerable<Header>> headers, IEnumerable<Header> noMapPluginHeaders)
        {
            Headers = headers;
            NoMapPluginHeaders = noMapPluginHeaders;
        }

        public IEnumerable<IHeader> GetAll(Identifier identifier) => Headers[identifier];

        public IEnumerator<Header> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<Header>
        {
            private readonly HeaderSet Source;

            private bool AreHeadersEnumerated = false;

            private readonly IEnumerator<Header> HeadersEnumerator;
            private readonly IEnumerator<Header> NoMapPluginHeadersEnumerator;

            private IEnumerator<Header> CurrentEnumerator => AreHeadersEnumerated ? NoMapPluginHeadersEnumerator : HeadersEnumerator;

            public Header Current => CurrentEnumerator.Current;
            object IEnumerator.Current => Current;

            public Enumerator(HeaderSet source)
            {
                Source = source;

                HeadersEnumerator = new EnumerableInDictionaryEnumerator<Identifier, Header>(Source.Headers);
                NoMapPluginHeadersEnumerator = Source.NoMapPluginHeaders.GetEnumerator();

                Reset();
            }

            public void Dispose()
            {
                HeadersEnumerator.Dispose();
                NoMapPluginHeadersEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (AreHeadersEnumerated)
                {
                    return NoMapPluginHeadersEnumerator.MoveNext();
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
                NoMapPluginHeadersEnumerator.Reset();

                AreHeadersEnumerated = false;
            }
        }
    }
}
