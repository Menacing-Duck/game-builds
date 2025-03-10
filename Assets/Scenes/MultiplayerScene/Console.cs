using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class Console : MonoBehaviour
{
    bool showConsole = false;
    string input = "";

    public static DebugCommand HOST;
    public static DebugCommand CLIENT;
    
    public static DebugCommand<string>JOIN;


    public List<object> commandList;

    private void Awake()
    {
        
        HOST = new DebugCommand("host", "hosts a server", "host", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            Debug.Log("Starting as Host...");
            NetworkManager.Singleton.StartHost();
            Debug.Log("server started");
        });

        
        CLIENT = new DebugCommand("client", "joins a server", "client", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            Debug.Log("Starting as Client on localhost ...");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1",7777);
            NetworkManager.Singleton.StartClient();
            Debug.Log("client started");
        });

        JOIN= new DebugCommand<string>("join","joins a server with the provided ip","join <ip_address>",(x)=>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;
            Debug.Log($"Joining server with ip {x} ....");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(x,7777);
            NetworkManager.Singleton.StartClient();
            Debug.Log("client started");
            
        });
        
        
        commandList = new List<object>
        {
            HOST,
            CLIENT,
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
        string[] properties = input.Split(" ");
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
                else if (commandList[i] as DebugCommand<string>!=null)
                {
                Debug.Log(properties[1]);
                    (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
                }
            }
        }
    }
}
