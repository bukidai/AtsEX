using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.MapStatements
{
    internal static class MapTextParser
    {
        public static List<TextWithPosition> GetStatementsFromText(string text)
        {
            string trimmedLine = text.ToLower();

            List<TextWithPosition> statements = new List<TextWithPosition>();

            {
                bool isInString = false;

                int lineIndex = 1;
                int notTrimmedLastLineBreakIndex = -1;

                int lastStatementEndIndex = -1;
                int notTrimmedLastStatementEndIndex = trimmedLine.Length - trimmedLine.TrimStart().Length - 1;

                int i = 0;
                int n = 0;
                while (i < trimmedLine.Length)
                {
                    switch (trimmedLine[i])
                    {
                        case '\n':
                            isInString = false;

                            lineIndex++;
                            notTrimmedLastLineBreakIndex = n;

                            lastStatementEndIndex = i;
                            notTrimmedLastStatementEndIndex = n;
                            break;

                        case '/':
                            if (!isInString && i + 1 < text.Length && text[i + 1] == '/')
                            {
                                int nextLineBreakIndex = text.IndexOf('\n', i);
                                if (nextLineBreakIndex == -1)
                                {
                                    return statements;
                                }
                                else
                                {
                                    i = nextLineBreakIndex;
                                }
                            }
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
                                    string statementText = trimmedLine.Substring(lastStatementEndIndex + 1, i - lastStatementEndIndex);
                                    string notTrimmedStatementText = text.Substring(notTrimmedLastStatementEndIndex + 1, n - notTrimmedLastStatementEndIndex);

                                    int headSpaceCount = notTrimmedStatementText.Length - notTrimmedStatementText.TrimStart().Length;
                                    statements.Add(new TextWithPosition(lineIndex, notTrimmedLastStatementEndIndex + headSpaceCount + 1 - notTrimmedLastLineBreakIndex, statementText));
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

        internal class TextWithPosition
        {
            public int LineIndex { get; }
            public int CharIndex { get; }
            public string Text { get; }

            public TextWithPosition(int lineIndex, int charIndex, string text)
            {
                LineIndex = lineIndex;
                CharIndex = charIndex;
                Text = text;
            }

            public override string ToString() => $"({LineIndex}, {CharIndex}): \"{Text}\"";
        }
    }
}
