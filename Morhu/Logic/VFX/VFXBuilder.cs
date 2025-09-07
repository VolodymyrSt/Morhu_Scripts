using Morhu.Deck;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Services.AudioSystem;
using UnityEngine;

namespace Assets.Morhu.Logic.VFX
{
    public class VFXBuilder
    {
        private readonly AssetProvider _assetProvider;
        private readonly IAudioService _audioService;

        public VFXBuilder(AssetProvider assetProvider, IAudioService audioService)
        {
            _assetProvider = assetProvider;
            _audioService = audioService;
        }

        public void BuildForCard(CardType cardType, Vector3 at, Transform parent, CardAbility cardAbility)
        {
            switch (cardType) 
            { 
                case CardType.Heart:
                    Build(AssetPath.HeartEffectPath, at, parent);
                    break;
                case CardType.Diamond:
                    Build(AssetPath.DiamondEffectPath, at, parent);
                    break;
                case CardType.Club:
                    Build(AssetPath.ClubEffectPath, at, parent);
                    break;
                case CardType.Spade:
                    Build(AssetPath.SpadeEffectPath, at, parent);
                    break;
                case CardType.RedJocker:
                    Build(AssetPath.RedJokerEffectPath, at, parent);
                    break;
                case CardType.BlackJocker:
                    {
                        if (cardAbility is AceAttackAbility)
                            Build(AssetPath.BlackJokerSpadeEffectPath, at, parent);
                        else if (cardAbility is AceHealAbility)
                            Build(AssetPath.BlackJokerHeartEffectPath, at, parent);
                        else if (cardAbility is AceStealingAbility)
                            Build(AssetPath.BlackJokerDiamondEffectPath, at, parent);
                        else 
                            Build(AssetPath.BlackJokerClubEffectPath, at, parent);
                    }
                    break;
            }
        }

        private void Build(string vfxPath, Vector3 at, Transform parent) => 
            _assetProvider.Instantiate<ParticleSystem>(vfxPath, at, parent);
    }
}
