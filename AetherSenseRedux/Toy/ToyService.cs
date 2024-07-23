using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AetherSenseReduxToo.Enums;
using AetherSenseReduxToo;
using Buttplug;
using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Lua;
using Buttplug.Core;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using AetherSenseReduxToo.Toy.Pattern;
using System.Reflection.Metadata.Ecma335;

namespace AetherSenseReduxToo.Toy;

#pragma warning disable IDE1006 // Naming Styles

public class ToyService
{
    private static WaitType _waitType;
    public static WaitType GetWaitType() => _waitType;
    private static List<Device> DevicePool = new List<Device>();
    public static List<Device> Devices() { return DevicePool; }    
    public static ButtplugClient? Client { get; set; } = null;

    private static void LogInfo(string message, params Object?[] args)
    {
        Service.LogInfo("[ASRToo ToyService] " + String.Format(message, args));
    }
    private static void LogDebug(string message, params Object?[] args)
    {
        Service.LogDebug("[ASRToo ToyService - Debug] " + String.Format(message, args));
    }
    private static void LogError(string message, params Object?[] args)
    {
        Service.LogError("[ASRToo ToyService - Error] " + String.Format(message, args));
    }
    public static event EventHandler<String> onLogInfo = delegate { };
    public static event EventHandler<String> onLogDebug = delegate { };
    public static event EventHandler<String> onLogError = delegate { };
    private static Exception? _lastException = null;
    public static Exception? LastException
    {
        get => _lastException;
        set
        {
            _lastException = value;
            if (value != null)
            {
                LogDebug("Exception::ToyService::" + value.Message.ToString());
            }
        }
    }
    public static ButtplugStatus Status { get; set; } = ButtplugStatus.Uninitialized;

    //Events
    public static event EventHandler onStarting = delegate { };
    public static event EventHandler onConnecting = delegate { };
    public static event EventHandler onConnected = delegate { };
    public static event EventHandler onStarted = delegate { };
    public static event EventHandler onError = delegate { };
    public static event EventHandler<Exception> onException = delegate { };
    public static event EventHandler<Device> onToyAdded = delegate { };
    public static event EventHandler<Device> onToyRemoved = delegate { };
    public static event EventHandler onScanning = delegate { };
    public static event EventHandler onScanComplete = delegate { };
    public static event EventHandler onPingTimeout = delegate { };
    public static event EventHandler onDisconnected = delegate { };
    public static event EventHandler onEnding = delegate { };

    //Incoming Events
    public static void BPDeviceAdded(ButtplugClientDevice device)
    {
        LogInfo($"Device {device.Name} Connected!");
        var nDevice = new Device(device, _waitType);
        DevicePool.Add(nDevice);
        onToyAdded(null, nDevice);
        if (!Service.Configuration.SeenDevices.Contains(nDevice.Name))
        {
            Service.Configuration.SeenDevices.Add(nDevice.Name);
        }
        nDevice.Start();
        LogDebug($"Device {device.Name} added to pool!");
    }
    public static void BPDeviceRemoved(ButtplugClientDevice device)
    {
        if (Status != ButtplugStatus.Connected)
        {
            return;
        }
        LogInfo($"Device {device.Name} Removed!");
        var toRemove = new List<Device>();
        lock (DevicePool)
        {
            foreach (Device adevice in DevicePool)
            {
                if (adevice.ClientDevice == device)
                {
                    try
                    {
                        adevice.Stop();
                    }
                    catch (Exception ex)
                    {
                        LogError("Could not stop device {0}, device disconnected?", device.Name);
                        LastException = ex;
                    }
                    toRemove.Add(adevice);
                    adevice.Dispose();
                }
            }
        }
        foreach (Device adevice in toRemove)
        {
            lock (DevicePool)
            {
                onToyRemoved(null, adevice);
                LogDebug($"Device {adevice.Name} removed from pool!");
                DevicePool.Remove(adevice);
            }

        }
    }
    public static void BPErrorReceived(ButtplugException ex)
    {
        LogDebug("Buttplug exception raised");
        LastException = ex;
    }
    public static void BPPingTimeout()
    {
        LogDebug("Buttplug Ping Timeout");
    }
    public static void BPScanningFinished()
    {
        LogInfo("Finished Scanning for Devices");
        StopScanTask = null;
        startScanTask = null;
    }
    public static void BPServerDisconnect()
    {
        if (Status != ButtplugStatus.Disconnecting)
        {
            LogError("Unexpected disconnect.");
        }
        Stop(false);
        LogDebug("Buttplug Disconnected");
        Status = ButtplugStatus.Disconnected;
    }


    public static void Initialize()
    {
        LastException = null;
        DevicePool.Clear();
        Status = ButtplugStatus.Uninitialized;

        LogDebug("Initialize- Running Benchmarks");
        var t = DoBenchmark();
        t.Wait();
        _waitType = t.Result;
    }

    public static void Start() { 

        LogDebug("Start- Starting Up");
        onStarting(null, EventArgs.Empty);

        //Reset things
        LastException = null;
        DevicePool.Clear();
        Status = ButtplugStatus.Disconnected;
        connect();
    }

    public static async Task<WaitType> DoBenchmark()
    {
        var result = WaitType.Slow_Timer;
        long[] times = new long[10];
        long sum = 0;
        double[] averages = new double[2];
        Stopwatch timer = new();
        LogInfo("Starting benchmark");


        LogDebug("DoBenchmark- Testing Task.Delay");

        for (int i = 0; i < times.Length; i++)
        {
            timer.Restart();
            await Task.Delay(1);
            times[i] = timer.Elapsed.Ticks;
        }
        foreach (long t in times)
        {
            LogDebug("DoBenchmark- {0}", t);
            sum += t;
        }
        averages[0] = (double)sum / times.Length / 10000;
        LogDebug("DoBenchmark- Average: {0}", averages[0]);

        LogDebug("DoBenchmark- Testing Thread.Sleep");
        times = new long[10];
        for (int i = 0; i < times.Length; i++)
        {
            timer.Restart();
            Thread.Sleep(1);
            times[i] = timer.Elapsed.Ticks;
        }
        sum = 0;
        foreach (long t in times)
        {
            LogDebug("DoBenchmark- {0}", t);
            sum += t;
        }
        averages[1] = (double)sum / times.Length / 10000;
        LogDebug("DoBenchmark- Average: {0}", averages[1]);

        if (averages[0] < 3)
        {
            result = WaitType.Use_Delay;

        }
        else if (averages[1] < 3)
        {
            result = WaitType.Use_Sleep;
        }

        switch (result)
        {
            case WaitType.Use_Delay:
                LogInfo("High resolution Task.Delay found, using delay in timing loops.");
                break;
            case WaitType.Use_Sleep:
                LogInfo("High resolution Thread.Sleep found, using sleep in timing loops.");
                break;
            default:
                LogInfo("No high resolution, CPU-friendly waits available, timing loops will be inaccurate.");
                break;
        }

        LogDebug("DoBenchmark- Completed benchmark");

        return result;

    }

    /*private static ButtplugStatus CheckButtplugStatus()
    {
        try
        {
            if (Client == null)
            {
                return ButtplugStatus.Uninitialized;
            }
            else if (Client.Connected && Status == ButtplugStatus.Connected)
            {
                return ButtplugStatus.Connected;
            }
            else if (Status == ButtplugStatus.Connecting)
            {
                return ButtplugStatus.Connecting;
            }
            else if (!Client.Connected && Status == ButtplugStatus.Connected)
            {
                return ButtplugStatus.Error;
            }
            else if (Status == ButtplugStatus.Disconnecting)
            {
                return ButtplugStatus.Disconnecting;
            }
            else if (LastException != null)
            {
                return ButtplugStatus.Error;
            }
            else
            {
                return ButtplugStatus.Disconnected;
            }
        }
        catch (Exception ex)
        {
            LogError("CheckButtplugStatus- " + ex.Message + " error when getting status");
            return ButtplugStatus.Error;
        }

    }*/

    public static bool isScanning()
    {
        if ((startScanTask != null) && (StopScanTask == null))
        {
            return true;
        }
        return false;
    }


    private static Task? startScanTask;
    public static bool StartScanning()
    {
        if (Client == null)
        {
            return false;
        }

        if (startScanTask != null)
        {
            LogInfo("Already Scanning");
        }

        startScanTask = Client.StartScanningAsync();
        startScanTask.ContinueWith(t =>
        {
            startScanTask = null;
            onScanComplete(null,EventArgs.Empty);   
        });
        return startScanTask.Status == TaskStatus.Running;
    }

    private static Task? StopScanTask;
    public static bool StopScanning()
    {
        if (Client == null)
        {
            return true;
        }

        if (StopScanTask == null)
        {
            LogInfo("Already Stopping Scanning");
        }

        StopScanTask = Client.StopScanningAsync();
        StopScanTask.ContinueWith(t =>
        {
            StopScanTask = null;
            onScanComplete(null, EventArgs.Empty);
        });
        return StopScanTask.Status == TaskStatus.Running;
    }

    private static bool Connected
    {
        get
        {
            if (Client != null)
            {
                return Client.Connected;
            }
            return false;
        }
    }

    public static Dictionary<string, double> ConnectedDevices
    {
        get
        {
            Dictionary<string, double> result = new();
            foreach (Device device in DevicePool)
            {
                result[device.Name] = device.UPS;
            }
            return result;
        }
    }

    private static Task? connectTask = null;
    public static void connect()
    {

        if (ToyService.Status == ButtplugStatus.Disconnected)
        {
            connectAttempts++;
            var connector = new ButtplugWebsocketConnector(new Uri(Service.Configuration.Address));

            Client = new ButtplugClient("ASRToo");
            //Attach events
            Client.ServerDisconnect += (aObject, e) =>
            {
                BPServerDisconnect();
            };
            Client.ScanningFinished += (aObject, e) =>
            {
                BPScanningFinished();
            };
            Client.DeviceAdded += (aObject, e) =>
            {
                BPDeviceAdded(e.Device);
            };
            Client.DeviceRemoved += (aObject, e) =>
            {
                BPDeviceRemoved(e.Device);
            };
            Client.PingTimeout += (aObject, e) =>
            {
                BPPingTimeout();
            };

            LogInfo("Connecting to Intiface on " + Service.Configuration.Address);
            Status = ButtplugStatus.Connecting;
            connectTask = Client.ConnectAsync(connector);
            connectTask.ContinueWith(t =>
            {
                onConnected(null, EventArgs.Empty);
                isConnected();
                connectTask = null;
            }, TaskContinuationOptions.OnlyOnRanToCompletion)
            .ContinueWith(t =>
            {
                if (t.Exception != null)
                    connectError(t.Exception);
                connectTask = null;
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        else if (Status == ButtplugStatus.Connecting)
        {
            LogInfo("Already connecting please wait");
        }
        else if (Status == ButtplugStatus.Error)
        {
            LogInfo("An error was encountered before, please reset in settings");
        }
        else if (Status == ButtplugStatus.Disconnecting)
        {
            LogInfo("Try to connect again in a momment");
        }
    }

    public static void isConnected()
    {
        connectAttempts = 0;
        connectFail = false;
        LogInfo("Connected");
        Status = ButtplugStatus.Connected;  
        StartScanning();  
    }

    public static void connectError(Exception ex)
    {
        if (ex.GetType().Name == "Buttplug.Client.ButtplugClientConnectorException")
        {
            // If our connection failed, because the server wasn't turned on,
            // SSL/TLS wasn't turned off, etc, we'll just print and exit
            // here. This will most likely be a wrapped exception.
            Status = ButtplugStatus.Error;
            LogError("Connection failure");
            LastException = ex;
        }
        else if(ex.GetType().Name == "Buttplug.Handshake.Exception")
        {
            // This means our client is newer than our server, and we need to
            // upgrade the server we're connecting to.
            Status = ButtplugStatus.Error;
            LogError("Connection failure- Server Client error (versions out of sync?)");
            LastException = ex;
        }
        else
        {
            Status = ButtplugStatus.Disconnected;
            LogError("Connection failure- Unknown error: "+ex.GetType());
            LastException = ex;
        }
        Stop(false);
    }

    public static void reset()
    {
        Stop(true);
        LogInfo("Resetting...");
        Start();
    }

    private static Task? disconnectTask = null;

    public static int connectAttempts = 0;
    public static bool connectFail = false;

    public static void disconnect()
    {
        if (Status == ButtplugStatus.Connected)
        {
            Status = ButtplugStatus.Disconnecting;

            if (Client == null)
            {
                Status = ButtplugStatus.Disconnected;
                disconnectTask = null;
                LogInfo("Not Connected");
            }
            else if (disconnectTask == null) {
                disconnectTask = Client.DisconnectAsync();
                disconnectTask.ContinueWith(t =>
                {
                    onDisconnected(null, EventArgs.Empty);
                    isDisconnected();
                    disconnectTask = null;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
                disconnectTask.ContinueWith(t =>
                {
                    if (t.Exception != null)
                        disconnectError(t.Exception);
                    disconnectTask = null;
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                LogInfo("Already disconnecting?");
            }
        }
    }

    public static void disconnectError(Exception ex) { 
        LogError("Buttplug failed to disconnect."); 
        LastException = ex;
        Status = ButtplugStatus.Error;
    }

    public static void isDisconnected()
    {
        Status = ButtplugStatus.Disconnected;
    }

    private static void CleanDevices()
    {
        lock (DevicePool)
        {
            foreach (Device device in DevicePool)
            {
                LogDebug("Stopping device {0}", device.Name);
                device.Stop();
                device.Dispose();
            }
            DevicePool.Clear();
        }
        LogDebug("Devices destroyed.");
    }
    private static void CleanButtplug()
    {
        if (Client == null)
        {
            Status = ButtplugStatus.Disconnected;
            return;
        }

        LogDebug("Buttplug Cleaning");
        Status = ButtplugStatus.Disconnected;
        Client.Dispose();
        Client = null;
        LogDebug("Buttplug destroyed.");
    }
    public static void Stop(bool expected)
    {
        //CleanTriggers();
        CleanDevices();
        if (expected)
        {
            connectAttempts = 0;
            disconnect();
        }
        else
        {
            connectFail = true;
        }
        if (!expected && Service.Configuration.Reconnect && (connectAttempts < 20))
        {
            Task.Delay(30000).ContinueWith(t => { LogInfo("Reconnecting"); Start(); }); 
        }
        else if (!expected && Service.Configuration.Reconnect && (connectAttempts >= 20))
        {
            connectAttempts = 0;
        }
        CleanButtplug();
    }

    public static void Shutdown()
    {
        Stop(true);
    }

    public static void DoPatternTest(dynamic patternConfig)
    {
        if (Status != ButtplugStatus.Connected)
        {
            return;
        }

        lock (DevicePool)
        {
            foreach (var device in DevicePool)
            {
                lock (device.Patterns)
                {
                    device.Patterns.Add(PatternFactory.GetPatternFromObject(patternConfig));
                }
            }
        }
    }

}