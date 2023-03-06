using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.Samples.MapPlugins.MapStatementTest
{
    internal class Argument
    {
        private readonly Dictionary<string, string> Parameters;

        public string Name { get; }

        private Argument(string name, Dictionary<string, string> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public static Argument FromStatement(IStatement source)
        {
            string[] sourceText = source.Argument.Split('?');
            string name = sourceText[0];

            switch (sourceText.Length)
            {
                case 1:
                    return new Argument(name, new Dictionary<string, string>());

                case 2:
                    string[] parameterTexts = sourceText[1].Split('&');
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (string parameterText in parameterTexts)
                    {
                        string[] pairText = parameterText.Split('=');
                        if (pairText.Length != 2)
                        {
                            throw new FormatException($"パラメータ '{parameterText}' のフォーマットは不正です。");
                        }

                        parameters.Add(pairText[0], pairText[1]);
                    }

                    return new Argument(sourceText[0], parameters);

                default:
                    throw new FormatException($"テキスト '{source.Argument}' のフォーマットは不正です。");
            }
        }

        public T GetParameter<T>(string key) => (T)Convert.ChangeType(Parameters[key], typeof(T));
    }
}
