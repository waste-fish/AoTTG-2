using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class GroundDodgeState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.GroundDodge;

        public GroundDodgeState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                if (hero.Animation.IsPlaying(HeroAnim.DODGE))
                {
                    if (!(hero.grounded || (hero.Animation[HeroAnim.DODGE].normalizedTime <= 0.6f)))
                        hero.Idle();
                    if (hero.Animation[HeroAnim.DODGE].normalizedTime >= 1f)
                        hero.Idle();
                }
            }
        }
    }
}
