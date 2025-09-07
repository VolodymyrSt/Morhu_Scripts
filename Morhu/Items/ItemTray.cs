using Morhu.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Morhu.Items
{
    public class ItemTray : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private Transform _itemsHolder;
        [SerializeField] private Transform _itemUsageHolder;

        private readonly List<Item> _items = new();

        public void AddItemToTray(Item item, ItemPresenter itemPresenter)
        {
            item.SetParent(_itemsHolder);
            itemPresenter.RemoveItem(item);
            _items.Add(item);
            UpdateItemsPosition();
        } 

        public void SetUpItemForUsage(Item item)
        {
            if (_items.Contains(item) && item != null)
            {
                item.SetParent(_itemUsageHolder);
                RemoveItemFromTray(item);
            }
        }

        //AI
        public bool TryGetItemFromTrayByType(ItemType type, out Item foundItem)
        {
            foundItem = null;

            if (_items == null || _items.Count == 0)
                return false;

            foundItem = _items.Find(item => item != null && item.Type == type);

            if (foundItem != null) 
                return true;
            return false;
        }

        //AI
        public bool IsItemFromTrayByType(ItemType type)
        {
            if (_items == null || _items.Count == 0)
                return false;

            var foundItem = _items.Find(item => item != null && item.Type == type);

            if (foundItem != null)
                return true;
            return false;
        }

        //AI
        public Item GetRandomItemFromTray()
        {
            if (_items == null || _items.Count == 0)
                return null;

            var validItems = _items.Where(item => item != null).ToList();
            if (validItems.Count == 0)
                return null;

            var randomIndex = Random.Range(0, validItems.Count);
            var randomItem = validItems[randomIndex];

            return randomItem;
        }

        public bool CanAddItemToTray() =>
            _items.Count < Constants.MAX_ITEM_CAPACITY;

        private void RemoveItemFromTray(Item item)
        {
            _items.Remove(item);
            UpdateItemsPosition();
        }

        private void UpdateItemsPosition()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Vector3 targetPostion;
                if (i == 2)
                {
                    targetPostion = new Vector3(_itemsHolder.position.x,
                        _itemsHolder.position.y, _itemsHolder.position.z + 0.22f);
                }
                else if (i == 3)
                {
                    targetPostion = new Vector3(_itemsHolder.position.x -0.22f,
                        _itemsHolder.position.y, _itemsHolder.position.z + 0.22f);
                }
                else
                {
                    targetPostion = new Vector3(_itemsHolder.position.x + i * -0.22f,
                    _itemsHolder.position.y, _itemsHolder.position.z);
                }

                _items[i].transform.position = targetPostion;
            }
        }
    }
}
