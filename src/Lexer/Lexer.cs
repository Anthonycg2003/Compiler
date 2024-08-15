using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
public class LexicalAnalyzer
{
    #region Regular_expretions
        const string card=@"\Acard";
        const string effect=@"\Aeffect";
        const string two_points=@"\A:";
        const string comma=@"\A,";
        const string left_bracket=@"\A\[";
        const string right_bracket=@"\A\]";
        const string left_key=@"\A\{";
        const string right_key=@"\A\}";
        const string left_parent=@"\A\(";
        const string right_parent=@"\A\)";
        const string minus=@"\A-";
        const string semi_colon=@"\A;";
        const string slash=@"\A/";
        const string star=@"\A\*";
        const string dot=@"\A\.";
        const string Caret=@"\A\^";
        const string type=@"\AType";
        const string name=@"\AName";
        const string Effect=@"\AEffect";
        const string faction=@"\AFaction";
        const string power=@"\APower";
        const string range=@"\ARange";
        const string Activation=@"\AOnActivation";
        const string PARAMS=@"\AParams";
        const string ACTION=@"\AAction";
        const string EQUAL=@"\A=";
        const string EQUAL_EQUAL=@"\A==";
        const string GREATER=@"\A>";
        const string GREATER_EQUAL=@"\A>\s*=";
        const string LESS=@"\A<";
        const string LESS_EQUAL=@"\A<\s*=";
        const string ARROBA=@"\A@";
        const string ARROBA_ARROBA=@"\A@\s*@";
        const string PLUS=@"\A\+";
        const string PLUS_PLUS=@"\A\+\s*\+";
        const string AND=@"\A&\s*&";
        const string OR=@"\A\|\s*\|";
        const string FALSE=@"\Afalse";
        const string TRUE=@"\Atrue";
        const string FOR=@"\Afor";
        const string WHILE=@"\Awhile";
        const string STRING=@"\A"+"\""+"[^\"]*"+"\"";
        const string LAMBDA=@"\A=>";
        const string NUMBER=@"\A[0-9]+(\.[0-9]+)?";
        const string IDENTIFIER=@"\A[a-zA-Z]+([0-9]+)?";
        const string Unknow=@"\A[\S]+";
        const string NumberType=@"\ANumber";
        const string StringType=@"\AString";
        const string BoolType=@"\ABool";
        const string Source=@"\ASource";
        const string Selector=@"\ASelector";
        const string Single=@"\ASingle";
        const string Predicate=@"\APredicate";
        const string weather=@"\Aweather";
        const string unit=@"\Aunit";
        const string boost=@"\Aboost";
        #endregion
    static readonly Dictionary<string,TokenType> Regular_Expretions=new Dictionary<string, TokenType>
    {
        {card,TokenType.CARD},{effect,TokenType.EFFECT},{faction,TokenType.FACTION},{power,TokenType.POWER},{range,TokenType.RANGE},{type,TokenType.TYPE},{Activation,TokenType.OnACTIVATION},{name,TokenType.NAME},{PARAMS,TokenType.PARAMS},{AND,TokenType.AND},{OR,TokenType.OR},{FOR,TokenType.FOR},{FALSE,TokenType.FALSE},{TRUE,TokenType.TRUE},{WHILE,TokenType.WHILE},{ACTION,TokenType.ACTION},{Effect,TokenType.effect},{NumberType,TokenType.NumberType},{StringType,TokenType.StringType},{BoolType,TokenType.BoolType},{Selector,TokenType.Selector},{Single,TokenType.Single},{Predicate,TokenType.Predicate},{Source,TokenType.Source},
        {weather,TokenType.WEATHER},{unit,TokenType.UNIT},{boost,TokenType.BOOST},
        {right_parent,TokenType.RIGHT_PAREN},{left_parent,TokenType.LEFT_PAREN}, {comma,TokenType.COMMA},{minus,TokenType.MINUS},{semi_colon,TokenType.SEMICOLON},{slash,TokenType.SLASH},{star,TokenType.STAR},{dot,TokenType.DOT},{two_points,TokenType.COLON},{right_key,TokenType.RIGHT_KEY},{left_key,TokenType.LEFT_KEY},{left_bracket,TokenType.LEFT_BRACE},{right_bracket,TokenType.RIGHT_BRACE},{Caret,TokenType.Caret},{LAMBDA,TokenType.LAMBDA},
        {EQUAL_EQUAL,TokenType.EQUAL_EQUAL},{EQUAL,TokenType.EQUAL},{GREATER_EQUAL,TokenType.GREATER_EQUAL},{GREATER,TokenType.GREATER},{LESS_EQUAL,TokenType.LESS_EQUAL},{LESS,TokenType.LESS},{ARROBA_ARROBA,TokenType.ARROBA_ARROBA},{ARROBA,TokenType.ARROBA},{PLUS_PLUS,TokenType.PLUS_PLUS},{PLUS,TokenType.PLUS},
        {STRING,TokenType.STRING},{NUMBER,TokenType.NUMBER},{IDENTIFIER,TokenType.IDENTIFIER},
        {Unknow,TokenType.UNKNOW},
    };
    private String source;
    private List<Token> tokens;
    List<CompilingError> Errors;
    int current=0;
    CodeLocation Lexer_location;
    public int end;
    public LexicalAnalyzer(string source) 
    {
        this.source=source;
        end=source.Length;
        tokens=new List<Token>();
        Errors=new List<CompilingError>();
        Lexer_location.column=1;
        Lexer_location.line=1;
    }
    public List<Token> GetTokens()
    {
        while (current<end)
        {
            ScanToken(current,end);
        }
        tokens.Add(new Token(TokenType.EOF,"null",Lexer_location));
        return tokens;
    }
    private void ScanToken(int start,int end)
    {
        string scan_source=source.Substring(start,end-start);
        if(Regex.IsMatch(scan_source,@"\A\n"))
        {
            Lexer_location.line++;
            Lexer_location.column=1;
            current++;
            return;
        }
        if(Regex.IsMatch(scan_source,@"\A\s"))
        {
            Lexer_location.column++;
            current++;
            return;
        }
        foreach(string Regular_Expretion in Regular_Expretions.Keys)
        {
            if(Regex.IsMatch(scan_source,Regular_Expretion))
            {
                Match match=Regex.Match(scan_source,Regular_Expretion);
                tokens.Add(new Token(Regular_Expretions[Regular_Expretion],scan_source.Substring(0,match.Length),Lexer_location));
                current=start+match.Length;
                Lexer_location.column+=match.Length;
                return;
            }
        }
    }
    public void Print_errors()
    {
        if(Errors.Count==0)
        {
            return;
        }
        foreach(CompilingError error in Errors)
        {
            Console.WriteLine(error.Code+" token "+"\""+error.Argument+"\""+" at line "+error.location.line+" and column "+error.location.column);
        }
    }
}
