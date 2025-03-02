using System;
using UnityEngine;

public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string commandId {get {return _commandId;}}
    public string commandDescription {get {return _commandDescription;}}
    public string commandFormat {get {return _commandFormat;}}

    public DebugCommandBase(string id, string description, string format){
        this._commandId=id;
        this._commandDescription=description;
        this._commandFormat=format;

    }
    
}

public class DebugCommand : DebugCommandBase{
    private Action command;
    public DebugCommand(string id, string desc, string form, Action command) : base (id, desc, form){
        this.command=command;
    }
    public void Invoke(){
    command.Invoke();
    }
}
