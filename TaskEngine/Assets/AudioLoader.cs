using System.Collections.Generic;
using System.Reflection;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace TaskEngine.Assets;

[RegisterInIl2Cpp]
public class AudioLoader : MonoBehaviour
{
    private static Dictionary<string, AudioClip> soundCache = new();
    public static List<FieldInfo> allAttributes = new();
    
    public void EnsureLoad()
    {
        foreach (var field in allAttributes)
        {
            TaskEnginePlugin.LogSource.LogInfo($"Initializing LoadAudio attribute");
            var attribute = field.GetCustomAttribute<LoadAudioAttribute>();
            LoadSound(attribute.Name, AssetLoader.LoadAssetFromFile<AudioClip>(attribute.BundleName, attribute.FileName));
            field.SetValue(null, GetSound(attribute.Name));
        }
    }
    
    public void LoadSound(string name, AudioClip clip)
    {
        if (clip != null)
        {
            soundCache[name] = clip;
            TaskEnginePlugin.LogSource.LogInfo($"Successfully cached AudioClip: {name}");
        }
        TaskEnginePlugin.LogSource.LogError($"Failed to cache AudioClip: {name}");
    }

    public static AudioClip GetSound(string name)
    {
        if (soundCache.ContainsKey(name)) return soundCache[name];
        TaskEnginePlugin.LogSource.LogError($"The AudioClip {name} hasn't loaded yet");
        return null;
    }
}