using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class LeviSkill : Skill
    {
        private const float CooldownLimit = 3.5f;
        private bool UsePhysics { get; set; }

        public LeviSkill(Hero hero) : base(hero)
        {
            Cooldown = CooldownLimit;
        }

        public override bool Use()
        {
            if (Hero.State != HumanState.Idle)
                return false;

            RaycastHit hit;
            Hero.AttackAnimation = HeroAnim.ATTACK5;
            Hero.PlayAnimation(HeroAnim.ATTACK5);
            Hero.Rigidbody.velocity += Vector3.up * 5f;
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
            if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
            {
                if (Hero.HookRight != null)
                {
                    Hero.HookRight.disable();
                    Hero.ReleaseIfIHookSb();
                }
                Hero.DashDirection = hit.point - Hero.transform.position;
                Hero.LaunchRightRope(hit.distance, hit.point, true, 1);
                Hero.rope.Play();
            }
            Hero.FacingDirection = Mathf.Atan2(Hero.DashDirection.x, Hero.DashDirection.z) * Mathf.Rad2Deg;
            Hero.TargetRotation = Quaternion.Euler(0f, Hero.FacingDirection, 0f);
            Hero.AttackLoop = 3;
            IsActive = true;
            UsePhysics = true;
            return true;
        }

        public override void OnUpdate()
        {
            if (Hero.Animation.IsPlaying(HeroAnim.ATTACK5)) return;
            IsActive = false;
        }

        public override void OnFixedUpdate()
        {
            if (!UsePhysics) return;

            if (Hero.State != HumanState.Attack || Hero.AttackAnimation != HeroAnim.ATTACK5 ||
                Hero.Animation[HeroAnim.ATTACK5].normalizedTime <= 0.4f) return;

            if (Hero.LaunchPointRight.magnitude > 0f)
            {
                Vector3 vector19 = Hero.LaunchPointRight - Hero.transform.position;
                vector19.Normalize();
                Hero.Rigidbody.AddForce(vector19 * 13f, ForceMode.Impulse);
            }
            Hero.Rigidbody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            UsePhysics = false;
        }
    }
}
