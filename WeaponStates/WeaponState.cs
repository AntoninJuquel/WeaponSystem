namespace WeaponSystem
{
    public abstract class WeaponState
    {
        protected WeaponStateController Controller;
        protected Weapon Weapon;
        protected bool Enabled;

        public WeaponState(WeaponStateController controller)
        {
            Controller = controller;
            Weapon = controller.Weapon;
        }

        public virtual void OnStateEnter()
        {
            Enabled = true;
        }

        public virtual void OnStateExit()
        {
            Enabled = false;
        }
    }
}