using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class JeanSkill : Skill
    {
        public int TimesUsed { get; set; }

        [SerializeField] private int timesAllowed = 1;

        public override bool Use()
        {
            if (!(Hero.State is HumanGrabState) || IsActive)
                return false;

            if (TimesUsed < timesAllowed && !Hero.Animation.IsPlaying(HeroAnim.GRABBED_JEAN))
            {
                Hero.PlayAnimation(HeroAnim.GRABBED_JEAN);
                TimesUsed++;
                IsActive = true;
                return true;
            }

            return false;
        }

        public override void OnUpdate()
        {
            if (Hero.Animation.IsPlaying(HeroAnim.GRABBED_JEAN) && Hero.Animation[HeroAnim.GRABBED_JEAN].normalizedTime > 0.64f)
            {
                Hero.BreakFreeFromGrab();
                Hero.Rigidbody.velocity = Vector3.up * 30f;
                IsActive = false;
            }
            else
                IsActive = true;
        }
    }
}
