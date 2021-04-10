using Assets.Scripts.Characters.Humans.Constants;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanSlideState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
                if (!Hero.IsGrounded)
                    Hero.SetState<HumanIdleState>(true);
        }

        public override Vector3 FixedUpdateMovement() // 81136
        {
            if (Hero.CurrentSpeed < Hero.Speed * 1.2f)
            {
                Hero.SetState<HumanIdleState>();
                Hero.SparksEM.enabled = false;
            }

            return Hero.Rigidbody.velocity * 0.99f;
        }


        public override void OnJump()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;
            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;

            Hero.SetState<HumanIdleState>();
            Hero.CrossFade(HeroAnim.JUMP, 0.1f);
            Hero.SparksEM.enabled = false;
        }

        public override void OnDodge()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;

            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;

            Hero.Dodge(false);
        }

        public override void OnMount()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;
            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;
            if (Hero.Horse == null || Hero.IsMounted || Vector3.Distance(Hero.Horse.transform.position, Hero.transform.position) > 15f)
                return;

            Hero.GetOnHorse();
        }
    }
}
