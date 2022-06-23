using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using AuditActions.SupportFuncs;

namespace AuditActions.AppTrackers
{
	internal class AcrobatTracker
	{
		static bool isThreadRun = false;
		static string CurrentPDF { get; set; }
    static string TempPDFname { get; set; }

		static Thread acrobatActionsTracker;

		public AcrobatTracker()
		{
			acrobatActionsTracker = new Thread(runAcrobatTracker);
		}

		public void startAT(string windowName)
		{
      CurrentPDF = windowName.Split('-')[0].Trim();
      if (!isThreadRun)
			{
				acrobatActionsTracker.Start();
				isThreadRun = true;
			}
		}
		public void stopAT()
		{
      AppTrack.writeLog("Завершено отслеживание файла " + TempPDFname, "acrobat");
      isThreadRun = false;
			try
			{
				acrobatActionsTracker.Abort();
			}
			catch (Exception) { }

			acrobatActionsTracker = new Thread(runAcrobatTracker);
    }
		public bool ATrunning{
			get
			{
				return acrobatActionsTracker.IsAlive;
			}
		}
		private static void runAcrobatTracker()
		{
			AppTrack.writeLog(Environment.UserName + ": Начато отслеживание файла " + CurrentPDF, "acrobat");
      TempPDFname = CurrentPDF;
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

        if (((Keys)vkCode).ToString() == "PrintScreen") AppTrack.writeLog(((Keys)vkCode).ToString(), "acrobat");
        //var image = Clipboard.GetImage();
        //Directory.CreateDirectory(pathString);
        //image.Save
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
  }
}
