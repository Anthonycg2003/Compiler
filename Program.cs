namespace Gwent
{
    class Program
    {
        static void Main(string[]args)
        {
            string text =File.ReadAllText("C:/Users/John/Desktop/programacion/Gwent++/Compiler/source.txt");
            LexicalAnalyzer lex=new LexicalAnalyzer(text);
            List<Token>tokens=lex.GetTokens();
            lex.Print_errors();
            Parser Parser=new Parser(tokens);
            ElementalProgram program=Parser.Parse();
            if(Parser.errors.Count!=0)
            {
                Parser.Print_errors();
            }
           else
            {
                SemanticAnalizer semanticAnalizer=new SemanticAnalizer(program);
                Interpreter interpreter=semanticAnalizer.Semantic_Analizer();
                if(semanticAnalizer.errors.Count!=0)
                {
                    semanticAnalizer.Print_errors();
                }
                else
                {
                    interpreter.Interpret(program);
                }
            }
        }
    }
}
