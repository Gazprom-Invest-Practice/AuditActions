using System;
using System.Runtime.InteropServices;

using AuditActions.SupportFuncs;

namespace AuditActions
{
	internal class Program
	{
		public static bool DebugMode { get; set; } = false;
		static void Main(string[] args)
		{
			FillCommands();
			try
			{
				Constants.LogPath = args[0] + '\\';
				for (int i = 1; i < args.Length; ++i)
				{
					Input.ReadCommand(args[i]);
				}

				if(!DebugMode)
				{
					var handle = GetConsoleWindow();
					ShowWindow(handle, SW_HIDE);
				}

				WinTracker.StartWinTracking();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		private static void FillCommands()
		{
			Constants.cmdFuncWithParam cfwp = Input.createLogFile;
			Constants.Commands.Add("acrobat", cfwp);
			Constants.Commands.Add("word", cfwp);

			cfwp = Input.turnOnDebugMode;
			Constants.Commands.Add("debug", cfwp);
		}

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_HIDE = 0;
	}
}
