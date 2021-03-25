using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class MikasaSkill : Skill
    {
        public override bool Use()
        {
            if (!(Hero.State is HumanIdleState))
                return false;

            Hero.AttackAnimation = HeroAnim.SPECIAL_MIKASA_0;
            Hero.PlayAnimation(HeroAnim.SPECIAL_MIKASA_0);
            Hero.Rigidbody.velocity = Vector3.up * 10f;
            IsActive = true;
            return true;
        }

        public override void OnFixedUpdate()
        {
            if (!Hero.IsGrounded)
                return;

            if (Hero.State is HumanAttackState && Hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0 &&
                Hero.Animation[Hero.AttackAnimation].normalizedTime >= 1f)
            {
                Hero.PlayAnimation(HeroAnim.SPECIAL_MIKASA_1);
                Hero.ResetAnimationSpeed();
                Hero.Rigidbody.velocity = Vector3.zero;
                Hero.CurrentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(0.2f, 0.3f, 0.95f);
                IsActive = false;
            }
        }
    }
}
