namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanChangeBladeState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                Hero.Equipment.Weapon.Reload();
                if (Hero.Animation[Hero.ReloadAnimation].normalizedTime >= 1f)
                    Hero.SetState<HumanIdleState>();
            }
        }
    }
}
