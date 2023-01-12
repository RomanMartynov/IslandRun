using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] private AudioSource _audioSource_BlockSounds;
    [SerializeField] private AudioSource _audioSource_PlayerSounds;
    [SerializeField] private AudioSource _audioSource_Music;

    public void PlayBlockSound(AudioClip blockAudioClip)
    {
        if (blockAudioClip != null)
            _audioSource_BlockSounds.PlayOneShot(blockAudioClip);
    }

    public void PlayPlayerSound(AudioClip playerAudioClip)
    {
        if (playerAudioClip != null)
            _audioSource_PlayerSounds.PlayOneShot(playerAudioClip);
    }
}
