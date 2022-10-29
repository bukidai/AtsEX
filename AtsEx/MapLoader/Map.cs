using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Plugins;
using AtsEx.Scripting.CSharp;

namespace AtsEx
{
    internal class Map
    {
        protected const string NoMapPluginHeader = "[[nompi]]";
        protected const string MapPluginUsingHeader = "<mpiusing>";

        public Dictionary<string, PluginBase> LoadedPlugins { get; }
        public List<LoadError> MapPluginUsingErrors { get; }

        protected Map(Dictionary<string, PluginBase> loadedPlugins, List<LoadError> mapPluginUsingErrors)
        {
            LoadedPlugins = loadedPlugins;
            MapPluginUsingErrors = mapPluginUsingErrors;
        }

        public static Map Load(string filePath, Plugins.PluginLoader pluginLoader, ILoadErrorResolver loadErrorResolver)
        {
            Dictionary<string, PluginBase> loadedPlugins = new Dictionary<string, PluginBase>();
            List<LoadError> mapPluginUsingErrors = new List<LoadError>();

            string fileName = Path.GetFileName(filePath);

            using (StreamReader sr = new StreamReader(filePath))
            {
                for (int i = 0; !sr.EndOfStream; i++)
                {
                    List<TextWithCharIndex> statements = GetStatementsFromLine(sr.ReadLine());
                    statements.ForEach(s =>
                    {
                        if (s.Text.StartsWith("include'") && s.Text.EndsWith("'") && s.Text.Length - s.Text.Replace("'", "").Length == 2)
                        {
                            string includePath = s.Text.Split('\'')[1];
                            if (includePath.StartsWith(MapPluginUsingHeader))
                            {
                                string mapPluginUsingRelativePath = includePath.Substring(MapPluginUsingHeader.Length);
                                string mapPluginUsingAbsolutePath = Path.Combine(Path.GetDirectoryName(filePath), mapPluginUsingRelativePath);

                                try
                                {
                                    PluginUsing mapPluginUsing = PluginUsing.Load(PluginType.MapPlugin, mapPluginUsingAbsolutePath);
                                    Dictionary<string, PluginBase> loadedMapPlugins = pluginLoader.Load(mapPluginUsing);
                                    AddRangeToLoadedPlugins(loadedMapPlugins);
                                }
                                catch (CompilationException ex)
                                {
                                    loadErrorResolver.Resolve(ex);
                                }

                                mapPluginUsingErrors.Add(new LoadError(null, fileName, i + 1, s.CharIndex + 1));
                            }
                            else if (includePath.StartsWith(NoMapPluginHeader))
                            {
                                mapPluginUsingErrors.Add(new LoadError(null, fileName, i + 1, s.CharIndex + 1));
                            }
                            else
                            {
                                string includeRelativePath = includePath;
                                string includeAbsolutePath = Path.Combine(Path.GetDirectoryName(filePath), includeRelativePath);
                                
                                Map includedMap = Load(includeAbsolutePath, pluginLoader, loadErrorResolver);

                                AddRangeToLoadedPlugins(includedMap.LoadedPlugins);
                                mapPluginUsingErrors.AddRange(includedMap.MapPluginUsingErrors);
                            }
                        }
                    });
                }
            }

            return new Map(loadedPlugins, mapPluginUsingErrors);


            void AddRangeToLoadedPlugins(IReadOnlyDictionary<string, PluginBase> plugins)
            {
                foreach (KeyValuePair<string, PluginBase> item in plugins)
                {
                    loadedPlugins.Add(item.Key, item.Value);
                }
            }
        }

        protected static List<TextWithCharIndex> GetStatementsFromLine(string line)
        {
            string trimmedLine = line.ToLower();

            List<TextWithCharIndex> statements = new List<TextWithCharIndex>();

            {
                bool isInString = false;
                int lastStatementEndIndex = -1;
                int notTrimmedLastStatementEndIndex = trimmedLine.Length - trimmedLine.TrimStart().Length - 1;

                int i = 0;
                int n = 0;
                while (i < trimmedLine.Length)
                {
                    switch (trimmedLine[i])
                    {
                        case '/':
                            if (!isInString && i + 1 < trimmedLine.Length && trimmedLine[i + 1] == '/') return statements;
                            break;

                        case '#':
                            return statements;

                        case '\'':
                            isInString = !isInString;
                            break;

                        case ' ':
                        case '\t':
                            if (!isInString)
                            {
                                trimmedLine = trimmedLine.Remove(i, 1);
                                i--;
                            }
                            break;

                        case ';':
                            if (!isInString)
                            {
                                trimmedLine = trimmedLine.Remove(i, 1);
                                i--;
                                if (i != lastStatementEndIndex)
                                {
                                    string statement = trimmedLine.Substring(lastStatementEndIndex + 1, i - lastStatementEndIndex);
                                    string notTrimmedStatement = line.Substring(notTrimmedLastStatementEndIndex + 1, n - notTrimmedLastStatementEndIndex);
                                    int headSpaceCount = notTrimmedStatement.Length - notTrimmedStatement.TrimStart().Length;
                                    statements.Add(new TextWithCharIndex(notTrimmedLastStatementEndIndex + headSpaceCount + 1, statement));
                                }

                                lastStatementEndIndex = i;
                                notTrimmedLastStatementEndIndex = n;
                            }
                            break;
                    }

                    i++;
                    n++;
                }
            }

            return statements;
        }

        protected class TextWithCharIndex
        {
            public int CharIndex { get; }
            public string Text { get; }

            public TextWithCharIndex(int charIndex, string text)
            {
                CharIndex = charIndex;
                Text = text;
            }
        }
    }
}
