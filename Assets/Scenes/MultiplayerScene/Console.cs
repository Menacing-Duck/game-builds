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
    public static DebugCommand SET; 

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

        
        SET = new DebugCommand("set", 
            "Sets a public value. Usage: set <GameObjectName> <ComponentName> <FieldName> <Value>", 
            "set <GameObjectName> <ComponentName> <FieldName> <Value>", 
            () =>
        {
            
            string[] tokens = input.Split(' ');
            if (tokens.Length < 5)
            {
                Debug.Log("Usage: set <GameObjectName> <ComponentName> <FieldName> <Value>");
                return;
            }

        
            string goName = tokens[1];
            string compName = tokens[2];
            string fieldName = tokens[3];
            
            string newValueStr = string.Join(" ", tokens.Skip(4).ToArray());

            
            GameObject go = GameObject.Find(goName);
            if (go == null)
            {
                Debug.Log($"GameObject '{goName}' not found.");
                return;
            }

            
            var componentType = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(assembly => assembly.GetTypes())
                                   .FirstOrDefault(type => type.Name == compName);
            if (componentType == null)
            {
                Debug.Log($"Component type '{compName}' not found.");
                return;
            }

            
            Component comp = go.GetComponent(componentType);
            if (comp == null)
            {
                Debug.Log($"Component '{compName}' not found on GameObject '{goName}'.");
                return;
            }

            
            var type = comp.GetType();
            var field = type.GetField(fieldName);
            if (field != null)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(newValueStr, field.FieldType);
                    field.SetValue(comp, convertedValue);
                    Debug.Log($"Field '{fieldName}' set to '{newValueStr}'.");
                }
                catch (Exception e)
                {
                    Debug.Log($"Error setting field: {e.Message}");
                }
                return;
            }

            
            var property = type.GetProperty(fieldName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(newValueStr, property.PropertyType);
                    property.SetValue(comp, convertedValue);
                    Debug.Log($"Property '{fieldName}' set to '{newValueStr}'.");
                }
                catch (Exception e)
                {
                    Debug.Log($"Error setting property: {e.Message}");
                }
                return;
            }

            Debug.Log($"No public field or writable property named '{fieldName}' found in component '{compName}'.");
        });

        
        commandList = new List<object>
        {
            HOST,
            JOIN,
            SET
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
