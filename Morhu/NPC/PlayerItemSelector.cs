using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Items;
using UnityEngine;

namespace Morhu.Player
{
    public class PlayerItemSelector
    {
        private readonly Camera _camera;
        private readonly ItemTray _playerItemTray;
        private readonly ItemPresenter _playerItemPresenter;
        private readonly EnergySystem _energySystem;
        private readonly IAssetProvider _assetProvider;
        private readonly CardHand _cardHand;
        private readonly CardHand _oponentCardHand;
        private readonly CardFactory _cardFactory;

        private Item _currentSelectedItem;

        private bool _canPlayerSelectItems = false;
        private bool _isPlayerSelectedItems = false;
        private int _countOfChosenItems = 0;
        private bool _canPlayerUseItems = false;
        private bool _isPlayerUsingItem = false;

        public PlayerItemSelector(Camera camera, ItemTray itemTray, ItemPresenter itemPresenter,
            EnergySystem energySystem, IAssetProvider assetProvider, CardHand cardHand, CardHand oponentCardHand, CardFactory cardFactory)
        {
            _camera = camera;
            _playerItemTray = itemTray;
            _playerItemPresenter = itemPresenter;
            _energySystem = energySystem;
            _assetProvider = assetProvider;
            _cardHand = cardHand;
            _oponentCardHand = oponentCardHand;
            _cardFactory = cardFactory;
        }

        public void Update()
        {
            if (_canPlayerSelectItems)
            {
                TryToSelectItem(false);
                TryToPickSelectedItem();
            }
            if (_canPlayerUseItems)
            {
                TryToSelectItem(true);
                TryToUseItem();
            }
        }

        private void TryToPickSelectedItem()
        {
            if (_countOfChosenItems < 2 && _currentSelectedItem != null
                && Input.GetMouseButtonDown(0) && _playerItemTray.CanAddItemToTray())
            {
                _currentSelectedItem.DisHighLight();
                _playerItemTray.AddItemToTray(_currentSelectedItem, _playerItemPresenter);
                _currentSelectedItem.SetIfCanBeUsedByPlayer(true);
                _currentSelectedItem = null;
                _countOfChosenItems++;

                if (_countOfChosenItems >= 2 || !_playerItemTray.CanAddItemToTray())
                    _isPlayerSelectedItems = true;
            }
        }

        private void TryToSelectItem(bool isForUsage)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out Item item))
                {
                    if (isForUsage)
                    {
                        if (_isPlayerUsingItem)
                        {
                            TryToClearSelectedItem();
                            return;
                        }

                        if (item.CanBeUsedByPlayer())
                        {
                            if (_currentSelectedItem != item)
                                SelectItem(item);
                            else
                                return;
                        }
                    }
                    else
                    {
                        if (_isPlayerSelectedItems)
                        {
                            TryToClearSelectedItem();
                            return;
                        }

                        if (item.CanBeSelectedByPlayer())
                        {
                            if (_currentSelectedItem != item)
                                SelectItem(item);
                            else
                                return;
                        }
                    }
                }
            }
            else
                TryToClearSelectedItem();
        }

        private void SelectItem(Item item)
        {
            TryToClearSelectedItem();
            _currentSelectedItem = item;
            item.HighLight();
        }

        private void TryToUseItem()
        {
            if (_currentSelectedItem != null && _currentSelectedItem.CanBeUsedByPlayer()
                && Input.GetMouseButtonDown(0) && !_isPlayerUsingItem)
            {
                _isPlayerUsingItem = true;
                var item = _currentSelectedItem;
                item.DisHighLight();
                item.SetIfCanBeUsedByPlayer(false);
                item.SetIfCanBeSelectedByPlayer(false);

                _playerItemTray.SetUpItemForUsage(item);
                item.UseAbility(true, _camera, _energySystem, _assetProvider, _cardHand, _oponentCardHand, _cardFactory, () =>
                {
                    item.DestroySelf();
                    _isPlayerUsingItem = false;
                });
            }
        }

        public void RestartSelector()
        {
            _countOfChosenItems = 0;
            _isPlayerSelectedItems = false;
        }

        public bool IsPlayerSelectedItems() => _isPlayerSelectedItems;

        public bool IsPlayerUsingItem() => _isPlayerUsingItem;

        public void SetIfPlayerCanSelectItems(bool canPlayerSelect) =>
            _canPlayerSelectItems = canPlayerSelect;

        public void SetIfPlayerCanUseItems(bool canPlayerUseItems) =>
            _canPlayerUseItems = canPlayerUseItems;

        private void TryToClearSelectedItem()
        {
            if (_currentSelectedItem != null)
            {
                _currentSelectedItem.DisHighLight();
                _currentSelectedItem = null;
            }
        }
    }
}
