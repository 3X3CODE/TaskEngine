using System.IO;
using BepInEx;

namespace TaskEngine;

public class CustomPaths
{
    public static readonly string pluginPath = Paths.PluginPath;
    public static readonly string taskFolder = Path.Combine(pluginPath, "CustomTasks");
}