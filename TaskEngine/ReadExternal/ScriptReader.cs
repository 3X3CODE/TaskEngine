using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using Reactor.Utilities.Attributes;
using TaskEngine.Assets;
using TaskEngine.MinigameBlock;
using UnityEngine;

namespace TaskEngine.ReadExternal;

// this script reads the CustomTasks folder, reads all DLL files, stores types of the public classes into a list

[RegisterInIl2Cpp]
public class ScriptReader : MonoBehaviour
{
    public static List<Type> AllCustomMinigames = new();
    
    public int customMinigameCount => AllCustomMinigames.Count;
    
    public void EnsureLoad()
    {
        string[] dllFiles = Directory.GetFiles(CustomPaths.taskFolder, "*.dll");
        foreach (string dll in dllFiles)
        {
            TaskEnginePlugin.LogSource.LogInfo("[ScriptReader] DLL found, proceeding to get classes");
            Assembly assembly = Assembly.LoadFrom(dll);
            Type[] types = assembly.GetTypes();
            
            var reference = assembly.GetReferencedAssemblies().FirstOrDefault(r => r.Name.Equals("TaskEngine", StringComparison.OrdinalIgnoreCase));
            if (reference == null) continue;
            
            var referenceVersion = reference.Version;
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (referenceVersion != currentVersion)
            {
                TaskEnginePlugin.LogSource.LogError($"The referenced Assembly version {referenceVersion} does not match current version {currentVersion}");
                continue;
            }

            foreach (Type type in types)
            {
                if (type.IsClass && type.IsPublic && type.IsSubclassOf(typeof(CustomMinigame)))
                {
                    if (AllCustomMinigames.Contains(type)) continue;
                    
                    AllCustomMinigames.Add(type);

                    // Specific initialization for the LoadAudio attribute
                    
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                    foreach (FieldInfo field in fields)
                    {
                        var attr = field.GetCustomAttribute<LoadAudioAttribute>();
                        
                        if (Attribute.IsDefined(field, typeof(LoadAudioAttribute)) && attr != null)
                        {
                            AudioLoader.allAttributes.Add(field);
                        }
                    }
                    
                    TaskEnginePlugin.LogSource.LogInfo($"[ScriptReader] Successfully added class: {type.Name}");
                }
            }
        }
    }

    public Type GetMinigameScript(string scriptName)
    {
        Type foundScript = null;
        
        TaskEnginePlugin.LogSource.LogInfo($"Attempting to find class: {scriptName}");
        
        foreach (Type script in AllCustomMinigames)
        {
            if (script.Name == scriptName)
            {
                ClassInjector.RegisterTypeInIl2Cpp(script);
                foundScript = script;
                TaskEnginePlugin.LogSource.LogInfo($"Found class: {scriptName}");
            }
        }
        
        if (foundScript == null) TaskEnginePlugin.LogSource.LogError($"The custom task script: {scriptName} wasn't found.");

        return foundScript;

    }
}