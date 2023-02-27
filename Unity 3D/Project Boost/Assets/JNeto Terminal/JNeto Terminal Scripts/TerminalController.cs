using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JNeto_Terminal.JNeto_Terminal_Scripts
{
    public sealed class TerminalController : MonoBehaviour
    {

        public GameObject terminalHandlerGameObject;
        private static ITerminalHandler _terminalHandler;
    
        private static bool _isMinimized;
        private static GameObject _terminal;
        private static TMP_Text _debuggingOutputText;
        private static TMP_InputField _inputField;
    
        private static GameObject _enterButton;
        private static GameObject _minimizeButton;

        private void Start()
        {
            _terminalHandler = terminalHandlerGameObject.GetComponent<ITerminalHandler>();
            _terminal = GameObject.Find("Terminal Body");
            _debuggingOutputText = GameObject.Find("DebuggingText").GetComponent<TMP_Text>();
            _inputField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
        
            // buttons
            _enterButton = GameObject.Find("EnterButton");
            _minimizeButton = GameObject.Find("MinimizeButton");
            
            // by default starts always with the terminal minimized
            MinimizeWindow();
            
            // method defined in ITerminalHandler
            _terminalHandler.CalledOnTerminalStart();
        } 
    
        private void Update()
        {
            StringBuilder outputText;
            _terminalHandler.ManageScrollViewDebuggingText(out outputText);
            _debuggingOutputText.text = outputText.ToString();
            OnEnterGetInputFromInputField();
        }

        // set false at Update() = >MinimizeOrMaximizeTerminal()
        // solves the bug with the selected state on the Unity Buttons
        private void LateUpdate()
        {
            _minimizeButton.GetComponent<Button>().interactable = true;
            _enterButton.GetComponent<Button>().interactable = true;
        } 
    
        // handled by the EnterButton Input Trigger Component
        public bool IsEnterButtonPressed { private get; set; }
        public void OnEnterGetInputFromInputField() {
        
            // solves the bug with the selected state on the Unity Buttons
            // will be set true on Late Update
            _enterButton.GetComponent<Button>().interactable = false;
        
            // the actual button programming
            if (Input.GetKeyDown(KeyCode.Return) || IsEnterButtonPressed) {
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
        
            // solves the bug with the selected state on the Unity Buttons
            // will be set true on Late Update
            _minimizeButton.GetComponent<Button>().interactable = false;
        }
        private static void MinimizeWindow()
        {
            _terminal.SetActive(false);
            _enterButton.SetActive(false);
            _isMinimized = true;
        }
        private static void MaximizeWindow()
        {
            _terminal.SetActive(true);
            _enterButton.SetActive(true);
            _isMinimized = false;
        }

        // Ultility
        public static void ClearInputField()
        {
            _inputField.text = String.Empty;
        }
    }
}
