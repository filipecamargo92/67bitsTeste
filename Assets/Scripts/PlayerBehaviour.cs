using UnityEngine;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour
{
    Animator anim;

    #region INSPECTOR
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask lootLayer;
    [SerializeField] LayerMask stackedEnemyLayer;
    [SerializeField] Transform stackPoint;
    [SerializeField] float stackOffset;
    [SerializeField] float inertiaSpeed;
    [SerializeField] SkinnedMeshRenderer skinMeshRender;
    [SerializeField] Material redMaterial;
    #endregion

    List<Transform> stackedEnemies = new List<Transform>();

    private void OnEnable()
    {
        Shop.OnPlayerSell += RemoveStackedEnemy;
        GameManager.OnPlayerSell += ChangeMaterial;
    }

    private void OnDisable()
    {
        Shop.OnPlayerSell -= RemoveStackedEnemy;
        GameManager.OnPlayerSell -= ChangeMaterial;
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            EnemyBehaviour otherBehaviour = other.GetComponent<EnemyBehaviour>();
            otherBehaviour?.PlayerHit();

            Vector3 toOther = other.transform.position - transform.position;
            Vector3 playerForward = transform.forward;
            float otherPosition = Vector3.Cross(playerForward, toOther).y;

            anim.SetTrigger(otherPosition >= 0 ? "RightHook" : "LeftHook");
        }
        else if ((lootLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            EnemyBehaviour otherBehaviour = other.GetComponentInParent<EnemyBehaviour>();
            if (otherBehaviour.readyForStack)
            {
                StackEnemy(otherBehaviour.transform, otherBehaviour);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateStackedEnemies();
    }

    private void StackEnemy(Transform enemy, EnemyBehaviour enemyBehaviour)
    {
        enemyBehaviour.SetRagdollState(false, true);
        enemyBehaviour.onStack = true;
        int layerMaskToInt = (int)Mathf.Log(stackedEnemyLayer.value, 2);
        SetLayer(enemy.gameObject, layerMaskToInt);
        stackedEnemies.Add(enemy);
    }

    private void RemoveStackedEnemy()
    {
        if (stackedEnemies.Count > 0)
        {
            Transform enemyToRemove = stackedEnemies[stackedEnemies.Count - 1];
            stackedEnemies.RemoveAt(stackedEnemies.Count - 1);
            Destroy(enemyToRemove.gameObject);
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager.IncreaseMoneyValue(5);
        }
    }

    private void UpdateStackedEnemies()
    {
        if (stackedEnemies.Count == 0) return;

        Vector3 targetPosition = stackPoint.position;
        stackedEnemies[0].position = targetPosition;

        for (int i = 1; i < stackedEnemies.Count; i++)
        {
            Transform enemy = stackedEnemies[i];
            Transform belowEnemy = stackedEnemies[i - 1];

            targetPosition = belowEnemy.position + Vector3.up * stackOffset;
            enemy.position = Vector3.Lerp(enemy.position, targetPosition, Time.deltaTime * inertiaSpeed);
        }

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        foreach (var enemy in stackedEnemies)
        {
            enemy.rotation = Quaternion.Slerp(enemy.rotation, targetRotation, Time.deltaTime * inertiaSpeed);
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

    private void ChangeMaterial() => skinMeshRender.material = redMaterial;
}
