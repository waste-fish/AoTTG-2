using System;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class SashaSkill : Skill
    {
        public SashaSkill(Hero hero) : base(hero)
        {
        }

        public override bool Use()
        {
            if (!hero.IsGrounded())
                return false;

            hero.AttackAnimation = HeroAnim.SPECIAL_SASHA;
            hero.PlayAnimation(HeroAnim.SPECIAL_SASHA);
            hero.ApplyBuff(BUFF.SpeedUp, 10f);

            return true;
        }

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
