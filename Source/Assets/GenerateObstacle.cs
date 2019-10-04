using UnityEngine;
using UnityEngine.Networking;

public class GenerateObstacle : NetworkBehaviour
{
    [SerializeField]
    private int obstacleCount;
    [SerializeField]
    private GameObject obstaclePrefab;

    [ServerCallback]
    private void Start()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab);
            NetworkServer.Spawn(newObstacle);
            newObstacle.GetComponent<SetRandomPosition>().ResetPosition(transform.localScale.x);
        }
    }
}
