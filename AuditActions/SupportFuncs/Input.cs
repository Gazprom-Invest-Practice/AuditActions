using System.IO;

namespace AuditActions.SupportFuncs
{
	internal class Input
	{
		public static void ReadCommand(string cmd)
		{
			if (Constants.Commands.ContainsKey(cmd))
			{
				Constants.cmdFuncWithParam cfwp;
				Constants.Commands.TryGetValue(cmd, out cfwp);

				cfwp.Invoke(cmd);
			}
			else throw new InvalidDataException("Invalid command");
		}

		public static void createLogFile(string appName)
		{
			File.Create(Constants.LogPath + appName + ".log");
			if (!File.Exists(Constants.LogPath + appName + ".log")) throw new FileNotFoundException("Invalid path for logs");
		}
		public static void turnOnDebugMode(string notUse)
		{
			Program.DebugMode = true;
		}

		public static void turnOnForbiddenMode(string notUse)
		{
			Program.ForbiddenMode = true;
		}

		public static void turnOnPrintTracking(string notUse)
		{
			Program.PrintTrackMode = true;
		}
	}
}