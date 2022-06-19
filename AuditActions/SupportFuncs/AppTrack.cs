using System;
using System.IO;
using System.Threading;

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

			if(log != string.Empty) sw.Write(prev_logs + '\n' + DateTime.Now + ": " + log);
			else sw.Write(DateTime.Now + ": " + log);

			Console.WriteLine(DateTime.Now + ": " + log);
			sw.Close();

			mut.ReleaseMutex();
    }
	}
}
