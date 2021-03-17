namespace Assets.Scripts.Characters.Humans.States
{
    public class ChangeBladeState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.ChangeBlade;

        public ChangeBladeState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                hero.Equipment.Weapon.Reload();
                if (hero.Animation[hero.reloadAnimation].normalizedTime >= 1f)
                    hero.Idle();
            }
        }
    }
}
