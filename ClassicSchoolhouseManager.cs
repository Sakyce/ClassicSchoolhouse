using Microsoft.Win32.SafeHandles;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ClassicSchoolhouse
{
    public class ClassicSchoolhouseManager : MonoBehaviour
    {
        private LoopingSoundObject chaos1;
        private LoopingSoundObject chaos2;
        private int closedElevators = 0;
        private EnvironmentController ec;
        private Color[,] originalColors;
        private System.Random rng;

        public void Initialize(EnvironmentController ec)
        {
            this.ec = ec;

            var chaos = ClassicSchoolhouse.assetManager.Get<SoundObject[]>("Chaos");

            chaos1 = ScriptableObject.CreateInstance<LoopingSoundObject>();
            chaos1.clips = new AudioClip[] { chaos[0].soundClip, chaos[1].soundClip };
            chaos1.mixer = chaos[0].mixerOverride;

            chaos2 = ScriptableObject.CreateInstance<LoopingSoundObject>();
            chaos2.clips = new AudioClip[] { chaos[2].soundClip, chaos[3].soundClip };
            chaos2.mixer = chaos[0].mixerOverride;

            originalColors = new Color[ec.levelSize.x, ec.levelSize.z];

            rng = new System.Random(Singleton<CoreGameManager>.Instance.Seed());

            FindHallwayToReplaceLight(rng);

            //ec.lights.ForEach(e => e.lightStrength = 1);
            
            // is this the backrooms
            // ec.lights.ForEach(e => ReplaceLightByBuzzing(e));
        }
        public List<Cell> GetNearLights(Cell cell, int rangeSquare)
        {
            //Debug.Log($"Getting nearby lights of {cell.position} at a distance of {rangeSquare}");

            var nearbyLights = new List<Cell>();
            foreach (var nearby in ec.lights)
            {
                var diffx = Math.Abs(nearby.position.x - cell.position.x);
                var diffz = Math.Abs(nearby.position.z - cell.position.z);
                //Debug.Log($"Distance of {diffx} {diffz}");
                if (diffx < rangeSquare && diffz < rangeSquare)
                {
                    nearbyLights.Add(nearby);
                }
            }
            return nearbyLights;
        }
        public void OnDestroy()
        {
            Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Transpose = 0;
            Singleton<MusicManager>.Instance.StopFile();
            for (int i = 0; i < Singleton<MusicManager>.Instance.MidiPlayer.Channels.Length; i++)
            {
                Singleton<MusicManager>.Instance.MidiPlayer.MPTK_ChannelEnableSet(i, true);
            }
        }

        public void FindHallwayToReplaceLight(System.Random rng)
        {
            var hallwaysLights = (from x in ec.lights where x.room.type == RoomType.Hall select x).ToArray();
            var randomLight =  hallwaysLights[rng.Next(0, hallwaysLights.Count())];
            ReplaceLightByBuzzing(randomLight);
        }
        public void ReplaceLightByBuzzing(Cell cell)
        {
            // Turn off nearby lights
            foreach (var nearby in GetNearLights(cell, 10))
            {
                if (nearby.room.type == RoomType.Hall)
                {
                    ec.SetLight(false, nearby);
                    nearby.lightOn = false;
                    nearby.hasLight = false;
                } else
                {
                    nearby.lightStrength /= 2;
                }
            }
            ec.SetLight(false, cell);
            cell.hasLight = false;
            cell.lightOn = false;

            // Add buzzing
            var audman = cell.tile.gameObject.AddComponent<PropagatedAudioManager>();
            audman.QueueAudio(ClassicSchoolhouse.assetManager.Get<SoundObject>("Buzz"));
            audman.volumeModifier *= 2;
            audman.maxDistance /= 2;
            audman.SetLoop(true);
        }

        // Copied from ClassicBaseManager and I feel absolutely no remorse
        private IEnumerator LightChanger(List<Cell> lights, bool on, float delay)
        {
            while (lights.Count > 0)
            {
                yield return new WaitForSeconds(delay);

                int index = UnityEngine.Random.Range(0, lights.Count);
                originalColors[lights[index].position.x, lights[index].position.z] = lights[index].lightColor;
                lights[index].lightColor = Color.red;
                ec.SetLight(on, lights[index]);
                lights.RemoveAt(index);
            }

        }
        // Also shamelessly copied from BBCR.
        public void ElevatorClosed()
        {
            closedElevators += 1;

            if (ec.elevators.Count < ClassicSchoolhouseMenu.elevatorMinimum.Value || ec.notebookTotal < ClassicSchoolhouseMenu.notebookMinimum.Value)
            {
                return;
            }

            if (closedElevators == 1) {
                var untitledList = new List<Cell>();
                foreach (Cell cell in ec.lights)
                {
                    if (cell.lightStrength <= 1)
                    {
                        cell.lightColor = Color.red;
                        ec.SetLight(true, cell);
                    }
                    else
                    {
                        untitledList.Add(cell);
                    }
                }
                Shader.SetGlobalColor("_SkyboxColor", Color.red);
                Singleton<MusicManager>.Instance.SetSpeed(0.1f);
                StartCoroutine(LightChanger(untitledList, true, 0.7f));
            } 
            else if (closedElevators == 2)
            {
                Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Transpose = UnityEngine.Random.Range(-24, -12);
                Singleton<MusicManager>.Instance.QueueFile(chaos1, true);
            } 
            else if (closedElevators == 3)
            {
                Singleton<MusicManager>.Instance.QueueFile(chaos2, true);
                ec.standardDarkLevel = new Color(0.2f, 0, 0);
                ec.FlickerLights(true);
                for (int i = 0; i < Singleton<MusicManager>.Instance.MidiPlayer.Channels.Length; i++)
                {
                    Singleton<MusicManager>.Instance.MidiPlayer.MPTK_ChannelEnableSet(i, false);
                }
            }
        }

        public static ClassicSchoolhouseManager Create(EnvironmentController ec)
        {
            var manager = ec.gameObject.AddComponent<ClassicSchoolhouseManager>();
            manager.Initialize(ec);
            return manager;
        }
    }
}
