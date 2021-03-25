using Assets.Scripts.Characters.Humans.Constants;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanGroundDodgeState : BaseHumanState
    {
        Vector3 zero;

        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                if (Hero.Animation.IsPlaying(HeroAnim.DODGE))
                {
                    if (!(Hero.IsGrounded || (Hero.Animation[HeroAnim.DODGE].normalizedTime <= 0.6f)))
                        Hero.SetState<HumanIdleState>();

                    if (Hero.Animation[HeroAnim.DODGE].normalizedTime >= 1f)
                        Hero.SetState<HumanIdleState>();
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if ((Hero.Animation[HeroAnim.DODGE].normalizedTime >= 0.2f) && (Hero.Animation[HeroAnim.DODGE].normalizedTime < 0.8f))
            {
                zero = ((-Hero.transform.forward * 2.4f) * Hero.Speed);
            }
            if (Hero.Animation[HeroAnim.DODGE].normalizedTime > 0.8f)
            {
                zero = (Hero.Rigidbody.velocity * 0.9f);
            }
        }
    }
}
