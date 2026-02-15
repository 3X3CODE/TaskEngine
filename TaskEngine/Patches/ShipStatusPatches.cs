using HarmonyLib;
using TaskEngine.ReadExternal;
using TaskEngine.TaskHelpers;
using UnityEngine;

namespace TaskEngine.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
public static class FungleStatusPatches
{
    [HarmonyPostfix]
    public static void applyTasks(ShipStatus __instance)
    {
        GameObject refHolder = new GameObject("ReferenceHolder");
        Executor executor = refHolder.AddComponent<Executor>();
        ScriptReader reader = refHolder.AddComponent<ScriptReader>();
        refHolder.SetActive(false);
        reader.EnsureLoad();
        executor.EnsureLoad();
        refHolder.transform.SetParent(__instance.transform);
        
        foreach (CustomTask task in executor.CustomTasks)
        {
            TaskEnginePlugin.LogSource.LogInfo("[ShipStatus] Proceeding to apply custom task");
            
            switch (task.ShipName)
            {
                case "Skeld":
                    
                    if (__instance.Type == ShipStatus.MapType.Ship)
                    {
                        TaskManager.InjectCustomMinigame(__instance, task.customType, task.minigame, task.baseType,
                            task.consoleName, task.maxStep, task.showStep);
                    }
                    break;
                
                case "Mira":

                    if (__instance.Type == ShipStatus.MapType.Hq)
                    {
                        TaskManager.InjectCustomMinigame(__instance, task.customType, task.minigame, task.baseType,
                            task.consoleName, task.maxStep, task.showStep);
                    }
                    break;
                
                case "Polus":
                    
                    if (__instance.Type == ShipStatus.MapType.Pb)
                    {
                        TaskManager.InjectCustomMinigame(__instance, task.customType, task.minigame, task.baseType,
                            task.consoleName, task.maxStep, task.showStep);
                    }
                    break;
                
                case "Airship":

                    if (__instance.Type == ShipStatus.MapType.Ship)
                    {
                        TaskManager.InjectCustomMinigame(__instance, task.customType, task.minigame, task.baseType,
                            task.consoleName, task.maxStep, task.showStep);
                    }
                    break;
                
                case "Fungle":
                    
                    if (__instance.Type == ShipStatus.MapType.Fungle)
                    {
                        TaskManager.InjectCustomMinigame(__instance, task.customType, task.minigame, task.baseType,
                            task.consoleName, task.maxStep, task.showStep);
                    }
                    break;
                
                default:
                    TaskEnginePlugin.LogSource.LogInfo($"Could not find the ship: {task.ShipName}");
                    break;
                
            }
        }

        foreach (ReplaceTask task in executor.ReplaceTasks)
        {
            TaskEnginePlugin.LogSource.LogInfo("[ShipStatus] Proceeding to replace task");
            switch (task.ship)
            {
                case "Skeld":
                    
                    if (__instance.Type == ShipStatus.MapType.Ship)
                    {
                        TaskManager.ReplaceBaseTask(__instance, task.firstTask, task.secondtask, task.firstConsole, task.secondConsole, task.overrideSteps, task.firstStep, task.showFirst, task.secondStep, task.showSecond);
                    }
                    break;
                
                case "Mira":

                    if (__instance.Type == ShipStatus.MapType.Hq)
                    {
                        TaskManager.ReplaceBaseTask(__instance, task.firstTask, task.secondtask, task.firstConsole, task.secondConsole, task.overrideSteps, task.firstStep, task.showFirst, task.secondStep, task.showSecond);
                    }
                    break;
                
                case "Polus":
                    
                    if (__instance.Type == ShipStatus.MapType.Pb)
                    {
                        TaskManager.ReplaceBaseTask(__instance, task.firstTask, task.secondtask, task.firstConsole, task.secondConsole, task.overrideSteps, task.firstStep, task.showFirst, task.secondStep, task.showSecond);
                    }
                    break;
                
                case "Airship":

                    if (__instance.Type == ShipStatus.MapType.Ship)
                    {
                        TaskManager.ReplaceBaseTask(__instance, task.firstTask, task.secondtask, task.firstConsole, task.secondConsole, task.overrideSteps, task.firstStep, task.showFirst, task.secondStep, task.showSecond);
                    }
                    break;
                
                case "Fungle":
                    
                    if (__instance.Type == ShipStatus.MapType.Fungle)
                    {
                        TaskManager.ReplaceBaseTask(__instance, task.firstTask, task.secondtask, task.firstConsole, task.secondConsole, task.overrideSteps, task.firstStep, task.showFirst, task.secondStep, task.showSecond);
                    }
                    break;
                
                default:
                    TaskEnginePlugin.LogSource.LogInfo($"Could not find the ship: {task.ship}");
                    break;
                
            }
        }
    }
}
