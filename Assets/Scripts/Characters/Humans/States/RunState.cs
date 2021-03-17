namespace Assets.Scripts.Characters.Humans.States
{
    public class RunState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Run;

        public RunState(Hero hero) : base(hero)
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
