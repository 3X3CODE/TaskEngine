using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using IEnumerator = System.Collections.IEnumerator;

namespace ShootSubmarines;

[RegisterInIl2Cpp]
public sealed class Submarine(nint ptr) : MonoBehaviour(ptr)
{
    public ShootSubmarinesMinigame minigame;

    private Collider2D hitCollider;
    private GameObject explode;
    private GameObject normal;
    private bool hasBeenShot;
    private Camera mainCam;
    public Rigidbody2D body;
    public BoxCollider2D zone;

    private void Start()
    {
        mainCam = Camera.main;

        hitCollider = GetComponent<Collider2D>();
        explode = transform.Find("Explode").gameObject;
        normal = transform.Find("Submarine").gameObject;
        explode.SetActive(false);
        body = GetComponent<Rigidbody2D>();
        body.velocity = Vector2.right * 3f;
    }

    private void Update()
    {
        if (minigame.IsFinished || hasBeenShot) return;
        
        if (!zone.OverlapPoint(transform.position))
        {
            Destroy(gameObject);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (hitCollider.OverlapPoint(mousePos)) this.StartCoroutine(Shoot());
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator Shoot()
    {
        body.simulated = false;
        hasBeenShot = true;
        normal.SetActive(false);
        explode.SetActive(true);
        minigame.Shoot();
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
