using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "AudioSystem")]
    public class SoundDataSO : ScriptableObject 
    {
        public AudioClip Clip;
        public AudioMixerGroup Mixer;
        public bool Loop = false;
        public bool PlayOnAwake = false;
        public bool FrequentSound = false;

        public bool Mute = false;
        public int Priority = 128;
        public float Volume = 1f;
        public float Pitch = 1f;

        public float MinDistance = 1f;
        public float MaxDistance = 500f;
    }


    [Serializable]
    public struct SoundDataStruct
    {
        public SoundDataSO Data;
        public string ID;
    }
}
