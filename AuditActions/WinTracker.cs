using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Threading;
using System.Collections.Generic;

using AuditActions.AppTrackers;
using AuditActions.SupportFuncs;

namespace AuditActions
{
	internal class WinTracker
	{
		static AcrobatTracker AT = new AcrobatTracker();
		public static KeyTracker KT = new KeyTracker();
		public static string CurrentPDF { get; set; }

		static Thread winTracker = new Thread(activateAcrobatTracker);

		public static List<string> printQueue = new List<string>();
		public static void StartWinTracking()
		{
			winTracker.Start();
		}
		private static void activateAcrobatTracker()
		{
			if (Program.PrintTrackMode) AppTrack.fillPrintQueue(ref printQueue);
			KT.startKT();

			while (true)
			{
				EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
				{
					hWnd = (IntPtr)GetForegroundWindow();

					if (GetWindowText(hWnd).Contains("- Adobe Acrobat") 
					|| GetWindowText(hWnd).Contains("Печать") || GetWindowText(hWnd).Contains("Выполнение"))
					{
						AT.startAT(GetWindowText(hWnd));
					}
					else
					{
						if (AT.ATrunning)
						{
							AT.stopAT();
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
