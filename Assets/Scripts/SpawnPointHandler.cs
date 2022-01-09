using UnityEngine;

public class SpawnPointHandler : MonoBehaviour
{
    [Header("NOTE: The order matters.")]
    [Tooltip("The first will be the first, second will be the second and so on")]
    [SerializeField] private Transform[] spawnPoints;

    private int _lastPickedSpawnPoint;

    private void Awake()
    {
        GameManager.Instance.SendGetSpawnPointsCallback(GetNextSpawnPoint);
    }

    /// <summary>
    /// Handles the spawn points cycle.
    /// </summary>
    /// <returns>Returns the next spawn point.</returns>
    private Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Length-1 == _lastPickedSpawnPoint)
        {
            _lastPickedSpawnPoint = 0;
        }
        else
        {
            _lastPickedSpawnPoint++;
        }
        
        return spawnPoints[_lastPickedSpawnPoint];
    }
}
