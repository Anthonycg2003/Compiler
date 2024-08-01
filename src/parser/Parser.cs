
using System.ComponentModel;
using System.Reflection.PortableExecutable;

class Parser
{
    private  List<Token> tokens;
    public List<CompilingError> errors;
    private int current = 0;
    bool PanicMode;
    public Parser(List<Token> tokens)
    {
        this.tokens=tokens;
        errors=new List<CompilingError>();
        PanicMode=false;
    }
    #region Internal Metods
    bool IsAtEnd()
    {
        if(current==tokens.Count-1)
        {
            return true;
        }
        return false;
    }
    Token peek()
    {
        return tokens[current];
    }
    Token previus()
    {
        return tokens[current-1];
    }
    Token advance()
    {
        if(!IsAtEnd()){current++;}
        return previus();
    }
    bool Check(TokenType type)
    {
        if(IsAtEnd()){return false;}
        return peek().Type==type;
    }
    bool match(TokenType type)
    {
        if(Check(type))
        {
            current++;
            return true;
        }
        return false;
    }
    Token Consume(TokenType type,string message)
    {
        if(match(type))
        {
            PanicMode=false;
            return previus();
        }
        errors.Add(new CompilingError(peek().Location,ErrorCode.Expected,message));
        if(PanicMode)
        {
            current++;
        }
        return peek();
    }
    void Consume(string message)
    {
        errors.Add(new CompilingError(peek().Location,ErrorCode.Expected,message));
        advance();
    }
    public void Print_errors()
    {
        foreach(CompilingError error in errors)
        {
            Console.WriteLine(error.Code+"=> "+error.Argument+" at line "+error.location.line+" and column "+error.location.column);
        }
    }
    #endregion
    public ElementalProgram Parse()
    {
        ElementalProgram program = new ElementalProgram(new CodeLocation());
        while(!IsAtEnd())
        {
            if(match(TokenType.CARD))
            {
                Card card=ParseCard();
                program.Cards[card.Name]=card;
            }
            else if(match(TokenType.EFFECT))
            {
                Effect effect=ParseEffect();
                program.Effects[effect.Name]=effect;
            }
            else
            {
                Consume("Card or effect declaration expected");
            }
        }
        return program;
    }
    Card ParseCard()
    {
        CodeLocation location=peek().Location;
        string? name=null;
        string? faction=null;
        Token? type=null;
        List<string>?ranges=null;
        int? power=null;
        CallEffect? effect=null;
        Consume(TokenType.LEFT_KEY,"{ expected");
        while(!Check(TokenType.RIGHT_KEY))
        {
        switch(peek().Type)
        {
            case TokenType.NAME:
            {
                if(name!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"Name property has been declared"));
                }
                advance();
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.STRING,"valid name expected");
                name=previus().Value;
                Consume(TokenType.COMMA,", expected");
                break;
            }
            case TokenType.FACTION:
            {
                if(faction!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"Faction property has been declared"));
                }
                advance();
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.STRING,"valid faction expected");
                faction=previus().Value;
                Consume(TokenType.COMMA,", expected");
                break;
            }
            case TokenType.TYPE:
            {
                if(type!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"Type property has been declared")); 
                }
                advance();
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.STRING,"valid type expected");
                type=previus();
                Consume(TokenType.COMMA,", expected");
                break;
            }
            case TokenType.POWER:
            {
                if(power!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"Power property has been declared"));
                }
                advance();
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.NUMBER,"valid power expected");
                power=Int32.Parse(previus().Value);
                Consume(TokenType.COMMA,", expected");
                break;
            }
            case TokenType.RANGE:
            {
                if(ranges!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"Range property has been declared"));
                }
                ranges=new List<string>();
                advance();
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.LEFT_BRACE,"[ expected");
                bool into_range=true;
                while(into_range)
                {
                    Consume(TokenType.STRING,"valid range expected");
                    ranges.Add(previus().Value);
                    if(!Check(TokenType.COMMA))
                    {
                        into_range=false;
                        break;
                    }
                    advance();
                }
                Consume(TokenType.RIGHT_BRACE,"] expected");
                Consume(TokenType.COMMA,", expected");
                break;
            }
            case TokenType.OnACTIVATION:
            {
                if(effect!=null)
                {
                    errors.Add(new CompilingError(peek().Location,ErrorCode.Invalid,"OnActivation property has been declared"));
                }
                CodeLocation codeLocation=peek().Location;
                advance();
                Consume(TokenType.LEFT_KEY,"{ expected");
                Consume(TokenType.effect,"keyword Effect expected");
                Consume(TokenType.COLON,": expected");
                Consume(TokenType.LEFT_KEY,"{ expected");
                Consume(TokenType.NAME,"Name expected");
                Consume(TokenType.COLON,": expected");
                Token effect_name=Consume(TokenType.STRING,"valid string expected");
                Consume(TokenType.COMMA,", expected");
                Dictionary<Token,Expression> arguments=new Dictionary<Token, Expression>();
                while(Check(TokenType.IDENTIFIER))
                {
                    Token param=peek();
                    current++;
                    Consume(TokenType.COLON,": expected");
                    Expression expression=ParseExpression();
                    arguments.Add(param,expression);
                    Consume(TokenType.COMMA,", expected");
                }
                Consume(TokenType.RIGHT_KEY,"} expected");
                Consume(TokenType.RIGHT_KEY,"} expected");
                effect=new CallEffect(codeLocation,arguments,effect_name.Value);
                break;
            }
            default:
            {
                Consume("card property expected");
                break;
            }
        }
        }
        Consume(TokenType.RIGHT_KEY,"} expected");
        if(name==null)
        {
            errors.Add(new CompilingError(location,ErrorCode.Expected,"card name expected"));
            name="null";
        }
        if(faction==null)
        {
            errors.Add(new CompilingError(location,ErrorCode.Expected,"card faction expected"));
            faction="null";
        }
        if(type==null)
        {
            errors.Add(new CompilingError(location, ErrorCode.Invalid,"the card should have a type"));
            type=new Token(TokenType.UNKNOW,"",new CodeLocation());
        }
        return new Card(location,name,faction,type,ranges,power,effect);
    }
    
    #region Statements AST
    Stmt ParseStmt()
    {
        if(match(TokenType.WHILE))
        {
            return ParseWhileStmt(peek().Location);
        }
        if(match(TokenType.SEMICOLON))
        {
            return new EmptyStmt(peek().Location);
        }
        return ParseExpressionStmt(peek().Location);
    }
    Stmt ParseExpressionStmt(CodeLocation location)
    {
        Expression expr=ParseExpression();
        Consume(TokenType.SEMICOLON,"; expected after expression");
        return new ExpressionStmt(expr,location);
    }
    Stmt ParseWhileStmt(CodeLocation location)
    {
        Consume(TokenType.LEFT_PAREN,"( expected");
        Expression condition=ParseExpression();
        Consume(TokenType.RIGHT_PAREN,") expected");
        Consume(TokenType.LEFT_KEY,"{ expected");
        List<Stmt>body=new List<Stmt>();
        while(peek().Type!=TokenType.RIGHT_KEY)
        {
            body.Add(ParseStmt());
        }
        current++;
        return new WhileStmt(condition,body,location);
    }
    Effect ParseEffect()
    {
        CodeLocation codeLocation=peek().Location;
        string? name=null;
        List<Token>Params=new List<Token>();
        List<Stmt>?body=null;
        Consume(TokenType.LEFT_KEY,"{ expected");
        while(!Check(TokenType.RIGHT_KEY))
        {
            switch(peek().Type)
            {
                case TokenType.NAME:
                {
                    advance();
                    Consume(TokenType.COLON,": expected");
                    Consume(TokenType.STRING,"valid name expected");
                    name=previus().Value;
                    Consume(TokenType.COMMA,", expected");
                    break;
                }
                case TokenType.ACTION:
                {
                    body=new List<Stmt>();
                    advance();
                    Consume(TokenType.COLON,": expected at Action declaration");
                    Consume(TokenType.LEFT_PAREN,"( expected at Action declaration");
                    Consume(TokenType.RIGHT_PAREN,") expected after param");
                    Consume(TokenType.LAMBDA,"=> expected after params");
                    Consume(TokenType.LEFT_KEY,"{ expected after =>");
                    while(!match(TokenType.RIGHT_KEY))
                    {
                        body.Add(ParseStmt());
                    }
                    break;
                }
                case TokenType.PARAMS:
                {
                    advance();
                    Consume(TokenType.COLON,": expected");
                    Consume(TokenType.LEFT_KEY,"{ expected");
                    do
                    {
                        if(!match(TokenType.RIGHT_KEY))
                        {
                            Params.Add(Consume(TokenType.IDENTIFIER,"Identifier in param expected"));
                            //Consume(TokenType.COLON,": expected after param identifier");
                            //consumir tipo
                        }
                    }
                    while(match(TokenType.COMMA));
                    Consume(TokenType.RIGHT_KEY,"} expected");
                    break;
                }
            }
        }
        Consume(TokenType.RIGHT_KEY,"} expected");
        if(name==null)
        {
            errors.Add(new CompilingError(codeLocation,ErrorCode.Expected,"effect name expected"));
            name="null";
        }
        if(body==null)
        {
            errors.Add(new CompilingError(codeLocation,ErrorCode.Expected,"effect Action expected"));
            body=new List<Stmt>();;
        }
        return new Effect(name,Params,body,codeLocation);
    }
    #endregion
    #region Expression AST
    Expression ParseExpression()
    {
        CodeLocation location=peek().Location;
        return ParseAssign(location);
    }
    Expression ParseAssign(CodeLocation Location)
    {
        Expression expr=ParseOr(Location);
        if(match(TokenType.EQUAL))
        {
            Expression right=ParseOr(Location);
            if(expr.GetType()==typeof(Variable))
            {
                return new AssignExpression(Location,((Variable)expr),right);
            }
            if(expr.GetType()==typeof(GetExpression))
            {
                GetExpression Get=(GetExpression)expr;
                return new SetExpression(Location,Get.name,Get.callee,right);
            }
            errors.Add(new CompilingError(Location,ErrorCode.Invalid,"Invalid assignment target"));
        }
        return expr;
    }
    Expression ParseOr(CodeLocation Location)
    {
        Expression expr=ParseAnd(Location);
        while(match(TokenType.OR))
        {
            Token Operator=previus();
            Expression right=ParseAnd(Location);
            expr=new BinaryExpression(Location,expr,Operator,right);
        }
        return expr;
    }
    Expression ParseAnd(CodeLocation Location)
    {
        Expression expr=ParseEquality(Location);
        while(match(TokenType.AND))
        {
            Token Operator=previus();
            Expression right=ParseEquality(Location);
           expr=new BinaryExpression(Location,expr,Operator,right);
        }
        return expr;
    }
    Expression ParseEquality(CodeLocation Location)
    {
        Expression expr=ParseComparisson(Location);
        if(match(TokenType.EQUAL_EQUAL))
        {
            Token tokenOperator=previus();
            Expression right=ParseComparisson(Location);
            expr=new BinaryExpression(Location,expr,tokenOperator,right);
        }
        return expr;
    }
    Expression ParseComparisson(CodeLocation Location)
    {
        Expression expr=ParseTerm(Location);
        if(match(TokenType.GREATER)||match(TokenType.GREATER_EQUAL)||match(TokenType.LESS_EQUAL)||match(TokenType.LESS))
        {
            Token tokenOperator=previus();
            Expression right=ParseTerm(Location);
            expr=new BinaryExpression(Location,expr,tokenOperator,right);
        }
        return expr;
    }
    Expression ParseTerm(CodeLocation Location)
    {
        Expression expr=ParseFactor(Location);
        if(match(TokenType.PLUS)||match(TokenType.MINUS))
        {
            Token tokenOperator=previus();
            Expression right=ParseFactor(Location);
            expr=new BinaryExpression(Location,expr,tokenOperator,right);
        }
        return expr;
    }
    Expression ParseFactor(CodeLocation Location)
    {
        Expression expr=ParsePow(Location);
        if(match(TokenType.STAR)||match(TokenType.SLASH))
        {
            Token tokenOperator=previus();
            Expression right=ParsePow(Location);
            expr=new BinaryExpression(Location,expr,tokenOperator,right);
        }
        return expr;
    }
    Expression ParsePow(CodeLocation Location)
    {
        Expression expr=ParseUnary(Location);
        if(match(TokenType.Caret))
        {
            Token tokenOperator=previus();
            Expression right=ParseUnary(Location);
            expr=new BinaryExpression(Location,expr,tokenOperator,right);
        }
        return expr;
    }
    Expression ParseUnary(CodeLocation Location)
    {
        if(match(TokenType.MINUS))
        {
            Token tokenOperator=previus();
            Expression right=ParseUnary(Location);
            return new Unary(right,tokenOperator,Location);
        }
        return ParseCall(Location);;
    }
    Expression ParseCall(CodeLocation Location)
    {
        Expression expr=ParseAtom(Location);
        while(match(TokenType.DOT))
        {
            Token name=Consume(TokenType.IDENTIFIER,"expected property name");
                if(match(TokenType.LEFT_PAREN))
                {
                    List<Expression>arguments=new List<Expression>();
                    if(!Check(TokenType.RIGHT_PAREN))
                    {
                        do
                        {
                            arguments.Add(ParseExpression());
                        }
                        while(match(TokenType.COMMA));
                    }
                    Consume(TokenType.RIGHT_PAREN,") expected");
                    expr=new MetodExpression(Location,name,arguments,expr);
                }
                else
                {
                    expr=new GetExpression(Location,name,expr);
                }
        }
        return expr;
    }
    Expression ParseAtom(CodeLocation Location)
    {
        if(match(TokenType.FALSE)){return new Atom(false,Location);}
        if(match(TokenType.TRUE)){return new Atom(true,Location);}
        if(match(TokenType.NUMBER)){return new Atom(Double.Parse(previus().Value),Location);}
        if(match(TokenType.STRING)){return new Atom(previus().Value,Location);}
        if(match(TokenType.LEFT_PAREN))
        {
            Expression expr=ParseExpression();
            Consume(TokenType.RIGHT_PAREN,") expected after group expression");
            return new Group(expr,Location);
        }
        if(match(TokenType.IDENTIFIER))
        {
            return new Variable(Location,previus().Value);
        }
        return null;//inalcanzable
    }
    #endregion
    
}   
