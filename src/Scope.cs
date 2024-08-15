using System.Dynamic;


public class Scope
{
    public Scope()
    {
        Ranges = new List<string>();
        Variables=new Dictionary<string, object>();
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
    /*public Scope Ancestor(int distance)
    {
        Scope scope=this;
        for(int i=0;i<distance;i++)
        {
            scope=scope.Parent;
        }
        return scope;
    }*/
    public object? GetValue(string name)
    {
        try
        {
            return Variables[name];
        }
        catch
        {
            if(Parent!=null)
            {
                return Parent.GetValue(name);
            }
            else
            {
                return null;
            }
        }
    }
    public void Set(string name,object value)
    {
        Variables[name]=value;
    }
}
