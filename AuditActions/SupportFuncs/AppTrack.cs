using System;
using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Threading;
using System.Windows.Forms;

namespace AuditActions.SupportFuncs
{
	internal class AppTrack
	{
    private static Mutex mut = new Mutex();
		public static void writeLog(string log, string appName)
    {     
      mut.WaitOne();

			var prev_logs = File.ReadAllText(Constants.LogPath + appName + ".log");
			StreamWriter sw = new StreamWriter(Constants.LogPath + appName + ".log");

			if(prev_logs != string.Empty) sw.Write(prev_logs + '\n' + DateTime.Now + ": " + log);
			else sw.Write(DateTime.Now + ": " + log);

			if(Program.DebugMode) Console.WriteLine(DateTime.Now + ": " + log);
			sw.Close();

			mut.ReleaseMutex();
    }

		public static void fillPrintQueue(ref List<string> pq)
		{
			bool flag = true;
			int maxCount = 50;
			int counter = 0;

			LocalPrintServer server = new LocalPrintServer();
			try
			{
				while (flag && counter < maxCount)
				{
					string[] dirs = Directory.GetFiles(@"C:\Windows\System32\spool\PRINTERS");

					if (dirs.Length > 0)
					{
						foreach (PrintSystemJobInfo jobInfo in server.DefaultPrintQueue.GetPrintJobInfoCollection())
						{
							jobInfo.Pause();
							pq.Add(jobInfo.Name);
							flag = false;
						}
					}
					++counter;
				}
			}
			catch (UnauthorizedAccessException)
			{
				writeLog("You must run application with administrator rights to track the print queue. Program was stopped."
						, "acrobat");
				Environment.Exit(1);
			}
		}
	}
}
