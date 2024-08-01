
using System.Data.SqlTypes;
using System.Linq.Expressions;

public class Atom:Expression
{
    public object Value;
    public Atom(object value, CodeLocation location) : base(location)
    {
        Value = value;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Atom(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Atom(this);
    }
}
public class Group:Expression
{
    public Expression expression;
    public Group(Expression expression, CodeLocation location) : base(location)
    {
        this.expression=expression;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Group(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Group(this);
    }
}
public class Unary:Expression
{
    public Expression rigth_expression;
    public Token Operator;
    public Unary(Expression expression,Token token, CodeLocation location) : base(location)
    {
        rigth_expression=expression;
        Operator=token;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Unary(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Unary(this);
    }
}
public class Variable:Expression
{
    public string name;
    public Variable(CodeLocation location,string name) : base(location)
    {
        this.name=name;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Variable(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Variable(this);
    }
}
public enum TypeDef
{
    Number,String,Bool,Implicit
}

