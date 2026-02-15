using System.Collections.Generic;

namespace TaskEngine.TaskHelpers;

public readonly struct CustomTaskTypes
{
    private static readonly Dictionary<TaskTypes, string> _names = new();
    public static List<TaskTypes> All { get; } = new();
    public readonly TaskTypes Id;
    public readonly string Name;
    
    public CustomTaskTypes(int id, string name)
    {
        this.Id = (TaskTypes)id;
        this.Name = name;

        _names[this.Id] = name;
        All.Add(this.Id);
    }
    
    public static implicit operator TaskTypes(CustomTaskTypes custom) => custom.Id;

    public static bool TryGetName(TaskTypes id, out string name)
    {
        return _names.TryGetValue(id, out name);
    }

    public static readonly CustomTaskTypes PlugLeaks = new(0x80, "Plug Leaks");
}