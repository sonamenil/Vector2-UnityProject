using System;
using System.Collections.Generic;

namespace Nekki.Vector.Core.Console
{
	public static class ConsoleDatabase
	{
		private class CommandData
		{
			public CommandFunction Func { get; private set; }

			public bool IgnoreCase { get; private set; }

			public CommandData(CommandFunction p_func, bool p_ignoreCase)
			{
				Func = p_func;
				IgnoreCase = p_ignoreCase;
			}
		}

		public delegate string CommandFunction(params string[] args);

		private const string _CommandArgsSeparator = " ";

		private static readonly char[] _ArgsSeparators = new char[1] { ' ' };

		private static readonly Dictionary<string, CommandData> _Commands = new Dictionary<string, CommandData>();

		public static bool HasCommand(string name)
		{
			name = name.ToLower();
			return _Commands.ContainsKey(name) && _Commands[name] != null;
		}

		public static string ExecuteCommand(string command)
		{
			if (!string.IsNullOrEmpty(command))
			{
				command = command.Trim(' ');
				int num = ((!command.Contains(" ")) ? command.Length : command.IndexOf(" ", StringComparison.Ordinal));
				string text = command.Substring(0, num).ToLower();
				command = command.Remove(0, num);
				command = command.Trim(' ');
				if (HasCommand(text))
				{
					CommandData commandData = _Commands[text];
					if (commandData.IgnoreCase)
					{
						command = command.ToLower();
					}
					string[] args = command.Split(_ArgsSeparators, StringSplitOptions.RemoveEmptyEntries);
					return commandData.Func(args);
				}
			}
			return "Unknown command!";
		}

		public static void RegisterCommand(string name, CommandFunction commandFunction, bool p_ignoreCase = true)
		{
			_Commands[name.ToLower()] = new CommandData(commandFunction, p_ignoreCase);
		}

		public static void UnregisterCommand(string name)
		{
			_Commands.Remove(name.ToLower());
		}
	}
}
