using HarmonyLib;
using ClassicSchoolhouse.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClassicSchoolhouse.patches
{
#if DEBUG

    [HarmonyPatch(typeof(EnvironmentController), nameof(EnvironmentController.NPCSpawner))]
    public static class InstantMap
    {
        static void Postfix(EnvironmentController __instance, ref IEnumerator __result)
        {
            void postfixAction() {
                foreach (var npc in __instance.npcs)
                {
                    __instance.map.AddArrow(npc.transform, Color.red);
                }
            }

            void prefixAction() {
                __instance.map.CompleteMap();
            }

            __result = new SimpleEnumerator() { enumerator = __result, postfixAction = postfixAction, prefixAction = prefixAction }.GetEnumerator();
        }
    }

    [HarmonyPatch(typeof(HappyBaldi), nameof(HappyBaldi.Activate))]
    public static class NoHappyBaldi
    {
        static void Prefix(HappyBaldi __instance)
        {
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<BaseGameManager>.Instance.BeginSpoopMode();
            __instance.ec.SpawnNPCs();
            //__instance.ec.GetBaldi().Despawn();
            __instance.activated = true;
            __instance.ec.StartEventTimers();
            __instance.sprite.enabled = false;
            Singleton<BaseGameManager>.Instance.CollectNotebooks(__instance.ec.notebookTotal);
            GameObject.Destroy(__instance.gameObject);
        }
    }

    public static class FastPlayer
    {
        public static void Play(MonoBehaviour behaviour)
        {
            behaviour.StartCoroutine(DelayGoToMainMenu(behaviour));
        }
        private static IEnumerator DelayGoToMainMenu(MonoBehaviour behaviour)
        {
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadSceneAsync("MainMenu").completed += (AsyncOperation _) => behaviour.StartCoroutine(DelayGoToGame());

        }
        private static IEnumerator DelayGoToGame()
        {
            yield return new WaitForSeconds(0.1f);
            GameLoader loader = null;
            while (loader == null)
            {
                loader = GameObject.FindObjectOfType<GameLoader>(includeInactive: true);
                yield return null;
            }
            loader.gameObject.SetActive(true);
            loader.Initialize(2);
            loader.LoadLevel(loader.list.scenes[2]);
        }
    }
    [HarmonyPatch(typeof(GameInitializer), nameof(GameInitializer.WaitForGenerator))]
    class FastBeginPlayPatch
    {

        static void Postfix(ref IEnumerator __result)
        {
            Action prefixAction = () => {
                  
            };
            Action postfixAction = () =>
            {
                Singleton<BaseGameManager>.Instance.BeginPlay();
            };
            Action<object> preItemAction = (item) => { };
            Action<object> postItemAction = (item) => { };
            Func<object, object> itemAction = (item) => { return item; };
            var myEnumerator = new SimpleEnumerator()
            {
                enumerator = __result,
                prefixAction = prefixAction,
                postfixAction = postfixAction,
                preItemAction = preItemAction,
                postItemAction = postItemAction,
                itemAction = itemAction
            };
            __result = myEnumerator.GetEnumerator();
        }
    }
#endif
}
