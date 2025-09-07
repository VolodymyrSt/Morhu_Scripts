using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Logic.HealthLogic;
using Morhu.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Morhu.Logic.EnergyLogic
{
    public class EnergySystem : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private List<CandleHandler> _candles = new();

        private HealthSystem _healthSystem;
        private IAudioService _audioService;

        public void Init(HealthSystem healthSystem, IAudioService audioService)
        {
            _healthSystem = healthSystem;
            _audioService = audioService;
        }

        public IEnumerator TryToIgniteAllCandles(Action onEnded)
        {
            _healthSystem.KillOneHeart();

            yield return new WaitForSeconds(Constants.LONG_ANIMATION_DURATION);

            if (_healthSystem.IsAllHeartsDone)
                _healthSystem.InvokeDeathEvent();
            else
            {
                for (int i = _candles.Count - 1; i >= 0; i--)
                {
                    _candles[i].IgniteSelf();
                    _audioService.BuildSound().WithParent(transform).Play("IgniteCandle");
                    yield return new WaitForSeconds(Constants.ANIMATION_DURATION);
                }
            }

            onEnded?.Invoke();
        }

        public void BlowOneCandle()
        {
            foreach (var candle in _candles)
            {
                if (!candle.IsBlew())
                {
                    _audioService.BuildSound().WithParent(transform).Play("BlowCandle");
                    candle.BlowSelf();
                    break;
                }
            }
        }

        public void IgniteOneCandle()
        {
            for (int i = _candles.Count - 1; i >= 0; i--)
            {
                if (_candles[i].IsBlew())
                {
                    _audioService.BuildSound().WithParent(transform).Play("IgniteCandle");
                    _candles[i].IgniteSelf();
                    break;
                }
            }
        }

        public int GetCountOfBlewCandles()
        {
            var count = 0;
            foreach (var candle in _candles)
            {
                if (candle.IsBlew())
                    count++;
            }
            return count;
        }

        public bool IsAllCandlesAreBlew()
        {
            foreach (var candle in _candles)
            {
                if (!candle.IsBlew())
                    return false;
            }

            return true;
        } 
    }
}
