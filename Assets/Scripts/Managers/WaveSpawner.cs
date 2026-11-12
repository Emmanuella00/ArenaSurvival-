using System.Collections;
using UnityEngine;

// =====================================================================
// WaveSpawner - ALGORITHM 2: wave scaling.
//
// Problem it solves: the game must get harder over time and never spawn
//   enemies on top of the player.
//
// Each wave runs three steps:
//   1. HOW MANY   CountForWave:  baseCount + (wave - 1) * growth   -> O(1)
//                 (3, 5, 7, 9, 11 enemies with the default settings)
//   2. WHICH TYPE PickEnemyType: weighted random. Heavy chance = 10% x wave
//                 (10% in wave 1, 50% in wave 5)                   -> O(1)
//   3. WHERE      FarthestSpawnPoint: linear search over all spawn
//                 points for the one farthest from the player      -> O(n)
// Whole wave: O(k x n) for k enemies and n spawn points.
//
// OBSERVER: subscribes to each Enemy's OnDied to count survivors.
// SINGLETON: tells GameManager.Instance when the last wave is cleared.
// =====================================================================
public class WaveSpawner : MonoBehaviour
{
    [Header("Prefabs and scene links")]
    [SerializeField] private MeleeRobot meleePrefab;
    [SerializeField] private HeavyRobot heavyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaypointPath patrolPath;
    [SerializeField] private Transform player;
    [SerializeField] private UIManager ui;

    [Header("Difficulty")]
    [SerializeField] private int totalWaves = 5;
    [SerializeField] private int baseCount = 3;
    [SerializeField] private int growth = 2;
    [SerializeField] private float heavyChancePerWave = 0.1f;

    [Header("Timing")]
    [SerializeField] private float timeBetweenWaves = 4f;
    [SerializeField] private float spawnInterval = 0.5f;

    private int wave;
    private int aliveCount;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        wave++;
        ui.UpdateWave(wave, totalWaves);
        yield return new WaitForSeconds(timeBetweenWaves);   // short break before the wave

        int count = CountForWave(wave);                      // STEP 1
        aliveCount = count;   // set BEFORE spawning, so early kills can't end the wave too soon

        for (int i = 0; i < count; i++)
        {
            Enemy prefab = PickEnemyType(wave);              // STEP 2
            Transform point = FarthestSpawnPoint();          // STEP 3

            // Small random offset so robots from the same point don't stack
            Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f));
            Enemy enemy = Instantiate(prefab, point.position + offset, point.rotation);
            enemy.Path = patrolPath;
            enemy.OnDied += HandleEnemyDied;                 // OBSERVER: subscribe

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // STEP 1 - linear growth, O(1)
    private int CountForWave(int waveNumber)
    {
        return baseCount + (waveNumber - 1) * growth;
    }

    // STEP 2 - weighted random choice, O(1)
    private Enemy PickEnemyType(int waveNumber)
    {
        float heavyChance = Mathf.Clamp01(heavyChancePerWave * waveNumber);
        if (Random.value < heavyChance) return heavyPrefab;
        return meleePrefab;
    }

    // STEP 3 - linear search for the spawn point farthest from the player, O(n)
    private Transform FarthestSpawnPoint()
    {
        Transform best = spawnPoints[0];
        float bestDistance = -1f;

        foreach (Transform point in spawnPoints)
        {
            float d = (point.position - player.position).sqrMagnitude;
            if (d > bestDistance)
            {
                bestDistance = d;
                best = point;
            }
        }
        return best;
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        enemy.OnDied -= HandleEnemyDied;   // OBSERVER: unsubscribe
        aliveCount--;
        if (aliveCount > 0) return;

        if (wave >= totalWaves) GameManager.Instance.Win();   // SINGLETON
        else StartCoroutine(StartNextWave());
    }
}
