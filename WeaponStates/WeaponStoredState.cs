using UnityEngine;

namespace WeaponSystem
{
    public class WeaponStoredState : WeaponState
    {
        private SpriteRenderer _sr;

        public WeaponStoredState(WeaponStateController controller) : base(controller)
        {
            _sr = controller.GetComponent<SpriteRenderer>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _sr.enabled = false;
        }
    }
}