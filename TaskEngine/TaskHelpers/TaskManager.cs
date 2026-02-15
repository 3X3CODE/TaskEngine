using System;
using System.IO;
using System.Text;
using BepInEx;
using UnityEngine;

namespace TaskEngine.TaskHelpers;

public class TaskManager
{
    string folderPath = Path.Combine(Paths.PluginPath, "CustomTasks");
    
    public NormalPlayerTask[] AllCustomTasks;
    
    public static void InjectCustomMinigame(ShipStatus shipStatus, TaskTypes customType, Minigame minigame, TaskTypes baseType, string consoleName, int maxStep, bool showStep)
    {
        TaskEnginePlugin.LogSource.LogInfo("Starting custom minigame injection");
        NormalPlayerTask minigameNPT = null;

        foreach (NormalPlayerTask playerTask in shipStatus.ShortTasks)
        {
            if (playerTask.TaskType == baseType)
            {
                minigameNPT = playerTask;
                TaskEnginePlugin.LogSource.LogInfo("BaseMinigame was found in Short Tasks");
            }
        }

        if (minigameNPT == null)
        {
            foreach (NormalPlayerTask playerTask in shipStatus.CommonTasks)
            {
                if (playerTask.TaskType == baseType)
                {
                    minigameNPT = playerTask;
                    TaskEnginePlugin.LogSource.LogInfo("BaseMinigame was found in Common Tasks");
                }
            }
        }
        
        if (minigameNPT == null)
        {
            foreach (NormalPlayerTask playerTask in shipStatus.LongTasks)
            {
                if (playerTask.TaskType == baseType)
                {
                    minigameNPT = playerTask;
                    TaskEnginePlugin.LogSource.LogInfo("BaseMinigame was found in Long Tasks");
                }
            }
        }

        if (minigameNPT == null) TaskEnginePlugin.LogSource.LogInfo("BaseMinigame wasn't found");

        minigameNPT.TaskType = customType;
        minigameNPT.MaxStep = maxStep;
        minigameNPT.ShowTaskStep = showStep;
        minigameNPT.MinigamePrefab = minigame;

        Console taskConsole = GameObject.Find(consoleName).GetComponent<Console>();
        taskConsole.TaskTypes[0] = minigameNPT.TaskType;
        
        TaskEnginePlugin.LogSource.LogInfo("Successfully completed Custom Task injection");
    }

    public static void ReplaceBaseTask(ShipStatus shipStatus, TaskTypes type1, TaskTypes type2, string firstConsole, string secondConsole, bool overrideSteps, int firstStep, bool showFirst, int secondStep, bool showSecond)
    {
        Console console1 = null;
        Console console2 = null;

        NormalPlayerTask task1 = null;
        NormalPlayerTask task2 = null;
        NormalPlayerTask defaultTask = new NormalPlayerTask();

        try
        {
            console1 = GameObject.Find(firstConsole).GetComponent<Console>();
            console2 = GameObject.Find(secondConsole).GetComponent<Console>();
        }
        catch (Exception e)
        {
            TaskEnginePlugin.LogSource.LogInfo($"Error in finding the consoles: {firstConsole}, {secondConsole}");
            return;
        }

        foreach (NormalPlayerTask task in shipStatus.ShortTasks)
        {
            if (task.TaskType == type1) task1 = task;
            if (task.TaskType == type2) task2 = task;
        }

        if (task1 == null && task2 == null)
        {
            foreach (NormalPlayerTask task in shipStatus.LongTasks)
            {
                if (task.TaskType == type1) task1 = task;
                if (task.TaskType == type2) task2 = task;
            }
        }
        if (task1 == null && task2 == null)
        {
            foreach (NormalPlayerTask task in shipStatus.CommonTasks)
            {
                if (task.TaskType == type1) task1 = task;
                if (task.TaskType == type2) task2 = task;
            }
        }

        Minigame task1Prefab = task1.MinigamePrefab;
        Minigame task2Prefab = task2.MinigamePrefab;
        int task1Step = task1.MaxStep;
        int task2Step = task2.MaxStep;
        bool task1StepShow = task1.ShowTaskStep;
        bool task2StepShow = task2.ShowTaskStep;
        
        try
        {
            if (task1 != null && task2 != null)
            {
                console1.TaskTypes[0] = type2;
                console2.TaskTypes[0] = type1;

                task1.TaskType = type2;
                task1.MinigamePrefab = task2Prefab;
                task1.MaxStep = task2Step;
                task1.ShowTaskStep = task2StepShow;
                
                task2.TaskType = type1;
                task2.MinigamePrefab = task1Prefab;
                task2.MaxStep = task1Step;
                task2.ShowTaskStep = task1StepShow;

                if (overrideSteps)
                {
                    task1.MaxStep = firstStep;
                    task1.ShowTaskStep = showFirst;
                    task2.MaxStep = secondStep;
                    task2.ShowTaskStep = showSecond;
                }
            }
        }
        catch (System.Exception e)
        {
            TaskEnginePlugin.LogSource.LogError("BaseTaskSwap failed");
            throw;
        }

    }
    
    public string RemoveSpaces(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (c != ' ') sb.Append(c);
        }

        return sb.ToString();
    }
}