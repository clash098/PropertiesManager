using BepInEx;
using BepInEx.Configuration;
using ComputerInterface.Behaviours;
using ComputerInterface.Interfaces;
using Photon.Pun;
using System.Linq;

namespace PropertiesManager
{
    [BepInPlugin("com.uhclash.gorillatag.propsmanager", "PropertiesManager", "1.0.0")]
    [BepInDependency("tonimacaroni.computerinterface", "1.8.0")]
    public class Plugin : BaseUnityPlugin, ICommandRegistrar
    {
        static ConfigEntry<bool> NukeConfig;
        static readonly ConfigDefinition NukeSetting = new("Property Nuker", "Enabled");
        public static bool NukeEnabled
        {
            get => NukeConfig.Value;
            set => NukeConfig.Value = value;
        }
        void Awake() => NukeConfig = Config.Bind(NukeSetting, false, new ConfigDescription($"Should PropertiesManager disable properties all together?"));
        void ICommandRegistrar.Initialize()
        {
            (this as ICommandRegistrar).RegisterCommands();
            Logger.LogMessage($"Initialized PropertiesManager V{Info.Metadata.Version}");
        }
        void ICommandRegistrar.RegisterCommands()
        {
            var commandHandler = CommandHandler.Singleton;

            commandHandler.AddCommand(new Command("myprops", null, args =>
            {
                return $"Properties:\n\n{string.Join('\n', PhotonNetwork.LocalPlayer.CustomProperties.Select(P => $"{P.Key}: {P.Value}"))}";
            }));

            commandHandler.AddCommand(new Command("clearprops", null, args =>
            {
                PhotonNetwork.SetPlayerCustomProperties(null);
                NetworkSystem.Instance.SetMyTutorialComplete();
                return "Cleared props!";
            }));

            commandHandler.AddCommand(new Command("propnuke", [typeof(bool)], args =>
            {
                if (args.Length == 0 || args[0] is not bool enabled) return "An unexpected error occured.";

                NukeEnabled = enabled;
                return enabled ? "The property nuke has been activated!" : "The property nuke has been disabled!";
            }));
        }
        void OnEnable() => HarmonyPatches.ApplyHarmonyPatches();
        void OnDisable() => HarmonyPatches.RemoveHarmonyPatches();
    }
}