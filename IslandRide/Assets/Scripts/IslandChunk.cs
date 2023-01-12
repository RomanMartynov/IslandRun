using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandChunk : MonoBehaviour
{
    private ChunkBlock[,] _chunkBlocks;
    private Transform _chunkParent;

    private Action _onChunkSpawned;
    private Action _onChunkExplode;

    private int _instantiatePositionX;
    private int _instantiatePositionZ;
    private int _roadPositionX;
    private int _roadPositionZ;

    private int[] _chunkEndPositions;
    private int[] _nextRoadPosition;

    private AudioHandler _audioHandler;

    public ChunkBlock[,] ChunkBlocks => _chunkBlocks;

    public int[] ChunkSize => new int[2] { _chunkBlocks.GetLength(0), _chunkBlocks.GetLength(1) };
    public int[] ChunkEndPositions => _chunkEndPositions;
    public int[] NextRoadPositions => _nextRoadPosition;

    public AudioHandler AudioHandler { set => _audioHandler = value; }

    public void CreateChunk(IslandChunksAsset islandChunkAsset, ChunkBlock[,] chunkBlocks, int[] instantiatePositions, int[] roadPositions, Action onChunkSpawned, Action onChunkExplode)
    {
        _instantiatePositionX = instantiatePositions[0];
        _instantiatePositionZ = instantiatePositions[1];

        _roadPositionX = roadPositions[0];
        _roadPositionZ = roadPositions[1];

        _chunkParent = transform;

        _onChunkSpawned = onChunkSpawned;
        _onChunkExplode = onChunkExplode;

        CenterChunkParent(chunkBlocks.GetLength(0) - 1, instantiatePositions);
        StartCoroutine(BuildRoad(islandChunkAsset, chunkBlocks));
    }

    private IEnumerator BuildRoad(IslandChunksAsset islandChunkAsset, ChunkBlock[,] chunkBlocks)
    {
        int blockX = _roadPositionX;
        int blockZ = _roadPositionZ;

        int passedRoadsCount = 0;

        bool endDefined = false;

        Destroy(chunkBlocks[blockX, blockZ].gameObject);
        chunkBlocks[blockX, blockZ] = Instantiate(islandChunkAsset.Chunk_Road, new Vector3(_instantiatePositionX + blockX, 0f, _instantiatePositionZ + blockZ), Quaternion.identity, _chunkParent);

        yield return new WaitForSeconds(0.1f);

        while (endDefined == false)
        {
            List<int[]> possibleDirections = new List<int[]>();

            for (int i = 0; i < 4; i++)
            {
                int[] possibleBuildDirection = GetBuildDirection(i);

                int possibleBlockX = blockX + possibleBuildDirection[0];
                int possibleBlockZ = blockZ + possibleBuildDirection[1];

                if (possibleBlockX < 0 || possibleBlockX >= chunkBlocks.GetLength(0) ||
                possibleBlockZ < 0 || possibleBlockZ >= chunkBlocks.GetLength(1))
                {
                    if (passedRoadsCount > (chunkBlocks.GetLength(0) * 2))
                    {
                        (chunkBlocks[blockX, blockZ] as RoadBlock).SetChunkExit();
                        _chunkEndPositions = new int[2] { blockX, blockZ };

                        endDefined = true;

                        goto End;
                    }
                }

                else
                {
                    if (chunkBlocks[possibleBlockX, possibleBlockZ] is FrameBlock)
                    {
                        possibleDirections.Add(possibleBuildDirection);
                    }
                }
            }

            if (possibleDirections.Count != 0)
            {
                int nextDirectionIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
                int[] nextDirection = possibleDirections[nextDirectionIndex];

                int possibleBuildX = blockX + nextDirection[0];
                int possibleBuildZ = blockZ + nextDirection[1];

                if (chunkBlocks[possibleBuildX, possibleBuildZ] is FrameBlock)
                {
                    Destroy(chunkBlocks[possibleBuildX, possibleBuildZ].gameObject);

                    chunkBlocks[possibleBuildX, possibleBuildZ] = Instantiate(islandChunkAsset.Chunk_Road, new Vector3(_instantiatePositionX + possibleBuildX, 0f, _instantiatePositionZ + possibleBuildZ), Quaternion.identity, _chunkParent);
                    passedRoadsCount++;

                    blockX = possibleBuildX;
                    blockZ = possibleBuildZ;

                    _audioHandler.PlayBlockSound(islandChunkAsset.Chunk_Road.AudioSpawn);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            else
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] possibleRoadDirection = GetBuildDirection(i);

                    int possibleRoadX = blockX + possibleRoadDirection[0];
                    int possibleRoadZ = blockZ + possibleRoadDirection[1];

                    if (possibleRoadX >= 0 && possibleRoadX < chunkBlocks.GetLength(0) &&
                        possibleRoadZ >= 0 && possibleRoadZ < chunkBlocks.GetLength(1))
                    {
                        blockX = possibleRoadX;
                        blockZ = possibleRoadZ;

                        passedRoadsCount++;

                        break;
                    }
                }
            }
        End:;
        }

        StartCoroutine(BuildGround(islandChunkAsset, chunkBlocks));
    }

    private IEnumerator BuildGround(IslandChunksAsset islandChunkAsset, ChunkBlock[,] chunkBlocks)
    {
        _audioHandler.PlayBlockSound(islandChunkAsset.Chunk_Ground.AudioSpawn);

        for (int x = 0; x < chunkBlocks.GetLength(0); x++)
        {
            for (int z = 0; z < chunkBlocks.GetLength(1); z++)
            {
                if (chunkBlocks[x, z] is FrameBlock)
                {
                    Destroy(chunkBlocks[x, z].gameObject);
                    chunkBlocks[x, z] = Instantiate(islandChunkAsset.Chunk_Ground, new Vector3(_instantiatePositionX + x, 0f, _instantiatePositionZ + z), Quaternion.identity, _chunkParent);
                    yield return new WaitForSeconds(0.03f);
                }
            }
        }

        _chunkBlocks = chunkBlocks;
        _onChunkSpawned?.Invoke();

        DestroyChunk();
    }

    private int[] GetBuildDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                return new int[] { 1, 0 };
            case 1:
                return new int[] { -1, 0 };
            case 2:
                return new int[] { 0, 1 };
            case 3:
                return new int[] { 0, -1 };
            default:
                throw new Exception("Incorrect direction!");
        }
    }

    private void CenterChunkParent(float size, int[] instantiatePositions)
    {
        List<Transform> chunkBlocks = new List<Transform>();

        foreach (Transform child in _chunkParent)
        {
            chunkBlocks.Add(child);
        }

        int instantiatePositionX = instantiatePositions[0];
        int instantiatePositionZ = instantiatePositions[1];

        _chunkParent.DetachChildren();
        _chunkParent.position = new Vector3(instantiatePositionX + (size / 2f), 0f, instantiatePositionZ + (size / 2f));

        foreach (Transform chunk in chunkBlocks)
        {
            chunk.SetParent(_chunkParent);
        }
    }

    private void DestroyChunk()
    {
        StartCoroutine(BlowUpChunk());
    }

    private IEnumerator BlowUpChunk()
    {
        yield return new WaitForSeconds(3f);

        _onChunkExplode?.Invoke();

        foreach (Transform child in _chunkParent)
        {
            Rigidbody block = child.gameObject.AddComponent<Rigidbody>();
            block.AddForce(Vector3.up * 3f, ForceMode.Impulse);

            if (child.TryGetComponent<Collider>(out Collider collider))
            {
                collider.enabled = false;
            }

            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(DestroyChunkBlocks());
    }

    private IEnumerator DestroyChunkBlocks()
    {
        yield return new WaitForSeconds(2f);

        foreach (Transform child in _chunkParent)
        {
            Destroy(child.gameObject);
        }

        Destroy(_chunkParent.gameObject);
    }
}
