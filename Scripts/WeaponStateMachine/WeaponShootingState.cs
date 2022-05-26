using System;
using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponShootingState : WeaponState
    {
        private IPullTrigger _trigger;
        private Coroutine _shooting;

        public WeaponShootingState(WeaponStateController stateController) : base(stateController)
        {
            _trigger = StateController.GetComponentInParent<IPullTrigger>();
        }

        private void OnReleaseTrigger(object sender, EventArgs args)
        {
            StateController.SwitchState(StateController.IdleState);
        }


        private IEnumerator Shooting()
        {
            while (true)
            {
                Debug.Log("Shoot");
                yield return new WaitForSeconds(60f / Weapon.roundPerMinute);
            }
        }

        public override void OnStateEnter()
        {
            _trigger.OnReleaseTrigger += OnReleaseTrigger;
            _shooting = StateController.StartCoroutine(Shooting());
        }

        public override void OnStateUpdate()
        {
        }

        public override void OnStateExit()
        {
            _trigger.OnReleaseTrigger -= OnReleaseTrigger;
            StateController.StopCoroutine(_shooting);
        }
    }
}