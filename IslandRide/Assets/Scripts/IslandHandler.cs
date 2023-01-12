using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _frameBuildingSeconds;

    [Header("IslandParentObject")]
    [SerializeField] private Transform _islandParent;

    [Header("ChunkAssets")]
    [SerializeField] private List<IslandChunksAsset> _islandChunksAssets;

    [Header("Prefabs")]
    [SerializeField] private FrameBlock _block_Frame;

    [Header("Components")]
    [SerializeField] private AudioHandler _audioHandler;

    [Header("Sounds")]
    [SerializeField] private AudioClip _audio_Explode;
    private int _chunkSize;

    public void SpawnChunk(int chunkSize)
    {
        _chunkSize = chunkSize;
        StartCoroutine(SpawnChunkFrame(chunkSize, new int[2] { 0, 0 }, new int[2] { 0, 0 }));
    }

    private IEnumerator SpawnChunkFrame(int chunkSize, int[] instantiatePositions, int[] roadPositions)
    {
        ChunkBlock[,] chunkBlocks = new ChunkBlock[chunkSize, chunkSize];

        int startPositionX = instantiatePositions[0];
        int startPositionZ = instantiatePositions[1];

        IslandChunk newChunk = SpawnChunkParent();

        _audioHandler.PlayBlockSound(_block_Frame.AudioSpawn);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                chunkBlocks[x, z] = Instantiate(_block_Frame, new Vector3(startPositionX + x, 0f, startPositionZ + z), Quaternion.identity, newChunk.transform);
                yield return new WaitForSeconds(_frameBuildingSeconds);
            }
        }

        IslandChunksAsset islandChunkAsset = GetRandomIslandAsset();

        newChunk.CreateChunk(islandChunkAsset, chunkBlocks, instantiatePositions, roadPositions,
        () =>
        {
            StartCoroutine(SpawnChunkFrame(_chunkSize, GetNextChunkPositions(newChunk.ChunkEndPositions, instantiatePositions, newChunk.ChunkSize),GetNextRoadPositions(newChunk.ChunkEndPositions, newChunk.ChunkSize)));
        },
        () =>
        {
            _audioHandler.PlayBlockSound(_audio_Explode);
        });
    }

    private int[] GetNextChunkPositions(int[] endPositions, int[] instantiatePositions, int[] chunkSize)
    {
        int endPositionX = endPositions[0];
        int endPositionZ = endPositions[1];

        int instantiatePositionX = instantiatePositions[0];
        int instantiatePositionZ = instantiatePositions[1];

        int chunkSizeX = chunkSize[0];
        int chunkSizeZ = chunkSize[1];

        if (endPositionX == 0)
        {
            return new int[2] { instantiatePositionX - chunkSizeX, instantiatePositionZ };
        }

        else if (endPositionX == (chunkSizeX - 1))
        {
            return new int[2] { instantiatePositionX + chunkSizeX, instantiatePositionZ };
        }

        else if (endPositionZ == 0)
        {
            return new int[2] { instantiatePositionX, instantiatePositionZ - chunkSizeZ };
        }

        else if (endPositionZ == (chunkSizeZ - 1))
        {
            return new int[2] { instantiatePositionX, instantiatePositionZ + chunkSizeZ };
        }

        else
        {
            throw new Exception("Incorrect end position");
        }
    }

    private int[] GetNextRoadPositions(int[] endPositions, int[] chunkSize)
    {
        int endPositionX = endPositions[0];
        int endPositionZ = endPositions[1];

        int chunkSizeX = chunkSize[0];
        int chunkSizeZ = chunkSize[1];

        if (endPositionX == 0)
        {
            return new int[2] { chunkSizeX - 1, endPositionZ };
        }

        else if (endPositionX == (chunkSizeX - 1))
        {
            return new int[2] { 0, endPositionZ };
        }

        else if (endPositionZ == 0)
        {
            return new int[2] { endPositionX, chunkSizeZ - 1 };
        }

        else if (endPositionZ == (chunkSizeZ - 1))
        {
            return new int[2] { endPositionX, 0 };
        }

        else
        {
            throw new Exception("Incorrect end position");
        }
    }

    private IslandChunksAsset GetRandomIslandAsset()
    {
        int islandAssetIndex = UnityEngine.Random.Range(0, _islandChunksAssets.Count);

        return _islandChunksAssets[islandAssetIndex];
    }

    private IslandChunk SpawnChunkParent()
    {
        GameObject chunkObject = new GameObject("Chunk", typeof(IslandChunk));
        chunkObject.transform.parent = _islandParent;

        IslandChunk islandChunk = chunkObject.GetComponent<IslandChunk>();
        islandChunk.AudioHandler = _audioHandler;

        return islandChunk;
    }
}
