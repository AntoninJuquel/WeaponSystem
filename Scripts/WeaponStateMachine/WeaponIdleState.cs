using System;
using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponIdleState : WeaponState
    {
        private IPullTrigger _trigger;

        public WeaponIdleState(WeaponStateController stateController) : base(stateController)
        {
            _trigger = StateController.GetComponentInParent<IPullTrigger>();
        }
        
        private void OnPullTrigger(object sender, EventArgs args)
        {
            StateController.SwitchState(StateController.ShootingState);
        }

        public override void OnStateEnter()
        {
            _trigger.OnPullTrigger += OnPullTrigger;
        }

        public override void OnStateUpdate()
        {
        }

        public override void OnStateExit()
        {
            _trigger.OnPullTrigger -= OnPullTrigger;
        }
    }
}