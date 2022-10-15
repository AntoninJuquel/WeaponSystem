using UnityEngine;

namespace WeaponSystem
{
    public class WeaponReadyState : WeaponState
    {
        private IHandleTrigger _trigger;
        private IReload _reload;
        private SpriteRenderer _sr;

        public WeaponReadyState(WeaponStateController controller) : base(controller)
        {
            _trigger = controller.GetComponentInParent<IHandleTrigger>();
            _reload = controller.GetComponentInParent<IReload>();
            _sr = controller.GetComponent<SpriteRenderer>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _trigger.OnPullTrigger += OnPullTrigger;
            _reload.OnReload += OnReload;
            _sr.enabled = true;
            _sr.sprite = Weapon.sprite;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            _trigger.OnPullTrigger -= OnPullTrigger;
            _reload.OnReload -= OnReload;
        }

        private void OnPullTrigger()
        {
            Controller.SwitchState(Controller.Warming);
        }

        private void OnReload()
        {
            Controller.SwitchState(Controller.Reloading);
        }
    }
}