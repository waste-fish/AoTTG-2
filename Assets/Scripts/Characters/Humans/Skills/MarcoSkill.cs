using System;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class MarcoSkill : Skill
    {
        public MarcoSkill(Hero hero) : base(hero)
        {
        }

        public override bool Use()
        {
            if (!hero.IsGrounded())
            {
                hero.SkillCDDuration = 0f;
                return false;
            }

            hero.AttackAnimation = (UnityEngine.Random.Range(0, 2) != 0) ? HeroAnim.SPECIAL_MARCO_1 : HeroAnim.SPECIAL_MARCO_0;
            hero.PlayAnimation(hero.AttackAnimation);

            return true;
        }

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
