using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class Console : MonoBehaviour
{
    bool showConsole = false;
    string input = "";

    public static DebugCommand HOST;
    public static DebugCommand JOIN;


    public List<object> commandList;

    private void Awake()
    {
        
        HOST = new DebugCommand("host", "hosts a server", "host", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            Debug.Log("Starting as Host...");
            NetworkManager.Singleton.StartHost();
        });

        
        JOIN = new DebugCommand("join", "joins a server", "join", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            Debug.Log("Starting as Client...");
            NetworkManager.Singleton.StartClient();
        });

        
        
        commandList = new List<object>
        {
            HOST,
            JOIN
        };
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            showConsole = !showConsole;
        }
    }

    private void OnGUI()
    {
        
        if (!showConsole)
            return;

        float y = 0f;
        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        
        GUI.SetNextControlName("ConsoleInput");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);

        
        if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() != "ConsoleInput")
        {
            GUI.FocusControl("ConsoleInput");
        }

        
        Event e = Event.current;
        if (e.type == EventType.KeyDown &&
            (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.character == '\n'))
        {
            HandleInput();

            
            input = "";
            showConsole = false;

            
            GUI.FocusControl(null);

            
            Input.ResetInputAxes();

            
            e.Use();
        }
    }

    private void HandleInput()
    {
        
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if (commandBase != null && input.Contains(commandBase.commandId))
            {
                DebugCommand command = commandList[i] as DebugCommand;
                if (command != null)
                {
                    command.Invoke();
                }
            }
        }
    }
}
