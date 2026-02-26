using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Il2CppInterop.Runtime;
using Reactor.Utilities.Attributes;
using TaskEngine.Assets;
using TaskEngine.TaskHelpers;
using UnityEngine;

namespace TaskEngine.ReadExternal;


[XmlRoot("Config")]
public class Config
{ 
    [XmlElement("EditActive")] public bool IsActive { get; set; }
    [XmlElement("CustomTask")] public List<CustomTaskDefinition> CustomTasks { get; set; } = new();
    [XmlElement("ReplaceTask")] public List<ReplaceTaskDefinition> ReplaceTasks { get; set; } = new();
}

#region XMLdefinition

public class CustomTaskDefinition
{
    [XmlAttribute("Task")] 
    public string Name { get; set; }
    public bool Active { get; set; }
    public string scriptName { get; set; }
    public string? monobehaviourName { get; set; }
    public string firstGameObject { get; set; }
    public string? secondGameObject { get; set; }
    public string bundleName { get; set; }
    public int taskType { get; set; }
    public TaskTypes replaceTask { get; set; }
    public string consoleName { get; set; }
    public int maxStep { get; set; }
    public bool showStep { get; set; }
    public string ship { get; set; }
    public ScaleData firstScale { get; set; }
    public ScaleData? secondScale { get; set; }
}

public class ReplaceTaskDefinition
{
    public bool Active { get; set; }
    public string ship { get; set; }
    public TaskTypes firstTask { get; set; }
    public TaskTypes secondTask { get; set; }
    public string firstConsole { get; set; }
    public string secondConsole { get; set; }
    public bool overrideStep { get; set; }
    public int firstStep { get; set; }
    public int secondStep { get; set; }
    public bool showFirst { get; set; }
    public bool showSecond { get; set; }
}

public class ScaleData
{
    [XmlAttribute("x")] public float X { get; set; }
    [XmlAttribute("y")] public float Y { get; set; }
    [XmlAttribute("z")] public float Z { get; set; }
}


#endregion


// this script creates/reads the XML files and hands over the tasks to be added to the ShipStatus patch

[RegisterInIl2Cpp]
public class Executor : MonoBehaviour
{
    private readonly string path = Path.Combine(CustomPaths.pluginPath, "taskConfig.xml");
    public List<CustomTask> CustomTasks = new();
    public List<ReplaceTask> ReplaceTasks = new();
        
    public void EnsureLoad()
    {
        TaskEnginePlugin.LogSource.LogInfo("[XMLReader] XML started loading");
        
        Config config = new Config();
        
        #region AddExampleValues
        
        config.CustomTasks.Add(new CustomTaskDefinition
        {
            Name = "Shoot Submarines",
            Active = true,
            scriptName = "ShootSubmarinesMinigame",
            monobehaviourName = "Submarine",
            firstGameObject = "ShootSubmarinesMinigame",
            secondGameObject = "Submarine",
            bundleName = "shootsubmarines",
            taskType = 0xc9,
            replaceTask = TaskTypes.AssembleArtifact,
            consoleName = "AssembleArtifactConsole",
            maxStep = 20,
            showStep = true,
            ship = "Fungle",
            firstScale = new ScaleData { X=0.3f, Y=0.3f, Z=0.3f },
            secondScale = new ScaleData { X=1, Y=1, Z=1}
        });
        
        config.ReplaceTasks.Add(new ReplaceTaskDefinition
        {
            ship = "Fungle",
            firstTask = TaskTypes.FixAntenna,
            secondTask = TaskTypes.PlayVideogame,
            firstConsole = "FixAntennaConsole",
            secondConsole = "PlayVideoGameConsole",
            firstStep = 1,
            secondStep = 20,
            showFirst = false,
            showSecond = true
        });
        
        #endregion
        
        #region LoadXMLsettings
        
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        if (!File.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, config);
            }
        }
        else
        {
            ExecuteModifications();
        }
        
        #endregion
    }

    public void ExecuteModifications()
    {
        Config config;
        
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            TaskEnginePlugin.LogSource.LogInfo("Trying to load taskConfig...");
            try
            {
                config = (Config)serializer.Deserialize(fs);
            }
            catch (Exception e)
            {
                TaskEnginePlugin.LogSource.LogError($"Error loading file: {e.Message}");
                return;
            } 
            TaskEnginePlugin.LogSource.LogInfo("Loaded taskConfig");
        }
        
        if (!config.IsActive) return;
        
        #region AddCustomTasksToList
        
        foreach (var task in config.CustomTasks)
        {
            if (!task.Active) continue;

            // A wrong TaskType is caught by the deserializer itself but im going to leave this in here
            
            if (!Enum.IsDefined(typeof(TaskTypes), task.replaceTask))
            {
                TaskEnginePlugin.LogSource.LogError($"The TaskType: {task.replaceTask} provided to replaceTask is invalid. Exiting...");
                continue;
            }
            
            var taskType = new CustomTaskTypes(task.taskType, task.Name);
            TaskRegistry.TaskNames[task.taskType] = task.Name;
            
            ScriptReader reader = gameObject.GetComponent<ScriptReader>();

            GameObject mainMinigameAsset = AssetLoader.LoadAssetFromFile<GameObject>(task.bundleName, task.firstGameObject);
            
            if (mainMinigameAsset == null)
            {
                TaskEnginePlugin.LogSource.LogError($"[XMLReader] Failed to load {task.firstGameObject}");
                continue;
            }
            
            GameObject mainMinigame = Instantiate(mainMinigameAsset);

            mainMinigame.transform.localScale = new Vector3(task.firstScale.X, task.firstScale.Y, task.firstScale.Z);
            mainMinigame.transform.SetParent(transform);
            mainMinigame.SetActive(true);
            
            GameObject secondMinigame = null;
            if (!string.IsNullOrEmpty(task.secondGameObject))
            {
                GameObject secondMinigameAsset = AssetLoader.LoadAssetFromFile<GameObject>(task.bundleName, task.secondGameObject);
                
                if (secondMinigameAsset == null)
                {
                    TaskEnginePlugin.LogSource.LogError($"[XMLReader] Failed to load {task.secondGameObject}");
                    continue;
                }
                
                secondMinigame = Instantiate(secondMinigameAsset);
                Type secondScript = reader.GetMinigameScript(task.monobehaviourName);

                if (secondScript == null)
                {
                    TaskEnginePlugin.LogSource.LogError($"[XMLReader] Failed to load script {task.monobehaviourName}");
                    continue;
                }
                    
                secondMinigame.AddComponent(Il2CppType.From(secondScript));
                secondMinigame.transform.SetParent(mainMinigame.transform);
                secondMinigame.transform.localScale = new Vector3(task.secondScale.X, task.secondScale.Y, task.secondScale.Z);
                secondMinigame.SetActive(false);
                
            }
            
            Type mainScript = reader.GetMinigameScript(task.scriptName);
            
            if (mainScript == null)
            {
                TaskEnginePlugin.LogSource.LogError($"[XMLReader] Failed to load script {task.scriptName}");
                continue;
            }
            
            Minigame script = (Minigame)mainMinigame.AddComponent(Il2CppType.From(mainScript));
                
            CustomTasks.Add(new CustomTask
            {
                ShipName = task.ship,
                customType = taskType,
                minigame = script,
                baseType = task.replaceTask,
                consoleName = task.consoleName,
                maxStep = task.maxStep,
                showStep = task.showStep
            });
            
            TaskEnginePlugin.LogSource.LogInfo($"Done creating Custom Task: {task.Name}");
        }
        #endregion

        #region AddReplaceTasksToList
        
        foreach (var task in config.ReplaceTasks)
        {
            if (!task.Active) continue;
            ReplaceTasks.Add(new ReplaceTask
            {
                ship = task.ship,
                firstConsole = task.firstConsole,
                secondConsole = task.secondConsole,
                firstTask = task.firstTask,
                secondtask = task.secondTask,
                overrideSteps = task.overrideStep,
                firstStep = task.firstStep,
                secondStep = task.secondStep,
                showFirst = task.showFirst,
                showSecond = task.showSecond
            });
            
            TaskEnginePlugin.LogSource.LogInfo($"Done replacing tasks: {task.firstConsole}, {task.secondConsole}");
        }
        
        #endregion
    }
}

#region ListClasses
public class CustomTask
{
    public string ShipName;
    public TaskTypes customType;
    public Minigame minigame;
    public TaskTypes baseType;
    public string consoleName;
    public int maxStep;
    public bool showStep;
}

public class ReplaceTask
{
    public string ship;
    public TaskTypes firstTask;
    public string firstConsole;
    public TaskTypes secondtask;
    public string secondConsole;
    public bool overrideSteps;
    public int firstStep;
    public bool showFirst;
    public int secondStep;
    public bool showSecond;
}

#endregion