using Assets.Morhu.Logic.EnergyLogic;
using Morhu.Logic.CameraLogic;
using Morhu.Logic.HealthLogic;
using Morhu.Player;
using Morhu.Util;
using System;
using System.Collections;
using UnityEngine;

namespace Morhu.Deck
{
    [Serializable]
    public abstract class CardAbility
    {
        public abstract IEnumerator Use(Card ownCard, Card oponentCard
            , CardSelector ownSelector, CardSelector oponentSelector,
            HealthSystem ownHealthSystem, HealthSystem oponentHealthSystem
            , CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem
            , bool needToBeSwitchedToDuelCamera, Action onEnded);

        protected bool CheckIfPlayerOrCharacterDied(Card ownCard, Card oponentCard, HealthSystem ownHealthSystem
            , HealthSystem oponentHealthSystem)
        {
            if (ownHealthSystem.IsAllHeartsDone)
            {
                if (ownCard != null && !ownCard.IsUsed() && ownCard.CardType == CardType.Heart)
                    return false;
                else
                {
                    ownHealthSystem.InvokeDeathEvent();
                    return true;
                }
            }
            else if (oponentHealthSystem.IsAllHeartsDone)
            {
                if (oponentCard != null && !oponentCard.IsUsed() && oponentCard.CardType == CardType.Heart)
                    return false;
                else
                {
                    oponentHealthSystem.InvokeDeathEvent();
                    return true;
                }
            }
            return false;
        }

        protected void SetCardRandomAceAbility(Card card)
        {
            var randomAbilityIndex = UnityEngine.Random.Range(0, 4);
            switch (randomAbilityIndex)
            {
                case 0:
                    card.SetCardAbility(new AceAttackAbility());
                    break;
                case 1:
                    card.SetCardAbility(new AceDefenceAbility());
                    break;
                case 2:
                    card.SetCardAbility(new AceHealAbility());
                    break;
                case 3:
                    card.SetCardAbility(new AceStealingAbility());
                    break;
            }
        }
    }

    [Serializable]
    public class AttackAbility : CardAbility
    {
        [Header("Setting")]
        [SerializeField] private int _countOfHeartsToTakeFromTarget;
        [SerializeField] private int _countOfHeartsToTakeFromSelf;
        [SerializeField] private bool _needToBockSelfNextSelection;

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            cameraSwitcher.SwitchTo(oponentHealthSystem.AttachedCamera);
            yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

            if (_countOfHeartsToTakeFromTarget > 0)
            {
                for (var i = 0; i < _countOfHeartsToTakeFromTarget; i++)
                {
                    oponentHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break;
                }
            }

            if (_countOfHeartsToTakeFromSelf > 0)
            {
                cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < _countOfHeartsToTakeFromSelf; i++)
                {
                    ownHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break;
                }
            }

            TryToBlockSelf(ownSelector);

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }

        private void TryToBlockSelf(CardSelector ownSelector)
        {
            if (_needToBockSelfNextSelection)
                ownSelector.Block();
        }
    }


    [Serializable]
    public class HealAbility : CardAbility
    {
        [Header("Setting")]
        [SerializeField] private int _countOfHeartsToHeal;
        [SerializeField] private bool _needToTakeBadEffectAway;
        [SerializeField] private bool _needToHealToCertainValue;
        [SerializeField] private int _certainValueOfAliveHearts;

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
            yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

            if (_needToHealToCertainValue)
            {
                var countOfHeartsToHeal = _certainValueOfAliveHearts - ownHealthSystem.GetCountOfAllAliveHearts();

                if (countOfHeartsToHeal > 0)
                {
                    for (var i = 0; i < countOfHeartsToHeal; i++)
                    {
                        ownHealthSystem.HealOneHeart();
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
            else
            {
                for (var i = 0; i < _countOfHeartsToHeal; i++)
                {
                    ownHealthSystem.HealOneHeart();
                    yield return new WaitForSeconds(0.3f);
                }
            }


            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            if (_needToTakeBadEffectAway)
                ownSelector.Unblock();

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }



    [Serializable]
    public class BlockOponentAbility : CardAbility
    {
        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            oponentSelector.Block();
            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }
    
    [Serializable]
    public class CopyAbility : CardAbility
    {
        [Header("Setting")]
        [SerializeField] private bool _needToBockSelfNextSelection;

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            if (oponentCard == null)
            {
                ownEnergySystem.BlowOneCandle();

                if (_needToBockSelfNextSelection) 
                    ownSelector.Block();

                yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
                ownCard.SetCardToBeUsed();
                onEnded?.Invoke();
                yield break;
            }

            if (_needToBockSelfNextSelection)
                ownSelector.Block();

            ownCard.SetCardAbility(oponentCard.GetCardAbility());
            yield return new WaitForEndOfFrame();
            ownCard.UseAbilityWithoutDissoleEffect(ownCard, oponentCard, ownSelector, oponentSelector, ownHealthSystem, oponentHealthSystem
                , cameraSwitcher, ownEnergySystem, needToBeSwitchedToDuelCamera, onEnded);
        }
    }  

    [Serializable]
    public class DefenceAbility : CardAbility
    {
        [Header("Setting")]
        [SerializeField] private int _countOfHeartsToTakeFromOponent;
        [SerializeField] private int _countOfHeartsToTakeFromSelf;

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            if (_countOfHeartsToTakeFromOponent > 0)
            {
                cameraSwitcher.SwitchTo(oponentHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < _countOfHeartsToTakeFromOponent; i++)
                {
                    oponentHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break; ;
                }
            }

            if (_countOfHeartsToTakeFromSelf > 0)
            {
                cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < _countOfHeartsToTakeFromSelf; i++)
                {
                    ownHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break;
                }
            }

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }

    //ACESS

    [Serializable]
    public class AceHealAbility : CardAbility
    {
        private readonly int _maxHeartsCount = 4; 

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
            yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

            for (var i = 0; i < _maxHeartsCount; i++)
            {
                if (ownHealthSystem.GetCountOfAllDiedHearts() == 0)
                    break;

                ownHealthSystem.HealOneHeart();
                yield return new WaitForSeconds(0.3f);
            }

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            ownSelector.Unblock();
            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }

    [Serializable]
    public class AceAttackAbility : CardAbility
    {
        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            cameraSwitcher.SwitchTo(oponentHealthSystem.AttachedCamera);
            yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

            if (oponentHealthSystem.GetCountOfAllAliveHearts() == 4)
            {
                for (var i = 0; i < 3; i++)
                {
                    oponentHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break;
                }
            }
            else
            {
                for (var i = 0; i < 2; i++)
                {
                    oponentHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);

                    if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                        yield break;
                }
            }

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }

    [Serializable]
    public class AceStealingAbility : CardAbility
    {
        private bool _isStolenAbilityEnded = false;

        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem , bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            if (oponentCard == null)
            {
                ownEnergySystem.BlowOneCandle();

                cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                ownHealthSystem.KillOneHeart();
                yield return new WaitForSeconds(0.3f);

                if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                    yield break;

                yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
                ownCard.SetCardToBeUsed();
                onEnded?.Invoke();
                yield break;
            }

            ownCard.SetCardAbility(oponentCard.GetCardAbility());
            yield return new WaitForEndOfFrame();
            ownCard.UseAbilityWithoutDissoleEffect(ownCard, oponentCard, ownSelector, oponentSelector, ownHealthSystem, oponentHealthSystem
                , cameraSwitcher, ownEnergySystem, needToBeSwitchedToDuelCamera, () =>{
                _isStolenAbilityEnded = true;
            });

            yield return new WaitUntil(() => _isStolenAbilityEnded);

            cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
            yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

            ownHealthSystem.KillOneHeart();
            yield return new WaitForSeconds(0.1f);

            if (CheckIfPlayerOrCharacterDied(ownCard, oponentCard, ownHealthSystem, oponentHealthSystem))
                yield break;


            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }

    [Serializable]
    public class AceDefenceAbility : CardAbility
    {
        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem, bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }

    //JOCKERS
    [Serializable]
    public class BlackJockerAbility : CardAbility
    {
        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem, bool needToBeSwitchedToDuelCamera, Action onEnded){
            ownEnergySystem.BlowOneCandle();
            yield return null;
        }

        public void InitJoker(Card card) =>
            SetCardRandomAceAbility(card);
    }

    [Serializable]
    public class RedJockerAbility : CardAbility
    {
        public override IEnumerator Use(Card ownCard, Card oponentCard, CardSelector ownSelector, CardSelector oponentSelector, HealthSystem ownHealthSystem,
            HealthSystem oponentHealthSystem, CameraSwitcher cameraSwitcher, EnergySystem ownEnergySystem, bool needToBeSwitchedToDuelCamera, Action onEnded)
        {
            ownEnergySystem.BlowOneCandle();

            var requiredHeartsForSelf = oponentHealthSystem.GetCountOfAllAliveHearts() -
                ownHealthSystem.GetCountOfAllAliveHearts();

            if (requiredHeartsForSelf > 0)
            {
                cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < requiredHeartsForSelf; i++)
                {
                    ownHealthSystem.HealOneHeart();
                    yield return new WaitForSeconds(0.3f);
                }
                cameraSwitcher.SwitchTo(oponentHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < requiredHeartsForSelf; i++)
                {
                    oponentHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else
            {
                requiredHeartsForSelf = -requiredHeartsForSelf;

                cameraSwitcher.SwitchTo(ownHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < requiredHeartsForSelf; i++)
                {
                    ownHealthSystem.KillOneHeart();
                    yield return new WaitForSeconds(0.3f);
                }

                cameraSwitcher.SwitchTo(oponentHealthSystem.AttachedCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);

                for (var i = 0; i < requiredHeartsForSelf; i++)
                {
                    oponentHealthSystem.HealOneHeart();
                    yield return new WaitForSeconds(0.3f);
                }
            }

            yield return new WaitForSeconds(Constants.WAIT_TIME_FOR_ABILITY_TO_END);

            if (needToBeSwitchedToDuelCamera)
            {
                cameraSwitcher.SwitchTo(SwitchedCameraType.DuelCamera);
                yield return new WaitForSeconds(Constants.CAMERAS_SWITCHED_TIME);
            }
            ownCard.SetCardToBeUsed();
            onEnded?.Invoke();
        }
    }
}
