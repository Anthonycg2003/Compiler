using System.Collections;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
public interface Funtion
{
    object? Call(object target,List<object>arguments);
}
public class DeckOf : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Player player=(Player)arguments[0];
        return Context_class.DeckOf(player);
    }
}
public class HandOf : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Player player=(Player)arguments[0];
        return Context_class.HandOf(player);
    }
}
public class Push : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Card card=(Card)arguments[0];
        ((PackOfCards)target).Push(card);
        return null;
    }
}
public class SendBottom : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Card card=(Card)arguments[0];
        ((PackOfCards)target).SendBottom(card);
        return null;
    }
}
public class Pop : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        return ((PackOfCards)target).Pop();
        
    }
}
public class Remove : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Card card=(Card)arguments[0];
        ((PackOfCards)target).Remove(card);
        return null;
    }
}
public class Shuffle : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        ((PackOfCards)target).Shuffle();
        return null;
    }
}
public class Add : Funtion
{
    public object? Call(object target,List<object> arguments)
    {
        Card card=(Card)arguments[0];
        ((PackOfCards)target).AddCard(card);
        return null;
    }
}

public interface GwentClass
{
    public string name { get; }
    public Dictionary<string, object?> Properties { get; set; }
    public Dictionary<string, Funtion> Metods { get; set; }
    public DataType dataType{get;}
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
            {"Deck",DeckOfPlayer},
            {"Hand",HandOfPlayer},
            {"Board",Board}
        };
        Metods = new Dictionary<string, Funtion>
        {
            {"HandOf",new HandOf()},{"DeckOfPlayer",new DeckOf()}
        };
    }
    public string name { get { return "context"; } }
    public DataType dataType{get{return DataType.Context;}}
    public static PackOfCards Board = new PackOfCards();
    public static Player TriggerPlayer = Player.none;
    public static PackOfCards HandOfPlayer = new PackOfCards();
    public static PackOfCards HandOfOpponent = new PackOfCards();
    public static PackOfCards DeckOfPlayer = new PackOfCards();
    public static PackOfCards DeckOfOpponent = new PackOfCards();
    public Dictionary<string, Funtion> Metods { get; set; }
    public Dictionary<string, object?> Properties { get; set; }
    public static PackOfCards HandOf(Player player)
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
    public static PackOfCards DeckOf(Player player)
    {
        if (player == Player.player)
        {
            return DeckOfPlayer;
        }
        else
        {
            return DeckOfOpponent;
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
            {"Push",new Push()},{"SendBottom",new SendBottom()}
            ,{"Pop",new Pop()},{"Remove",new Remove()}
            ,{"Shuffle",new Shuffle()},{"Add",new Add()},
        };
    }
    public PackOfCards(CallEffect callEffect,Interpreter interpreter)
    {
        Properties = new Dictionary<string, object?>
        {
        };
        Metods = new Dictionary<string, Funtion>
        {
            {"Push",new Push()},{"SendBottom",new SendBottom()}
            ,{"Pop",new Pop()},{"Remove",new Remove()}
            ,{"Shuffle",new Shuffle()},{"Add",new Add()},
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
    public DataType dataType{get{return DataType.PackOfCards;}}
    public string name { get { return "pack"; } }
    public Dictionary<string, Funtion> Metods { get; set; }
    public Dictionary<string, object?> Properties { get; set; }
    public void Push(Card card)
    {
        cards.Push(card);
    }
    public void SendBottom(Card card)
    {
        cards.Append(card);
    }
    public Card Pop()
    {
        return cards.Pop();
    }
    public void Remove(Card card)
    {
        cards.ToList().Remove(card);
    }
    public void Shuffle()
    {
        Random random = new Random();
        cards.OrderBy(x => random.Next());
    }
   public  void AddCard(Card card)
    {
        cards.Push(card);
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
public enum DataType
{
    Number,Bool,String,PackOfCards,Context,Card,Null,Player
}
public class GetExpression : Expression
{
    public Token name;
    public Expression callee;
    public override DataType dataType{get;set;}
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
    public override DataType dataType{get;set;}
    public MetodExpression(CodeLocation location, Token name, List<Expression> arguments, Expression calleer) : base(location)
    {
        callee = calleer;
        this.arguments = arguments;
        this.name = name;
        dataType=DataType.Null;
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
public class Metod
{
    public DataType []Params;
    public DataType ReturnType;
    public int Arity;
    public Metod(int Arity,DataType ReturnType,params DataType []ParamsDatatype)
    {
        this.Arity=Arity;
        this.ReturnType=ReturnType;
        Params=ParamsDatatype;
    }
}