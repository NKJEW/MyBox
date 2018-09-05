﻿using System;
using System.IO;
using UnityEngine;

namespace MyBox
{
	public class Logger
	{
		private const string LogFile = "customLog.txt";
		private const string TimeFormat = "MM-dd_HH-mm-ss";

		public static bool Disabled;

		public static string Session { get; }
		public static string Version { get; }


		private const int MaxMessageLength = 4000;

		static Logger()
		{
			Session = Guid.NewGuid().ToString();
			Version = "Version not initiated";

			AppDomain.CurrentDomain.UnhandledException += (sender, args) => Log(args.ExceptionObject as Exception);
			Application.logMessageReceived += (condition, trace, type) => Log($"Console Log ({type}): {condition}{Environment.NewLine}{trace}");
		}


		public static void Log(string text)
		{
			if (Application.isEditor) return;
			if (Disabled) return;

			string path = Path.Combine(Application.dataPath, LogFile);

			if (text.Length > MaxMessageLength) text = text.Substring(0, MaxMessageLength) + "...<trimmed>";

			try
			{
				if (!File.Exists(path))
				{
					using (StreamWriter sw = File.CreateText(path))
					{
						sw.WriteLine(GetCurrentTime() + " || Log created" + Environment.NewLine);
						sw.WriteLine(GetCurrentTime() + ": " + text);
					}
				}
				else
				{
					using (StreamWriter sw = File.AppendText(path))
					{
						sw.WriteLine(GetCurrentTime() + ": " + text);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}


			string GetCurrentTime()
			{
				return DateTime.Now.ToString(TimeFormat);
			}
		}

		public static void Log(Exception ex)
		{
			Log("Exception:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
		}
	}
}