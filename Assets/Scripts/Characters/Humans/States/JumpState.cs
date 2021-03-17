namespace Assets.Scripts.Characters.Humans.States
{
    public class JumpState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Jump;

        public JumpState(Hero hero) : base(hero)
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
