using System.Data;
using System.Reflection.Metadata.Ecma335;

public class Effect : Stmt//,Callable
{
    public List<Token> Params;
    public List <Stmt> body;
    public string Name { get; set; }
    public Effect(string id,List<Token> Params,List<Stmt>body,CodeLocation location):base(location)
    {
        this.Name=id;
        this.Params=Params;
        this.body=body;
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Effect(this);
    }
    public int ParamNumber()
    {
        return Params.Count;
    }
    /*public object Call(Interpreter interpreter,List<object> arguments)
    {
        Scope last=interpreter.Scope;
        interpreter.Scope=last.CreateChild();
        for(int i=0;i<arguments.Count;i++)
        {
            interpreter.Scope.Set(Action.Params[i].Value,arguments[i]);
        }
        interpreter.ExecuteBlock(Action.body);
        interpreter.Scope=last;
        return null;
    }*/
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_Effect(this);
    }
}



