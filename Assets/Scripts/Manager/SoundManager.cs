using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Seunghak.Common
{
    public class SoundManager : UnitySingleton<SoundManager>
    {
        [SerializeField] private AudioSource bgmAudioSource;
        
        private string currentPlayBGM;
        public void PlayBGM(string soundName, float soundVolume)
        {
            AudioClip resourceAudio = GameResourceManager.Instance.LoadObject(soundName) as AudioClip;

            if(resourceAudio==null)
            { 
                return;
            }

            if (bgmAudioSource.clip.name.Equals(resourceAudio.name))
            {
                return;
            }
            StartCoroutine(PlayBGM(resourceAudio, soundVolume));

        }
        private IEnumerator PlayBGM(AudioClip playingClip,float soundVolume)
        {
            while (bgmAudioSource.volume >0)
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
        public void PlaySound(string soundName, float soundVolume, bool isLoop)
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

            yield break;
        }
    }
}