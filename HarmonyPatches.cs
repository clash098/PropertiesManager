using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PropertiesManager;
public class HarmonyPatches
{
    static Harmony instance;
    static bool isPatched = false;
    public static bool IsPatched
    {
        get => isPatched;
        set
        {
            if (IsPatched == value || (instance == null && !value)) return;
            isPatched = value;

            if (value)
            {
                instance ??= Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, "com.uhclash.gorillatag.propsmanager");
                return;
            }

            instance.UnpatchSelf();
            instance = null;
        }
    }
    public static void ApplyHarmonyPatches() => IsPatched = true;
    public static void RemoveHarmonyPatches() => IsPatched = false;
    [HarmonyPatch(typeof(LoadBalancingClient), "OpSetPropertiesOfActor", MethodType.Setter), HarmonyPrefix, HarmonyPriority(100)]
    public static void PropertyPatch(int actorNr, ref Hashtable actorProperties)
    {
        if (!Plugin.NukeEnabled || actorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;

        actorProperties = new() { ["didTutorial"] = PlayerPrefs.GetString("didTutorial", "nope") == "done" };
    }
}