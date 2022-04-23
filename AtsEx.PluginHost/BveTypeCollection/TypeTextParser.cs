using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal static class TypeTextParser
    {
        public static TypeMemberNameCollectionBase.TypeInfoBase Parse(string text)
        {
            try
            {
                IEnumerable<TypeMemberNameCollectionBase.TypeInfoBase> typeParams = null;
                List<int> arrayDimensionCounts = new List<int>();

                string name = "";
                for (int i = 0; i < text.Length;)
                {
                    switch (text[i])
                    {
                        case '`':
                            {
                                int bracketStartIndex = text.IndexOf('[', i);
                                if (bracketStartIndex == -1) throw new FormatException();

                                int typeParamCount = int.Parse(text.Substring(i + 1, bracketStartIndex - i - 1));
                                IEnumerable<string> splittedText = SplitArrayText(text.Substring(bracketStartIndex + 1), typeParamCount, true);
                                typeParams = splittedText.Take(typeParamCount).Select(param => Parse(param));

                                string left = splittedText.Last();
                                i = text.Length - left.Length;
                            }
                            break;

                        case '[':
                            {
                                int bracketEndIndex = text.IndexOf(']', i);
                                if (bracketEndIndex == -1) throw new FormatException();

                                string commas = text.Substring(i + 1, bracketEndIndex - i - 1);
                                if (commas.Any(x => x != ',')) throw new FormatException();

                                if (bracketEndIndex != text.Length - 1 && text[bracketEndIndex + 1] != '[') throw new FormatException();

                                arrayDimensionCounts.Add(commas.Length + 1);

                                i = bracketEndIndex + 1;
                            }
                            break;

                        default:
                            name += text[i];
                            i++;
                            break;
                    }
                }

                TypeMemberNameCollectionBase.TypeInfoBase result;
                if (typeParams is null)
                {
                    result = new TypeMemberNameCollectionBase.TypeInfo(name);
                }
                else
                {
                    result = new TypeMemberNameCollectionBase.GenericTypeInfo(name, typeParams);
                }

                arrayDimensionCounts.ForEach(dimensionCount =>
                {
                    result = new TypeMemberNameCollectionBase.ArrayTypeInfo(result, dimensionCount);
                });

                return result;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException("テキストのフォーマットが不正です。", ex);
            }
        }

        public static IEnumerable<string> SplitArrayText(string text, int takeCount = -1, bool returnLeft = false)
        {
            int lastCommaIndex = -1;
            int tookCount = 0;
            while (true)
            {
                int nextCommaIndex = GetNextCommaIndex(text, lastCommaIndex + 1, true);
                if (nextCommaIndex == -1) break;
                yield return text.Substring(lastCommaIndex + 1, nextCommaIndex - lastCommaIndex - 1).Trim();
                tookCount++;
                lastCommaIndex = nextCommaIndex;

                if (tookCount == takeCount)
                {
                    if (returnLeft) break; else yield break;
                }
            }
            
            if (lastCommaIndex < text.Length) yield return text.Substring(lastCommaIndex + 1).Trim();
        }

        static int GetNextCommaIndex(string text, int startIndex, bool returnEndIndex)
        {
            if (text.Length <= startIndex) return -1;

            int bracketNest = 0;
            for (int i = startIndex; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '[':
                        bracketNest++;
                        break;
                    case ']':
                        bracketNest--;
                        if (bracketNest == -1)
                        {
                            return returnEndIndex ? i : -1;
                        }
                        break;
                }

                if (text[i] == ',' && bracketNest == 0)
                {
                    return i;
                }
            }

            return returnEndIndex ? text.Length : -1;
        }
    }
}
