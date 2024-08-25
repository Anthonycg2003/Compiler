
using System.Data.SqlTypes;
using System.Linq.Expressions;

public class Atom:Expression
{
    public object Value;
    public override DataType dataType{get;set;}
    public Atom(object value, CodeLocation location) : base(location)
    {
        Value = value;
        Type type=value.GetType();
        if(type==typeof(bool))
        {
            dataType=DataType.Bool;
        }
        else if(type==typeof(double))
        {
            dataType=DataType.Number;
        }
        else if(type==typeof(string))
        {
            dataType=DataType.String;
        }
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
    public override DataType dataType{get;set;}
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
    public override DataType dataType{get;set;}
    public Unary(Expression expression,Token token, CodeLocation location) : base(location)
    {
        rigth_expression=expression;
        Operator=token;
        dataType=expression.dataType;
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
    public override DataType dataType{get;set;}
    public Variable(CodeLocation location,string name) : base(location)
    {
        this.name=name;
        dataType=DataType.Null;
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


