using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Threading;

using AuditActions.AppTrackers;

namespace AuditActions
{
	internal class WinTracker
	{
		static AcrobatTracker AT = new AcrobatTracker();
		public static string CurrentPDF { get; set; }

		static Thread winTracker = new Thread(activateAcrobatTracker);
		public static void StartWinTracking()
		{
			winTracker.Start();
		}
		private static void activateAcrobatTracker()
		{
			while (true)
			{
				EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
				{
					hWnd = (IntPtr)GetForegroundWindow();

					if (GetWindowText(hWnd).Contains("- Adobe Acrobat"))
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
