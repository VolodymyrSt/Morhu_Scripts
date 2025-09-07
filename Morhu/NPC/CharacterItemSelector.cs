using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Items;
using System;
using System.Collections;
using UnityEngine;

namespace Morhu.NPC
{
    public class CharacterItemSelector
    {
        private const int REQUIRED_BLEW_CANDLES_FOR_MATCHES = 2;

        private readonly Camera _camera;
        private readonly ItemPresenter _characterItemPresenter;
        private readonly ItemTray _characterItemTray;
        private readonly EnergySystem _energySystem;
        private readonly IAssetProvider _assetProvider;
        private readonly CardHand _cardHand;
        private readonly CardHand _oponentCardHand;
        private readonly CardFactory _cardFactory;

        public CharacterItemSelector(Camera camera, ItemTray itemTray, ItemPresenter itemPresenter,
            EnergySystem energySystem, IAssetProvider assetProvider, CardHand cardHand, CardHand oponentCardHand
            , CardFactory cardFactory)
        {
            _camera = camera;
            _characterItemPresenter = itemPresenter;
            _characterItemTray = itemTray;
            _energySystem = energySystem;
            _assetProvider = assetProvider;
            _cardHand = cardHand;
            _oponentCardHand = oponentCardHand;
            _cardFactory = cardFactory;
        }

        public void TryToUseItemAbility(Action onItemUsageEnded)
        {
            if (TryUseMatches(onItemUsageEnded)) return;
            if (TryUseMagnifyingGlassCombo(onItemUsageEnded)) return;
            if (TryUseLuckyCube(onItemUsageEnded)) return;
            if (TryUseRandomItem(onItemUsageEnded)) return;

            onItemUsageEnded?.Invoke();
        }

        public IEnumerator TryToSelectTwoItems(Action onSelectionEnded)
        {
            if (TryToSelectItem())
                yield return new WaitForSeconds(0.5f);

            if (TryToSelectItem())
                yield return new WaitForSeconds(0.5f);

            onSelectionEnded.Invoke();
        }

        private bool TryUseMatches(Action onItemUsageEnded)
        {
            if (_energySystem.GetCountOfBlewCandles() >= REQUIRED_BLEW_CANDLES_FOR_MATCHES &&
                _characterItemTray.TryGetItemFromTrayByType(ItemType.Matches, out Item matches))
            {
                UseItem(matches, onItemUsageEnded);
                return true;
            }
            return false;
        }


        private bool TryUseMagnifyingGlassCombo(Action onItemUsageEnded)
        {
            if (_characterItemTray.TryGetItemFromTrayByType(ItemType.MagnifyingGlass, out Item magnifyingGlass) &&
                _characterItemTray.TryGetItemFromTrayByType(ItemType.VoodooDoll, out Item voodooDoll))
            {
                UseItem(magnifyingGlass, () => UseItem(voodooDoll, onItemUsageEnded));
                return true;
            }
            return false;
        }

        private bool TryUseLuckyCube(Action onItemUsageEnded)
        {
            if (_characterItemTray.TryGetItemFromTrayByType(ItemType.LuckyCube, out Item luckyCube) &&
                _cardHand.AreAllCardHaveSameType())
            {
                UseItem(luckyCube, onItemUsageEnded);
                return true;
            }
            return false;
        }

        private bool TryUseRandomItem(Action onItemUsageEnded)
        {
            if (!_characterItemTray.CanAddItemToTray())
            {
                var item = _characterItemTray.GetRandomItemFromTray();

                if (item != null)
                {
                    UseItem(item, onItemUsageEnded);
                    return true;
                }
            }
            return false;
        }

        private void UseItem(Item item, Action onItemUsageEnded)
        {
            _characterItemTray.SetUpItemForUsage(item);
            item.UseAbility(false, _camera, _energySystem, _assetProvider, _cardHand
                     , _oponentCardHand, _cardFactory, onItemUsageEnded);
        }

        private bool TryToSelectItem()
        {
            if (_characterItemTray.CanAddItemToTray())
            {
                if (_energySystem.GetCountOfBlewCandles() >= REQUIRED_BLEW_CANDLES_FOR_MATCHES &&
                _characterItemPresenter.TryToGetItemByType(ItemType.Matches, out Item matches))
                {
                    _characterItemTray.AddItemToTray(matches, _characterItemPresenter);
                    return true;
                }
                else if (_cardHand.AreAllCardHaveSameType() &&
                    _characterItemPresenter.TryToGetItemByType(ItemType.LuckyCube, out Item luckyCube))
                {
                    _characterItemTray.AddItemToTray(luckyCube, _characterItemPresenter);
                    return true;
                }
                else if (_characterItemPresenter.TryToGetItemByType(ItemType.MagnifyingGlass, out Item magnifyingGlass) &&
                    _characterItemTray.IsItemFromTrayByType(ItemType.VoodooDoll))                    
                {
                    _characterItemTray.AddItemToTray(magnifyingGlass, _characterItemPresenter);
                    return true;
                }
                else if (_characterItemPresenter.TryToGetItemByType(ItemType.VoodooDoll, out Item voodooDoll) &&
                    _characterItemTray.IsItemFromTrayByType(ItemType.MagnifyingGlass))
                {
                    _characterItemTray.AddItemToTray(voodooDoll, _characterItemPresenter);
                    return true;
                }
                else
                {
                    var randomItem = _characterItemPresenter.GetRandomItem();
                    _characterItemTray.AddItemToTray(randomItem, _characterItemPresenter);
                    return true;
                }
            }
            return false;
        }
    }
}
