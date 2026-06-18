using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance { get; private set; }

    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip loseClip;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource bgmSource;

    [SerializeField] private float bgmFadeDuration = 7f;

    private void Awake()
    {
        instance = this;
    }

    public void shotPlay()
    {
        if (audioSource.clip != null)
            audioSource.Stop();

        audioSource.clip = shotClip;
        audioSource.Play();
    }

    public void losePlay()
    {
        if (audioSource.clip != null)
            audioSource.Stop();

        audioSource.clip = loseClip;
        audioSource.Play();

        StartCoroutine(FadeBGM(loseClip.length));
    }

    private IEnumerator FadeBGM(float loseAudioLength)
    {
        float fadeTime = 0f;
        float startVolume = bgmSource.volume;

        bgmSource.volume = 0f;

        yield return new WaitForSecondsRealtime(loseAudioLength-4);

        while(fadeTime < bgmFadeDuration)
        {
            fadeTime += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume, fadeTime / bgmFadeDuration);
            yield return null;
        }

        bgmSource.volume = startVolume;
    }
}
