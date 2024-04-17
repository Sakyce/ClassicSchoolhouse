using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassicSchoolhouse.patches
{
    [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Initialize))]
    internal class InsertClassicSchoolhouseManager
    {
        static bool Prefix(BaseGameManager __instance)
        {
            ClassicSchoolhouseManager.Create(__instance.ec);
            return true;
        }
    }
}
