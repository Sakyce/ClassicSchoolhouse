using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using System.IO;
using UnityEngine;

namespace ClassicSchoolhouse
{
	[HarmonyPatch(typeof(WarningScreen), nameof(WarningScreen.Advance))]
	public static class PreventWarningScreenAdvancePatch
	{
		public static bool Prefix(WarningScreen __instance) => !ChangeWarningScreenPatch.active;
	}
	[HarmonyPatch(typeof(WarningScreen), nameof(WarningScreen.Start))]
	public static class ChangeWarningScreenPatch
	{
		public static bool active = false;
		public static bool Prefix(WarningScreen __instance)
		{
			string path = AssetLoader.GetModPath(ClassicSchoolhouse.Instance);
			Debug.Log(path != null ? path : "NO PATH ?");
			if (Directory.Exists(path)) { 
				return true; 
			}

			ChangeWarningScreenPatch.active = true;
			__instance.textBox.text = "Your <color=blue>Classic Schoolhouse</color> installation seems to be <color=red>broken</color> and you should <color=yellow>fix it by reading the instructions provided with the mod!</color>\nThe dlls and dependencies are correctly installed but not the assets! The assets folder should be inside BALDI_DATA/StreamingAssets/Modded\n\n<alpha=#AA>PRESS ALT + F4 TO CLOSE THIS GAME";
			return false;
		}
	}
}