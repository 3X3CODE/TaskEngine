using HarmonyLib;
using TaskEngine.TaskHelpers;

namespace TaskEngine.Patches;

// due to CustomTaskTypes != TaskTypes, we need to make sure the task name shows up properly
// currently bypassing the translator for now

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(TaskTypes))]
public class TranslationControllerPatches
{
    public static bool Prefix(TaskTypes task, ref string __result) 
    {
        int id = (int)task;

        if (TaskRegistry.IsCustomTask(id))
        {
            if (TaskRegistry.TaskNames.TryGetValue(id, out string name)) 
            {
                __result = name;
                return false;
            }

            __result = $"Custom Task ({id})";
            return false;
        }

        return true;
    }
}