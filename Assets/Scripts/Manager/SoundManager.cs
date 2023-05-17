using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Seunghak.Common
{
    public class SoundManager : UnitySingleton<SoundManager>
    {
        [SerializeField] private AudioSource bgmAudioSource;
        
        private string currentPlayBGM;
        public void PlayBGM(string soundName, float soundVolume,bool soundLoop)
        {

            //리소스 로드 및 bgm 플레이
        }

    }
}