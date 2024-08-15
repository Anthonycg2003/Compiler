
using System.Globalization;
using System.Security.Cryptography;

public class ExpressionStmt:Stmt
{
    public Expression expression;
    public ExpressionStmt(Expression expression, CodeLocation location) : base(location)
    {
        this.expression=expression;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_DeclarationExpression(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_DeclarationExpression(this);
    }
}
public class WhileStmt:Stmt
{
    public Expression condition;
    public List <Stmt> body;
    public WhileStmt(Expression expression,List<Stmt> body, CodeLocation location) : base(location)
    {
        this.body=body;
        condition=expression;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_WhileDeclaration(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_WhileDeclaration(this);
    }
}
public class EmptyStmt:Stmt//;
{
    public EmptyStmt(CodeLocation location) : base(location)
    {
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_DeclarationEmpty(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_DeclarationEmpty(this);
    }
}
public class PredicateStmt:Stmt//;
{
    public Expression condition;
    public ParamsPredicate paramsPredicate;

    public PredicateStmt(CodeLocation location,Expression expression,ParamsPredicate paramsPredicate) : base(location)
    {
        condition=expression;
        this.paramsPredicate=paramsPredicate;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
    }
}
public enum ParamsPredicate
{
    unit,card,boost,weather
}