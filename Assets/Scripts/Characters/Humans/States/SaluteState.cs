using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class SaluteState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Salute;

        public SaluteState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                if (hero.Animation[HeroAnim.SALUTE].normalizedTime >= 1f)
                    hero.Idle();
            }
        }
    }
}
