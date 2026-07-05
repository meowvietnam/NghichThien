using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    void OnEnable()
    {
        if(this.audioSource == null)
        {
            audioSource = this.GetComponentInChildren<AudioSource>(true);
        }    
        if (audioSource != null && audioSource.clip != null)
        {
            SoundManager.Instance.PlaySound(new SoundManager.AudioInfo {clip = audioSource.clip , isLoop =false, isMusic = false },false);
        }
    }
}
