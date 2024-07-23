using System;
using System.Collections.Generic;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud;
using Dalamud.Utility;

namespace AetherSenseReduxToo;

public class Service
{
    public static void Initialize(IDalamudPluginInterface pluginInterface)
        => pluginInterface.Create<Service>();

    public const string PluginName = "AetherSenseReduxToo";

    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static ICommandManager Commands { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;

    public static Configuration Configuration { get; set; } = null!;
    public static WindowSystem WindowSystem { get; } = new(PluginName);
    public static ClientLanguage Language { get; set; }

    public static string Status = @"-";

    public static void LogInfo(string message, params Object[] args)
    {
        if ((Configuration == null) || (Configuration.LogLevel >= 2)) 
        AddLog(string.Format("I: "+message, args));
        PluginLog.Info(message,args);
    }
    public static void LogDebug(string message, params Object[] args)
    {
        if ((Configuration == null) || (Configuration.LogLevel >= 3))
            AddLog(string.Format("D: " + message, args));
        PluginLog.Debug(message,args);
    }
    public static void LogError(string message, params Object[] args)
    {
        if ((Configuration == null) || (Configuration.LogLevel >= 1))
            AddLog(string.Format("E: " + message, args));
        PluginLog.Error(message, args);
    }
    public static void LogError(Exception ex,string message, params Object[] args)
    {
        if ((Configuration == null) || (Configuration.LogLevel >= 1))
            AddLog(string.Format("Ex: " + message + ex.Message, args));
        PluginLog.Error(ex,message, args);
    }
    public static void LogAlways(string message, params Object[] args)
    {
        AddLog(string.Format(message, args));
        PluginLog.Info(message, args);
    }


    public static void Save()
    {
        Configuration.Save();
    }

    private const int MaxLogSize = 50;
    public static Queue<string> LogMessages = new();
    public static bool OpenConsole; 
    public static void AddLog(string msg)
    {
        if (LogMessages.Count >= MaxLogSize)
        {
            LogMessages.Dequeue();
        }

        LogMessages.Enqueue(msg);
        PluginLog.Debug(msg);
    }

}