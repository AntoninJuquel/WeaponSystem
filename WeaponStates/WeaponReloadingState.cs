using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponReloadingState : WeaponState
    {
        private Coroutine _reload;
        private WaitForSeconds _reloadTime;

        public WeaponReloadingState(WeaponStateController controller) : base(controller)
        {
            _reloadTime = new WaitForSeconds(Weapon.magazine.reloadTime);
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _reload = Controller.StartCoroutine(Reload());
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            Controller.StopCoroutine(_reload);
        }

        private IEnumerator Reload()
        {
            yield return _reloadTime;
            Weapon.magazine.ammo = Weapon.magazine.size;
            Controller.SwitchState(Controller.Ready);
        }
    }
}