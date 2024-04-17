using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassicSchoolhouse.patches
{
    [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ElevatorClosed))]
    internal class ElevatorClosedEffect
    {
        static bool Prefix(BaseGameManager __instance)
        {
            __instance.Ec.gameObject.GetComponent<ClassicSchoolhouseManager>().ElevatorClosed();
            return true;
        }
    }
}
