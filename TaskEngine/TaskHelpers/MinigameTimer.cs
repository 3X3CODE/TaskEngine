using UnityEngine;
namespace TaskEngine.TaskHelpers;

public class MinigameTimer
{
    public bool _isRunning { get; private set; }
    public float _startTime { get; private set; }
    public float _endTime { get; private set; }
    public float _desiredTime;

    public void Start()
    {
        _startTime = Time.time;
        _isRunning = true;
    }

    public void Stop()
    {
        _endTime = Time.time;
        _isRunning = false;
    }

    public float GetElapsed()
    {
        if (_isRunning)
        {
            return Time.time - _startTime;
        }

        return _endTime - _startTime;
    }

    public bool IsQualified()
    {
        return GetElapsed() >= _desiredTime;
    }

    public void Reset()
    {
        _startTime = 0f;
        _endTime = 0f;
        _isRunning = false;
    }
}