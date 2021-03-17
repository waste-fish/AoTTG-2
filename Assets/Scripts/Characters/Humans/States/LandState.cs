using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class LandState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Land;

        public LandState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.Animation.IsPlaying(HeroAnim.DASH_LAND) && (hero.Animation[HeroAnim.DASH_LAND].normalizedTime >= 1f))
                    hero.Idle();
        }
    }
}
