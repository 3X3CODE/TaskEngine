using Reactor.Utilities.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootSubmarines;

// converted these scripts from inside the project to external on 2026.02.13

[RegisterInIl2Cpp]
public sealed class ShootSubmarinesMinigame(nint ptr) : Minigame(ptr)
{
    public float xStartPos;
    private Transform background;
    private BoxCollider2D closeCollider;
    private BoxCollider2D _lifeZone;
    private Camera mainCam;
    private float delay;
    private int remaining = 20;
    private float timer;
    public GameObject subPrefab;
    public bool IsFinished => MyNormTask.taskStep >= MyNormTask.MaxStep;

    private void Awake()
    {
        Minigame self = this;
        self.TransType = TransitionType.SlideBottom;

        gameObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -50f);
        gameObject.layer = LayerMask.NameToLayer("UI");
    }

    private void Start()
    {
        base.StartCoroutine(CoAnimateOpen());
        
        remaining = MyNormTask.MaxStep - MyNormTask.taskStep;
        
        mainCam = Camera.main;

        background = transform.Find("Background");
       
        closeCollider = background.GetComponent<BoxCollider2D>();
        _lifeZone = transform.Find("LifeZone").GetComponent<BoxCollider2D>();
        
        xStartPos = -19f;
        
        subPrefab = FindObjectOfType<Submarine>(true).gameObject;
        
        MyNormTask.MaxStep = 20;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (IsFinished) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (!closeCollider.OverlapPoint(mousePos))
            {
                StartCoroutine(CoDestroySelf());
                return;
            }
            
        }

        if (timer >= delay)
        {
            timer = 0;
            delay = Random.Range(0.75f, 1.75f);

            GameObject newBomb = Instantiate(subPrefab, transform);
            newBomb.SetActive(true);
            Submarine submarine = newBomb.GetComponent<Submarine>();
            submarine.minigame = this;
            submarine.zone = _lifeZone;
            Vector3 pos = newBomb.transform.position;
            pos.x = xStartPos;
            pos.y = Random.Range(-8f, 15f);
            pos.z = -2f;
            newBomb.transform.localPosition = pos;
        }
    }

    public void Shoot()
    {
        if (amClosing != CloseState.None || IsFinished) return;
        remaining--;

        if (MyNormTask)
        {
            MyNormTask.ShowTaskStep = false;
            MyNormTask.NextStep();
            MyNormTask.ShowTaskStep = true;

            if (MyNormTask.taskStep == MyNormTask.MaxStep)
            {
                base.StartCoroutine(CoStartClose());
                base.StartCoroutine(CoDestroySelf());
            }
        }
        else
        {
            if (remaining == 0)
            {
                base.StartCoroutine(CoStartClose());
                base.StartCoroutine(CoDestroySelf());
            }
        }
    }
}
