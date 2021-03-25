using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class JeanSkill : Skill
    {
        public JeanSkill(Hero hero) : base(hero)
        {
        }

        public int TimesUsed { get; set; }

        private const int TimesAllowed = 1;
        
        public override bool Use()
        {
            if (!(hero.SquidState is HumanGrabState) || IsActive)
                return false;

            if (TimesUsed < TimesAllowed && !hero.Animation.IsPlaying(HeroAnim.GRABBED_JEAN))
            {
                hero.PlayAnimation(HeroAnim.GRABBED_JEAN);
                TimesUsed++;
                IsActive = true;
                return true;
            }

            return false;
        }

        public override void OnUpdate()
        {
            if (hero.Animation.IsPlaying(HeroAnim.GRABBED_JEAN) && hero.Animation[HeroAnim.GRABBED_JEAN].normalizedTime > 0.64f)
            {
                hero.BreakFreeFromGrab();
                hero.Rigidbody.velocity = Vector3.up * 30f;
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }

        }
    }
}
