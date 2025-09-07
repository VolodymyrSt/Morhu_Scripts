using Assets.Morhu.Logic.EnergyLogic;
using DG.Tweening;
using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Items.ScriptableObjects;
using Morhu.UI.ToolTip;
using Morhu.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Items
{
    public enum ItemType { VoodooDoll, Matches, MagnifyingGlass, LuckyCube}

    public class Item : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private ItemDescriptionSO _itemDescriptionSO;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ItemType _type;
        [SerializeReference, SubclassSelector] private ItemAbility _ability;

        private ToolTipTrigger _toolTipTrigger;
        private IAudioService _audioService;
        private List<Material> _materials;

        private bool _canBeSelectedByPlayer = false;
        private bool _canBeUsedByPlayer = false;
        private bool _isUsingSelf = false;

        public ItemType Type => _type;

        public void Init(ToolTipHandler toolTipHandler, IAudioService audioService, bool canBeSelectedByPlayer, IPersistanceDataService persistanceDataService)
        {
            SetIfCanBeSelectedByPlayer(canBeSelectedByPlayer);

            _materials = new List<Material>(_meshRenderer.materials);
            _audioService = audioService;

            var itemName = _itemDescriptionSO.GetDataByLanguage(persistanceDataService.Data.Language).Name;
            var itemAbility = _itemDescriptionSO.GetDataByLanguage(persistanceDataService.Data.Language).AbilityDescription;
            var itemUsageDescription = _itemDescriptionSO.GetDataByLanguage(persistanceDataService.Data.Language).HowToUse;

            _toolTipTrigger = new ToolTipTrigger(toolTipHandler, itemName, itemAbility, itemUsageDescription, ToolTipPosition.BottomRight);
        }

        private void Update()
        {
            if (_isUsingSelf)
                _ability.Tick();
        }

        public void UseAbility(bool isPlayer, Camera camera, EnergySystem ownEnergySystem, IAssetProvider assetProvider
            , CardHand cardHand, CardHand oponentCardHand, CardFactory cardFactory, Action onUsed)
        {
            _isUsingSelf = true;
            StartCoroutine(_ability.Use(isPlayer, _audioService, this, camera, ownEnergySystem, assetProvider, cardHand, oponentCardHand, cardFactory, onUsed));
        }


        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
        }

        public void HighLight()
        {
            _toolTipTrigger.Show();
            transform.DOScale(1.05f, 0.1f)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() =>
                    {
                        _audioService.BuildSound().WithParent(transform).Play("SelectItem");
                    });
        }

        public void DisHighLight()
        {
            transform.DOKill();
            _toolTipTrigger.Hide();
            transform.DOScale(1f, 0.1f)
                .SetEase(Ease.Linear)
                .Play();
        }

        public bool CanBeSelectedByPlayer() => _canBeSelectedByPlayer;
        public bool CanBeUsedByPlayer() => _canBeUsedByPlayer;

        public void SetIfCanBeUsedByPlayer(bool value) =>
            _canBeUsedByPlayer = value;   
        public void SetIfCanBeSelectedByPlayer(bool value) =>
            _canBeSelectedByPlayer = value;

        public void DestroySelf() => Destroy(gameObject);

        public void AddDissolvingMaterial(Material material) =>
            _materials.Add(material);

        public IEnumerator TaggleDissolveEffect(Action onDissolved)
        {
            var endWaitTime = 0.3f;
            var visibleAmountMultiplayer = 1f;
            var currentVisibleAmount = 0f;
            var limitVisibleAmount = 1f;

            PerformShakeAnimation();

            yield return new WaitForSeconds(Constants.LONG_ANIMATION_DURATION);

            _audioService.BuildSound().WithParent(transform).Play("DissolveItem");
            while (currentVisibleAmount <= limitVisibleAmount)
            {
                foreach (var material in _materials)
                {
                    currentVisibleAmount += visibleAmountMultiplayer * Time.deltaTime;
                    material.SetFloat("_visble_amount", currentVisibleAmount);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(endWaitTime);

            onDissolved?.Invoke();
        }

        public void PerformShakeAnimation()
        {
            transform.DOKill();
            transform.DOShakeRotation(Constants.LONG_ANIMATION_DURATION, 5, 10, 25)
                .SetEase(Ease.InOutCubic)
                .Play();
        }
    }
}
