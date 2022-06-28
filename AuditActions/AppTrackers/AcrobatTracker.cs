using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Printing;

using AuditActions.SupportFuncs;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AuditActions.AppTrackers
{
	internal class AcrobatTracker
	{
    //static KeyTracker KT = new KeyTracker();

		static bool isThreadRun = false;
		static string CurrentPDF { get; set; }
    static string TempPDFname { get; set; }

		static Thread acrobatActionsTracker;

		public AcrobatTracker()
		{
			acrobatActionsTracker = new Thread(runAcrobatTracker);
		}

		public void startAT(string windowName)
		{
			KeyTracker.mode = KeyTracker.KTmode.Acrobat;
			if (!windowName.Contains("Печать") && !windowName.Contains("Выполнение")) CurrentPDF = windowName.Split('-')[0].Trim();
      if (!isThreadRun)
			{
				acrobatActionsTracker.Start();
				isThreadRun = true;
			}
		}
		public void stopAT()
		{
      AppTrack.writeLog("Завершено отслеживание файла " + TempPDFname, "acrobat");
			KeyTracker.mode = KeyTracker.KTmode.Win;
			isThreadRun = false;
			try
			{
				acrobatActionsTracker.Abort();
			}
			catch (Exception) { }

			acrobatActionsTracker = new Thread(runAcrobatTracker);
    }
		public bool ATrunning
    {
			get
			{
				return acrobatActionsTracker.IsAlive;
			}
		}
		private static void runAcrobatTracker()
		{
			AppTrack.writeLog(Environment.UserName + ": Начато отслеживание файла " + CurrentPDF, "acrobat");
      TempPDFname = CurrentPDF;

			while (true)
			{
				if (Program.PrintTrackMode)
				{
					var tempPrintQueue = new List<string>();
					AppTrack.fillPrintQueue(ref tempPrintQueue);

					if (WinTracker.printQueue.Count != tempPrintQueue.Count)
					{
						AppTrack.writeLog("Print", "acrobat");
						WinTracker.printQueue = tempPrintQueue;
					}
				}
			}
    }
  }
}
