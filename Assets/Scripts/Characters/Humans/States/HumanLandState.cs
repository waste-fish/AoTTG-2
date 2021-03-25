using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanLandState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.Animation.IsPlaying(HeroAnim.DASH_LAND) && (_hero.Animation[HeroAnim.DASH_LAND].normalizedTime >= 1f))
                    _hero.SetState<HumanIdleState>();
        }
    }
}
