using ConsolePresentation.Entities.Commands;

namespace ConsolePresentation.Entities.CommandChainLinks;

public abstract class CommandChainLinkBase
{
    protected CommandChainLinkBase? Next { get; private set; }
    public void AddNext(CommandChainLinkBase next)
    {
        if (Next is null)
        {
            Next = next;
            return;
        }
        
        Next.AddNext(next);
    }
    
    public abstract ICommand Handle(string[] args);
}