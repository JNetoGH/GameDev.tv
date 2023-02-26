
using System.Text;

public interface ITerminalHandler
{
    public abstract void CalledOnTerminalStart();
    public abstract void TreatCommands(string[] commands);
    public abstract void ManageDebuggingText(out StringBuilder text);
}
