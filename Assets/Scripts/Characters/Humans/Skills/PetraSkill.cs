using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class PetraSkill : Skill
    {
        private const float CooldownLimit = 3.5f;
        private bool UsePhysics { get; set; }

        public PetraSkill(Hero hero) : base(hero)
        {
            Cooldown = CooldownLimit;
        }

        public override bool Use()
        {
            if (!(hero.SquidState is HumanIdleState))
                return false;

            RaycastHit hit;
            hero.AttackAnimation = HeroAnim.SPECIAL_PETRA;
            hero.PlayAnimation(HeroAnim.SPECIAL_PETRA);
            hero.Rigidbody.velocity += Vector3.up * 5f;
            var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
            if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
            {
                if (hero.HookRight != null)
                {
                    hero.HookRight.disable();
                    hero.ReleaseIfIHookSb();
                }
                if (hero.HookLeft != null)
                {
                    hero.HookLeft.disable();
                    hero.ReleaseIfIHookSb();
                }
                hero.DashDirection = hit.point - hero.transform.position;
                hero.LaunchLeftRope(hit.distance, hit.point, true);
                hero.LaunchRightRope(hit.distance, hit.point, true);
                hero.rope.Play();
            }
            hero.FacingDirection = Mathf.Atan2(hero.DashDirection.x, hero.DashDirection.z) * Mathf.Rad2Deg;
            hero.TargetRotation = Quaternion.Euler(0f, hero.FacingDirection, 0f);
            hero.AttackLoop = 3;
            IsActive = true;
            UsePhysics = true;
            return true;
        }

        public override void OnUpdate()
        {
            if (hero.Animation.IsPlaying(HeroAnim.SPECIAL_PETRA)) return;
            IsActive = false;
        }



        public override void OnFixedUpdate()
        {
            if (UsePhysics)
                AddUseForce();

            if (hero.Grounded && !(hero.SquidState is HumanAttackState))
            {
                if (hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedTime > 0.35f &&
                    hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedSpeed < 0.48f)
                {
                    hero.Rigidbody.AddForce(hero.gameObject.transform.forward * 200f);
                }
            }
        }

        private void AddUseForce()
        {
            if (!(hero.SquidState is HumanAttackState) || hero.AttackAnimation != HeroAnim.SPECIAL_PETRA ||
                hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedTime <= 0.4f) return;

            if (hero.LaunchPointRight.magnitude > 0f)
            {
                Vector3 vector19 = hero.LaunchPointRight - hero.transform.position;
                vector19.Normalize();
                hero.Rigidbody.AddForce(vector19 * 13f, ForceMode.Impulse);
            }

            if (hero.LaunchPointLeft.magnitude > 0f)
            {
                Vector3 vector20 = hero.LaunchPointLeft - hero.transform.position;
                vector20.Normalize();
                hero.Rigidbody.AddForce(vector20 * 13f, ForceMode.Impulse);
                if (hero.HookRight != null)
                {
                    hero.HookRight.disable();
                    hero.ReleaseIfIHookSb();
                }
                if (hero.HookLeft != null)
                {
                    hero.HookLeft.disable();
                    hero.ReleaseIfIHookSb();
                }
            }

            hero.Rigidbody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            UsePhysics = false;
        }
    }
}
