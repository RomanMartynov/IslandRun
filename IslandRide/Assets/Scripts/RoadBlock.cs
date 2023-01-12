using UnityEngine;

public class RoadBlock : ChunkBlock
{
    [Header("Sounds")] 
    [SerializeField] private AudioClip _audio_Riding;

    [Header("Objects")] 
    [SerializeField] private GameObject _chunkEndMark;
    private bool _isChunkExit;

    public bool IsChunkExit => _isChunkExit;
    public AudioClip AudioRiding => _audio_Riding;

    public void SetChunkExit()
    {
        _isChunkExit = true;
        _chunkEndMark.SetActive(true);
    }
}
