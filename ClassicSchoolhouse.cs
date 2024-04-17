using BepInEx;
using ClassicSchoolhouse.patches;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using System.IO;
using UnityEngine;

namespace ClassicSchoolhouse
{
    [BepInPlugin("sakyce.baldiplus.classic.schoolhouse", "ClassicSchoolhouse", "3.0.4.2")]
    [BepInDependency("mtm101.rulerp.baldiplus.endlessfloors", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
    public class ClassicSchoolhouse : BaseUnityPlugin
    {
        public static AssetManager assetManager = new AssetManager();

#pragma warning disable CS8618 // Initialized in Awake
        public string modpath;
        public static ClassicSchoolhouse Instance;
#pragma warning restore CS8618

        public void Awake()
        {
            // Patching
            Harmony harmony = new Harmony("sakyce.baldiplus.classic.schoolhouse");
            harmony.PatchAllConditionals();
            
            modpath = AssetLoader.GetModPath(this);
            Instance = this;

            // Registering
            LoadingEvents.RegisterOnAssetsLoaded(this.OnAssetsLoaded, false);
            try { EndlessFloorsCompatibility.Initialize(); } catch (FileNotFoundException) { }

            // Options
            CustomOptionsCore.OnMenuInitialize += ClassicSchoolhouseMenu.OnMenuInitialize;
            ClassicSchoolhouseMenu.Setup();

#if DEBUG
            //FastPlayer.Play(this);
#endif
        }

        private void OnAssetsLoaded()
        {
            assetManager.Add("Chaos", new SoundObject[]
            {
                // When reaching the second elevator
                ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "Chaos_EarlyLoopStart.wav"), "", SoundType.Music, Color.black, 0f), 
                ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "Chaos_EarlyLoop.wav"), "", SoundType.Music, Color.black, 0f),
                
                // When reaching the third elevator
                ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "Chaos_Buildup.mp3"), "", SoundType.Music, Color.black, 0f), 
                ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "Chaos_FinalLoop.wav"), "", SoundType.Music, Color.black, 0f),
            });
            assetManager.Add("Buzz", ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "ElectricBuzz.wav"), "* BUZZ *", SoundType.Effect, Color.white));
        }
    }
}
