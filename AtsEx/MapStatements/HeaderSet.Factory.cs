using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal partial class HeaderSet
    {
        private readonly static Identifier NoMapPluginHeader;
        private readonly static string NoMapPluginHeaderFullName;

        private const string HeaderNameOpenBracket = "<";
        private const string HeaderNameCloseBracket = ">";

        static HeaderSet()
        {
            NoMapPluginHeader = new Identifier(Namespace.Root, "nompi");
            NoMapPluginHeaderFullName = $"[[{NoMapPluginHeader.FullName}]]";
        }

        public static HeaderSet FromMap(string filePath)
        {
            (IDictionary<Identifier, IEnumerable<Header>> headers, IEnumerable<Header> noMapPluginHeaders) = Load(filePath);
            return new HeaderSet(headers, noMapPluginHeaders);
        }

        private static (IDictionary<Identifier, IEnumerable<Header>> Headers, IEnumerable<Header> NoMapPluginHeaders) Load(string filePath)
        {
            ConcurrentDictionary<Identifier, IEnumerable<Header>> headers = new ConcurrentDictionary<Identifier, IEnumerable<Header>>();
            List<Header> noMapPluginHeaders = new List<Header>();

            string fileName = Path.GetFileName(filePath);

            string text;
            using (StreamReader sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }

            List<MapTextParser.TextWithPosition> statements = MapTextParser.GetStatementsFromText(text);
            statements.ForEach(s =>
            {
                if (s.Text.StartsWith("include'") && s.Text.EndsWith("'") && s.Text.Length - s.Text.Replace("'", "").Length == 2)
                {
                    string includePath = s.Text.Split('\'')[1];
                    int headerCloseBracketIndex = includePath.IndexOf(HeaderNameCloseBracket);
                    if (includePath.StartsWith(HeaderNameOpenBracket) && headerCloseBracketIndex != -1)
                    {
                        string headerFullName = includePath.Substring(HeaderNameOpenBracket.Length, headerCloseBracketIndex - HeaderNameOpenBracket.Length);
                        string headerArgument = includePath.Substring(headerCloseBracketIndex + HeaderNameCloseBracket.Length);

                        Identifier identifier = Identifier.Parse(headerFullName);
                        Header header = new Header(identifier, headerArgument, s.LineIndex, s.CharIndex);
                        if (header.Name.Namespace is null || !header.Name.Namespace.IsChildOf(Namespace.Root)) return;

                        List<Header> list = headers.GetOrAdd(identifier, new List<Header>()) as List<Header>;
                        list.Add(header);
                    }
                    else if (includePath.StartsWith(NoMapPluginHeaderFullName))
                    {
                        string headerArgument = includePath.Substring(NoMapPluginHeaderFullName.Length);

                        Header header = new Header(NoMapPluginHeader, headerArgument, s.LineIndex, s.CharIndex);
                        noMapPluginHeaders.Add(header);
                    }
                    else
                    {
                        string includeRelativePath = includePath;
                        string includeAbsolutePath = Path.Combine(Path.GetDirectoryName(filePath), includeRelativePath);

                        if (!File.Exists(includeAbsolutePath)) return;

                        (IDictionary<Identifier, IEnumerable<Header>> headersInIncludedMap, IEnumerable<Header> noMapPluginHeadersInIncludedMap) = Load(includeAbsolutePath);

                        foreach (KeyValuePair<Identifier, IEnumerable<Header>> pair in headersInIncludedMap)
                        {
                            List<Header> list = headers.GetOrAdd(pair.Key, new List<Header>()) as List<Header>;
                            list.AddRange(pair.Value);
                        }

                        noMapPluginHeaders.AddRange(noMapPluginHeadersInIncludedMap);
                    }
                }
            });

            return (headers, noMapPluginHeaders);
        }
    }
}
