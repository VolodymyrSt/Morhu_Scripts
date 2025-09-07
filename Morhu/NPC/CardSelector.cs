using System;

namespace Morhu.Player
{
    public abstract class CardSelector
    {
        private bool _isBlocked = false;

        public abstract void TryToChooseCardForDuel();

        public bool IsBlocked() => _isBlocked;

        public void SetBlock(bool value) => 
            _isBlocked = value;

        public void Block() => _isBlocked = true;
        public void Unblock() => _isBlocked = false;
    }
}
