using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanGroundDodgeState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                if (_hero.Animation.IsPlaying(HeroAnim.DODGE))
                {
                    if (!(_hero.Grounded || (_hero.Animation[HeroAnim.DODGE].normalizedTime <= 0.6f)))
                        _hero.SetState<HumanIdleState>();

                    if (_hero.Animation[HeroAnim.DODGE].normalizedTime >= 1f)
                        _hero.SetState<HumanIdleState>();
                }
            }
        }
    }
}
