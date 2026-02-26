using System;
using System.Collections;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;

namespace TaskEngine.MinigameBlock;

#region DelayBlock

public class Delay : IMinigameBlock
{
    private float time;
    
    public Delay(float duration) => time = duration;
    
    public IEnumerator Execute()
    {
        yield return new WaitForSeconds(time);
    }
}

#endregion

#region DetectMouseCollision Block
public class DetectMouseCollision : IMinigameBlock
{
    private Collider2D collider;
    private Action onCollision;
    private IEnumerator runCoroutine;
    private bool startCoroutine;
    private MonoBehaviour runner;
    private int repeatCount;

    public bool IsClicked = false;

    public DetectMouseCollision(Collider2D hitCollider, Action onHit, IEnumerator runIEnumerator, bool startIEnumerator, int amount, MonoBehaviour gameObject)
    {
        collider = hitCollider;
        onCollision = onHit;
        runCoroutine = runIEnumerator;
        startCoroutine = startIEnumerator;
        runner = gameObject;
        repeatCount = amount;
    }
    
    public IEnumerator Execute()
    {
        runner.StartCoroutine(HitDetect());
        yield break;
    }
    
    public IEnumerator HitDetect()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0) && ( IsWithinRange() || IsInfinite() ) )
            {
                Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (collider.OverlapPoint(clickPos))
                {
                    IsClicked = true;
                    if (IsWithinRange()) repeatCount--;
                    if (!startCoroutine)onCollision?.Invoke();
                    if (startCoroutine && runCoroutine != null) yield return runner.StartCoroutine(runCoroutine);
                    yield return null;
                }

                IsClicked = false;
            }
            
            yield return null;
        }
    }

    public bool IsWithinRange()
    {
        return repeatCount > 0;
    }

    public bool IsInfinite()
    {
        return repeatCount == -1;
    }
}

#endregion

#region CheckColliderOverlap Block

public class CheckColliderOverlap : IMinigameBlock
{
    private bool isInvert;
    private bool repeat;
    private Vector3 position;
    private Collider2D collider;
    private IEnumerator coroutine;
    private MonoBehaviour behaviour;
    
    private bool shouldRun = true;

    public CheckColliderOverlap(Collider2D checkCollider, Vector3 _position, IEnumerator run, bool shouldInvert,
        bool constantRun, MonoBehaviour script)
    {
        collider = checkCollider;
        position = _position;
        isInvert = shouldInvert;
        repeat = constantRun;
        coroutine = run;
        behaviour = script;
    }

    public IEnumerator Execute()
    {
        while (true)
        {
            if (repeat)
            {
                yield return behaviour.StartCoroutine(CheckOverlap());
                yield return null;
            }
            if (!repeat && shouldRun)
            {
                yield return behaviour.StartCoroutine(CheckOverlap());
                shouldRun = false;
                yield return null;
            }
        }
    }

    private IEnumerator CheckOverlap()
    {
        if (isOverlapped() && !isInvert)
        {
            yield return behaviour.StartCoroutine(coroutine);
            yield return null;
        }
        if (!isOverlapped() && isInvert)
        {
            yield return behaviour.StartCoroutine(coroutine);
            yield return null;
        }
        
    }

    public bool isOverlapped()
    {
        return collider.OverlapPoint(position);
    }

    public void UpdatePosition(Vector3 newPos)
    {
        position = newPos;
    }
}

#endregion

#region PlayAudio Block

public class PlayAudio : IMinigameBlock
{
    private AudioClip audio;
    private bool loop;

    public PlayAudio(AudioClip audioClip, bool repeat)
    {
        audio = audioClip;
        loop = repeat;
    }

    public IEnumerator Execute()
    {
        yield return SoundManager.Instance.PlaySound(audio, loop);
        yield return null;
    }
}

#endregion

#region DragBlock

public class Drag : IMinigameBlock
{
    public bool _isDragging;
    public DetectMouseCollision collisionReader;
    public IEnumerator onDragStart;
    public IEnumerator onDragFinish;
    
    Vector2 clickPos;
    float targetDistance;
    float distance;
    private Vector2 offset;
    
    private GameObject target;
    private bool proximityDrag;
    private float radius;
    private Collider2D collider;
    private bool hasTarget;
    private Vector3 endPos;
    private float targetRadius;
    private MonoBehaviour runner;

    public Drag(GameObject _target, bool _proximityDrag, float _radius, Collider2D _collider, bool _hasTarget,
        Vector3 _endPos, float _targetRadius, IEnumerator targetCoroutine, IEnumerator startCoroutine, MonoBehaviour _runner, DetectMouseCollision clickDetector)
    {
        target = _target;
        proximityDrag = _proximityDrag;
        radius = _radius;
        collider = _collider;
        hasTarget = _hasTarget;
        endPos = _endPos;
        targetRadius = _targetRadius;
        runner = _runner;
        collisionReader = clickDetector;
        onDragStart = startCoroutine;
        onDragFinish = targetCoroutine;
    }
    
    public IEnumerator Execute()
    {
        while (true)
        {
            clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                distance = Vector3.Distance(clickPos, target.transform.position);
                
                if (ColliderDragQualified())
                {
                    _isDragging = true;
                    offset = (Vector2)target.transform.position - clickPos;
                    if (onDragStart != null) runner.StartCoroutine(onDragStart);
                }
                
                if (ProximityDragQualified())
                {
                    _isDragging = true;
                    offset = (Vector2)target.transform.position - clickPos;
                    if (onDragStart != null) runner.StartCoroutine(onDragStart);
                }
            }

            if (_isDragging && Input.GetMouseButton(0))
            {
                if ( ColliderDragQualified() || ProximityDragQualified())
                {
                    Vector3 newPos = new Vector3(clickPos.x + offset.x, clickPos.y + offset.y, target.transform.position.z);
                    target.transform.position = newPos;
                    if (hasTarget && endPos != null) targetDistance = Vector3.Distance(endPos, target.transform.position);

                    if (HasTargetPos())
                    {
                        _isDragging = false;
                        target.transform.position = endPos;
                        if (onDragFinish != null) yield return runner.StartCoroutine(onDragFinish);
                    }
                    
                    if (Input.GetMouseButtonUp(0))
                    {
                        _isDragging = false;
                        if (onDragFinish != null) yield return runner.StartCoroutine(onDragFinish);
                    }
                }
            }
            yield return null;
        }
    }

    public bool ColliderDragQualified()
    {
        if (collider != null)
        {
            return collider.OverlapPoint(clickPos) || (collisionReader != null && collisionReader.IsClicked);
        }
        else return false;
    }
    public bool ProximityDragQualified()
    {
        if (proximityDrag)
        {
            return distance < radius;
        }
        else return false;
    }

    public bool HasTargetPos()
    {
        if (hasTarget && endPos != null && targetRadius > 0)
        {
            return targetDistance < targetRadius;
        }
        else return false;
    }
}

#endregion