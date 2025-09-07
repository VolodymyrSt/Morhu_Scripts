using DG.Tweening;
using Di;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.Localization;
using Morhu.UI.ToolTip;
using Morhu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Items
{
    public class ItemPresenter : ConstructedMonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private Transform _handle;
        [SerializeField] private Transform _trayForItems;
        [SerializeField] private Transform _startPointForItems;
        [SerializeField] private bool _canItemsBeSelectedByPlayer;

        private ItemFactory _itemFactory;
        private ToolTipHandler _toolTipHandler;
        private IAudioService _audioService;
        private IPersistanceDataService _persistanceDataService;
        private List<Item> _items;

        public override void Construct(IResolvable container)
        {
            _itemFactory = container.Resolve<ItemFactory>();
            _toolTipHandler = container.Resolve<ToolTipHandler>();
            _audioService = container.Resolve<IAudioService>();
            _persistanceDataService = container.Resolve<IPersistanceDataService>();
        }

        public void OpenAndPresentItems(Action onPresented)
        {
            var createdItems = _itemFactory.CreateAndInit(_startPointForItems.position, _startPointForItems,
                _canItemsBeSelectedByPlayer, _toolTipHandler, _audioService, _persistanceDataService);
            _items = new List<Item>(createdItems);

            _audioService.BuildSound().WithParent(transform).Play("OpenItemPresenter");

            var targetRotation = Quaternion.Euler(120f, 0f, 0f);
            _handle.DOLocalRotateQuaternion(targetRotation, Constants.LONG_ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() => {
                    var targetXPosition = 0.14f;
                    _trayForItems.DOLocalMoveX(targetXPosition, Constants.LONG_ANIMATION_DURATION)
                        .SetEase(Ease.Linear)
                        .Play()
                        .OnComplete(() => onPresented?.Invoke());
                });
        }

        public void Close()
        {
            var targetXPosition = -0.105f;

            _trayForItems.DOLocalMoveX(targetXPosition, Constants.LONG_ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .Play()
                .OnComplete(() =>
                {
                    var targetRotation = Quaternion.Euler(0f, 0f, 0f);

                    _audioService.BuildSound().WithParent(transform).Play("CloseItemPresenter");
                    _handle.DOLocalRotateQuaternion(targetRotation, Constants.LONG_ANIMATION_DURATION)
                        .SetEase(Ease.Linear)
                        .Play()
                        .OnComplete(() =>
                        {
                            foreach (var item in _items)
                                item.DestroySelf();
                            _items.Clear();
                        });
                });
        }

        public bool TryToGetItemByType(ItemType type, out Item searchedItem)
        {
            searchedItem = null;
            if (_items.Count == 0) 
                return false;

            foreach (var item in _items)
            {
                if (item.Type == type)
                {
                    searchedItem = item;
                    return true;
                }
            }

            return false;
        }

        public Item GetRandomItem()
        {
            var randomIndex = UnityEngine.Random.Range(0, _items.Count);
            var randomItem = _items[randomIndex];
            _items.RemoveAt(randomIndex);
            return randomItem;
        }

        public void RemoveItem(Item item) =>
            _items.Remove(item);
    }
}
