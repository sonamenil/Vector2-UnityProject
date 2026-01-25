using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nekki.Vector.Core.ABTest;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.CameraEffects;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Generator.Test;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.InputControllers;
using Nekki.Vector.GUI.Scenes.Archive;
using Nekki.Vector.GUI.Scenes.ArchiveCategory;
using Nekki.Vector.GUI.Scenes.Boosterpack;
using Nekki.Vector.GUI.Scenes.Run;
using Nekki.Vector.GUI.Scenes.Terminal;
using UnityEngine;

namespace Nekki.Vector.Core.Console
{
	public static class ConsoleCommands
	{
		public const string CommandInfo_Help = "help - show all commands";

		public const string CommandInfo_Clear = "clear - clear console";

		public const string CommandInfo_Money1 = "m1 [int] - add money1";

		public const string CommandInfo_Money2 = "m2 [int] - add money2";

		public const string CommandInfo_Money3 = "m3 [int] - add money3";

		public const string CommandInfo_Quit = "q - quit from run";

		public const string CommandInfo_User = "user copy | list | switch | print | printm | v - actions by user";

		public const string CommandInfo_Reset = "reset [string] - user | settings | all";

		public const string CommandInfo_Reload = "reload user | all - to reload user from user.xml or reload all data";

		public const string CommandInfo_Resolution = "resolution [width] [height] (fullscreen) - to set resolution";

		public const string CommandInfo_Play = "play [room1] [room2] ... [roomN] (:seed) - to play specific rooms";

		public const string CommandInfo_Effect = "effect [type | list | clear] - to show camera effect, to list effect types or to clear from any\noffmessageofscreen 1 or 0";

		public const string CommandInfo_GameSpeed = "gs [float] - set game speed";

		public const string CommandInfo_NodePosition = "xy [node name] - get node position";

		public const string CommandInfo_Strings = "strings - log StringsBuffer";

		public const string CommandInfo_SkipTutorial = "s - skip all tutorials";

		public const string CommandInfo_EscapeUser = "es - switch to escape";

		public const string CommandInfo_Zone2User = "z2 - switch to zone2";

		public const string CommandInfo_FinishFloor = "w - finish floor";

		public const string CommandInfo_TestGenerator = "testgenerator [floor] [iterations] - test generator";

		public const string CommandInfo_TestRoom = "testroom [roomName] [floor] [position] (writeCombination int) - test generator";

		public const string CommandInfo_SaveUser = "u - save user.xml";

		public const string CommandInfo_Demo = "demo (demoname) - load demo file\nwithout arguments load last demo file";

		public const string CommandInfo_SwitchChoices = "j - switch choices debug data and set state to default";

		public const string CommandInfo_Preset = "preset [presetName] - create item by preset or help to help";

		public const string CommandInfo_GameCenter = "gc - game center/ google play games, achievements, sign in/out";

		public const string CommandInfo_ADS = "ad - advesting, params: v or video, rw (reward video), ow(offer wall)";

		public const string CommandInfo_SetCounter = "setcounter [namespace] [name] [value] - set value of counter";

		public const string CommandInfo_GetCounter = "getcounter [namespace] [name] - print value of counter";

		public const string CommandInfo_Server = "server on | off - send or not data to server";

		public const string CommandInfo_Release = "release (int) - set build to release mode";

		public const string CommandInfo_RegenerateB = "r - regenerate shop/terminal items";

		public const string CommandInfo_RegenerateM = "rm (-d) - regenerate missions, optional -d to not show dialog";

		public const string CommandInfo_Loss = "x - loss";

		public const string CommandInfo_AddBp = "bp (int) - add/remove boosterpacks";

		public const string CommandInfo_OpenBp = "bpo (int) - open boosterpacks without animation";

		public const string CommandInfo_AntiAliasing = "aa [int] - set antialiasing level";

		public const string CommandInfo_SaveMe = "v - use saveme (only in run)";

		public const string CommandInfo_AddCoupon = "cp (int) - add all type coupons";

		public const string CommandInfo_AddCard = "card [name] (n) - add n cards with name to user (example - \"card BonusTokensFromTricks_2_Legs 5 \")";

		public const string CommandInfo_Paid = "paid [int] - set PaidVersion to 0 or 1";

		public const string CommandInfo_LevelUp = "lvlup - level up all cards if need";

		public const string CommandInfo_Payment = "payment [command] - some test for payment (\"restore\" - restore purchases, \"products\" - update products from server)";

		public const string CommandInfo_SaveMeGodMode = "sm [int] - set save me god mode enabled/disabled (always enable 'Watch video' button and infinity attempt)";

		public const string CommandInfo_Bundle = "bundle [command] - some test for bundles (\"on\" - enable asset bundles, \"off\" - disable asset bundles, \"printm\" - print config content, \"requests\" - create required request, \"dialog\" - show bundle requests dialog, \"reset\" - reset bundles cache)\"";

		public const string CommandInfo_Url = "url [id] - open selected url (\"app\" - application url, \"sf2\" - banner sf2 url, \"sf3\" - banner sf3 url, \"faq\" - FAQ url, \"ost\" - OST url, \"support\" - support url)\"";

		public const string CommandInfo_Profiler = "pf [id] - some profiler commands (\"mc\" - clean memory)";

		public static void Init()
		{
			ConsoleUI.OnConsoleActive += OnConsoleActive;
			ConsoleDatabase.RegisterCommand("help", Help);
			ConsoleDatabase.RegisterCommand("clear", Clear);
			ConsoleDatabase.RegisterCommand("m1", Money1);
			ConsoleDatabase.RegisterCommand("m2", Money2);
			ConsoleDatabase.RegisterCommand("m3", Money3);
			ConsoleDatabase.RegisterCommand("q", Quit);
			ConsoleDatabase.RegisterCommand("user", UserCommand);
			ConsoleDatabase.RegisterCommand("reset", Reset);
			ConsoleDatabase.RegisterCommand("reload", Reload);
			ConsoleDatabase.RegisterCommand("resolution", Resolution);
			ConsoleDatabase.RegisterCommand("play", Play);
			ConsoleDatabase.RegisterCommand("effect", Effect);
			ConsoleDatabase.RegisterCommand("gs", SetGameSpeed);
			ConsoleDatabase.RegisterCommand("xy", NodeCordinates);
			ConsoleDatabase.RegisterCommand("strings", LogStringBuffer);
			ConsoleDatabase.RegisterCommand("s", SkipTutorials);
			ConsoleDatabase.RegisterCommand("z2", SwitchToZone2User);
			ConsoleDatabase.RegisterCommand("es", SwitchToEscapeUser);
			ConsoleDatabase.RegisterCommand("w", FinishLevel);
			ConsoleDatabase.RegisterCommand("preset", UsePreset, false);
			ConsoleDatabase.RegisterCommand("testgenerator", TestGenerator);
			ConsoleDatabase.RegisterCommand("testroom", TestRoom);
			ConsoleDatabase.RegisterCommand("u", SaveUser);
			ConsoleDatabase.RegisterCommand("demo", LoadDemo);
			ConsoleDatabase.RegisterCommand("j", SwitchChoicesDebugDataAndSetDefault);
			ConsoleDatabase.RegisterCommand("gc", GameCenterCommand);
			ConsoleDatabase.RegisterCommand("ad", Advesting);
			ConsoleDatabase.RegisterCommand("setcounter", SetCounter, false);
			ConsoleDatabase.RegisterCommand("getcounter", GetCounter, false);
			ConsoleDatabase.RegisterCommand("abgroup", SetABGroup);
			ConsoleDatabase.RegisterCommand("server", ServerOnOff);
			ConsoleDatabase.RegisterCommand("release", SetRelease);
			ConsoleDatabase.RegisterCommand("r", RegenerateBasket);
			ConsoleDatabase.RegisterCommand("rm", RegenerateMissions);
			ConsoleDatabase.RegisterCommand("x", Loss);
			ConsoleDatabase.RegisterCommand("bp", GiveBoosterpacks);
			ConsoleDatabase.RegisterCommand("bpo", OpenBoosterpacks);
			ConsoleDatabase.RegisterCommand("zone", SetZone);
			ConsoleDatabase.RegisterCommand("aa", SetAntiAliasing);
			ConsoleDatabase.RegisterCommand("v", SaveMe);
			ConsoleDatabase.RegisterCommand("cp", GiveCoupon);
			ConsoleDatabase.RegisterCommand("card", GiveCard, false);
			ConsoleDatabase.RegisterCommand("paid", PaidVersion);
			ConsoleDatabase.RegisterCommand("lvlup", CardsLevelUp);
			ConsoleDatabase.RegisterCommand("payment", PaymentCommand);
			ConsoleDatabase.RegisterCommand("sm", SaveMeGodMode);
			ConsoleDatabase.RegisterCommand("bundle", BundleCommand);
			ConsoleDatabase.RegisterCommand("url", UrlCommand);
			ConsoleDatabase.RegisterCommand("pf", ProfilerCommand);
			Application.logMessageReceived += ApplicationLogSubsctiption;
		}

		private static void ApplicationLogSubsctiption(string condition, string stacktrace, LogType type)
		{
			switch (type)
			{
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				if (!string.IsNullOrEmpty(stacktrace))
				{
					stacktrace = " - " + stacktrace;
				}
				ConsoleUI.Log(condition + stacktrace);
				break;
			case LogType.Warning:
			case LogType.Log:
				break;
			}
		}

		public static void OnConsoleActive(bool state)
		{
			HudPanel module = UIModule.GetModule<HudPanel>();
			if (state != RunMainController.IsPaused && RunMainController.IsRunNow)
			{
				if (module != null && RunMainController.IsPaused && HudPanel.IsPopupActive)
				{
					module.SetPauseState(false, true);
				}
				else
				{
					RunMainController.IsPauseForced(!RunMainController.IsPaused);
				}
				RunMainController.IsDebugPaused = RunMainController.IsPaused;
			}
			else if (module != null)
			{
				module.StopCountdown();
			}
			KeyboardController.SetEnabledAll(!state);
		}

		private static string Help(params string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("All commands:\n");
			stringBuilder.AppendLine("help - show all commands");
			stringBuilder.AppendLine("clear - clear console");
			stringBuilder.AppendLine("m1 [int] - add money1");
			stringBuilder.AppendLine("m2 [int] - add money2");
			stringBuilder.AppendLine("m3 [int] - add money3");
			stringBuilder.AppendLine("q - quit from run");
			stringBuilder.AppendLine("user copy | list | switch | print | printm | v - actions by user");
			stringBuilder.AppendLine("reset [string] - user | settings | all");
			stringBuilder.AppendLine("reload user | all - to reload user from user.xml or reload all data");
			stringBuilder.AppendLine("resolution [width] [height] (fullscreen) - to set resolution");
			stringBuilder.AppendLine("play [room1] [room2] ... [roomN] (:seed) - to play specific rooms");
			stringBuilder.AppendLine("effect [type | list | clear] - to show camera effect, to list effect types or to clear from any\noffmessageofscreen 1 or 0");
			stringBuilder.AppendLine("gs [float] - set game speed");
			stringBuilder.AppendLine("xy [node name] - get node position");
			stringBuilder.AppendLine("s - skip all tutorials");
			stringBuilder.AppendLine("es - switch to escape");
			stringBuilder.AppendLine("z2 - switch to zone2");
			stringBuilder.AppendLine("w - finish floor");
			stringBuilder.AppendLine("testgenerator [floor] [iterations] - test generator");
			stringBuilder.AppendLine("testroom [roomName] [floor] [position] (writeCombination int) - test generator");
			stringBuilder.AppendLine("u - save user.xml");
			stringBuilder.AppendLine("demo (demoname) - load demo file\nwithout arguments load last demo file");
			stringBuilder.AppendLine("j - switch choices debug data and set state to default");
			stringBuilder.AppendLine("preset [presetName] - create item by preset or help to help");
			stringBuilder.AppendLine("gc - game center/ google play games, achievements, sign in/out");
			stringBuilder.AppendLine("ad - advesting, params: v or video, rw (reward video), ow(offer wall)");
			stringBuilder.AppendLine("setcounter [namespace] [name] [value] - set value of counter");
			stringBuilder.AppendLine("getcounter [namespace] [name] - print value of counter");
			stringBuilder.AppendLine("server on | off - send or not data to server");
			stringBuilder.AppendLine("release (int) - set build to release mode");
			stringBuilder.AppendLine("r - regenerate shop/terminal items");
			stringBuilder.AppendLine("rm (-d) - regenerate missions, optional -d to not show dialog");
			stringBuilder.AppendLine("x - loss");
			stringBuilder.AppendLine("bp (int) - add/remove boosterpacks");
			stringBuilder.AppendLine("bpo (int) - open boosterpacks without animation");
			stringBuilder.AppendLine("aa [int] - set antialiasing level");
			stringBuilder.AppendLine("v - use saveme (only in run)");
			stringBuilder.AppendLine("cp (int) - add all type coupons");
			stringBuilder.AppendLine("card [name] (n) - add n cards with name to user (example - \"card BonusTokensFromTricks_2_Legs 5 \")");
			stringBuilder.AppendLine("paid [int] - set PaidVersion to 0 or 1");
			stringBuilder.AppendLine("lvlup - level up all cards if need");
			stringBuilder.AppendLine("payment [command] - some test for payment (\"restore\" - restore purchases, \"products\" - update products from server)");
			stringBuilder.AppendLine("sm [int] - set save me god mode enabled/disabled (always enable 'Watch video' button and infinity attempt)");
			stringBuilder.AppendLine("bundle [command] - some test for bundles (\"on\" - enable asset bundles, \"off\" - disable asset bundles, \"printm\" - print config content, \"requests\" - create required request, \"dialog\" - show bundle requests dialog, \"reset\" - reset bundles cache)\"");
			stringBuilder.AppendLine("url [id] - open selected url (\"app\" - application url, \"sf2\" - banner sf2 url, \"sf3\" - banner sf3 url, \"faq\" - FAQ url, \"ost\" - OST url, \"support\" - support url)\"");
			stringBuilder.AppendLine("pf [id] - some profiler commands (\"mc\" - clean memory)");
			return stringBuilder.ToString().TrimEnd('\r', '\n');
		}

		public static string Clear(params string[] args)
		{
			ConsoleUI.Clear();
			return "Ok";
		}

		public static string Money1(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: m1 [int]";
			}
			try
			{
				int num = int.Parse(args[0]);
				DataLocal current = DataLocal.Current;
				current.Money1 = (int)current.Money1 + num;
				DataLocal.Current.Save(false);
				return "Money1 OK " + DataLocal.Current.Money1;
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string Money2(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: m2 [int]";
			}
			try
			{
				int num = int.Parse(args[0]);
				DataLocal current = DataLocal.Current;
				current.Money2 = (int)current.Money2 + num;
				DataLocal.Current.Save(false);
				return "Money2 OK " + DataLocal.Current.Money2;
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string Money3(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: m3 [int]";
			}
			try
			{
				int num = int.Parse(args[0]);
				DataLocal current = DataLocal.Current;
				current.Money3 = (int)current.Money3 + num;
				DataLocal.Current.Save(false);
				return "Money3 OK " + DataLocal.Current.Money3;
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string Quit(params string[] args)
		{
			Manager.Quit();
			return "Quit";
		}

		public static string UserCommand(params string[] args)
		{
			if (args.Length == 0)
			{
				return "No Arguments! Try: user copy/list/switch/print/printm/v";
			}
			if (args[0] == "copy")
			{
				string arg = UserSwitcher.SaveCurentUser();
				return string.Format("User copy to file {0}", arg);
			}
			if (args[0] == "list")
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] userList = UserSwitcher.GetUserList();
				for (int i = 0; i < userList.Length; i++)
				{
					stringBuilder.AppendFormat("{0}. {1}\n", i + 1, userList[i]);
				}
				return stringBuilder.ToString();
			}
			if (args[0] == "switch")
			{
				if (args.Length != 2)
				{
					return "user switch must have index user file";
				}
				int result;
				if (!int.TryParse(args[1], out result))
				{
					return "user switch must have int index";
				}
				UserSwitcher.SwitchByIndex(result - 1);
				return "User was switched";
			}
			if (args[0] == "print")
			{
				return File.ReadAllText(DataLocal.FilePath);
			}
			if (args[0] == "printm")
			{
				return DataLocal.Current.SaveToString();
			}
			if (args[0] == "v")
			{
				Settings.UserValidationOn = args[1] == "1";
				Settings.Save();
				return "Validation state: " + ((!Settings.UserValidationOn) ? "Off" : "On");
			}
			return string.Format("unknown argument {0}", args[0]);
		}

		public static string Reset(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: reset";
			}
			switch (args[0])
			{
			case "user":
				DataLocal.Reset();
				DataLocal.UserDontSave = false;
				GameRestorer.RemoveBackup();
				AudioManager.Init();
				QuestManager.Reset();
				QuestManager.Init();
				StarterPacksManager.PrepareStarterPacks();
				Manager.Load(SceneKind.Main);
				return "User has been reset!";
			case "settings":
				DataLocal.UserDontSave = false;
				Settings.Reset();
				Manager.Load(SceneKind.Main);
				return "Settings has been reset!";
			case "all":
				DataLocal.Reset();
				DataLocal.UserDontSave = false;
				Settings.Reset();
				GameRestorer.RemoveBackup();
				AudioManager.Init();
				Manager.Load(SceneKind.Main);
				return "User and settings has been reset!";
			default:
				return "Unknown arguments!";
			}
		}

		public static string Reload(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: reset user";
			}
			switch (args[0])
			{
			case "user":
				DataLocal.Reload();
				DataLocal.UserDontSave = false;
				Manager.Load(SceneKind.Main);
				return "User has been Reload!";
			case "all":
				PresetsManager.Reload();
				DataLocal.Reload();
				DataLocal.UserDontSave = false;
				GameManager.ReloadAll();
				AchievementsManager.Reload();
				Manager.Load(SceneKind.Main);
				return "All data has benn reloaded";
			default:
				return "Unknown arguments!";
			}
		}

		public static string Resolution(params string[] args)
		{
			if (args == null || args.Length < 2)
			{
				return "Wrong Arguments! Try: resolution [width] [height] (fullscreen)";
			}
			try
			{
				int num = int.Parse(args[0]);
				int num2 = int.Parse(args[1]);
				if (args.Length > 2)
				{
					bool fullscreen = bool.Parse(args[2]);
					Screen.SetResolution(num, num2, fullscreen);
				}
				else
				{
					Screen.SetResolution(num, num2, Screen.fullScreen);
				}
				return string.Format("Set screen resolution to {0} x {1}", num, num2);
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string Play(params string[] args)
		{
			KeyboardController.SetEnabledAll(true);
			try
			{
				if (args == null || args.Length < 1)
				{
					return "Wrong Arguments! Try: play [room1] [room2] ... [roomN] (:seed)";
				}
				int p_seed = -1;
				List<string> list = new List<string>();
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith(":"))
					{
						p_seed = int.Parse(args[i].Remove(0, 1));
					}
					else
					{
						list.Add(args[i]);
					}
				}
				int p_floor = Settings.PlayCommandFloor - 1;
				if (!GeneratorHelper.PreparePlayCommand(list, p_floor, p_seed))
				{
					return "No such rooms to play!";
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string toLoadRoom in GeneratorHelper.ToLoadRooms)
				{
					stringBuilder.Append(toLoadRoom + ", ");
				}
				Manager.Load(SceneKind.Run);
				return "Start playing " + stringBuilder.ToString().TrimEnd(',', ' ');
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				if (ex is FormatException)
				{
					return "Note! Parameter [:seed] is an int with \":\" before it!";
				}
				return ex.ToString();
			}
		}

		public static string Effect(params string[] args)
		{
			try
			{
				if (args == null || args.Length < 1)
				{
					return "Wrong Arguments! Try: effect [type | list | clear]";
				}
				if (args[0].ToLower() == "clear")
				{
					CameraEffectManager.Clear();
					return "Removed all image effects.";
				}
				if (args[0].ToLower() == "list")
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("Image effect types:");
					string[] names = Enum.GetNames(typeof(CameraEffectType));
					foreach (string value in names)
					{
						stringBuilder.AppendLine(value);
					}
					return stringBuilder.ToString().TrimEnd('\r', '\n');
				}
				Dictionary<string, CameraEffectType> dictionary = new Dictionary<string, CameraEffectType>();
				foreach (int value3 in Enum.GetValues(typeof(CameraEffectType)))
				{
					dictionary[((CameraEffectType)value3).ToString().ToLower()] = (CameraEffectType)value3;
				}
				CameraEffectType value2;
				if (dictionary.TryGetValue(args[0].ToLower(), out value2))
				{
					CameraEffectManager.Show(value2);
					return string.Concat("Added ", value2, " image effect.");
				}
				return "There is no such camera effect with name: " + args[0];
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string SetGameSpeed(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: set time value";
			}
			float result = 0f;
			if (float.TryParse(args[0], out result))
			{
				if (result < 0f)
				{
					return "Game speed can not be negative!";
				}
				Time.timeScale = result;
				return "Game speed has been changed!";
			}
			return "Unknown arguments!";
		}

		public static string NodeCordinates(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: set node name";
			}
			if (!Manager.IsRun)
			{
				return "It's run command";
			}
			ModelHuman modelHuman = RunMainController.Models[0];
			ModelNode modelNode = modelHuman.NodeToLow(args[0]);
			if (modelNode == null)
			{
				return "Node not found";
			}
			return "X=" + modelNode.Start.X + " Y=" + modelNode.Start.Y + " Z=" + modelNode.Start.Z;
		}

		public static string LogStringBuffer(params string[] args)
		{
			return StringBuffer.Log();
		}

		public static string SkipTutorials(params string[] p_args)
		{
			UserSwitcher.SwitchToTutorialEndUser();
			DataLocal.Current.CounterController.CounterFloor = 0;
			return "Tutorials turned off";
		}

		public static string SwitchToZone2User(params string[] p_args)
		{
			string p_id = "default";
			if (p_args.Length > 0)
			{
				p_id = p_args[0];
			}
			UserSwitcher.SwitchToZone2User(p_id);
			DataLocal.Current.CounterController.CounterFloor = 0;
			return "Switched to zone2";
		}

		public static string SwitchToEscapeUser(params string[] p_args)
		{
			UserSwitcher.SwitchToEscapeUser();
			DataLocal.Current.CounterController.CounterFloor = 0;
			return "Switched to Escape User";
		}

		public static string FinishLevel(params string[] args)
		{
			if (Manager.IsRun)
			{
				RunMainController.SimulateWinEvent();
				return "Level finished";
			}
			return "You canэt finish level now";
		}

		public static string UsePreset(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "No Arguments! Try: preset [string]";
			}
			try
			{
				string text = args[0].ToLower();
				if (text == "help")
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("preset dstunts - unlock all stunts \n");
					stringBuilder.Append("preset dtimer -  no death in because of timer anymore \n");
					stringBuilder.Append("preset dhack - ability to hack all locks \n");
					stringBuilder.Append("preset dcard <name> -- get speciefic card by name \n");
					stringBuilder.Append("preset dcoupon <name> -- get speciefic coupon by name \n");
					stringBuilder.Append("preset godmode - all debug presets just listed \n");
					stringBuilder.Append("preset dnone - cancel all debug presets \n");
					stringBuilder.Append("preset activatepack - grants SuperPack \n");
					stringBuilder.Append("preset key_blue - blue key \n");
					stringBuilder.Append("preset key_red - red key \n");
					stringBuilder.Append("preset key_yellow - yellow key \n");
					return stringBuilder.ToString();
				}
				MainRandom.InitRandomIfNotYet();
				Preset presetByName = PresetsManager.GetPresetByName(text, true);
				if (presetByName != null)
				{
					int i = 1;
					for (int num = args.Length; i < num; i++)
					{
						StringBuffer.AddString("Arg" + (i - 1), args[i]);
					}
					presetByName.RunPreset();
					DataLocal.Current.Save(false);
					return "preset was used";
				}
				return "invalid preset name: " + text;
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string TestGenerator(params string[] args)
		{
			if (args == null || args.Length < 2)
			{
				return "No Arguments! Try: testgenerator [int] [int] (int)";
			}
			try
			{
				int p_floor = int.Parse(args[0]);
				int p_iteration = int.Parse(args[1]);
				int p_seed = ((args.Length != 3) ? (-1) : int.Parse(args[2]));
				ApplicationController.Current.StartCoroutine(GeneratorTester.StartTest(p_iteration, p_floor, p_seed));
				return "Test Start";
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public static string TestRoom(params string[] args)
		{
			if (args == null || args.Length < 3)
			{
				return "No Arguments! Try: testroom [string] [floor int] [position int] (writeCombination int)";
			}
			int p_floor = int.Parse(args[1]);
			int p_position = int.Parse(args[2]);
			bool p_writeCombination = args.Length != 4 || int.Parse(args[3]) != 0;
			ApplicationController.Current.StartCoroutine(RoomGeneratorTest.StartTest(args[0].ToLower(), p_position, p_floor, p_writeCombination));
			return "Ok";
		}

		public static string SaveUser(params string[] args)
		{
			if (args.Length > 0)
			{
				DataLocal.Current.DumpSave();
				return "User dump successfully saved!";
			}
			DataLocal.Current.Save(true);
			return "User successfully saved!";
		}

		public static string LoadDemo(params string[] args)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.Demo);
			if (!directoryInfo.Exists)
			{
				return "No demo files!";
			}
			FileInfo[] files = directoryInfo.GetFiles("*.dmo", SearchOption.AllDirectories);
			if (args == null || args.Length < 1)
			{
				if (files.Length == 0)
				{
					return "No demo files!";
				}
				files = files.OrderByDescending((FileInfo t) => t.CreationTime).ToArray();
				Demo.Play(files[0].GetFullName());
				return "Play demo name: " + files[0].Name;
			}
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				if (Path.GetFileNameWithoutExtension(fileInfo.Name).ToLower() == args[0])
				{
					Demo.Play(fileInfo.GetFullName());
					return "Play demo name: " + args[0];
				}
			}
			return "Not file demo name: " + args[0];
		}

		public static string SwitchChoicesDebugDataAndSetDefault(params string[] p_args)
		{
			if (Manager.IsRun)
			{
				RunStats.Current.LabelChoicesData.gameObject.SetActive(!RunStats.Current.LabelChoicesData.gameObject.activeSelf);
				Settings.Visual.ShowChoices = RunStats.Current.LabelChoicesData.gameObject.activeSelf;
				Settings.Save();
				return "Switch choices debug data: " + Settings.Visual.ShowChoices.ToString().ToLower();
			}
			return "Can't switch choices debug data now!";
		}

		public static string GameCenterCommand(params string[] args)
		{
			if (args.Length == 0)
			{
				return "[GameCenter]: No Arguments! Try: gc signin/signout/show/reset/log";
			}
			if (args[0] == "signin")
			{
				GameCenterController.SignIn();
				return string.Format("gc - signin, authenticated = {0}", UnityEngine.Social.localUser.authenticated);
			}
			if (args[0] == "signout")
			{
				GameCenterController.SignOut();
				return string.Format("gc - signout, authenticated = {0}", UnityEngine.Social.localUser.authenticated);
			}
			if (args[0] == "show")
			{
				GameCenterController.ShowAchiements();
				return "gc show, ShowAchiementsUI";
			}
			if (args[0] == "reset")
			{
				GameCenterController.ResetAchievements();
				return "gc reset, ResetAchievements";
			}
			if (args[0] == "log")
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("gc log, LogAchievements");
				AchievementsManager.Log(stringBuilder);
				return stringBuilder.ToString();
			}
			return string.Format("unknown argument {0}", args[0]);
		}

		public static string Advesting(params string[] args)
		{
			if (args.Length == 0 || args.Length > 1)
			{
				return "No Arguments or more 1!";
			}
			switch (args[0].ToLower())
			{
			case "v":
			case "video":
				ADSystem.Current.OnAdReady += delegate(ADSystem.ADType obj)
				{
					if (obj == ADSystem.ADType.InterstitialAd)
					{
						StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "v_start" } });
						ADSystem.Current.ShowInterstitialAD();
					}
				};
				ADSystem.Current.RequestInterstitialAD();
				break;
			case "rw":
				ADSystem.Current.OnAdReady += delegate(ADSystem.ADType obj)
				{
					if (obj == ADSystem.ADType.RewardedVideo)
					{
						StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "rv_start" } });
						ADSystem.Current.ShowRewardedVideo();
					}
				};
				ADSystem.Current.RequestRewardedVideo(false);
				break;
			case "ow":
				ADSystem.Current.OnAdReady += delegate(ADSystem.ADType obj)
				{
					if (obj == ADSystem.ADType.TapjoyOfferWall)
					{
						StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "tjow_start" } });
						ADSystem.Current.ShowTapjoyOfferwall();
					}
				};
				ADSystem.Current.RequestTapjoyOfferWall();
				break;
			}
			return "Ok";
		}

		public static string SetCounter(params string[] args)
		{
			if (args.Length < 3)
			{
				return "No enought arguments! Try \"setcounter (namespace) (counter) (value)\"";
			}
			string p_nameSpace = args[0];
			string p_name = args[1];
			int p_value = int.Parse(args[2]);
			CounterController.Current.CreateCounterOrSetValue(p_name, p_value, p_nameSpace);
			DataLocal.Current.Save(false);
			return "Ok";
		}

		public static string GetCounter(params string[] p_args)
		{
			if (p_args.Length < 2)
			{
				return "No enought arguments! Try \"getcounter (namespace) (counter)\"";
			}
			string text = p_args[0];
			string text2 = p_args[1];
			int num = CounterController.Current.GetUserCounter(text2, text);
			DataLocal.Current.Save(false);
			return text + "." + text2 + "=" + num;
		}

		public static string SetABGroup(params string[] args)
		{
			if (args.Length == 0)
			{
				ABTestManager.SetABGroup(string.Empty, string.Empty);
				return "Reset AB-group to \"\"";
			}
			if (args.Length == 2)
			{
				string text = args[0];
				string hash = args[1];
				ABTestManager.SetABGroup(text, hash);
				return "Set AB-group to " + text + " with hash: hash";
			}
			return "Too many arguments";
		}

		public static string ServerOnOff(params string[] args)
		{
			if (args.Length != 1)
			{
				return "Try \"server on\" or \"server off\"";
			}
			if (args[0] == "on")
			{
				Settings.IsServerOn = true;
				Settings.Save();
			}
			else
			{
				Settings.IsServerOn = false;
				Settings.Save();
			}
			return "Ok";
		}

		public static string SetRelease(params string[] p_args)
		{
			bool flag = true;
			if (p_args.Length >= 1)
			{
				flag = ((!(p_args[0] == "0")) ? true : false);
			}
			if (Settings.IsReleaseBuild != flag)
			{
				Settings.IsReleaseBuild = flag;
				Settings.Save();
				ApplicationController.Quit();
			}
			return "Ok";
		}

		public static string RegenerateBasket(params string[] p_args)
		{
			if (Manager.IsTerminal)
			{
				Scene<TerminalScene>.Current.Regenerate();
				return "Regenerate terminal items";
			}
			return "Nothing to regenerate";
		}

		public static string RegenerateMissions(params string[] p_args)
		{
			if (Manager.IsShop)
			{
				MissionsManager.CheckMissions();
				MissionsManager.GenerateMissions();
				if (p_args.Length > 0 && p_args[0] == "-d")
				{
					return "Mission items have been regenerated (no dialog)";
				}
				DialogNotificationManager.ShowMissionsDialog(true, 0);
				DialogNotificationManager.DialogsQueue.ShowNext();
				return "Mission items have been regenerated";
			}
			return "Can't regenerate";
		}

		public static string Loss(params string[] p_args)
		{
			if (Manager.IsRun)
			{
				RunMainController.SimulateLossEvent(ModelHuman.ModelState.Loss);
				return "Ok";
			}
			return "It works only in run!";
		}

		public static string GiveBoosterpacks(params string[] p_args)
		{
			int result = 1;
			if (p_args.Length > 0)
			{
				int.TryParse(p_args[0], out result);
			}
			if (result > 0)
			{
				Preset presetByName = PresetsManager.GetPresetByName("BaseBoosterpack");
				for (int i = 0; i < result; i++)
				{
					presetByName.RunPreset();
				}
			}
			else if (result < 0)
			{
				BoosterpacksManager.BoosterpackQuantity = Mathf.Max(0, BoosterpacksManager.BoosterpackQuantity + result);
			}
			DataLocal.Current.Save(false);
			BoosterpackPanel module = UIModule.GetModule<BoosterpackPanel>();
			if (module != null && module.IsActive)
			{
				module.UpdateOpenButton();
			}
			ArchivePanel module2 = UIModule.GetModule<ArchivePanel>();
			if (module2 != null && module2.IsActive)
			{
				module2.Refresh();
			}
			ArchiveCategoryPanel module3 = UIModule.GetModule<ArchiveCategoryPanel>();
			if (module3 != null && module3.IsActive)
			{
				module3.Refresh();
			}
			BottomPanel module4 = UIModule.GetModule<BottomPanel>();
			if (module4 != null)
			{
				module4.UpdateBoosterpackAnnounce();
			}
			return "Ok";
		}

		public static string OpenBoosterpacks(params string[] p_args)
		{
			int result = 1;
			if (p_args.Length > 0)
			{
				int.TryParse(p_args[0], out result);
			}
			if (BoosterpacksManager.BoosterpackQuantity == 0)
			{
				return "Has no boosterpacks to open!";
			}
			if (BoosterpackItemsManager.IsBoosterpackOpening)
			{
				return "Get current boosterpack reward first!";
			}
			while (result > 0 && BoosterpacksManager.BoosterpackQuantity > 0)
			{
				BoosterpackItemsManager.OpenBoosterpackAndGiveRewards();
				result--;
			}
			BoosterpackPanel module = UIModule.GetModule<BoosterpackPanel>();
			if (module != null && module.IsActive)
			{
				module.UpdateOpenButton();
			}
			ArchivePanel module2 = UIModule.GetModule<ArchivePanel>();
			if (module2 != null && module2.IsActive)
			{
				module2.Refresh();
			}
			ArchiveCategoryPanel module3 = UIModule.GetModule<ArchiveCategoryPanel>();
			if (module3 != null && module3.IsActive)
			{
				module3.Refresh();
			}
			BottomPanel module4 = UIModule.GetModule<BottomPanel>();
			if (module4 != null)
			{
				module4.UpdateArchiveAnnounce();
				module4.UpdateBoosterpackAnnounce();
			}
			return "Ok";
		}

		public static string SetZone(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "Try \"zone [int]\"";
			}
			int result = 0;
			if (!int.TryParse(p_args[0], out result))
			{
				return "Incorrect zone number. Try \"zone [int]\"";
			}
			switch (result)
			{
			case 1:
				ZoneManager.CurrentZone = Zone.Zone1;
				break;
			case 2:
				ZoneManager.CurrentZone = Zone.Zone2;
				break;
			default:
				return "error - no such zone '" + result + "' detected";
			}
			return "zone Ok";
		}

		public static string SetAntiAliasing(params string[] p_args)
		{
			int antiAliasing = 0;
			if (p_args.Length > 0)
			{
				antiAliasing = int.Parse(p_args[0]);
			}
			QualitySettings.antiAliasing = antiAliasing;
			return "Set antiAliasing to " + antiAliasing + "!";
		}

		public static string SaveMe(params string[] p_args)
		{
			if (Manager.IsRun)
			{
				RunMainController.SimulateLossEvent(ModelHuman.ModelState.DeadlyDamage);
				return "Ok";
			}
			return "It works only in run!";
		}

		public static string GiveCoupon(params string[] p_args)
		{
			int result = 1;
			if (p_args.Length > 0)
			{
				int.TryParse(p_args[0], out result);
			}
			CouponsManager.AddAllCoupons(result);
			DataLocal.Current.Save(false);
			ArchiveCategoryPanel module = UIModule.GetModule<ArchiveCategoryPanel>();
			if (module != null && module.IsActive)
			{
				module.Refresh();
			}
			return "Ok";
		}

		public static string GiveCard(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "Try \"card [name] (count) \"";
			}
			string p_cardName = p_args[0];
			int result = 1;
			if (p_args.Length > 1)
			{
				int.TryParse(p_args[1], out result);
			}
			CardsGroupAttribute cardsGroupAttribute = CardsGroupAttribute.Create(p_cardName);
			if (cardsGroupAttribute.AddToUser())
			{
				result--;
			}
			cardsGroupAttribute.UserIncrementCardProgress(result);
			DataLocal.Current.Save(false);
			BottomPanel module = UIModule.GetModule<BottomPanel>();
			if (module != null)
			{
				module.UpdateArchiveAnnounce();
			}
			return "Ok";
		}

		public static string PaidVersion(params string[] p_args)
		{
			if (p_args.Length < 1)
			{
				return "Wrong number of arguments! Should be only 1. Try 'paid 1'";
			}
			int result = 0;
			if (!int.TryParse(p_args[0], out result))
			{
				return string.Format("Can't parse '{0}' into int. Try 'paid 1'", p_args[0]);
			}
			bool isPaidVersion = result == 1;
			DataLocal.Current.IsPaidVersion = isPaidVersion;
			DataLocal.Current.Save(false);
			return string.Format("PaidVerson='{0}' OK", result);
		}

		public static string CardsLevelUp(params string[] p_args)
		{
			if (Manager.IsRun)
			{
				return "Can't levelup cards in run!";
			}
			int num = DataLocalHelper.MakeCardsLevelUp();
			if (num > 0)
			{
				ArchivePanel module = UIModule.GetModule<ArchivePanel>();
				if (module != null && module.IsActive)
				{
					module.Refresh();
				}
				ArchiveCategoryPanel module2 = UIModule.GetModule<ArchiveCategoryPanel>();
				if (module2 != null && module2.IsActive)
				{
					module2.Refresh();
				}
				BottomPanel module3 = UIModule.GetModule<BottomPanel>();
				if (module3 != null)
				{
					module3.UpdateArchiveAnnounce();
				}
			}
			return "LevelUp " + num + " cards.";
		}

		public static string PaymentCommand(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "No Arguments! Try: payment restore";
			}
			if (p_args[0] == "restore")
			{
				PaymentController.Current.RestorePurchases();
				return "Ok";
			}
			if (p_args[0] == "products")
			{
				ProductManager.Current.GetProductsData();
				return "Ok";
			}
			return "Unknown subcommand!";
		}

		public static string SaveMeGodMode(params string[] p_args)
		{
			if (p_args.Length < 1)
			{
				return "Wrong number of arguments! Should be only 1. Try 'sm 1'";
			}
			int result = 0;
			if (!int.TryParse(p_args[0], out result))
			{
				return string.Format("Can't parse '{0}' into int. Try 'sm 1'", p_args[0]);
			}
			ControllerSaveMe.SaveMeGodMode = result == 1;
			return string.Format("SaveMe GodeMode='{0}' OK", result);
		}

		public static string BundleCommand(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "No Arguments! Try: bundle [on/off/request/dialog/reset]";
			}
			if (p_args[0] == "on" || p_args[0] == "off")
			{
				Settings.IsAssetBundleOn = p_args[0] == "on";
				Settings.Save();
				return "Ok";
			}
			if (p_args[0] == "request")
			{
				if (p_args.Length < 3)
				{
					return "Need more arguments! Try: bundle [request] [id] [version]";
				}
				string text = p_args[1];
				int result = 0;
				int.TryParse(p_args[2], out result);
				BundleManager.CreateBundleRequestWithCheckingUpdate(text, result, true);
				return string.Format("Create bundle request: {0}, version: {1}", text, result);
			}
			if (p_args[0] == "dialog")
			{
				if (Settings.IsAssetBundleOn && BundleManager.IsUpdateAvailable)
				{
					DialogNotificationManager.ShowBundleRequestDialog(BundleManager.IsRequiredUpdateAvailable, BundleManager.RequestsTotalContentLengthInMb);
				}
				return "Ok";
			}
			if (p_args[0] == "reset")
			{
				VectorPaths.ResetBundleCache();
				return "Ok";
			}
			return "Unknown command!";
		}

		public static string UrlCommand(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "No Arguments! Try: url [app/sf2/sf3/faq/ost/support]";
			}
			string empty = string.Empty;
			switch (p_args[0])
			{
			case "app":
				empty = UrlManager.ApplicationUrl;
				break;
			case "sf2":
				empty = UrlManager.BannerSF2Url;
				break;
			case "sf3":
				empty = UrlManager.BannerSF3Url;
				break;
			case "faq":
				empty = UrlManager.FAQUrl;
				break;
			case "ost":
				empty = UrlManager.OSTUrl;
				break;
			case "support":
				empty = UrlManager.SupportUrl;
				break;
			default:
				return "Unknown url";
			}
			if (!string.IsNullOrEmpty("url"))
			{
				Application.OpenURL(empty);
			}
			return "Open URL: " + empty;
		}

		public static string ProfilerCommand(params string[] p_args)
		{
			if (p_args.Length == 0)
			{
				return "No Arguments! Try: pf [mc]";
			}
			switch (p_args[0])
			{
			case "mc":
				Resources.UnloadUnusedAssets();
				GC.Collect();
				return "Ok";
			default:
				return "Unknown profiler command!";
			}
		}
	}
}
