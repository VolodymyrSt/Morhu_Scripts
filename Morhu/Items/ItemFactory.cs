using Morhu.Factory;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.Localization;
using Morhu.UI.ToolTip;
using Morhu.Util;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Morhu.Items
{
    public class ItemFactory : BaseFactory<List<Item>>
    {
        public ItemFactory(IAssetProvider assetProvider) : base(assetProvider){}

        public override List<Item> Create(Vector3 at, Transform parent)
        {
            var offset = Constants.ITEM_BETWEEN_OFFSET;
            var items = new List<Item>();

            for (int i = 0; i < Constants.MAX_COUNT_OF_ITEMS_TO_PRESENT; i++)
            {
                var item = AssetProvider.Instantiate<Item>(AssetPath.GetRandomItemPath(), at + i * Vector3.forward * offset, parent);
                items.Add(item);
            }
            return items;
        }

        public List<Item> CreateAndInit(Vector3 at, Transform parent, bool canBeSelectedByPlayer, ToolTipHandler toolTipHandler,
            IAudioService audioService, IPersistanceDataService persistanceDataService)
        {
            var items = Create(at, parent);

            foreach (var item in items)
                item.Init(toolTipHandler, audioService, canBeSelectedByPlayer, persistanceDataService);

            return items;
        }
    }
}
