using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Morhu.Infrustructure.AssetManagement
{
    public static class AssetPath
    {
        public static string SoundEmitterPath = "Audio/SoundEmitter";
        public static string SoundHolderPath = "Audio/SoundHolder";

        public static string MainCameraPath = "Camera/MainCamera";

        //deck Effects
        public static string SpadeEffectPath = "Effects/Sword";
        public static string HeartEffectPath = "Effects/HolyEffect";
        public static string ClubEffectPath = "Effects/Shield";
        public static string RedJokerEffectPath = "Effects/Swap";
        public static string DiamondEffectPath = "Effects/Mirror";

        public static string BlackJokerDiamondEffectPath = "Effects/Diamond";
        public static string BlackJokerHeartEffectPath = "Effects/Heart";
        public static string BlackJokerSpadeEffectPath = "Effects/Spade";
        public static string BlackJokerClubEffectPath = "Effects/Club";

        //deck
        public static string SpadeAcePath = "Deck/Spade/Spade_Ace";
        public static string SpadeKingPath = "Deck/Spade/Spade_King";
        public static string SpadeQueenPath = "Deck/Spade/Spade_Queen";
        public static string SpadeJackPath = "Deck/Spade/Spade_Jack";

        public static string ClubAcePath = "Deck/Club/Club_Ace";
        public static string ClubKingPath = "Deck/Club/Club_King";
        public static string ClubQueenPath = "Deck/Club/Club_Queen";
        public static string ClubJackPath = "Deck/Club/Club_Jack";

        public static string DiamondAcePath = "Deck/Diamond/Diamond_Ace";
        public static string DiamondKingPath = "Deck/Diamond/Diamond_King";
        public static string DiamondQueenPath = "Deck/Diamond/Diamond_Queen";
        public static string DiamondJackPath = "Deck/Diamond/Diamond_Jack";

        public static string HeartAcePath = "Deck/Heart/Heart_Ace";
        public static string HeartKingPath = "Deck/Heart/Heart_King";
        public static string HeartQueenPath = "Deck/Heart/Heart_Queen";
        public static string HeartJackPath = "Deck/Heart/Heart_Jack";

        public static string BlackJokerPath = "Deck/Jokers/Black_Joker";
        public static string RedJokerPath = "Deck/Jokers/Red_Joker";

        //items
        public static string LuckyCubePath = "Items/LuckyCube";
        public static string MagnifyingGlassPath = "Items/MagnifyingGlass";
        public static string MatchesPath = "Items/Matches";
        public static string VoodoDollPath = "Items/VoodoDoll";

        private readonly static string[] _allCardPaths = new string[]
        {
                SpadeAcePath, SpadeKingPath, SpadeQueenPath, SpadeJackPath,
                ClubAcePath, ClubKingPath, ClubQueenPath, ClubJackPath,
                DiamondAcePath, DiamondKingPath, DiamondQueenPath, DiamondJackPath,
                HeartAcePath, HeartKingPath, HeartQueenPath, HeartJackPath,
                BlackJokerPath, RedJokerPath
        };

        private readonly static string[] _allItemPaths = new string[]
        {
                LuckyCubePath, MagnifyingGlassPath,
                MatchesPath, VoodoDollPath
        };

        private readonly static List<string> _allUsedCardPaths = new();

        public static string[] GetShuffleCardPaths()
        {
            string[] shuffledDeck = (string[])_allCardPaths.Clone();

            for (int i = shuffledDeck.Length - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                var temp = shuffledDeck[i];
                shuffledDeck[i] = shuffledDeck[randomIndex];
                shuffledDeck[randomIndex] = temp;
            }

            return shuffledDeck;
        }

        public static string GetRandomItemPath()
        {
            string[] itemPaths = (string[])_allItemPaths.Clone();
            var randomIndex = Random.Range(0, itemPaths.Length);
            return itemPaths[randomIndex];
        }
        
        public static string GetRandomUsedCardPath()
        {
            var validPaths = _allUsedCardPaths.Where(path => path != null).ToList();

            if (validPaths.Count == 0)
            {
                var randomCardIndex = Random.Range(0, _allCardPaths.Length - 1);
                return _allCardPaths[randomCardIndex];
            }

            var randomIndex = Random.Range(0, validPaths.Count);
            return validPaths[randomIndex];
        }

        public static void AddUsedCardPath(string path) =>
            _allUsedCardPaths.Add(path); 

        public static void RemoveUsedCardPath(string path) =>
            _allUsedCardPaths.Remove(path);

        public static void ClearUsedCardPaths() =>
            _allUsedCardPaths.Clear();
    }
}
