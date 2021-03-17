using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.Skills;
using Assets.Scripts.Characters.Titan;
using Assets.Scripts.Constants;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class IdleState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Idle;

        private bool skillFailed;
        private bool bothGunsHaveBullet;
        private bool noBulletsInEitherGun;
        private bool gunsHaveABullet;

        public IdleState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                if (hero.grounded)
                {
                    if (!hero.Animation.IsPlaying(HeroAnim.JUMP) && !hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                    {
                        hero.Idle();
                        hero.CrossFade(HeroAnim.JUMP, 0.1f);
                        hero.SparksEM.enabled = false;

                        if (hero.Horse != null && !hero.IsMounted && Vector3.Distance(hero.Horse.transform.position, hero.transform.position) < 15f)
                            hero.GetOnHorse();
                    }
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (!skillFailed)
            {
                hero.checkBoxLeft.ClearHits();
                hero.checkBoxRight.ClearHits();
                if (hero.grounded)
                {
                    hero.Rigidbody.AddForce((hero.transform.forward * 200f));
                }
                hero.PlayAnimation(hero.AttackAnimation);
                hero.Animation[hero.AttackAnimation].time = 0f;
                hero.SquidState = new AttackState(hero);
                if (hero.grounded || hero.AttackAnimation == HeroAnim.ATTACK3_1 || hero.AttackAnimation == HeroAnim.ATTACK5 || hero.AttackAnimation == HeroAnim.SPECIAL_PETRA)
                    hero.AttackReleased = true;
                else
                    hero.AttackReleased = false;
                hero.SparksEM.enabled = false;
            }

            if (bothGunsHaveBullet)
            {
                hero.SquidState = new AttackState(hero);
                hero.CrossFade(hero.AttackAnimation, 0.05f);
                hero.GunDummy.transform.position = hero.transform.position;
                hero.GunDummy.transform.rotation = hero.transform.rotation;
                hero.GunDummy.transform.LookAt(hero.GunTarget);
                hero.AttackReleased = false;
                hero.FacingDirection = hero.GunDummy.transform.rotation.eulerAngles.y;
                hero.TargetRotation = Quaternion.Euler(0f, hero.FacingDirection, 0f);
            }
            else if (noBulletsInEitherGun && (hero.grounded || (GameSettings.PvP.AhssAirReload.Value)))
                hero.Reload();

            bothGunsHaveBullet = false;
            noBulletsInEitherGun = false;
            gunsHaveABullet = false;
        }

        public override void OnAttack()
        {
            if (!hero.useGun)
                ProcessBladeAttack();
            else
                ProcessGunAttack();
        }

        public override void OnAttackRelease()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.useGun)
                    if (gunsHaveABullet)
                    {
                        if (hero.grounded)
                        {
                            if (hero.leftGunHasBullet && hero.rightGunHasBullet)
                            {
                                if (hero.IsLeftHandHooked)
                                    hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                                else
                                    hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            }
                            else if (hero.leftGunHasBullet)
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            else if (hero.rightGunHasBullet)
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                        }
                        else if (hero.leftGunHasBullet && hero.rightGunHasBullet)
                        {
                            if (hero.IsLeftHandHooked)
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;
                            else
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        }
                        else if (hero.leftGunHasBullet)
                            hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        else if (hero.rightGunHasBullet)
                            hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;

                        if (hero.leftGunHasBullet || hero.rightGunHasBullet)
                            bothGunsHaveBullet = true;
                        else
                            noBulletsInEitherGun = true;
                    }
        }

        private void ProcessBladeAttack()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                bool left = hero.TargetMoveDirection.x < 0f;
                bool right = hero.TargetMoveDirection.x > 0f;
                if (hero.NeedLean)
                {
                    bool rand = Random.value <= 0.5f;

                    if (left)
                        hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else if (right)
                        hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                    else if (hero.LeanLeft)
                        hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else
                        hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                }
                else if (left)
                    hero.AttackAnimation = HeroAnim.ATTACK2;
                else if (right)
                    hero.AttackAnimation = HeroAnim.ATTACK1;
                else if (hero.lastHook != null && hero.lastHook.TryGetComponent<TitanBase>(out var titan))
                {
                    if (titan.Body.Neck != null)
                        hero.AttackAccordingToTarget(titan.Body.Neck);
                    else
                        skillFailed = true;
                }
                else if ((hero.HookLeft != null) && (hero.HookLeft.transform.parent != null))
                {
                    var neck = hero.HookLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (neck != null)
                        hero.AttackAccordingToTarget(neck);
                    else
                        hero.AttackAccordingToMouse();
                }
                else if ((hero.HookRight != null) && (hero.HookRight.transform.parent != null))
                {
                    var transform2 = hero.HookRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (transform2 != null)
                        hero.AttackAccordingToTarget(transform2);
                    else
                        hero.AttackAccordingToMouse();
                }
                else
                {
                    var nearestTitan = hero.FindNearestTitan();
                    if (nearestTitan != null)
                    {
                        var neck = nearestTitan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                        if (neck != null)
                            hero.AttackAccordingToTarget(neck);
                        else
                            hero.AttackAccordingToMouse();
                    }
                    else
                        hero.AttackAccordingToMouse();
                }
            }
        }

        private void ProcessGunAttack()
        {
            if (hero.leftGunHasBullet)
            {
                hero.LeftArmAim = true;
                hero.RightArmAim = false;
            }
            else
            {
                hero.LeftArmAim = false;

                if (hero.rightGunHasBullet)
                    hero.RightArmAim = true;
                else
                    hero.RightArmAim = false;
            }
        }

        public override void OnSpecialAttack()
        {
            if (!hero.useGun)
                ProcessBladeSpecialAttack();
            else
                ProcessGunSpecialAttack();

            ProcessGun();
        }

        public override void OnSpecialAttackRelease()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.useGun)
                    if (!(hero.Skill is BombPvpSkill))
                    {
                        if (hero.leftGunHasBullet && hero.rightGunHasBullet)
                        {
                            if (hero.grounded)
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH;
                            else
                                hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH_AIR;

                            bothGunsHaveBullet = true;
                        }
                        else if (!hero.leftGunHasBullet && !hero.rightGunHasBullet)
                            noBulletsInEitherGun = true;
                        else
                            gunsHaveABullet = true;
                    }
        }

        private void ProcessBladeSpecialAttack()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                skillFailed = false;
                if (hero.skillCDDuration > 0f)
                    skillFailed = true;
                else
                {
                    hero.skillCDDuration = hero.skillCDLast;
                    var skillSuccess = !hero.Skill.Use();
                    if (!skillSuccess)
                        skillFailed = true;
                }
            }
        }
        private void ProcessGunSpecialAttack()
        {
            hero.LeftArmAim = true;
            hero.RightArmAim = true;
        }

        private void ProcessGun()
        {
            if (hero.LeftArmAim || hero.RightArmAim)
            {
                RaycastHit hit3;
                Ray ray3 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
                if (Physics.Raycast(ray3, out hit3, 1E+07f, mask.value))
                {
                    hero.GunTarget = hit3.point;
                }
            }
        }

        public override void OnReload()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.Animation.IsPlaying(hero.StandAnimation) || !hero.grounded)
                    hero.Reload();
        }

        public override void OnSalute()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.Animation.IsPlaying(hero.StandAnimation))
                    hero.Salute();
        }

        public override void OnItem1() => hero.ShootFlare(1);
        public override void OnItem2() => hero.ShootFlare(2);
        public override void OnItem3() => hero.ShootFlare(3);

        public override void OnMount()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.Horse != null && hero.IsMounted)
                    hero.GetOffHorse();
        }
    }
}