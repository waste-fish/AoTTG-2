namespace Assets.Scripts.Characters.Humans.States
{
    public class DieState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Die;

        public DieState(Hero hero) : base(hero)
        {
        }

        public override void OnUpdate()
        {
        }

        public override void OnAttack()
        {
        }

        public override void OnSpecialAttack()
        {
        }
    }
}
