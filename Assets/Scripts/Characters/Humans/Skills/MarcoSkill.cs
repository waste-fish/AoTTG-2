﻿using System;
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
            if (!Hero.IsGrounded())
            {
                Hero.skillCDDuration = 0f;
                return false;
            }

            Hero.AttackAnimation = (UnityEngine.Random.Range(0, 2) != 0) ? HeroAnim.SPECIAL_MARCO_1 : HeroAnim.SPECIAL_MARCO_0;
            Hero.PlayAnimation(Hero.AttackAnimation);

            return true;
        }

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
