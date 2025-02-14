using System;
using System.Runtime.InteropServices;
using SR2E.Menus;

namespace SR2E.Managers;

public static class SR2ELogManager
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    static MelonLogger.Instance mlog;
    const string logName = "SR2E-Console";
    public static void Start()
    {
        mlog = new MelonLogger.Instance(logName);
        MelonLogger.MsgDrawingCallbackHandler += (c1, c2, s1, s2) => { if (SR2EEntryPoint.mLLogToSR2ELog) SendMessage($"[{s1}]: {s2}", false); };
        MelonLogger.ErrorCallbackHandler += (s, s1) => { if (SR2EEntryPoint.mLLogToSR2ELog) SendError($"[{s}]: {s1}", false); };
        MelonLogger.WarningCallbackHandler += (s, s1) => { if (SR2EEntryPoint.mLLogToSR2ELog) SendWarning($"[{s}]: {s}", false); };
    }
    /// <summary>
    /// Display a message in the console
    /// </summary>
    public static void SendMessage(string message)
    {
        SendMessage(message, SR2EEntryPoint.SR2ELogToMLLog);
    }

    /// <summary>
    /// Display a message in the log
    /// </summary>
    
    
    public static void SendMessage(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (message.StartsWith($"[{logName}]")) return;
        if (message.StartsWith("[UnityExplorer]")) return;
        if (message.StartsWith("[]:")) return;
        if (doMLLog&&(message.Contains("\n") || internal_logMLForSingleLine)) mlog.Msg(message);
        SR2EConsole console = GM<SR2EConsole>();
        if(console!=null) console.SendMessage(message);
    }
    
    
    /// <summary>
    /// Display an error in the log
    /// </summary>
    public static void SendError(string message)
    {
        SendError(message, SR2EEntryPoint.SR2ELogToMLLog);
    }

    /// <summary>
    /// Display an error in the log
    /// </summary>
    public static void SendError(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (message.StartsWith($"[{logName}]")) return;
        if (message.StartsWith("[UnityExplorer]")) return;
        if (message.StartsWith("[]:")) return;
        if (doMLLog&&(message.Contains("\n") || internal_logMLForSingleLine)) mlog.Error(message);
        SR2EConsole console = GM<SR2EConsole>();
        if(console!=null) console.SendError(message);
    }

    /// <summary>
    /// Display an error in the log
    /// </summary>
    public static void SendWarning(string message)
    {
        SendWarning(message, SR2EEntryPoint.SR2ELogToMLLog);
    }

    /// <summary>
    /// Display an error in the log
    /// </summary>
    public static void SendWarning(string message, bool doMLLog, bool internal_logMLForSingleLine = true)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (message.StartsWith($"[{logName}]")) return;
        if (message.StartsWith("[UnityExplorer]")) return;
        if (message.StartsWith("[]:")) return;
        if (doMLLog&&(message.Contains("\n") || internal_logMLForSingleLine)) mlog.Warning(message);
        SR2EConsole console = GM<SR2EConsole>();
        if(console!=null) console.SendWarning(message);
    }
    /// <summary>
    /// Display a message
    /// </summary>
    public static void Alert(string message)
    {
        Alert("Alert!", message,SR2EEntryPoint.SR2ELogToMLLog);
    }
    /// <summary>
    /// Display a message
    /// </summary>
    public static void Alert(string title, string message)
    {
        Alert(title,message, SR2EEntryPoint.SR2ELogToMLLog);
    }
    /// <summary>
    /// Display a message
    /// </summary>
    public static void Alert(string title, string message, bool doMLLog, bool internal_logMLForSingleLine = true)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (message.StartsWith($"[{logName}]")) return;
        if (message.StartsWith("[UnityExplorer]")) return;
        if (message.StartsWith("[]:")) return;
        if (doMLLog&&(message.Contains("\n") || internal_logMLForSingleLine)) mlog.Msg(message);
        MessageBox(IntPtr.Zero, message, title, 0);
    }

}