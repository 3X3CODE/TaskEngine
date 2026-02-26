using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using TaskEngine.MinigameBlock;
using IEnumerator = System.Collections.IEnumerator;

namespace ShootSubmarines;

[RegisterInIl2Cpp]
public sealed class Submarine(nint ptr) : ShootSubmarinesMinigame(ptr)
{
    public ShootSubmarinesMinigame minigame;

    // Below is an example on how to load an AudioClip using the LoadAudio attribute
    
    //[LoadAudio("testClip", "shootsubmarines", "shootClip")]
    //public static AudioClip testClip;
    
    private Collider2D hitCollider;
    private GameObject explode;
    private GameObject normal;
    private bool hasBeenShot;
    private Camera mainCam;
    public Rigidbody2D body;
    public BoxCollider2D zone;
    
    public DetectMouseCollision collisionDetector;
    public CheckColliderOverlap destroyDetector;
    public Delay wait;
    public Drag myDrag;
    public Sequencer mySequence;

    private void Start()
    {
        mainCam = Camera.main;

        hitCollider = GetComponent<Collider2D>();
        explode = transform.Find("Explode").gameObject;
        normal = transform.Find("Submarine").gameObject;
        explode.SetActive(false);
        body = GetComponent<Rigidbody2D>();
        body.velocity = Vector2.right * 3f;
        
        collisionDetector = new DetectMouseCollision(hitCollider, null, Shoot(), true, -1, this);
        destroyDetector = new CheckColliderOverlap(zone, transform.position, DestroySelf(), true, true, this);
        wait = new Delay(0.25f);
        myDrag = new Drag(gameObject, true, 3f, null, true, new Vector3(-5f, -10f, -50f), 1f, null, null, this, null);
        
        mySequence = new Sequencer(this);
        mySequence.AddBlock(collisionDetector);
        //mySequence.AddBlock(myDrag);
        mySequence.AddBlock(destroyDetector);
        this.StartCoroutine(mySequence.RunBlocks());
    }

    private void Update()
    {
        if (hasBeenShot) return;
        
        destroyDetector.UpdatePosition(transform.position);
    }

    [HideFromIl2Cpp]
    private IEnumerator Shoot()
    {
        body.simulated = false;
        hasBeenShot = true;
        normal.SetActive(false);
        explode.SetActive(true);
        minigame.Shoot();
        yield return this.StartCoroutine(wait.Execute());
        Destroy(gameObject);
    }
    
    [HideFromIl2Cpp]
    private IEnumerator DestroySelf()
    {
        Destroy(gameObject);
        yield return null;
    }
}
