using System.Data;
using System.Reflection.Metadata.Ecma335;

public abstract class ASTNode
{
    public ASTNode(CodeLocation location)
    {
        Location = location;
    }

    public CodeLocation Location {get; set;}
    public abstract void CheckSemantic(SemanticAnalizer semanticAnalizer);
    
}
public abstract class Expression:ASTNode
{
    public Expression(CodeLocation location) : base (location) { }
    public abstract object Accept(IVisitorExpression visitor);
    public abstract DataType dataType{get;set;}
}
public abstract class Stmt:ASTNode
{
    public abstract void Accept(IVisitorDeclaration visitor);
    public Stmt(CodeLocation location) : base (location){}
}

