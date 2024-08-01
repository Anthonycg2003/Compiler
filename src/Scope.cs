using System.Dynamic;


public class Scope
{
    public Scope()
    {
        Ranges = new List<string>();
        Variables=new Dictionary<string, object>()
        {
            {"context",new Context_class()}
        };
    }
    public Scope? Parent { get; set; }

    public List<string> Ranges { get; set; }
    public Dictionary<string,object> Variables{get;set;}

    public Scope CreateChild()
    {
        Scope child = new Scope();
        child.Parent = this;
            
        return child;
    }
    public Scope Ancestor(int distance)
    {
        Scope scope=this;
        for(int i=0;i<distance;i++)
        {
            scope=scope.Parent;
        }
        return scope;
    }
    public object GetAt(int distance,string name)
    {
        return Ancestor(distance).Variables[name];
    }
    public object GetValue(string name)
    {
        return Variables[name];
        throw new Exception("The variable "+name+" does not exist in the current context");
        //error en tiempo de ejecucion o excepcion variable no definida
    }
    public void Set(string name,object value)
    {
        Variables[name]=value;
    }
    public void SetAt(string name,object value,int distance)
    {
        Ancestor(distance).Set(name,value);
    }
}
