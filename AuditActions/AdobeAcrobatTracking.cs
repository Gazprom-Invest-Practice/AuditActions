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
		static AcrobatTracker AT = new AcrobatTracker();
		public static string CurrentPDF { get; set; }

		static Thread winTracker = new Thread(activatorAcrobatTracker);
		public static void StartAcrobatTracking()
		{
			var fs = File.Create(SupportFuncs.logFilePath);
			fs.Close();
			winTracker.Start();
		}
		private static void activatorAcrobatTracker()
		{
			while (true)
			{
				EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
				{
					hWnd = (IntPtr)GetForegroundWindow();

					if (GetWindowText(hWnd).Contains("- Adobe Acrobat Reader DC (64-bit)"))
					{
						CurrentPDF = GetWindowText(hWnd).Split('-')[0].Trim();
						AT.startAT(CurrentPDF);
					}
					else
					{
						if (AT.ATrunning)
						{
							AT.stopAT();
							SupportFuncs.writeLog("Завершено отслеживание файла " + CurrentPDF);
						}
					}
					return true;
				}, IntPtr.Zero);
			}
		}

		public static bool isPrtSc { get; set; } = false;

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
