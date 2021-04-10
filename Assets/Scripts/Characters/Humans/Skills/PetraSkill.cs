using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.States;
using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public class PetraSkill : Skill
    {
        private bool UsePhysics { get; set; }

        public override bool Use()
        {
            if (!(Hero.State is HumanIdleState))
                return false;

            RaycastHit hit;
            Hero.AttackAnimation = HeroAnim.SPECIAL_PETRA;
            Hero.PlayAnimation(HeroAnim.SPECIAL_PETRA);
            Hero.Rigidbody.velocity += Vector3.up * 5f;
            var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
            if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
            {
                if (Hero.RightHookProjectile != null)
                {
                    Hero.RightHookProjectile.disable();
                    Hero.ReleaseIfIHookSb();
                }
                if (Hero.LeftHookProjectile != null)
                {
                    Hero.LeftHookProjectile.disable();
                    Hero.ReleaseIfIHookSb();
                }
                Hero.DashDirection = hit.point - Hero.transform.position;
                Hero.LaunchLeftRope(hit.distance, hit.point, true);
                Hero.LaunchRightRope(hit.distance, hit.point, true);
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
            if (Hero.Animation.IsPlaying(HeroAnim.SPECIAL_PETRA)) return;
            IsActive = false;
        }



        public override void OnFixedUpdate()
        {
            if (UsePhysics)
                AddUseForce();

            if (Hero.IsGrounded && !(Hero.State is HumanAttackState))
            {
                if (Hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedTime > 0.35f &&
                    Hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedSpeed < 0.48f)
                {
                    Hero.Rigidbody.AddForce(Hero.gameObject.transform.forward * 200f);
                }
            }
        }

        private void AddUseForce()
        {
            if (!(Hero.State is HumanAttackState) || Hero.AttackAnimation != HeroAnim.SPECIAL_PETRA ||
                Hero.Animation[HeroAnim.SPECIAL_PETRA].normalizedTime <= 0.4f) return;

            if (Hero.LaunchPointRight.magnitude > 0f)
            {
                Vector3 vector19 = Hero.LaunchPointRight - Hero.transform.position;
                vector19.Normalize();
                Hero.Rigidbody.AddForce(vector19 * 13f, ForceMode.Impulse);
            }

            if (Hero.LaunchPointLeft.magnitude > 0f)
            {
                Vector3 vector20 = Hero.LaunchPointLeft - Hero.transform.position;
                vector20.Normalize();
                Hero.Rigidbody.AddForce(vector20 * 13f, ForceMode.Impulse);
                if (Hero.RightHookProjectile != null)
                {
                    Hero.RightHookProjectile.disable();
                    Hero.ReleaseIfIHookSb();
                }
                if (Hero.LeftHookProjectile != null)
                {
                    Hero.LeftHookProjectile.disable();
                    Hero.ReleaseIfIHookSb();
                }
            }

            Hero.Rigidbody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            UsePhysics = false;
        }
    }
}
