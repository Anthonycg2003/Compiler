using System.Data;
using System.Reflection.Metadata.Ecma335;


public class ElementalProgram:Stmt
{
    public ElementalProgram(CodeLocation location) : base (location)
    {
        Errors = new List<CompilingError>();
        Cards = new Dictionary<string, Card>();
        Effects=new Dictionary<string, Effect>();
    }

    public List<CompilingError> Errors { get; set; }
    public Dictionary<string, Card> Cards { get; set; }
    public Dictionary<string, Effect> Effects { get; set; }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_Program(this);
    }
    public override string ToString()
    {
        string s = "";
        foreach (Effect effect in Effects.Values)
        {
            s = s + "\n" + effect.ToString();
        }
        foreach (Card card in Cards.Values)
        {
            s += "\n" + card.ToString();
        }
        return s;
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Program(this);
    }
}
