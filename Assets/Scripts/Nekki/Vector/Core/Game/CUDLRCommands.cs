using System;
using System.IO;
using System.Text;
using CUDLR;
using Nekki.Vector.Core.Console;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class CUDLRCommands
	{
		[Command("help", "help - prints commands", false)]
		public static void Help()
		{
			CUDLR.Console.Log(string.Format("Commands:{0}", CUDLR.Console.Instance.HelpData));
		}

		[Command("clear", "clear - clears CUDLR console output", false)]
		public static void Clear()
		{
			CUDLR.Console.Instance.ConsoleOutput.Clear();
		}

		[Command("uset", "uset - replace user on device", true)]
		public static void UserReplace(string[] p_args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in p_args)
			{
				stringBuilder.Append(value);
			}
			string text = string.Format("{0}/user_CUDLR_{1}.xml", VectorPaths.SavedUsersExternal, DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss"));
			if (!Directory.Exists(VectorPaths.SavedUsersExternal))
			{
				Directory.CreateDirectory(VectorPaths.SavedUsersExternal);
			}
			File.WriteAllText(text, stringBuilder.ToString());
			Debug.Log("User successfully saved in - " + text);
			UserSwitcher.SwitchToUser(text);
		}

		[Command("m1", "m1 [int] - add money1", true)]
		public static void Money1(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Money1, p_args);
		}

		[Command("m2", "m2 [int] - add money2", true)]
		public static void Money2(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Money2, p_args);
		}

		[Command("m3", "m3 [int] - add money3", true)]
		public static void Money3(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Money3, p_args);
		}

		[Command("q", "q - quit from run", true)]
		public static void Quit(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Quit, p_args);
		}

		[Command("user", "user copy | list | switch | print | printm | v - actions by user", true)]
		public static void User(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.UserCommand, p_args);
		}

		[Command("reset", "reset [string] - user | settings | all", true)]
		public static void Reset(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Reset, p_args);
		}

		[Command("reload", "reload user | all - to reload user from user.xml or reload all data", true)]
		public static void Reload(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Reload, p_args);
		}

		[Command("resolution", "resolution [width] [height] (fullscreen) - to set resolution", true)]
		public static void Resolution(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Resolution, p_args);
		}

		[Command("play", "play [room1] [room2] ... [roomN] (:seed) - to play specific rooms", true)]
		public static void Play(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Play, p_args);
		}

		[Command("effect", "effect [type | list | clear] - to show camera effect, to list effect types or to clear from any\noffmessageofscreen 1 or 0", true)]
		public static void Effect(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Effect, p_args);
		}

		[Command("gs", "gs [float] - set game speed", true)]
		public static void GameSpeed(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetGameSpeed, p_args);
		}

		[Command("xy", "xy [node name] - get node position", true)]
		public static void NodeCordinates(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.NodeCordinates, p_args);
		}

		[Command("strings", "strings - log StringsBuffer", true)]
		public static void LogStringBuffer(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.LogStringBuffer, p_args);
		}

		[Command("s", "s - skip all tutorials", true)]
		public static void SkipTutorial(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SkipTutorials, p_args);
		}

		[Command("es", "es - switch to escape", true)]
		public static void BeforeEscape(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SwitchToEscapeUser, p_args);
		}

		[Command("z2", "z2 - switch to zone2", true)]
		public static void Zone2(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SwitchToZone2User, p_args);
		}

		[Command("w", "w - finish floor", true)]
		public static void FinishFloor(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.FinishLevel, p_args);
		}

		[Command("preset", "preset [presetName] - create item by preset or help to help", true)]
		public static void Preset(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.UsePreset, p_args);
		}

		[Command("testgenerator", "testgenerator [floor] [iterations] - test generator", true)]
		public static void TestGenerator(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.TestGenerator, p_args);
		}

		[Command("testroom", "testroom [roomName] [floor] [position] (writeCombination int) - test generator", true)]
		public static void TestRoom(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.TestRoom, p_args);
		}

		[Command("u", "u - save user.xml", true)]
		public static void SaveUser(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SaveUser, p_args);
		}

		[Command("demo", "demo (demoname) - load demo file\nwithout arguments load last demo file", true)]
		public static void Demo(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.LoadDemo, p_args);
		}

		[Command("j", "j - switch choices debug data and set state to default", true)]
		public static void SwitchChoices(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SwitchChoicesDebugDataAndSetDefault, p_args);
		}

		[Command("gc", "gc - game center/ google play games, achievements, sign in/out", true)]
		public static void GameCenter(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.GameCenterCommand, p_args);
		}

		[Command("ad", "ad - advesting, params: v or video, rw (reward video), ow(offer wall)", true)]
		public static void ADS(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Advesting, p_args);
		}

		[Command("setcounter", "setcounter [namespace] [name] [value] - set value of counter", true)]
		public static void SetCounter(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetCounter, p_args);
		}

		[Command("getcounter", "getcounter [namespace] [name] - print value of counter", true)]
		public static void GetCounter(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.GetCounter, p_args);
		}

		[Command("abgroup", "abgroup - ...", true)]
		public static void ABGroup(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetABGroup, p_args);
		}

		[Command("server", "server on | off - send or not data to server", true)]
		public static void Server(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.ServerOnOff, p_args);
		}

		[Command("release", "release (int) - set build to release mode", true)]
		public static void Release(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetRelease, p_args);
		}

		[Command("r", "r - regenerate shop/terminal items", true)]
		public static void RegenerateBasket(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.RegenerateBasket, p_args);
		}

		[Command("rm", "rm (-d) - regenerate missions, optional -d to not show dialog", true)]
		public static void RegenerateMissions(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.RegenerateMissions, p_args);
		}

		[Command("x", "x - loss", true)]
		public static void Loss(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.Loss, p_args);
		}

		[Command("bp", "bp (int) - add/remove boosterpacks", true)]
		public static void AddBoosterpacks(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.GiveBoosterpacks, p_args);
		}

		[Command("bpo", "bpo (int) - open boosterpacks without animation", true)]
		public static void OpenBoosterpacks(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.OpenBoosterpacks, p_args);
		}

		[Command("zone", "zone [integer] - set zone", true)]
		public static void Zone(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetZone, p_args);
		}

		[Command("aa", "aa [int] - set antialiasing level", true)]
		public static void AntiAliasing(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SetAntiAliasing, p_args);
		}

		[Command("v", "v - use saveme (only in run)", true)]
		public static void SaveMe(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SaveMe, p_args);
		}

		[Command("cp", "cp (int) - add all type coupons", true)]
		public static void AddCoupon(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.GiveCoupon, p_args);
		}

		[Command("card", "card [name] (n) - add n cards with name to user (example - \"card BonusTokensFromTricks_2_Legs 5 \")", true)]
		public static void AddCard(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.GiveCard, p_args);
		}

		[Command("paid", "paid [int] - set PaidVersion to 0 or 1", true)]
		public static void Paid(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.PaidVersion, p_args);
		}

		[Command("lvlup", "lvlup - level up all cards if need", true)]
		public static void LevelUp(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.CardsLevelUp, p_args);
		}

		[Command("payment", "payment [command] - some test for payment (\"restore\" - restore purchases, \"products\" - update products from server)", true)]
		public static void Payment(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.PaymentCommand, p_args);
		}

		[Command("sm", "sm [int] - set save me god mode enabled/disabled (always enable 'Watch video' button and infinity attempt)", true)]
		public static void SaveMeGodMode(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.SaveMeGodMode, p_args);
		}

		[Command("bundle", "bundle [command] - some test for bundles (\"on\" - enable asset bundles, \"off\" - disable asset bundles, \"printm\" - print config content, \"requests\" - create required request, \"dialog\" - show bundle requests dialog, \"reset\" - reset bundles cache)\"", true)]
		public static void Bundle(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.BundleCommand, p_args);
		}

		[Command("url", "url [id] - open selected url (\"app\" - application url, \"sf2\" - banner sf2 url, \"sf3\" - banner sf3 url, \"faq\" - FAQ url, \"ost\" - OST url, \"support\" - support url)\"", true)]
		public static void Url(string[] p_args)
		{
			RunConsoleCommand(ConsoleCommands.UrlCommand, p_args);
		}

		private static void RunConsoleCommand(Func<string[], string> p_consoleCommand, string[] p_args)
		{
			string message = p_consoleCommand(p_args);
			Debug.Log(message);
		}
	}
}
