using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class Console : MonoBehaviour
{
    bool showDebugInfo = false;
    bool showConsole = false;
    string input = "";

    public GameObject spawnPrefab;

    public static DebugCommand HOST;
    public static DebugCommand CLIENT;
    public static DebugCommand DISCONNECT;
    public static DebugCommand<string> JOIN;
    public static DebugCommand SHOW_DEBUG;
    public static DebugCommand SPAWN;
    public static DebugCommand<string, string> EDIT;

    public List<object> commandList;

    void Awake()
    {
        HOST = new DebugCommand("host", "hosts a server", "host", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) return;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", 7777);
            NetworkManager.Singleton.StartHost();
        });

        CLIENT = new DebugCommand("client", "joins a server", "client", () =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) return;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
            NetworkManager.Singleton.StartClient();
        });

        JOIN = new DebugCommand<string>("join", "joins with ip", "join <ip>", x =>
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) return;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(x, 7777);
            NetworkManager.Singleton.StartClient();
        });

        DISCONNECT = new DebugCommand("disconnect", "disconnect", "disconnect", () =>
        {
            NetworkManager.Singleton.Shutdown();
        });

        SHOW_DEBUG = new DebugCommand("show_debug", "toggle debug", "show_debug", () =>
        {
            showDebugInfo = !showDebugInfo;

        });

        SPAWN = new DebugCommand("spawn", "spawn prefab", "spawn", () =>
        {
            if (spawnPrefab == null) return;
            Vector3 pos = Camera.main ? Camera.main.transform.position + Vector3.forward * 5 : Vector3.zero;
            Instantiate(spawnPrefab, pos, Quaternion.identity);
        });

        EDIT = new DebugCommand<string, string>("edit", "edit hovered value", "edit <field> <value>", (field, val) =>
{
    Vector2 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Collider2D hit = Physics2D.OverlapPoint(m);
    if (hit == null) return;

    GameObject tgt = hit.gameObject;

    object ConvertVal(string s, Type tp)
    {
        if (tp.IsEnum) return Enum.Parse(tp, s, true);
        return Convert.ChangeType(s, tp);
    }

    foreach (Component c in tgt.GetComponents<Component>())
    {
        Type ct = c.GetType();

        FieldInfo fi = ct.GetField(field, BindingFlags.Public | BindingFlags.Instance);
        if (fi != null)
        {
            if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(NetworkVariable<>))
            {
                object nv = fi.GetValue(c);
                Type inner = fi.FieldType.GetGenericArguments()[0];
                object v = ConvertVal(val, inner);
                fi.FieldType.GetProperty("Value").SetValue(nv, v);
            }
            else
            {
                object v = ConvertVal(val, fi.FieldType);
                fi.SetValue(c, v);
            }
            return;
        }

        PropertyInfo pi = ct.GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
        if (pi != null && pi.CanWrite)
        {
            if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(NetworkVariable<>))
            {
                object nv = pi.GetValue(c);
                Type inner = pi.PropertyType.GetGenericArguments()[0];
                object v = ConvertVal(val, inner);
                pi.PropertyType.GetProperty("Value").SetValue(nv, v);
            }
            else
            {
                object v = ConvertVal(val, pi.PropertyType);
                pi.SetValue(c, v);
            }
            return;
        }
    }
});


        commandList = new List<object> { HOST, CLIENT, JOIN, DISCONNECT, SHOW_DEBUG, SPAWN, EDIT };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) showConsole = !showConsole;

        
        
            foreach (GameObject go in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                go.SendMessage(showDebugInfo ? "ShowDebugInfo" : "HideDebugInfo", SendMessageOptions.DontRequireReceiver);
            }
        

        
    }

    void OnGUI()
    {
        if (!showConsole) return;

        GUI.Box(new Rect(0, 0, Screen.width, 30), "");
        GUI.SetNextControlName("ConsoleInput");
        input = GUI.TextField(new Rect(10, 5, Screen.width - 20, 20), input);

        if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() != "ConsoleInput")
            GUI.FocusControl("ConsoleInput");

        Event e = Event.current;
        if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.character == '\n'))
        {
            HandleInput();
            input = "";
            showConsole = false;
            GUI.FocusControl(null);
            Input.ResetInputAxes();
            e.Use();
        }
    }

    void HandleInput()
    {
        string[] props = input.Split(" ");
        foreach (object obj in commandList)
        {
            DebugCommandBase baseCmd = obj as DebugCommandBase;
            if (baseCmd == null || props.Length == 0) continue;
            if (baseCmd.commandId != props[0]) continue;

            if (obj is DebugCommand cmd) cmd.Invoke();
            else if (obj is DebugCommand<string> cmd1 && props.Length > 1) cmd1.Invoke(props[1]);
            else if (obj is DebugCommand<string, string> cmd2 && props.Length > 2) cmd2.Invoke(props[1], props[2]);
        }
    }
}
