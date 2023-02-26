using System.Text;
using UnityEngine;


public class TerminalImplementation: MonoBehaviour, ITerminalHandler
{
    private PlayerController _playerControllerScript;
    
    public void CalledOnTerminalStart()
    {
        _playerControllerScript = GameObject.Find("Player (Spongy)").GetComponent<PlayerController>();
    }

    public void TreatCommands(string[] commands)
    {
        if (commands.Length != 2) return;
        if (commands[0].ToLower().Equals("speed")) 
        {
            int newSpeed = 500;
            int.TryParse(commands[1], out newSpeed);
            _playerControllerScript.speed = newSpeed;
        }
        else if (commands[0].ToLower().Equals("cooldown_hit"))
        {
            if (commands[1].ToUpper() == "TRUE" || commands[1].ToUpper() == "FALSE")
            {
                bool outResult;
                bool.TryParse(commands[1], out outResult);
                _playerControllerScript.enableHitInWallCooldown = outResult;
            }
        }
        else if (commands[0].ToLower().Equals("total_hits"))
        {
            int newTotal = 0;
            int.TryParse(commands[1], out newTotal);
            PlayerController.TotHits = newTotal;
        }
    }

    public void ManageDebuggingText(out StringBuilder text)
    {
        text = new StringBuilder();
        text.AppendLine($"Timer: {Time.time}s");
        text.AppendLine(
            $"Player (x:{_playerControllerScript.transform.position.x:F2} " +
            $" y:{_playerControllerScript.transform.position.y:F2}" +
            $" z:{_playerControllerScript.transform.position.z:F2})");
        text.AppendLine($"Total Hits: {PlayerController.TotHits}");
        text.AppendLine($"Speed: {_playerControllerScript.speed}");
        text.AppendLine($"Are Hits in CoolDown Enable? {_playerControllerScript.enableHitInWallCooldown}");

    }
}
