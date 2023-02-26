using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TerminalController : MonoBehaviour
{

    public GameObject terminalHandlerGameObject;
    private static ITerminalHandler _terminalHandler;
    
    private static bool _isMinimized;
    private static GameObject _terminal;
    private static TMP_Text _debuggingOutputText;
    private static TMP_InputField _inputField;
    
    void Start()
    {
        _terminalHandler = terminalHandlerGameObject.GetComponent<ITerminalHandler>();
        _terminal = GameObject.Find("Terminal Body");
        _debuggingOutputText = GameObject.Find("DebuggingText").GetComponent<TMP_Text>();
        _inputField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
        _terminalHandler.CalledOnTerminalStart();
    } 
    
    void Update()
    {
        StringBuilder outputText;
        _terminalHandler.ManageDebuggingText(out outputText);
        _debuggingOutputText.text = outputText.ToString();
        OnEnterGetInputFromInputField();
    }

    // handled by the terminal InputField event
    public bool IsInputFieldSelected { get; set; }
    public void OnEnterGetInputFromInputField() {
        if (IsInputFieldSelected && Input.GetKeyDown(KeyCode.Return)) {
            string input = _inputField.text;
            string[] splitInputs = input.Split(" ");
            _terminalHandler.TreatCommands(splitInputs);
            ClearInputField();
        }
    }
    
    // handled by the terminal minimize button event
    public static void MinimizeOrMaximizeTerminal()
    {
        if (_isMinimized) MaximizeWindow();
        else MinimizeWindow();
    }
    private static void MinimizeWindow()
    {
        _terminal.SetActive(false);
        _isMinimized = true;
    }
    private static void MaximizeWindow()
    {
        _terminal.SetActive(true);
        _isMinimized = false;
    }

    // Ultility
    public void ClearInputField()
    {
        _inputField.text = String.Empty;
    }
}
