using System.Collections.Generic;

namespace TaskEngine.TaskHelpers;

// contains the name of each task and hands it over to TranslationControllerPatches

public static class TaskRegistry
{
    public const int MIN = 199;
    public const int MAX = 1001;

    public static readonly Dictionary<int, string> TaskNames = new()
    {
        { 0xc8, "Plug Leaks" }
    };

    public static bool IsCustomTask(int id) => id >= MIN && id <= MAX;
}