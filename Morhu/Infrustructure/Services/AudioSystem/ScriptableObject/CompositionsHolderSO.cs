using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CompositionsHolder", menuName = "AudioSystem")]
    public class CompositionsHolderSO : ScriptableObject
    {
        [SerializeField] private List<CompositionData> _compositions = new();

        public SoundDataSO GetDataById(string id)
        {
            foreach (var composition in _compositions)
            {
                foreach (var sound in composition.Sounds)
                {
                    if (sound.ID == id)
                        return sound.Data;
                }
            }
            throw new Exception($"Couldn`t find clip with id {id}");
        }
    }

    [Serializable]
    public class CompositionData
    {
        public CompositionCategory CompositionCategory;
        public List<SoundDataStruct> Sounds = new();
    }

    public enum CompositionCategory
    {
        Cards, Character, Items, Candle, Heart, Music, Other
    }
}
