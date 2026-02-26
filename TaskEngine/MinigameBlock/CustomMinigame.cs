using System;
using Reactor.Utilities.Attributes;
using TaskEngine.TaskHelpers;
using UnityEngine;

namespace TaskEngine.MinigameBlock;

[RegisterInIl2Cpp]
public class CustomMinigame : Minigame
{
    public CustomMinigame(IntPtr ptr) : base(ptr) { }
    
    private MinigameTimer myTaskTimer = new MinigameTimer();
    private Collider2D closeCollider;
    private Vector2 mousePos;
    
    protected new ReadOnlyNPT MyNormTask { 
        get { return new ReadOnlyNPT(base.MyNormTask); }
        set { TaskEnginePlugin.LogSource.LogError("Accessing MyNormTask is prohibited while using CustomMinigame. Contact developers for more information. TE 0.1.0"); }
    }
    
    public bool showTaskStep { get; protected set; } = false;
    public int taskMaxStep { get; protected set; } = 1;

    private void Start()
    {
        TaskEnginePlugin.LogSource.LogInfo($"Opening task: {base.MyNormTask.TaskType}");
        
        StartCoroutine(CoAnimateOpen());
        myTaskTimer.Start();
        OnMinigameStart();

        myTaskTimer._desiredTime = 5f;
        closeCollider = transform.Find("Background").GetComponent<Collider2D>();

        base.MyNormTask.ShowTaskStep = showTaskStep;
        base.MyNormTask.MaxStep = taskMaxStep;
    }

    private void Update()
    {
        OnMinigameUpdate();
        
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!closeCollider.OverlapPoint(mousePos))
            {
                myTaskTimer.Stop();
                myTaskTimer.Reset();
                StartCoroutine(CoDestroySelf());
            }
        }
    }

    protected void EndTask()
    {
        if (myTaskTimer._isRunning && myTaskTimer.IsQualified() && isTaskComplete())
        {
            TaskEnginePlugin.LogSource.LogInfo($"Closing task: {base.MyNormTask.TaskType}. Your time is {myTaskTimer.GetElapsed()}");
            StartCoroutine(CoStartClose());
            StartCoroutine(CoDestroySelf());
            myTaskTimer.Stop();
            myTaskTimer.Reset();
        }
        else
        {
            TaskEnginePlugin.LogSource.LogError("Timer is not running / the task hasn't met the minimum time requirement");
        }
    }

    protected bool isTaskComplete()
    {
        return base.MyNormTask.taskStep >= base.MyNormTask.MaxStep;
    }

    protected void NextStep()
    {
        if (MyNormTask.taskStep == MyNormTask.MaxStep - 1)
        {
            if (!myTaskTimer.IsQualified()) return;
        }
        
        base.MyNormTask.ShowTaskStep = false;
        base.MyNormTask.NextStep();
        base.MyNormTask.ShowTaskStep = showTaskStep;
    }

    public virtual void OnMinigameStart()
    {
        TaskEnginePlugin.LogSource.LogError("You have not provided the MinigameStart method");
    }

    public virtual void OnMinigameUpdate()
    {
        TaskEnginePlugin.LogSource.LogError("You have not provided the MinigameUpdate method");
    }
}