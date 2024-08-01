using System.Collections;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
public interface GwentClass
{
    public string name{get;}
    public Dictionary<string,object?>Properties{get;set;}
    public Dictionary<string,Funtion>Metods{get;set;}
    public object? Get(string property)
    {
        if(Properties.ContainsKey(property))
        {
            return Properties[property];
        }
        throw new Exception("property "+property+ " in "+name);
    }
    public Funtion FindMetod(string metod)
    {
        if(Metods.ContainsKey(metod))
        {
            return Metods[metod];
        }
        throw new Exception("metod "+metod+ " in "+name);
    }
}
public class Context_class:GwentClass
{
    public Context_class()
    {
        Properties=new Dictionary<string, object?>
        {
            {"TriggerPlayer",TriggerPlayer},
            {"Board",Board}
        };
        Metods=new Dictionary<string, Funtion>
        {
            {"HandOf",new Funtion(1,HandOf,"HandOf")}
        };
    }
    public string name{get{return "context";}}
    public static PackOfCards Board=new PackOfCards();
    public static Player TriggerPlayer=Player.none;
    public static PackOfCards HandOfPlayer=new PackOfCards();
    public static PackOfCards HandOfOpponent=new PackOfCards();
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}
    static PackOfCards HandOf(Player player)
    {
        if(player==Player.player)
        {
            return HandOfPlayer;
        }
        else
        {
            return HandOfOpponent;
        }
    }
    static void Push(Card card)
    {

    }
}
public class PackOfCards:GwentClass,IEnumerable
{
    public PackOfCards()
    {
        Properties=new Dictionary<string, object?>
        {
        };
        Metods=new Dictionary<string, Funtion>
        {
            {"Push",new Funtion(1,Push,"Push")},{"SendBottom",new Funtion(1,SendBottom,"SendBottom")}
            ,{"Pop",new Funtion(0,Pop,"Pop")},{"Remove",new Funtion(1,Remove,"Remove")}
            ,{"Shuffle",new Funtion(0,Shuffle,"Shuffle")}
        };
    }
    Stack<Card> cards=new Stack<Card>();
    public string name{get{return "pack";}}
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}
    void Push(Card card)
    {
        cards.Push(card);
    }
    void SendBottom(Card card)
    {
        cards.Append(card);
    }
    Card Pop()
    {
        return cards.Pop();
    }
    void Remove(Card card)
    {
        cards.ToList().Remove(card);
    }
    void Shuffle()
    {
        Random random=new Random();
        cards.OrderBy(x=>random.Next());
    }

    public IEnumerator GetEnumerator()
    {
        return cards.GetEnumerator();
    }
}
public enum Player
{
    none,player,opponent
}
public class Funtion
{
    public string name;
    public int Arity;
    public Delegate Delegate;
    public Funtion(int n,Delegate Delegate,string name)
    {
        this.name=name;
        Arity=n;
        this.Delegate=Delegate;
    }
}
public class GetExpression:Expression
{
    public Token name;
    public Expression callee;
    public GetExpression(CodeLocation location,Token get,Expression callee) : base(location)
    {
        this.callee=callee;
        name=get;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Get(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Get(this);
    }
}
public class MetodExpression:Expression//llamada a funciones
{
    public Expression calleer;
    public List<Expression> arguments;
    public Token name;
    public MetodExpression(CodeLocation location,Token name,List<Expression>arguments,Expression calleer) : base(location)
    {
        this.calleer=calleer;
        this.arguments=arguments;
        this.name=name;
    }
    public override object Accept(IVisitorExpression visitor)
    {
        return visitor.Visit_Metod(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Metod(this);
    }
}
