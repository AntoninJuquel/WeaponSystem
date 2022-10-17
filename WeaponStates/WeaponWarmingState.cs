using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponWarmingState : WeaponState
    {
        private IHandleTrigger _trigger;
        private Coroutine _warming;
        private WaitForSeconds _warmTime;

        public WeaponWarmingState(WeaponStateController controller) : base(controller)
        {
            _trigger = controller.GetComponentInParent<IHandleTrigger>();
            _warmTime = new WaitForSeconds(Weapon.FireMode.warmTime);
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _trigger.OnReleaseTrigger += OnReleaseTrigger;
            _warming = Controller.StartCoroutine(Warming());
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            _trigger.OnReleaseTrigger -= OnReleaseTrigger;
            if (_warming != null) Controller.StopCoroutine(_warming);
        }

        private IEnumerator Warming()
        {
            yield return _warmTime;
            Controller.SwitchState(Controller.Shooting);
        }

        private void OnReleaseTrigger()
        {
            Controller.SwitchState(Controller.Ready);
        }
    }
}