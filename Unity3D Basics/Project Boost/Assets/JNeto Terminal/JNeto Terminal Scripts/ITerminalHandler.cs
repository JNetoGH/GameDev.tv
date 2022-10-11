using System.Text;

namespace JNeto_Terminal.JNeto_Terminal_Scripts
{
    public interface ITerminalHandler
    {
        public abstract void CalledOnTerminalStart();
        public abstract void TreatCommands(string[] commands);
        public abstract void ManageScrollViewDebuggingText(out StringBuilder text);
    }
}
