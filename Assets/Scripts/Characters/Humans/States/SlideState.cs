namespace Assets.Scripts.Characters.Humans.States
{
    public class SlideState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Slide;

        public SlideState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (!hero.grounded)
                    hero.SquidState = new IdleState(hero);
        }
    }
}
