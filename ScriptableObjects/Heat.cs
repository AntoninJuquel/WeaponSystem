using System;
using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Heat")]
    public class Heat : ScriptableObject
    {
        [SerializeField] private float amount;
        [SerializeField] private float capacity;
        [SerializeField] private float gainRate;
        [SerializeField] private float recoveryRate;
        [SerializeField] private float recoveryDelay;
        private Weapon _weapon;
        private MonoBehaviour _owner;
        private Coroutine _heat;

        private void HeatUp()
        {
            amount += gainRate;
            if (amount >= capacity) _weapon.OverHeat();
        }

        private IEnumerator HeatDown()
        {
            yield return new WaitForSeconds(recoveryDelay);

            while (amount > 0)
            {
                amount -= Time.deltaTime * recoveryRate;
                yield return null;
            }

            amount = 0;
            if (_weapon.State == State.Cooling) _weapon.State = State.Ready;
        }

        private void OnShoot(object sender, EventArgs args)
        {
            HeatUp();
            if (_heat != null) _owner.StopCoroutine(_heat);
            _heat = _owner.StartCoroutine(HeatDown());
        }

        public Heat Clone(Weapon weapon, GameObject owner)
        {
            var heat = Instantiate(this);
            heat._owner = owner.GetComponent<WeaponInventory>();
            heat._weapon = weapon;
            heat._weapon.onShoot += heat.OnShoot;
            return heat;
        }

        private void OnDisable()
        {
            if (_weapon) _weapon.onShoot -= OnShoot;
        }
    }
}