
public class BinaryExpression : Expression
{
    public BinaryExpression(CodeLocation location,Expression left,Token Operator,Expression right) : base(location)
    {
        Left=left;
        Right=right;
        this.Operator=Operator;
        switch(Operator.Type)
        {
            case TokenType.PLUS:
            dataType=DataType.Number;
            break;
            case TokenType.MINUS:
            dataType=DataType.Number;
            break;
            case TokenType.STAR:
            dataType=DataType.Number;
            break;
            case TokenType.SLASH:
            dataType=DataType.Number;
            break;
            case TokenType.Caret:
            dataType=DataType.Number;
            break;
            case TokenType.GREATER:
            dataType=DataType.Bool;
            break;
            case TokenType.GREATER_EQUAL:
            dataType=DataType.Bool;
            break;
            case TokenType.LESS:
            dataType=DataType.Bool;
            break;
            case TokenType.LESS_EQUAL:
            dataType=DataType.Bool;
            break;
            case TokenType.AND:
            dataType=DataType.Bool;
            break;
            case TokenType.OR:
            dataType=DataType.Bool;
            break;
            case TokenType.ARROBA:
            dataType=DataType.String;
            break;
            case TokenType.ARROBA_ARROBA:
            dataType=DataType.String;
            break;
            case TokenType.EQUAL_EQUAL:
            dataType=DataType.Bool;
            break;
        }
    }
    public override DataType dataType{get;set;}
    public Expression Left { get; set; }
    public Expression Right { get; set; }
    public Token Operator;
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Binary(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Binary(this);
    }
}
public class AssignExpression : Expression
{
    public AssignExpression(CodeLocation location,Variable variable,Expression right) : base(location)
    {
        Right=right;
        this.variable=variable;
        dataType=right.dataType;
    }
    public Expression Right { get; set; }
    public Variable variable;
    public override DataType dataType{get;set;}
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Assign(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Assign(this);
    }
}
public class SetExpression:Expression
{
    public Expression value;
    public GetExpression getExpression;
    public override DataType dataType{get;set;}
    public SetExpression(CodeLocation location,GetExpression getExpression,Expression value) : base(location)
    {
        dataType=value.dataType;
        this.value=value;
        this.getExpression=getExpression;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Set(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Set(this);
    }
}
