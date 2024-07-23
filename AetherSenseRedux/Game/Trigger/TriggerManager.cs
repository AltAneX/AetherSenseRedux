using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace AetherSenseReduxToo.Game.Trigger;

public class TriggerManager
{
    private static void LogInfo(string message, params Object?[] args)
    {
        Service.LogInfo("[ASRToo Trigger Manager] " + String.Format(message, args));
    }
    private static void LogDebug(string message, params Object?[] args)
    {
        Service.LogDebug("[ASRToo Trigger Manager - Debug] " + String.Format(message, args));
    }
    private static void LogError(string message, params Object?[] args)
    {
        Service.LogError("[ASRToo Trigger Manager - Error] " + String.Format(message, args));
    }

    private static void LogChatter(string message, params Object?[] args)
    {
        Service.LogAlways("[Log Line] " + String.Format(message, args));
    }

    private static List<ChatTrigger>? ChatTriggerPool;

    public static void Initialize()
    {
        LogDebug("Initialize");
        ChatTriggerPool = new List<ChatTrigger>();
    }

    public static void Start()
    {
        LogDebug("Starting");
        InitTriggers();
    }
    private static void CleanTriggers()
    {
        if (ChatTriggerPool != null)
        {
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                LogDebug("Stopping chat trigger {0}", t.Name);
                t.Stop();
            }
            Service.Chat.ChatMessage -= OnChatReceived;
            ChatTriggerPool.Clear();
            LogInfo("Triggers destroyed.");
        }
    }

    private static void InitTriggers()
    {
        LogDebug("InitTriggers");
        if (ChatTriggerPool != null)
        {
            foreach (var d in Service.Configuration.Triggers)
            {
                var Trigger = TriggerFactory.GetTriggerFromConfig(d);
                if (Trigger.Type == "ChatTrigger")
                {
                    ChatTriggerPool.Add((ChatTrigger)Trigger);
                }
                else
                {
                    LogError("Invalid trigger type {0} created.", Trigger.Type);
                }
            }

            foreach (ChatTrigger t in ChatTriggerPool)
            {
                LogDebug("Starting chat trigger {0}", t.Name);
                t.Start();
            }

            Service.Chat.ChatMessage += OnChatReceived;
            LogInfo("Triggers created");
        }
    }
    private static void OnChatReceived(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        LogDebug("OnChatReceived");
        if (ChatTriggerPool != null)
        {
            ChatMessage chatMessage = new(type, timestamp, ref sender, ref message, ref isHandled);
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                t.Queue(chatMessage);
            }
            if (Service.Configuration.LogChat)
            {
                LogChatter(chatMessage.ToString());
            }
        }
    }

    public static void reset()
    {
        LogDebug("reset");
        Stop();
        Start();
    }

    public static void Stop()
    {
        LogDebug("Stop");
        CleanTriggers();
    }

    public static void Shutdown()
    {
        LogDebug("Shutdown");
        Stop();
    }
}
