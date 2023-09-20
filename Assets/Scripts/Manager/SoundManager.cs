using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Seunghak.Common
{
    public class SoundManager : UnitySingleton<SoundManager>
    {
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource voiceAudioSource;
        [SerializeField] private List<AudioSource> fxAudioSources;
        private string currentPlayBGM;
        public void PlayBGM(string soundName, float soundVolume)
        {
            AudioClip resourceAudio = GameResourceManager.Instance.LoadObject(soundName) as AudioClip;

            if (resourceAudio == null)
            {
                return;
            }

            if (bgmAudioSource.clip.name.Equals(resourceAudio.name))
            {
                return;
            }
            StartCoroutine(PlayBGM(resourceAudio, soundVolume));

        }
        private IEnumerator PlayBGM(AudioClip playingClip, float soundVolume)
        {
            while (bgmAudioSource.volume > 0)
            {
                bgmAudioSource.volume -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            bgmAudioSource.volume = soundVolume;
            bgmAudioSource.clip = playingClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            yield break;
        }
        public void PlaySound(string soundName, float soundVolume, bool isLoop/*,type도 넣어줄것*/)
        {
            AudioClip resourceAudio = GameResourceManager.Instance.LoadObject(soundName) as AudioClip;

            if (resourceAudio == null)
            {
                return;
            }

            StartCoroutine(PlayVoice(resourceAudio, soundVolume));

        }
        private IEnumerator PlayVoice(AudioClip playingClip, float soundVolume)
        {
            voiceAudioSource.volume = soundVolume;
            voiceAudioSource.clip = playingClip;
            voiceAudioSource.loop = true;
            voiceAudioSource.Play();
            yield break;

        }
        private IEnumerator PlayFx(AudioClip playingClip, float soundVolume)
        {
            bool isPlaying = false;
            for(int i=0;i< fxAudioSources.Count; i++)
            {
                if (!fxAudioSources[i].isPlaying)
                {
                    fxAudioSources[i].volume = soundVolume;
                    fxAudioSources[i].clip = playingClip;
                    fxAudioSources[i].loop = false;
                    fxAudioSources[i].Play();
                    isPlaying = true;
                }
            }
            if (!isPlaying)
            {
                GameObject newObject = new GameObject();
                AudioSource newaudio = newObject.AddComponent<AudioSource>();
                fxAudioSources.Add(newaudio);
                newaudio.volume = soundVolume;
                newaudio.clip = playingClip;
                newaudio.loop = false;
                newaudio.Play();
            }
            yield break;
        }
    }
}