using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal class MapLoader
    {
        protected const string NoMapPluginHeader = "[[nompi]]";
        protected const string MapPluginUsingHeader = "<mpiusing>";

        public List<AtsExPluginInfo> LoadedPlugins { get; } = new List<AtsExPluginInfo>();
        public Dictionary<string, List<(int, int)>> RemoveErrorIncludePositions { get; } = new Dictionary<string, List<(int, int)>>();

        protected PluginLoader PluginLoader { get; }

        public MapLoader(PluginLoader pluginLoader)
        {
            PluginLoader = pluginLoader;
        }

        public bool Load()
        {
            string mapFilePath = BveHacker.Instance.ScenarioInfo.RouteFiles.SelectedFile.Path;
            return Load(mapFilePath);
        }

        protected bool Load(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            if (!RemoveErrorIncludePositions.ContainsKey(filePath)) RemoveErrorIncludePositions.Add(filePath, new List<(int, int)>());

            using (StreamReader sr = new StreamReader(filePath))
            {
                for (int i = 0; !sr.EndOfStream; i++)
                {
                    List<KeyValuePair<int, string>> statements = GetStatementsFromLine(sr.ReadLine());
                    statements.ForEach(s =>
                    {
                        if (s.Value.StartsWith("include'") && s.Value.EndsWith("'") && s.Value.Length - s.Value.Replace("'", "").Length == 2)
                        {
                            string includePath = s.Value.Split('\'')[1];
                            if (includePath.StartsWith(MapPluginUsingHeader))
                            {
                                string mapPluginListRelativePath = includePath.Substring(MapPluginUsingHeader.Length);
                                string mapPluginListAbsolutePath = Path.Combine(Path.GetDirectoryName(filePath), mapPluginListRelativePath);
                                IEnumerable<AtsExPluginInfo> loadedMapPlugins = PluginLoader.LoadFromList(PluginType.MapPlugin, mapPluginListAbsolutePath);
                                LoadedPlugins.AddRange(loadedMapPlugins);

                                RemoveErrorIncludePositions[filePath].Add((i + 1, s.Key + 1));
                            }
                            else if (includePath.StartsWith(NoMapPluginHeader))
                            {
                                RemoveErrorIncludePositions[filePath].Add((i + 1, s.Key + 1));
                            }
                            else
                            {
                                string includeRelativePath = includePath.Substring(MapPluginUsingHeader.Length);
                                string includeAbsolutePath = Path.Combine(Path.GetDirectoryName(filePath), includeRelativePath);
                                Load(includeAbsolutePath);
                            }
                        }
                    });
                }
            }

            return true;
        }

        protected List<KeyValuePair<int, string>> GetStatementsFromLine(string line)
        {
            string trimmedLine = line.ToLower();

            List<KeyValuePair<int, string>> statements = new List<KeyValuePair<int, string>>();

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
                                    statements.Add(new KeyValuePair<int, string>(notTrimmedLastStatementEndIndex + headSpaceCount + 1, statement));
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
    }
}
