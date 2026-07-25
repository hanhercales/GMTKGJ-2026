using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
public class TimerDropSpawner : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private GameObject[] dropPool;
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.3f;
    [SerializeField] private float spawnDelay = 0.3f;
    
    [Header("Throw Settings")]
    [SerializeField] private Vector2 targetRangeMin = new Vector2(-4f, -3f);
    [SerializeField] private Vector2 targetRangeMax = new Vector2(4f, -1f);
    [SerializeField] private float flySpeed = 8f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.TryGetComponent(out ThrowTask _)) return;
        if (Random.value > dropChance) return;

        Vector2 contactPoint = other.GetContact(0).point;
        StartCoroutine(SpawnRandomDropDelayed(contactPoint));
    }

    private IEnumerator SpawnRandomDropDelayed(Vector2 spawnPos)
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnRandomDrop(spawnPos);
    }
    
    private void SpawnRandomDrop(Vector2 spawnPos)
    {
        if(dropPool.Length == 0) return;
        
        GameObject prefab = dropPool[Random.Range(0, dropPool.Length)];
        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);

        Vector2 targetPos = new Vector2(Random.Range(targetRangeMin.x, targetRangeMax.x), Random.Range(targetRangeMin.y, targetRangeMax.y));
        
        Vector2 dir = (targetPos - spawnPos).normalized;
        float angle =  Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        StartCoroutine(FlyToTarget(instance.transform, targetPos));
    }

    private IEnumerator FlyToTarget(Transform t, Vector2 targetPos)
    {
        while (t != null && Vector2.Distance(t.position, targetPos) > 0.05f)
        {
            t.position = Vector2.MoveTowards(t.position, targetPos, Time.deltaTime * flySpeed);
            yield return null;
        }
    }
}
