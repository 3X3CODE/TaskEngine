using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TaskEngine.TaskHelpers;

namespace TaskEngine;

// Developed by 3X3C | 2026.02.08 | TaskEngine
// Designed to enhance your task experience

// Last checked 2026.02.15

// Why are you reading this?

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.None)]
public partial class TaskEnginePlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static new ManualLogSource LogSource = null!;
    
    public override void Load()
    {
        _ = CustomTaskTypes.PlugLeaks;

        if (!Directory.Exists(CustomPaths.taskFolder))
        {
            Directory.CreateDirectory(CustomPaths.taskFolder);
            return;
        }

        LogSource = base.Log;
        
        ReactorCredits.Register("TaskEngine", "0.0.1", false, null);
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}