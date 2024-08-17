
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public interface IVisitorExpression
{
    object Visit_Atom(Atom expression);
    object Visit_Binary(BinaryExpression expression);
    object Visit_Group(Group expression);
    object Visit_Unary(Unary expression);
    object Visit_Variable(Variable expression);
    object Visit_Assign(AssignExpression expression);
    
    object? Visit_Metod(MetodExpression declaration);
    object Visit_Get(GetExpression expression);
    object Visit_Set(SetExpression expression);
}
public interface IVisitorDeclaration
{
    void Visit_DeclarationExpression(ExpressionStmt declaration);
    void Visit_WhileDeclaration(WhileStmt declaration);
    void Visit_ForDeclaration(ForStmt declaration);
    void Visit_Card(Card expression);
    void Visit_Effect(Effect expression);
    void Visit_Program(ElementalProgram program);
    void Visit_DeclarationEmpty(EmptyStmt declaration);
    void Visit_CallEffect(CallEffect expression);
}
