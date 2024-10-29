using System;

namespace Game
{
    public class UpgradeAction
    {
        public Action OnComplete;
        private Action _action;
        
        public UpgradeAction(Action action)
        {
            _action = action;
        }

        public void Invoke()
        {
            _action?.Invoke();
            OnComplete?.Invoke();
        }
    }

}
