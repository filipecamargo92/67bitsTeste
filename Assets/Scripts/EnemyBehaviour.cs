using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    Animator anim;
    CapsuleCollider mainCollider;
    Rigidbody rb;

    [SerializeField] Transform pivot;
    [SerializeField] Rigidbody[] ragdollRigidbodies;
    [SerializeField] Collider[] ragdollColliders;
    [HideInInspector] public bool readyForStack = false;
    [HideInInspector] public bool onStack = false;

    bool wasHit;
    Vector3 direction;

    #region INSPECTOR
    [SerializeField] private LayerMask lootLayer;
    #endregion

    private void OnEnable()
    {
        PlayerEvents.OnPlayerPunch += HandleHitAnimation;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerPunch -= HandleHitAnimation;
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        mainCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollState(false);
    }

    private void Start()
    {
        float ranx = Random.Range(-1f, 1f);
        float ranz = Random.Range(-1f, 1f);
        direction = new Vector3(ranx, 0f, ranz).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public void PlayerHit()
    {
        wasHit = true;
        int layerMaskToInt = (int)Mathf.Log(lootLayer.value, 2);
        SetLayer(gameObject, layerMaskToInt);
    }

    private void HandleHitAnimation()
    {
        if (wasHit)
        {
            SetRagdollState(true);
            StartCoroutine(ReadyForStack());
        }
    }

    private void SetLayer(GameObject go, int newLayer)
    {
        go.layer = newLayer;
        foreach (Transform child in go.transform)
        {
            SetLayer(child.gameObject, newLayer);
        }
    }

    public void SetRagdollState(bool state, bool stackMode = false)
    {
        if(!onStack)
        {
            if(stackMode)
            {
                pivot.position = transform.position;
                Vector3 rotation = pivot.eulerAngles;
                rotation.x = 90f;
                pivot.eulerAngles = rotation;
                foreach (Rigidbody rb in ragdollRigidbodies)
                {
                    rb.isKinematic = true;
                }

                foreach (Collider col in ragdollColliders)
                {
                    col.enabled = false;
                }

                mainCollider.enabled = false;
                anim.enabled = false;
                rb.isKinematic = true;
            }
            else
            {
                foreach (Rigidbody rb in ragdollRigidbodies)
                {
                    rb.isKinematic = !state;
                }

                foreach (Collider col in ragdollColliders)
                {
                    col.enabled = state;
                }

                mainCollider.enabled = !state;
                anim.enabled = !state;
                rb.isKinematic = state;
            }
        }
    }

    IEnumerator ReadyForStack()
    {
        yield return new WaitForSeconds(1f);
        readyForStack = true;
    }
}