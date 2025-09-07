using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Morhu.Infrustructure.Services.AudioSystem
{
    [Serializable]
    public class SoundData 
    {
        public AudioClip Clip;
        public AudioMixerGroup Mixer;
        public bool Loop;
        public bool PlayOnAwake;
        public bool FrequentSound;

        public bool Mute = false;
        public int Priority = 128;
        public float Volume = 1f;
        public float Pitch = 1f;

        public float MinDistance = 1f;
        public float MaxDistance = 500f;

        public SoundData(AudioClip clip, AudioMixerGroup mixer, bool loop, bool playOnAwake
            , bool frequentSound, bool mute, int priority, float volume,
            float pitch, float minDistance, float maxDistance)
        {
            Clip = clip;
            Mixer = mixer;
            Loop = loop;
            PlayOnAwake = playOnAwake;
            FrequentSound = frequentSound;

            Mute = mute;
            Priority = priority;
            Volume = volume;
            Pitch = pitch;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
        }
    }
}
