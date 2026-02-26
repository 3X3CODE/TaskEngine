using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Il2CppInterop.Runtime;

namespace TaskEngine.Assets;

// this assetloader code was copied from MainMenuEnhanced
public class AssetLoader
{
    public static T LoadAsset<T>(string bundleName, string assetName) where T : Object
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        string resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(name => name.Contains(bundleName));

        if (resourceName == null)
        {
            return null;
        }

        using (Stream s = asm.GetManifestResourceStream(resourceName))
        {
            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, buffer.Length);

            AssetBundle bundle = AssetBundle.LoadFromMemory(buffer);

            if (bundle == null)
            {
                return null;
            }

            var asset = bundle.LoadAsset(assetName, Il2CppType.Of<T>());
            T prefab = asset.Cast<T>();
            bundle.Unload(false);
            return prefab;
        }
    }
    
    public static T LoadAssetFromFile<T>(string bundleName, string assetName) where T : Object
    {
        string fullPath = Path.Combine(CustomPaths.taskFolder, bundleName);

        if (!File.Exists(fullPath))
        {
            TaskEnginePlugin.LogSource.LogError($"[AssetLoader] File not found at: {fullPath}");
            return null;
        }
        
        AssetBundle bundle = AssetBundle.LoadFromFile(fullPath);

        if (bundle == null)
        {
            TaskEnginePlugin.LogSource.LogError($"[AssetLoader] Failed to load AssetBundle at {fullPath}");
            return null;
        }
        
        var asset = bundle.LoadAsset(assetName, Il2CppType.Of<T>());
    
        if (asset == null)
        {
            TaskEnginePlugin.LogSource.LogError($"[AssetLoader] Failed to load Asset: {assetName} from bundle: {bundleName}");
            bundle.Unload(true);
            return null;
        }

        T prefab = asset.Cast<T>();
        
        bundle.Unload(false);

        return prefab;
    }
}
