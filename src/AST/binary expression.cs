﻿
public class BinaryExpression : Expression
{
    public BinaryExpression(CodeLocation location,Expression left,Token Operator,Expression right) : base(location)
    {
        Left=left;
        Right=right;
        this.Operator=Operator;
    }
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
    }
    public Expression Right { get; set; }
    public Variable variable;
    
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
    public SetExpression(CodeLocation location,GetExpression getExpression,Expression value) : base(location)
    {
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
