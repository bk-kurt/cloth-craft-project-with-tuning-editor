using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private AudioSource audioSource;

    public void PlayAudioClip(int clipNo, float pitch = 1, float volume = 1)
    {
        var clipToPlay = clips[clipNo];
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.PlayOneShot(clipToPlay);
    }

    public void PlayAudioClipWithLoop(int clipNo, float duration, float pitch = 1, float volume = 1)
    {
        AudioSource loopedAudioSource = transform.AddComponent<AudioSource>();
        loopedAudioSource.loop = true;
        var clipToPlay = clips[clipNo];
        loopedAudioSource.clip = clipToPlay;

        loopedAudioSource.pitch =pitch;
        loopedAudioSource.volume = 0;


        StartCoroutine(PlayLoopedSound(volume,duration, loopedAudioSource));
    }

    private IEnumerator PlayLoopedSound(float volume,float duration, AudioSource loopedAudioSource)
    {
        loopedAudioSource.Play();
        loopedAudioSource.DOFade(volume, 0.5f);
        yield return new WaitForSeconds(duration);
        loopedAudioSource.DOFade(0, 0.5f).OnComplete(() =>
        {
            loopedAudioSource.Stop();
            Destroy(loopedAudioSource);
        });
    }
}