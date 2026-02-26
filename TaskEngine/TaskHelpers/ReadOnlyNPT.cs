namespace TaskEngine.TaskHelpers;

// sole purpose is to prevent users from bypassing the mod's methods with MyNormTask

public class ReadOnlyNPT
{
    private readonly NormalPlayerTask origin;

    public ReadOnlyNPT(NormalPlayerTask original)
    {
        origin = original;
    }
        
    public NormalPlayerTask.TaskLength Length { get { return origin.Length; } }
    public int taskStep { get { return origin.taskStep; } }
    public int MaxStep { get { return origin.MaxStep; } }
    public bool ShowTaskStep { get { return origin.ShowTaskStep; } }
    public bool ShowTaskTimer { get { return origin.ShowTaskTimer; } }
    public NormalPlayerTask.TimerState TimerStarted { get { return origin.TimerStarted; } }
    public float TaskTimer { get { return origin.TaskTimer; } }
    public byte[] Data { get { return origin.Data; } }
    public ArrowBehaviour Arrow { get { return origin.Arrow; } }
    public TaskTypes TaskType { get { return origin.TaskType; } }
}