using Godot;
using System;

namespace GodotUtilities;

public static class Log
{
    private static string Timestamp => DateTime.Now.ToString("HH:mm:ss");

    public static void Err(params object[] what)
    {
        GD.PrintErr(Format("ERR", what));
        GD.PrintErr(System.Environment.StackTrace);
    }

    public static void Warn(params object[] what)
    {
        GD.Print(Format("WARN", what));
        GD.Print("\n");
    }

    public static void Info(params object[] what)
    {
        GD.Print(Format("INFO", what));
        GD.Print("\n");
    }

    public static void Debug(params object[] what)
    {
        if (OS.IsDebugBuild())
        {
            GD.Print(Format("DBG", what));
            GD.PrintRaw("\n");
        }
    }

    private static string Format(string level, object[] what)
    {
        var message = string.Join(" ", what);
        return $"[{Timestamp}] [{level}] {message}";
    }
}

