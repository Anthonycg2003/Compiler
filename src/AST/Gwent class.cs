using System.Collections;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
public interface GwentClass
{
    public string name { get; }
    public Dictionary<string, object?> Properties { get; set; }
    public Dictionary<string, Funtion> Metods { get; set; }
    public object? Get(string property)
    {
        try
        {
            return Properties[property];
        }
        catch
        {
            return null;
        }
    }
    public Funtion? FindMetod(string metod)
    {
        try
        {
            return Metods[metod];
        }
        catch
        {
            return null;
        }
    }
}
public class Context_class : GwentClass
{
    public Context_class()
    {
        Properties = new Dictionary<string, object?>
        {
            {"TriggerPlayer",TriggerPlayer},
            {"Board",Board}
        };
        Metods = new Dictionary<string, Funtion>
        {
            {"HandOf",new Funtion(1,HandOf,"HandOf")}
        };
    }
    public string name { get { return "context"; } }
    public static PackOfCards Board = new PackOfCards();
    public static Player TriggerPlayer = Player.none;
    public static PackOfCards HandOfPlayer = new PackOfCards();
    public static PackOfCards HandOfOpponent = new PackOfCards();
    public Dictionary<string, Funtion> Metods { get; set; }
    public Dictionary<string, object?> Properties { get; set; }
    static PackOfCards HandOf(Player player)
    {
        if (player == Player.player)
        {
            return HandOfPlayer;
        }
        else
        {
            return HandOfOpponent;
        }
    }
}
public class PackOfCards : GwentClass, IEnumerable
{
    public PackOfCards()
    {
        Properties = new Dictionary<string, object?>
        {
        };
        Metods = new Dictionary<string, Funtion>
        {
            {"Push",new Funtion(1,Push,"Push")},{"SendBottom",new Funtion(1,SendBottom,"SendBottom")}
            ,{"Pop",new Funtion(0,Pop,"Pop")},{"Remove",new Funtion(1,Remove,"Remove")}
            ,{"Shuffle",new Funtion(0,Shuffle,"Shuffle")}
        };
    }
    public PackOfCards(CallEffect callEffect,Interpreter interpreter)
    {
        Properties = new Dictionary<string, object?>
        {
        };
        Metods = new Dictionary<string, Funtion>
        {
            {"Push",new Funtion(1,Push,"Push")},{"SendBottom",new Funtion(1,SendBottom,"SendBottom")}
            ,{"Pop",new Funtion(0,Pop,"Pop")},{"Remove",new Funtion(1,Remove,"Remove")}
            ,{"Shuffle",new Funtion(0,Shuffle,"Shuffle")}
        };
        Selector selector = callEffect.Selector;
        List<Card> source=new List<Card>();
        string variable_name="";
        switch (selector.Source)
        {
            case SourceType.board:
                foreach (Card card in Context_class.Board.cards)
                {
                    source.Add(card);
                }
                break;
            case SourceType.hand:

                break;
            case SourceType.otherHand:
                break;
        }
        switch (selector.predicateStmt.paramsPredicate)
        {
            case ParamsPredicate.card:
                variable_name="card";
                break;
            case ParamsPredicate.unit:
                variable_name="unit";
                foreach(Card card in source)
                {
                    if(card.Type.Value!="\"Oro\""&&card.Type.Value!="\"Plata\"")
                    {
                        source.Remove(card);
                    }
                }
                break;
            case ParamsPredicate.boost:
                variable_name="boost";
                foreach(Card card in source)
                {
                    if(card.Type.Value!="\"Aumento\"")
                    {
                        source.Remove(card);
                    }
                }
                break;
            case ParamsPredicate.weather:
                variable_name="weather";
                foreach(Card card in source)
                {
                    if(card.Type.Value!="\"Clima\"")
                    {
                        source.Remove(card);
                    }
                }
                break;
        }
        foreach(Card card in source)
        {
            interpreter.Define(variable_name,card);
            if((bool)interpreter.Evaluate(selector.predicateStmt.condition))
            {
                cards.Push(card);
            }
        }
    }
    Stack<Card> cards = new Stack<Card>();
    public string name { get { return "pack"; } }
    public Dictionary<string, Funtion> Metods { get; set; }
    public Dictionary<string, object?> Properties { get; set; }
    public void Push(Card card)
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
        Random random = new Random();
        cards.OrderBy(x => random.Next());
    }

    public IEnumerator GetEnumerator()
    {
        return cards.GetEnumerator();
    }
}
public enum Player
{
    none, player, opponent
}
public class Funtion
{
    public string name;
    public int Arity;
    public Delegate Delegate;
    public Funtion(int n, Delegate Delegate, string name)
    {
        this.name = name;
        Arity = n;
        this.Delegate = Delegate;
    }
}
public class GetExpression : Expression
{
    public Token name;
    public Expression callee;
    public GetExpression(CodeLocation location, Token get, Expression callee) : base(location)
    {
        this.callee = callee;
        name = get;
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
public class MetodExpression : Expression//llamada a funciones
{
    public Expression callee;
    public List<Expression> arguments;
    public Token name;
    public MetodExpression(CodeLocation location, Token name, List<Expression> arguments, Expression calleer) : base(location)
    {
        callee = calleer;
        this.arguments = arguments;
        this.name = name;
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
