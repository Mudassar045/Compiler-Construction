/******************************************************************************
 *                           *** LEXICAL ANALYZER ***                         *
 *  NAME: Mudassar Ali                                                        *
 *  Roll: Bcsf15m045                                                          *
 *  Date: 14 - Nov - 2017                                                     *
 *                                                                            *   
 * ****************************************************************************/

using System;
namespace LexicalAnalyzer
{
    class Program
    {
        public static string readFileContent(string filename)
        {
            string pInputContainer;
            pInputContainer = System.IO.File.ReadAllText(filename);
            return pInputContainer;
        }
        static void Main(string[] args)
        {
            String pInputBuffer;
            Lexer LexicalAnal = new Lexer();
            // ---- reading file contents ---
            pInputBuffer = readFileContent(@"program.txt");
            // --- Object Instantiation ---
            string pTokenCatcher;
            while (pInputBuffer != null && pInputBuffer.Length > 0)
            {
                pInputBuffer = pInputBuffer.Trim(' ', '\t', '\n', '\r');  // Space and Tab trimmer
                pTokenCatcher = LexicalAnal.TokenGenerator(ref pInputBuffer);
                System.Console.Write(pTokenCatcher);
            }
        }
    }
}

