namespace Assets.Scripts.Characters.Humans.States
{
    public class TitanState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.TitanForm;

        public TitanState(Hero hero) : base(hero)
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
