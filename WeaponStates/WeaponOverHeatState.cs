using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponOverHeatState : WeaponState
    {
        private Coroutine _overHeating;
        private WaitForSeconds _heatRecoveryDelay;

        public WeaponOverHeatState(WeaponStateController controller) : base(controller)
        {
            _heatRecoveryDelay = new WaitForSeconds(Weapon.cooler.heatRecoveryDelay);
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _overHeating = Controller.StartCoroutine(OverHeating());
        }

        private IEnumerator OverHeating()
        {
            if (_overHeating != null)
            {
                yield return _overHeating;
                yield break;
            }

            yield return _heatRecoveryDelay;

            while (Weapon.cooler.heatAmount > 0)
            {
                Weapon.cooler.heatAmount -= Weapon.cooler.heatRecoveryRate;
                yield return null;
            }

            Weapon.cooler.heatAmount = 0;

            if (Enabled)
                Controller.SwitchState(Controller.Ready);

            _overHeating = null;
        }
    }
}