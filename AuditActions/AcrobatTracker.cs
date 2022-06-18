using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuditActions
{
	internal class AcrobatTracker
	{
		static bool isThreadRun = false;
		static string CurrentPDF { get; set; }

		static Thread acrobatActionsTracker;

		public AcrobatTracker()
		{
			acrobatActionsTracker = new Thread(runAcrobatTracker);
		}

		public void startAT(string PDFname)
		{
			CurrentPDF = PDFname;
			if (!isThreadRun)
			{
				acrobatActionsTracker.Start();
				isThreadRun = true;
			}
		}
		public void stopAT()
		{
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
			SupportFuncs.writeLog("Начато отслеживание файла " + CurrentPDF);
			//while (true)
			//{
       _hookID = SetHook(_proc);
       Application.Run();
       UnhookWindowsHookEx(_hookID);
      //}
		}

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static string[] RKey = new string[] { "Ф", "Ы", "В", "А", "Й", "Ц", "У", "К", "Е", "Н", "Г", "Ш", "Щ", "З", "Х", "Ъ", "П", "Р", "О", " ", "Л", "Д", "Ж", "Э", "Ё", "Я", "Ч", "С", "М", "И", "Т", "Ь", "Б", "Ю", "." };
    private static string[] EKey = new string[] { "A", "S", "D", "F", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "G", "H", "J", "Space", "K", "L", ";", "'", "`", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/" };
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

        if (((Keys)vkCode).ToString() == "PrintScreen") SupportFuncs.writeLog(((Keys)vkCode).ToString());
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
