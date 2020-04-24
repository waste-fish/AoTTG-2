﻿using UnityEngine;

namespace Assets.Scripts.Characters.Titan.Attacks
{
    public class SmashAttack : BoomAttack
    {
        protected override string Effect { get; set; } = "FX/boom1";
        protected override float BoomTimer { get; set; } = 0.45f;
        protected override string AttackAnimation { get; set; } = "attack_front_ground";

        public override bool CanAttack(MindlessTitan titan)
        {
            Vector3 vector18 = titan.Target.transform.position - titan.transform.position;
            var angle = -Mathf.Atan2(vector18.z, vector18.x) * 57.29578f;
            var between = -Mathf.DeltaAngle(angle, titan.gameObject.transform.rotation.eulerAngles.y - 90f);
            if (Mathf.Abs(between) >= 90f || between <= 0 ||
                titan.TargetDistance >= titan.AttackDistance * 0.75f) return false;

            TitanBodyPart = titan.TitanBody.AttackFrontGround;
            return true;
        }
    }
}
