using BepInEx.Configuration;
using MTM101BaldAPI.OptionsAPI;
using System;
using UnityEngine;

namespace ClassicSchoolhouse
{
    internal class ClassicSchoolhouseMenu : MonoBehaviour
    {
        public static ConfigEntry<bool> enableEscape;
        public static ConfigEntry<int> notebookMinimum;
        public static ConfigEntry<int> elevatorMinimum;

        private MenuToggle toggleSchoolhouseEscape;

        private void Initialize(OptionsMenu __instance)
        {
            toggleSchoolhouseEscape = CustomOptionsCore.CreateToggleButton(__instance,
                new Vector2(20f, 0), "Enable Escape",
                enableEscape.Value,
                "Enable the Schoolhouse Escape effect from BBCR."
            );
            toggleSchoolhouseEscape.transform.SetParent(transform, false);
            toggleSchoolhouseEscape.hotspot.GetComponent<StandardMenuButton>().OnPress.AddListener(() => enableEscape.Value = toggleSchoolhouseEscape.Value);
        }

        internal static void OnMenuInitialize(OptionsMenu __instance)
        {
            var obj = CustomOptionsCore.CreateNewCategory(__instance, "Classic Schoolhouse");
            var phontyMenu = obj.AddComponent<ClassicSchoolhouseMenu>();
            phontyMenu.Initialize(__instance);
        }
        /// <summary>
        /// Triggered when launching the mod, mostly to setup BepInEx config bindings.
        /// </summary>
        internal static void Setup()
        {
            enableEscape = ClassicSchoolhouse.Instance.Config.Bind("SchoolhouseEscape", "EnableEscape", true, "Enable the Classic Remastered like escape effect.");
            notebookMinimum = ClassicSchoolhouse.Instance.Config.Bind("SchoolhouseEscape", "MinimumNotebooks", 1, "Minimum amount of total notebooks to enable the effect.");
            elevatorMinimum = ClassicSchoolhouse.Instance.Config.Bind("SchoolhouseEscape", "MinimumElevators", 1, "Minimum amount of total elevators to enable the effect.");
        }
    }
}
