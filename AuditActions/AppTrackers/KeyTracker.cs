using AuditActions.SupportFuncs;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AuditActions.AppTrackers
{
	internal class KeyTracker
  {
    bool isThreadRun = false;
    Thread keyPressTracker;

    public static KTmode mode { get; set; } = KTmode.Win;
  
    public KeyTracker()
    {
      keyPressTracker = new Thread(runKeyTracker);
    }

    public void startKT()
    {
      if (!isThreadRun)
      {
        keyPressTracker.SetApartmentState(ApartmentState.STA);
        keyPressTracker.Start();
        isThreadRun = true;
      }
    }
    public void stopKT()
    {
      isThreadRun = false;
      try
      {
        UnhookWindowsHookEx(_hookID);
        keyPressTracker.Abort();
      }
      catch (Exception) { }

      keyPressTracker = new Thread(runKeyTracker);
    }
    public bool KTrunning
    {
      get
      {
        return keyPressTracker.IsAlive;
      }
    }
    private static void runKeyTracker()
    {
      _hookID = SetHook(_proc);
      Application.Run();

      UnhookWindowsHookEx(_hookID);
    }

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
            GetModuleHandle(curModule.ModuleName), 0);
      }
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
      if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
      {
        int vkCode = Marshal.ReadInt32(lParam);

        if (mode == KTmode.Acrobat)
        {
          if (((Keys)vkCode).ToString() == "PrintScreen")
          {
            if (Program.ForbiddenMode)
            {
              AppTrack.writeLog(((Keys)vkCode).ToString() + " was cancelled", "acrobat");
              return (IntPtr)1;
            }
            AppTrack.writeLog(((Keys)vkCode).ToString(), "acrobat");
          }

          if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
          {
            if (((Keys)vkCode).ToString() == "S")
            {
              //CTRL + SHIFT + S
              if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
              {
                AppTrack.writeLog("CTRL + SHIFT + S", "acrobat");
              }
              else
              {
                //CTRL + S
                if (Program.ForbiddenMode)
                {
                  AppTrack.writeLog("Saving by CTRL + S was cancelled", "acrobat");
                  return (IntPtr)1;
                }
                AppTrack.writeLog("Saving by CTRL + S", "acrobat");
              }
            }
          }

          if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Alt) == Keys.Alt)
          {
            if (((Keys)vkCode).ToString() == "Space")
            {
              foreach (var process in Process.GetProcessesByName("AuditActions.exe"))
              {
                process.Kill();
              }
              Environment.Exit(1);
            }
          }
        }

        else if (mode == KTmode.Win)
        {
          if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Alt) == Keys.Alt)
          {
            if (((Keys)vkCode).ToString() == "Space")
            {
              foreach (var process in Process.GetProcessesByName("AuditActions.exe"))
              {
                process.Kill();
              }
              Environment.Exit(1);
            }
          }
        }
      }
      return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    public enum KTmode
		{
      Win,
      Acrobat
		}
  }
}
