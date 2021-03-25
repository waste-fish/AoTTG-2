using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class MikasaSkill : Skill
    {
        public MikasaSkill(Hero hero) : base(hero)
        {
        }

        public override bool Use()
        {
            if (!(hero.SquidState is HumanIdleState))
                return false;

            hero.AttackAnimation = HeroAnim.SPECIAL_MIKASA_0;
            hero.PlayAnimation(HeroAnim.SPECIAL_MIKASA_0);
            hero.Rigidbody.velocity = Vector3.up * 10f;
            IsActive = true;
            return true;
        }

        public override void OnUpdate()
        {
            // Ignored
        }

        public override void OnFixedUpdate()
        {
            if (!hero.Grounded)
                return;

            if (hero.SquidState is HumanAttackState && hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0 &&
                hero.Animation[hero.AttackAnimation].normalizedTime >= 1f)
            {
                hero.PlayAnimation(HeroAnim.SPECIAL_MIKASA_1);
                hero.ResetAnimationSpeed();
                hero.Rigidbody.velocity = Vector3.zero;
                hero.CurrentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(0.2f, 0.3f, 0.95f);
                IsActive = false;
            }
        }
    }
}
