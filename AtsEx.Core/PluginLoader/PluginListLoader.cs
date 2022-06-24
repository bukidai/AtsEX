using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    internal static class PluginListLoader
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(PluginListLoader), "Core");

        public static IEnumerable<RecognizedDll> LoadFrom(string absolutePath)
        {
            string baseDirectory = Path.GetDirectoryName(absolutePath);
            using (StreamReader sr = new StreamReader(absolutePath))
            {
                for (int i = 1; !sr.EndOfStream; i++)
                {
                    string line = sr.ReadLine();
                    string validText = line.Split(';')[0];
                    if (validText == "") continue;

                    string pluginPath = Path.Combine(baseDirectory, validText);
                    if (!File.Exists(pluginPath))
                    {
                        throw new BveFileLoadException(string.Format(Resources.GetString("PluginNotFound").Value, pluginPath), Path.GetFileName(absolutePath), i);
                    }

                    yield return new RecognizedDll(i, baseDirectory, validText);
                }
            }
        }

        public struct RecognizedDll
        {
            public int LineIndex { get; }
            public string RelativePath { get; }
            public string AbsolutePath { get; }

            public RecognizedDll(int lineIndex, string baseDirectory, string relativePath)
            {
                LineIndex = lineIndex;
                RelativePath = relativePath;
                AbsolutePath = Path.Combine(baseDirectory, relativePath);
            }
        }
    }
}
