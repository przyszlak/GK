﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[System.Serializable]
public class SoundEvent
{
    public string name;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public AudioMixerGroup output;
    [Range(0f, 1f)] public float minVolume = 1f;
    [Range(0f, 1f)] public float maxVolume = 1f;
    [Range(0f, 3f)] public float minPitch = 1f;
    [Range(0f, 3f)] public float maxPitch = 1f;
    [Range(-1f, 1f)] public float stereoPan = 0f;
    [Range(0f, 5f)] public float delayTime = 0f;
    [Range(0f, 5f)] public float randomizeDelay = 0f;
    public bool avoidRepeat = false;
    public bool delay = false;
    public bool randomizeLoop = false;
    public bool loop = false;
    public bool mute = false;
    public bool playOnAwake = false;
    [HideInInspector] public bool playCalled = false;

    public void Play()
    {
        playCalled = true;

        float randomVolume = Random.Range(minVolume, maxVolume);
        float randomPitch = Random.Range(minPitch, maxPitch);

        audioSource.volume = randomVolume;
        audioSource.pitch = randomPitch;
        audioSource.panStereo = stereoPan;
        audioSource.loop = loop;
        audioSource.mute = mute;
        audioSource.outputAudioMixerGroup = output;

        if(!delay && !avoidRepeat)
        {
            if (audioClips.Length > 0)
            {
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.Play();
            }
        }

        else if(avoidRepeat && !delay)
        {
            if (audioClips.Length > 0)
            {
                int r = Random.Range(1, audioClips.Length);

                if (audioClips.Length == 1)
                    r = 0;

                audioSource.clip = audioClips[r];
                audioSource.Play();
                audioClips[r] = audioClips[0];
                audioClips[r] = audioSource.clip;
            }
            else
            {

            }
        }

        else if (avoidRepeat && delay)
        {
            if (audioClips.Length > 0)
            {
                float _delay = Random.Range(delayTime - randomizeDelay, delayTime + randomizeDelay);
                int r = Random.Range(1, audioClips.Length);

                if (audioClips.Length == 1)
                    r = 0;

                audioSource.clip = audioClips[r];
                audioSource.PlayDelayed(_delay);
                audioClips[r] = audioClips[0];
                audioClips[r] = audioSource.clip;
            }
        }

        else if (!avoidRepeat && delay)
        {
            if (audioClips.Length > 0)
            {
                float _delay = Random.Range(delayTime - randomizeDelay, delayTime + randomizeDelay);
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.PlayDelayed(_delay);
            }
        }
    }

    public void Stop()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance = null;

    [SerializeField]
    SoundEvent[] SoundEvents;

    private bool changingSound = false;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        for(int i = 0; i < SoundEvents.Length; i++)
        {
            if(SoundEvents[i].audioSource == null)
            {
                GameObject gameObject = new GameObject("SoundEvent_" + i + "_" + SoundEvents[i].name);
                gameObject.transform.SetParent(this.transform);
                SoundEvents[i].audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (SoundEvents[i].playOnAwake)
            {
                SoundEvents[i].Play();
            }
        }
    }

    private void Update()
    {
        InitializeCoroutine();
    }

    void InitializeCoroutine()
    {
        StartCoroutine(RepeatPlaySound());
    }

    public void PlaySound(string name)
    {
        SoundEvent s = Array.Find(SoundEvents, sound => sound.name == name);

        if(s == null)
        {
            Debug.LogWarning("SoundManager: Sound not found in SoundEvents [" + name + "]");
            return;
        }

        s.Play();
    }
    
    public void StopSound(string name)
    {
        SoundEvent s = Array.Find(SoundEvents, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("SoundManager: Sound not found in SoundEvents [" + name + "]");
            return;
        }

        s.Stop();
    }

    public bool IsSoundPlaying(string name)
    {
        SoundEvent s = Array.Find(SoundEvents, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("SoundManager: Sound not found in SoundEvents [" + name + "]");
            return false;
        }

        return s.IsPlaying();
    }

    IEnumerator RepeatPlaySound()
    {
        for (int i = 0; i < SoundEvents.Length; i++)
        {
            if(SoundEvents[i].playCalled && SoundEvents[i].randomizeLoop && SoundEvents[i].avoidRepeat)
            {
                if (!SoundEvents[i].delay)
                {
                    yield return new WaitForSeconds(SoundEvents[i].audioSource.clip.length);
                }

                else
                {
                    float delay = Random.Range(SoundEvents[i].delayTime - SoundEvents[i].randomizeDelay, SoundEvents[i].delayTime + SoundEvents[i].randomizeDelay);
                    yield return new WaitForSeconds(SoundEvents[i].audioSource.clip.length + delay);
                }

                if (!SoundEvents[i].audioSource.isPlaying)
                {
                    PlaySound(SoundEvents[i].name);
                }
            }
        }
    }

    public void ChangeMusicInicjalizeCoroution(string from, string to)
    {
        StartCoroutine(ChangeMusic(from, to));
    }

    IEnumerator ChangeMusic(string from, string to)
    {
        if (changingSound)
            StopCoroutine(ChangeMusic(from, to));
        else
            changingSound = true;
        SoundEvent SoundNowPlaying = Array.Find(SoundEvents, sound => sound.name == from);
        SoundEvent SoundToPlay = Array.Find(SoundEvents, sound => sound.name == from);
        if (SoundNowPlaying == null)
        {
            Debug.LogWarning("SoundManager: Sound not found in SoundEvents [" + from + "]");
            yield return null;
        }
        else if (SoundToPlay == null)
        {
            Debug.LogWarning("SoundManager: Sound not found in SoundEvents [" + to + "]");
            yield return null;
        }
        else
        {
            while (SoundNowPlaying.audioSource.volume > 0f)
            {
                yield return new WaitForSeconds(0.025f);
                SoundNowPlaying.audioSource.volume -= 0.01f;
            }

            yield return new WaitForSeconds(0.2f);

            StopSound(from);
            PlaySound(to);
            changingSound = false;
        }
    }

}
