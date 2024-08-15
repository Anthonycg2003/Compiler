using System.Dynamic;

public class Token
{
    public string Value { get; private set; }
    public TokenType Type { get; private set; }
    public CodeLocation Location{ get; private set;}

    public Token(TokenType type, string value, CodeLocation location)
    {
        this.Type = type;
        this.Value = value;
        Location=location;
    }

    public override string ToString()
    {
        return Type+" "+Value;
    }  
    public override bool Equals(object? obj)
    {
        if(obj!=null && obj.GetType()==typeof(Token))
        {
            return Value==((Token)obj).Value;
        }
        return false;
    }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }  
}


public enum TokenType
{
    // Single-character tokens.
  LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,LEFT_KEY,RIGHT_KEY,
  COMMA, MINUS, SEMICOLON, SLASH, STAR,DOT,COLON,Caret,LAMBDA,
  // One or two character tokens.
  EQUAL, EQUAL_EQUAL,
  GREATER, GREATER_EQUAL,
  LESS, LESS_EQUAL,
  ARROBA,ARROBA_ARROBA,
  PLUS,PLUS_PLUS,

  // Literals.
  IDENTIFIER, STRING, NUMBER,UNKNOW,
  // Keywords.
  AND, FALSE, FOR, OR, TRUE, WHILE,CARD,EFFECT,NAME,PARAMS,ACTION,TYPE,FACTION,POWER,RANGE,OnACTIVATION,effect,NumberType,StringType,BoolType,UNIT,BOOST,WEATHER,
  Selector,Source,Single,Predicate,
  //End 
  EOF
}
public struct CodeLocation
{
    public int line;
    public int column;
}