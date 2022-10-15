using UnityEngine;

namespace WeaponSystem
{
    public class WeaponFloorState : WeaponState
    {
        private Rigidbody2D _rb;
        private Collider2D _col;

        public WeaponFloorState(WeaponStateController controller) : base(controller)
        {
            _rb = controller.GetComponent<Rigidbody2D>();
            _col = controller.GetComponent<Collider2D>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _rb.simulated = true;
            _col.enabled = true;
            _rb.AddForce(Controller.transform.right * 10f, ForceMode2D.Impulse);
            _rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            _rb.simulated = false;
            _col.enabled = false;
        }
    }
}