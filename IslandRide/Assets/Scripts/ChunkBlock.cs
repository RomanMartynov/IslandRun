using UnityEngine;

public abstract class ChunkBlock : MonoBehaviour
{
    [Header("Sounds")] 
    [SerializeField] private AudioClip _audio_Spawn;

    public AudioClip AudioSpawn => _audio_Spawn; 

    [SerializeField]
    private void OnEnable()
    {
        gameObject.isStatic = true;
    }
}
