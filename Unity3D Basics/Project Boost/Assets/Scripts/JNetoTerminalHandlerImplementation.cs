using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JNeto_Terminal.JNeto_Terminal_Scripts;
using UnityEngine;

public class JNetoTerminalHandlerImplementation : MonoBehaviour, ITerminalHandler
{
    private GameObject _rocket;
    private RocketMovement _rocketScript;
    private Rigidbody _rocketRigidbody;
    
    public List<GameObject> obstacles = new List<GameObject>();
    private bool _showObstaclesTerminalMsg = true;
    
    public void CalledOnTerminalStart()
    {
        _rocket = GameObject.Find("Rocket");
        _rocketScript = _rocket.GetComponent<RocketMovement>();
        _rocketRigidbody = _rocket.GetComponent<Rigidbody>();
    }

    public void TreatCommands(string[] commands)
    {
        switch (commands[0].ToLower())
        {
            case "obstacles":
            {
                if (commands[1].ToLower().Equals("on")) _showObstaclesTerminalMsg = true;
                else if (commands[1].ToLower().Equals("off")) _showObstaclesTerminalMsg = false;
                TurnObstaclesOnOff(_showObstaclesTerminalMsg);
                break;
            }
            case "gravity":    Physics.gravity = new Vector3(0, float.Parse(commands[1])); break;
            case "air":        _rocketRigidbody.drag = float.Parse(commands[1]); break;
            case "e_thrust":   _rocketScript.rocketEngineThrust = float.Parse(commands[1]); break;
            case "rot_thrust": _rocketScript.rotationThrust = float.Parse(commands[1]); break;
        }
    }

    public void ManageScrollViewDebuggingText(out StringBuilder text)
    {
        Vector3 rocketVel = _rocketRigidbody.velocity;
        Vector3 rocketPos = _rocket.transform.position;
        Vector3 rocketRot = _rocket.transform.rotation.eulerAngles;
        Vector3 rocketMovDir = _rocketScript.MoveDirection;

        text = new StringBuilder();
        text.AppendLine($"FPS: {(int)Math.Ceiling(FPS_Counter.fps)}");
        text.AppendLine(" ");
        text.AppendLine("ROCKET");
        text.AppendLine($"velocity ( x: {rocketVel.x:F2} | y: {rocketVel.y:F2} | z: {rocketVel.z:F2} )");
        text.AppendLine($"position ( x: {rocketPos.x:F2} | y: {rocketPos.y:F2} | z: {rocketPos.z:F2} )");
        text.AppendLine($"rotation ( x: {rocketRot.x:F2} | y: {rocketRot.y:F2} | z: {rocketRot.z:F2} )");
        text.AppendLine($"move dir ( x: {rocketMovDir.x:F2} | y: {rocketMovDir.y:F2} | z: {rocketMovDir.z:F2} )");
        text.AppendLine($"engine thrust: {_rocketScript.rocketEngineThrust}");
        text.AppendLine($"rotation thrust: {_rocketScript.rotationThrust}");
        text.AppendLine(" ");
        text.AppendLine("WORLD");
        text.AppendLine($"gravity ( x: {Physics.gravity.x} | y: {Physics.gravity.y} | z: {Physics.gravity.z} )");
        text.AppendLine($"air resistance: {_rocketRigidbody.drag}");
        text.AppendLine($"obstacles: {_showObstaclesTerminalMsg}");
    }

    private void TurnObstaclesOnOff(bool on_off)
    {
        foreach (GameObject obstacle in obstacles)
            obstacle.SetActive(on_off);
    }
    
    
    
}
