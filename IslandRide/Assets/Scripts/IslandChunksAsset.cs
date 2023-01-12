using UnityEngine;

[CreateAssetMenu(menuName = "IslandChunksAsset")]
public class IslandChunksAsset : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private string _title;

    [Header("Chunks")]
    [SerializeField] private RoadBlock _chunk_Road;
    [SerializeField] private ChunkBlock _chunk_Ground;

    public string Title => _title;
    public RoadBlock Chunk_Road => _chunk_Road;
    public ChunkBlock Chunk_Ground => _chunk_Ground;

}
