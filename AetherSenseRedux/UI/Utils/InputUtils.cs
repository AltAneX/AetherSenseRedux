using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AetherSenseReduxToo.UI.Utils;

// I got this from the XIVDeck _plugin, ty KazWolfe
internal static class InputUtil
{
    private const uint WM_KEYUP = 0x101;
    private const uint WM_KEYDOWN = 0x100;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint FindWindowEx(nint hWndParent, nint hWndChildAfter, string lpszClass, string? lpszWindow);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(nint hWnd, out int lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern nint SendMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    public static bool TryFindGameWindow(out nint hwnd)
    {
        hwnd = nint.Zero;
        while (true)
        {
            hwnd = FindWindowEx(nint.Zero, hwnd, "FFXIVGAME", null);
            if (hwnd == nint.Zero) break;
            GetWindowThreadProcessId(hwnd, out var pid);
            if (pid == Process.GetCurrentProcess().Id) break;
        }
        return hwnd != nint.Zero;
    }

    public static void SendKeycode(nint hwnd, int keycode)
    {
        SendMessage(hwnd, WM_KEYDOWN, keycode, 0);
        SendMessage(hwnd, WM_KEYUP, keycode, 0);
    }
}