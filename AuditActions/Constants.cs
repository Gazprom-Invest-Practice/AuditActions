using System;
using System.Collections.Generic;

namespace AuditActions
{
	using Command = String;
	internal class Constants
	{
		public static string LogPath { get; set; } = "";

		public static Dictionary<Command, cmdFuncWithParam> Commands = new Dictionary<Command, cmdFuncWithParam>();

		//Support
		public delegate void cmdFuncWithParam(string appName);
		public delegate void cmdFunc();
	}
}
