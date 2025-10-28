using System;
using Ami.BroAudio;
using UnityEngine;

namespace Work.PSB.Code.Managers
{
    public class BackMusicPlayer : MonoBehaviour
    {
        [SerializeField] private SoundID backMusic;

        private void Start()
        {
            BroAudio.Play(backMusic); 
        }
        
    }
}