using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private IslandHandler _islandSpawner;

    private int _completedIslandsCount;
    private int _islandChunkSize = 6;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Start()
    {
        CreateIsland();
    }

    private void CreateIsland()
    {
        _islandSpawner.SpawnChunk(_islandChunkSize);
    }
}
