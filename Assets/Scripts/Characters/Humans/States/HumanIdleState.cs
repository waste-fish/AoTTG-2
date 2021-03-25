using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.Skills;
using Assets.Scripts.Characters.Titan;
using Assets.Scripts.Constants;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanIdleState : BaseHumanState
    {
        private bool? skillFailed;
        private bool bothGunsHaveBullet;
        private bool noBulletsInEitherGun;
        private bool gunsHaveABullet;

        public override void OnEnter()
        {
            if (_previous is HumanAttackState)
                Hero.FalseAttack();

            Hero.CrossFade(Hero.StandAnimation, 0.1f);
            Hero.Rigidbody.useGravity = true;
        }

        public override void OnFixedUpdate()
        {
            if (skillFailed.HasValue && !skillFailed.Value)
            {
                Hero.checkBoxLeft.ClearHits();
                Hero.checkBoxRight.ClearHits();
                if (Hero.IsGrounded)
                {
                    Debug.Log("Force Added");
                    Hero.Rigidbody.AddForce(Hero.transform.forward * 200f);
                }
                Hero.PlayAnimation(Hero.AttackAnimation);
                Hero.Animation[Hero.AttackAnimation].time = 0f;
                Hero.SetState<HumanAttackState>();
                //if (Hero.IsGrounded || Hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0 || Hero.AttackAnimation == HeroAnim.SPECIAL_LEVI || Hero.AttackAnimation == HeroAnim.SPECIAL_PETRA)
                //    Hero.AttackReleased = true;
                //else
                //    Hero.AttackReleased = false;
                Hero.SparksEM.enabled = false;
            }

            if (bothGunsHaveBullet)
            {
                Hero.SetState<HumanAttackState>();
                Hero.CrossFade(Hero.AttackAnimation, 0.05f);
                Hero.GunDummy.transform.position = Hero.transform.position;
                Hero.GunDummy.transform.rotation = Hero.transform.rotation;
                Hero.GunDummy.transform.LookAt(Hero.GunTarget);
                //Hero.AttackReleased = false;
                Hero.FacingDirection = Hero.GunDummy.transform.rotation.eulerAngles.y;
                Hero.TargetRotation = Quaternion.Euler(0f, Hero.FacingDirection, 0f);
            }
            else if (noBulletsInEitherGun && (Hero.IsGrounded || (GameSettings.PvP.AhssAirReload.Value)))
                Hero.Reload();

            bothGunsHaveBullet = false;
            noBulletsInEitherGun = false;
            gunsHaveABullet = false;

            FixedUpdateGrounded();
        }

        private void FixedUpdateGrounded()
        {
            if (!Hero.IsGrounded)
                return;

            var vector8 = new Vector3(Hero.TargetMoveDirection.x, 0f, Hero.TargetMoveDirection.y);
            var resultAngle = Hero.GetGlobalFacingDirection(Hero.TargetMoveDirection.x, Hero.TargetMoveDirection.y);
            Hero.Zero = Hero.GetGlobaleFacingVector3(resultAngle);
            var num6 = (vector8.magnitude <= 0.95f) ? ((vector8.magnitude >= 0.25f) ? vector8.magnitude : 0f) : 1f;
            Hero.Zero *= num6;
            Hero.Zero *= Hero.Speed;

            if ((Hero.BuffTime > 0f) && (Hero.CurrentBuff == BUFF.SpeedUp))
                Hero.Zero *= 4f;

            if (Hero.TargetMoveDirection != Vector2.zero)
            {
                if (((!Hero.Animation.IsPlaying(HeroAnim.RUN_1) && !Hero.Animation.IsPlaying(HeroAnim.JUMP)) && !Hero.Animation.IsPlaying(HeroAnim.RUN_SASHA)) && (!Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON) || (Hero.Animation[HeroAnim.HORSE_GET_ON].normalizedTime >= 0.5f)))
                {
                    if ((Hero.BuffTime > 0f) && (Hero.CurrentBuff == BUFF.SpeedUp))
                        Hero.CrossFade(HeroAnim.RUN_SASHA, 0.1f);
                    else
                        Hero.CrossFade(HeroAnim.RUN_1, 0.1f);
                }
            }
            else
            {
                if (!(((Hero.Animation.IsPlaying(Hero.StandAnimation) || (Hero.State is HumanLandState)) || (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))) || Hero.Animation.IsPlaying(HeroAnim.GRABBED)))
                {
                    Hero.CrossFade(Hero.StandAnimation, 0.1f);
                    Hero.Zero *= 0f;
                }
                resultAngle = -874f;
            }

            if (resultAngle != -874f)
            {
                Hero.FacingDirection = resultAngle;
                Hero.TargetRotation = Quaternion.Euler(0f, Hero.FacingDirection, 0f);
            }
        }

        public override void OnAttack()
        {
            if (!Hero.UseGun)
                ProcessBladeAttack();
            else
                ProcessGunAttack();
        }

        public override void OnAttackRelease()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
                if (Hero.UseGun)
                    if (gunsHaveABullet)
                    {
                        if (Hero.IsGrounded)
                        {
                            if (Hero.LeftGunHasBullet && Hero.RightGunHasBullet)
                            {
                                if (Hero.IsLeftHandHooked)
                                    Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                                else
                                    Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            }
                            else if (Hero.LeftGunHasBullet)
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            else if (Hero.RightGunHasBullet)
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                        }
                        else if (Hero.LeftGunHasBullet && Hero.RightGunHasBullet)
                        {
                            if (Hero.IsLeftHandHooked)
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;
                            else
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        }
                        else if (Hero.LeftGunHasBullet)
                            Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        else if (Hero.RightGunHasBullet)
                            Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;

                        if (Hero.LeftGunHasBullet || Hero.RightGunHasBullet)
                            bothGunsHaveBullet = true;
                        else
                            noBulletsInEitherGun = true;
                    }
        }

        private void ProcessBladeAttack()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                bool left = Hero.TargetMoveDirection.x < 0f;
                bool right = Hero.TargetMoveDirection.x > 0f;
                if (Hero.NeedLean)
                {
                    bool rand = Random.value <= 0.5f;

                    if (left)
                        Hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else if (right)
                        Hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                    else if (Hero.LeanLeft)
                        Hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else
                        Hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                }
                else if (left)
                    Hero.AttackAnimation = HeroAnim.ATTACK2;
                else if (right)
                    Hero.AttackAnimation = HeroAnim.ATTACK1;
                else if (Hero.LastHook != null && Hero.LastHook.TryGetComponent<TitanBase>(out var titan))
                {
                    if (titan.Body.Neck != null)
                        Hero.AttackAccordingToTarget(titan.Body.Neck);
                    else
                        skillFailed = true;
                }
                else if ((Hero.HookLeft != null) && (Hero.HookLeft.transform.parent != null))
                {
                    var neck = Hero.HookLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (neck != null)
                        Hero.AttackAccordingToTarget(neck);
                    else
                        Hero.AttackAccordingToMouse();
                }
                else if ((Hero.HookRight != null) && (Hero.HookRight.transform.parent != null))
                {
                    var transform2 = Hero.HookRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (transform2 != null)
                        Hero.AttackAccordingToTarget(transform2);
                    else
                        Hero.AttackAccordingToMouse();
                }
                else
                {
                    var nearestTitan = Hero.FindNearestTitan();
                    if (nearestTitan != null)
                    {
                        var neck = nearestTitan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                        if (neck != null)
                            Hero.AttackAccordingToTarget(neck);
                        else
                            Hero.AttackAccordingToMouse();
                    }
                    else
                        Hero.AttackAccordingToMouse();
                }
            }
        }

        private void ProcessGunAttack()
        {
            if (Hero.LeftGunHasBullet)
            {
                Hero.LeftArmAim = true;
                Hero.RightArmAim = false;
            }
            else
            {
                Hero.LeftArmAim = false;

                if (Hero.RightGunHasBullet)
                    Hero.RightArmAim = true;
                else
                    Hero.RightArmAim = false;
            }
        }

        public override void OnSkill()
        {
            if (!Hero.UseGun)
                ProcessBladeSpecialAttack();
            else
                ProcessGunSpecialAttack();

            ProcessGun();

            if (Hero.Skill != null)
            {
                if (!Hero.Skill.IsActive)
                {
                    if (!Hero.Skill.Use())
                    {
                        if (Hero.NeedLean)
                        {
                            if (Hero.LeanLeft)
                                Hero.AttackAnimation = (Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                            else
                                Hero.AttackAnimation = (Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                        }
                        else
                            Hero.AttackAnimation = HeroAnim.ATTACK1;

                        Hero.PlayAnimation(Hero.AttackAnimation);
                    }
                }
            }
        }

        public override void OnSkillRelease()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                if (Hero.UseGun)
                {
                    if (!(Hero.Skill is BombPvpSkill))
                    {
                        if (Hero.LeftGunHasBullet && Hero.RightGunHasBullet)
                        {
                            if (Hero.IsGrounded)
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH;
                            else
                                Hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH_AIR;

                            bothGunsHaveBullet = true;
                        }
                        else if (!Hero.LeftGunHasBullet && !Hero.RightGunHasBullet)
                            noBulletsInEitherGun = true;
                        else
                            gunsHaveABullet = true;
                    }
                }
            }
        }

        private void ProcessBladeSpecialAttack()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                skillFailed = false;
                if (Hero.SkillCDDuration > 0f)
                    skillFailed = true;
                else
                {
                    Hero.SkillCDDuration = Hero.SkillCDLast;
                    var skillSuccess = !Hero.Skill.Use();
                    if (!skillSuccess)
                        skillFailed = true;
                }
            }
        }
        private void ProcessGunSpecialAttack()
        {
            Hero.LeftArmAim = true;
            Hero.RightArmAim = true;
        }

        private void ProcessGun()
        {
            if (Hero.LeftArmAim || Hero.RightArmAim)
            {
                RaycastHit hit3;
                Ray ray3 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
                if (Physics.Raycast(ray3, out hit3, Hero.HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                    Hero.GunTarget = hit3.point;
            }
        }

        public override void OnReload()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
                if (Hero.Animation.IsPlaying(Hero.StandAnimation) || !Hero.IsGrounded)
                    Hero.Reload();
        }

        public override void OnSalute()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
                if (Hero.Animation.IsPlaying(Hero.StandAnimation))
                    Hero.SetState<HumanIdleState>();
        }

        public override void OnItem1() => Hero.ShootFlare(1);
        public override void OnItem2() => Hero.ShootFlare(2);
        public override void OnItem3() => Hero.ShootFlare(3);

        public override void OnJump()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;
            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;

            if (!Hero.IsGrounded)
                return;

            Debug.Log("Jump from idle");

            Hero.SetState<HumanIdleState>();
            Hero.CrossFade(HeroAnim.JUMP, 0.1f);
            Hero.SparksEM.enabled = false;
        }

        public override void OnDodge()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;

            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;

            Hero.Dodge(false);
        }

        public override void OnMount()
        {
            if (Hero.TitanForm || Hero.IsCannon)
                return;

            if (Hero.IsMounted)
            {
                Hero.GetOffHorse();
                return;
            }

            if (Hero.Animation.IsPlaying(HeroAnim.JUMP) || Hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                return;
            if (Hero.Horse == null || Hero.IsMounted || Vector3.Distance(Hero.Horse.transform.position, Hero.transform.position) > 15f)
                return;

            Hero.GetOnHorse();
        }
    }
}