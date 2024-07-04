namespace Gwent
{
    class Program
    {
        static void Main(string[]args)
        {
            string text =File.ReadAllText("C:/Users/John/Desktop/programacion/Gwent++/source.txt");
            LexicalAnalyzer lex=new LexicalAnalyzer(text);
            lex.GetTokens();
            lex.Print_tokens();
            lex.Print_errors();
        }
    }
}
