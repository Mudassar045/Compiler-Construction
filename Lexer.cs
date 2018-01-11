using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalyzer
{
    class Lexer
    {
        private int mForwardPointer = 0;
        private int mLineNumberPointer = 1;
        private int mLexemeStartPointer = 0;
        private int mCurrentStatePointer = 0;
        private int mLastVisitedFinalStatePointer = -1;
        private int mIndexWiseLastFinalStateVistedPointer = -1;

        string[] mKeywords = { "abstract", "as", "base", "bool", "break", "by",
            "byte", "case", "catch", "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate", "do", "double",
            "descending", "explicit", "event", "extern", "else", "enum",
            "false", "finally", "fixed", "float", "for", "foreach", "from",
            "goto", "group", "if", "implicit", "in", "int", "interface",
            "internal", "into", "is", "lock", "long", "new", "null", "namespace",
            "object", "operator", "out", "override", "orderby",  "params",
            "private", "protected", "public", "readonly", "ref", "return",
            "switch", "struct", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "select",  "this",
            "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", "var", "virtual", "volatile",
            "void", "while", "where", "yield" };

        string[] mSeparator = { ";", "{", "}", "\r", "\n", "\r\n" };

        string[] mComments = { "//", "/*", "*/" };

        string[] mOperators = { "+", "-", "*", "/", "%", "&","(",")","[","]",
            "|", "^", "!", "~", "&&", "||",",",
            "++", "--", "<<", ">>", "==", "!=", "<", ">", "<=",
            ">=", "=", "+=", "-=", "*=", "/=", "%=", "&=", "|=",
            "^=", "<<=", ">>=", ".", "[]", "()", "?:", "=>", "??" };
        string[] mOtherSymbols = {};

        public Lexer() { }

        // --- Check Functions ---

        public string Parse(string buffer)
        {
            StringBuilder str = new StringBuilder();
            int ok;
            if (Int32.TryParse(buffer, out ok))
            {

                str.Append("< int, " + buffer + " >");
                str.Append(Environment.NewLine);
                return str.ToString();
            }

            if (buffer.Equals("\r\n"))
            {
                return "\r\n";
            }

            if (CheckKeyword(buffer) == true)
            {
                str.Append("< keyword, " + buffer + " >");
                str.Append(Environment.NewLine);
                return str.ToString();
            }

            if (CheckOperator(buffer) == true)
            {
                str.Append("< operator, " + buffer + " >");
                str.Append(Environment.NewLine);
                return str.ToString();
            }
            if (CheckDelimiter(buffer) == true)
            {
                str.Append("< separator, " + buffer + " >");
                str.Append(Environment.NewLine);
                return str.ToString();
            }
            if (CheckOtherSymbols(buffer) == true)
            {
                str.Append("< symbol, " + buffer + " >");
                str.Append(Environment.NewLine);
                return str.ToString();
            }
            str.Append("< identifier, " + buffer + ">");
            str.Append(Environment.NewLine);
            return str.ToString();
        }
        private bool CheckKeyword(string buffer)
        {
            if (Array.IndexOf(mKeywords, buffer) > -1)
                return true;
            return false;
        }
        private bool CheckDelimiter(string buffer)
        {
            if (Array.IndexOf(mSeparator, buffer) > -1)
                return true;
            return false;
        }
        private bool CheckOperator(string buffer)
        {
            if (Array.IndexOf(mOperators, buffer) > -1)
                return true;
            return false;
        }
        public string TokenGenerator(ref string buffer)
        {

            StringBuilder pTokenContainer = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                if (CheckDelimiter(buffer[i].ToString()))
                {
                    if (i + 1 < buffer.Length && CheckDelimiter(buffer.Substring(i, 2)))
                    {
                        pTokenContainer.Append(buffer.Substring(i, 2));
                        buffer = buffer.Remove(i, 2);
                        return Parse(pTokenContainer.ToString());
                    }
                    else
                    {
                        pTokenContainer.Append(buffer[i]);
                        buffer = buffer.Remove(i, 1);
                        return Parse(pTokenContainer.ToString());
                    }

                }
                else if (CheckOperator(buffer[i].ToString()))
                {
                    if (i + 1 < buffer.Length && (CheckOperator(buffer.Substring(i, 2))))
                        if (i + 2 < buffer.Length && CheckOperator(buffer.Substring(i, 3)))
                        {
                            pTokenContainer.Append(buffer.Substring(i, 3));
                            buffer = buffer.Remove(i, 3);
                            return Parse(pTokenContainer.ToString());
                        }
                        else
                        {
                            pTokenContainer.Append(buffer.Substring(i, 2));
                            buffer = buffer.Remove(i, 2);
                            return Parse(pTokenContainer.ToString());
                        }
                    else if (CheckComments(buffer.Substring(i, 2)))
                    {
                        if (buffer.Substring(i, 2).Equals("//"))
                        {
                            do
                            {
                                i++;
                            } while (buffer[i] != '\n');
                            buffer = buffer.Remove(0, i + 1);
                            buffer = buffer.Trim(' ', '\t', '\r', '\n');
                            i = -1;
                        }
                        else
                        {
                            do
                            {
                                i++;
                            } while (buffer.Substring(i, 2).Equals("*/") == false);
                            buffer = buffer.Remove(0, i + 2);
                            buffer = buffer.Trim(' ', '\t', '\r', '\n');
                            i = -1;
                        }

                    }
                    else
                    {
                        int ok;
                        if (buffer[i] == '-' && Int32.TryParse(buffer[i + 1].ToString(), out ok))
                            continue;
                        pTokenContainer.Append(buffer[i]);
                        buffer = buffer.Remove(i, 1);
                        return Parse(pTokenContainer.ToString());
                    }

                }
                else
                    if (buffer[i] == '\'')
                {
                    int j = i + 1;
                    if (buffer[j] == '\\')
                        j += 2;
                    else
                        j++;

                    pTokenContainer.Append("<literal constant, ").Append(buffer.Substring(i, j - i + 1)).Append(" >");
                    buffer = buffer.Remove(i, j - i + 1);
                    return pTokenContainer.ToString();
                }
                else
                    if (buffer[i] == '"')
                {
                    int j = i + 1;
                    while (buffer[j] != '"')
                        j++;
                    pTokenContainer.Append("< literal constant, ").Append(buffer.Substring(i, j - i + 1)).Append(" >");
                    pTokenContainer.Append(Environment.NewLine);
                    buffer = buffer.Remove(i, j - i + 1);
                    return pTokenContainer.ToString();
                }
                else
                    if (buffer[i + 1].ToString().Equals(" ") || CheckDelimiter(buffer[i + 1].ToString()) == true || CheckOperator(buffer[i + 1].ToString()) == true)
                {
                    if (Parse(buffer.Substring(0, i + 1)).Contains("int") && buffer[i + 1] == '.')
                    {
                        int j = i + 2;
                        while (buffer[j].ToString().Equals(" ") == false && CheckDelimiter(buffer[j].ToString()) == false && CheckOperator(buffer[j].ToString()) == false)
                            j++;
                        int ok;

                        if (Int32.TryParse(buffer.Substring(i + 2, j - i - 2), out ok))
                        {
                            if (buffer.Substring(0, j).Contains("."))
                            {
                                pTokenContainer.Append("< double, ").Append(buffer.Substring(0, j)).Append(" >");
                            }
                            pTokenContainer.Append(Environment.NewLine);
                            buffer = buffer.Remove(0, j);
                            return pTokenContainer.ToString();
                        }

                    }
                    pTokenContainer.Append(buffer.Substring(0, i + 1));
                    buffer = buffer.Remove(0, i + 1);
                    return Parse(pTokenContainer.ToString());
                }


            }
            return null;
        }
        public bool CheckComments(string buffer)
        {

            if (Array.IndexOf(mComments, buffer) > -1)
                return true;
            return false;
        }
        public bool CheckOtherSymbols(string buffer)
        {
            if (Array.IndexOf(mOtherSymbols, buffer) > -1)
                return true;
            return false;
        }
        private void PointerResetter()
        {
            mForwardPointer = 0;
            mLineNumberPointer = 1;
            mLexemeStartPointer = 0;
            mCurrentStatePointer = 0;
            mLastVisitedFinalStatePointer = -1;
            mIndexWiseLastFinalStateVistedPointer = -1;
        }
        private void Ungetch() { }
        private void ErrorCorrector() { }
        public void TransitionTableLoader() { }
    }
}
