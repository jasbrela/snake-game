using System.Collections;
using Blocks;
using UnityEngine;

public class SpawnPointHandler : MonoBehaviour
{
    [Header("Cycle Information")]
    [Tooltip("Check for ordered spawn, uncheck for random spawn.")]
    [SerializeField] private bool doesTheOrderMatters;
    
    [Space(10)][Header("Spawn Points Lists")]
    [SerializeField] private Transform[] leftSpawnPoints;
    [SerializeField] private Transform[] rightSpawnPoints;

    private int _lastPickedRightSpawnPointIndex;
    private int _lastPickedLeftSpawnPointIndex;

    private bool _lastSpawnedLeft;
    
    private void Awake()
    {
        GameManager.Instance.SendGetSpawnPointsCallback(GetNextSpawnPoint);
        
        // So it will start on point ZERO
        _lastPickedLeftSpawnPointIndex = leftSpawnPoints.Length;
        _lastPickedRightSpawnPointIndex = rightSpawnPoints.Length;

        StartCoroutine(CheckForOutOfBoundsPoints());
    }

    /// <summary>
    /// Checks for out of bounds points, then delete them.
    /// </summary>
    private IEnumerator CheckForOutOfBoundsPoints()
    {
        yield return new WaitForEndOfFrame(); // Because the Spawn Positions will only
                                              // get updated after the first frame.
        CheckList(ref leftSpawnPoints);
        CheckList(ref rightSpawnPoints);

        void CheckList (ref Transform[] list)
        {
            BoxCollider2D bounds = BlockManager.Instance.GetBoundsCollider();
            
            for (int i = list.Length - 1; i >= 0; i--)
            {
                if (-bounds.size.y/2 > list[i].position.y)
                {
                    Destroy(list[i].gameObject);
                }
                else
                {
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Handles the spawn points cycle.
    /// </summary>
    /// <returns>Returns the next spawn point.</returns>
    private Vector3 GetNextSpawnPoint()
    {
        if (doesTheOrderMatters)
        {
            if (_lastSpawnedLeft)
            {
                _lastSpawnedLeft = false;
                return rightSpawnPoints[IncreaseAndGetIndex(ref rightSpawnPoints,
                        ref _lastPickedRightSpawnPointIndex)].position;
            }

            _lastSpawnedLeft = true;
            return leftSpawnPoints[IncreaseAndGetIndex(ref leftSpawnPoints,
                    ref _lastPickedLeftSpawnPointIndex)].position;
        }
        
        float chance = Random.Range(0, 1f);
        return chance >= 0.5f
            ? GetRandomSpawnPoint(ref leftSpawnPoints, ref _lastPickedLeftSpawnPointIndex)
            : GetRandomSpawnPoint(ref rightSpawnPoints, ref _lastPickedRightSpawnPointIndex);
    }

    /// <summary>
    /// Get a random position that was not picked immediately before.
    /// </summary>
    /// <param name="list">Reference of the spawn points list</param>
    /// <param name="lastIndexPicked">The last index picked of this side</param>
    /// <returns>A Vector3 of the randomized spawn point position.</returns>
    private Vector3 GetRandomSpawnPoint(ref Transform[] list, ref int lastIndexPicked)
    {
        var randomIndex = Random.Range(0, list.Length);
        if (randomIndex == lastIndexPicked && lastIndexPicked == list.Length - 1)
        {
            randomIndex = 0;
        }

        lastIndexPicked = randomIndex;
        return list[randomIndex].position;

    }

    /// <summary>
    /// Increases the index, then returns a new index.
    /// </summary>
    /// <param name="list">Reference of the spawn points list</param>
    /// <param name="lastIndexPicked">The last index picked of this side</param>
    /// <returns>The increased new index</returns>
    private int IncreaseAndGetIndex(ref Transform[] list, ref int lastIndexPicked)
    {
        lastIndexPicked++;
        
        if (lastIndexPicked >= list.Length)
        {
            lastIndexPicked = 0;
        }
        
        return lastIndexPicked;
    }
    
}
