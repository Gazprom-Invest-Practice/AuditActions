using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Threading;
using System.IO;

namespace AuditActions
{
	internal class AdobeAcrobatTracking
	{
		static string CurrentPDF { get; set; }

	  static Thread winTracker = new Thread(activatorAcrobatTracker);

		static Thread acrobatActionsTracker = new Thread(AcrobatTracker);
		public static void StartAcrobatTracking()
		{
			winTracker.Start();
		}

		private static void activatorAcrobatTracker()
		{
			while (true)
			{
				EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
				{
					hWnd = (IntPtr)GetForegroundWindow();
					Console.WriteLine(GetWindowText(hWnd));
					if (acrobatActionsTracker.ThreadState != System.Threading.ThreadState.Running
						&& GetWindowText(hWnd).Contains("- Adobe Acrobat Reader DC (64-bit)"))
					{
						CurrentPDF = GetWindowText(hWnd).Split('-')[0].Trim();
						acrobatActionsTracker.Start();
					}
					else
					{
						if (acrobatActionsTracker.ThreadState == System.Threading.ThreadState.Running)
							acrobatActionsTracker.Join();
						acrobatActionsTracker.Interrupt();
					}
					return true;
				}, IntPtr.Zero);
			}
		}

		private static void AcrobatTracker()
		{
			while (true)
			{
				using (StreamWriter sw = File.AppendText("C:/PolyGo/acrobatLog.txt"))
				{
					sw.WriteLine("Начато отслеживание файла " + CurrentPDF + ' ' + DateTime.Now);
					sw.Close();
				}
				if (Console.ReadKey().Equals(ConsoleKey.PrintScreen))
				{
					using (StreamWriter sw = File.AppendText("C:/PolyGo/acrobatLog.txt"))
					{
						sw.WriteLine("Пользователь нажал printscreen, время: " + DateTime.Now.ToString());
						sw.Close();
					}
				}
			}
		}

		//user32.dll import
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetForegroundWindow();

		public static string GetWindowText(IntPtr hWnd)
		{
			int len = GetWindowTextLength(hWnd) + 1;
			StringBuilder sb = new StringBuilder(len);
			len = GetWindowText(hWnd, sb, len);
			return sb.ToString(0, len);
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		// Nested Types
		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	}
}
