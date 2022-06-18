using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuditActions
{
	internal class SupportFuncs
	{
    private static Mutex mut = new Mutex();
		public static string logFilePath = "acrobatLog.txt";
		public static void writeLog(string text)
    {     
      mut.WaitOne();

			var log = File.ReadAllText(logFilePath);
			StreamWriter sw = new StreamWriter(logFilePath);

			if(log != String.Empty) sw.Write(log + '\n' + DateTime.Now + ": " + text);
			else sw.Write(DateTime.Now + ": " + text);

			Console.WriteLine(DateTime.Now + ": " + text);
			sw.Close();

			mut.ReleaseMutex();
    }
	}
}
