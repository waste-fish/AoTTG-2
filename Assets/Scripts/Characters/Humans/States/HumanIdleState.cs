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
        private bool skillFailed;
        private bool bothGunsHaveBullet;
        private bool noBulletsInEitherGun;
        private bool gunsHaveABullet;

        public override void OnEnter()
        {
            if (_previous is HumanAttackState)
                _hero.FalseAttack();

            _hero.CrossFade(_hero.StandAnimation, 0.1f);
        }

        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                if (_hero.Grounded)
                {
                    if (!_hero.Animation.IsPlaying(HeroAnim.JUMP) && !_hero.Animation.IsPlaying(HeroAnim.HORSE_GET_ON))
                    {
                        _hero.SetState<HumanIdleState>();
                        _hero.CrossFade(HeroAnim.JUMP, 0.1f);
                        _hero.SparksEM.enabled = false;

                        if (_hero.Horse != null && !_hero.IsMounted && Vector3.Distance(_hero.Horse.transform.position, _hero.transform.position) < 15f)
                            _hero.GetOnHorse();
                    }
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (!skillFailed)
            {
                _hero.checkBoxLeft.ClearHits();
                _hero.checkBoxRight.ClearHits();
                if (_hero.Grounded)
                {
                    _hero.Rigidbody.AddForce((_hero.transform.forward * 200f));
                }
                _hero.PlayAnimation(_hero.AttackAnimation);
                _hero.Animation[_hero.AttackAnimation].time = 0f;
                _hero.SetState<HumanAttackState>();
                if (_hero.Grounded || _hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0 || _hero.AttackAnimation == HeroAnim.SPECIAL_LEVI || _hero.AttackAnimation == HeroAnim.SPECIAL_PETRA)
                    _hero.AttackReleased = true;
                else
                    _hero.AttackReleased = false;
                _hero.SparksEM.enabled = false;
            }

            if (bothGunsHaveBullet)
            {
                _hero.SetState<HumanAttackState>();
                _hero.CrossFade(_hero.AttackAnimation, 0.05f);
                _hero.GunDummy.transform.position = _hero.transform.position;
                _hero.GunDummy.transform.rotation = _hero.transform.rotation;
                _hero.GunDummy.transform.LookAt(_hero.GunTarget);
                _hero.AttackReleased = false;
                _hero.FacingDirection = _hero.GunDummy.transform.rotation.eulerAngles.y;
                _hero.TargetRotation = Quaternion.Euler(0f, _hero.FacingDirection, 0f);
            }
            else if (noBulletsInEitherGun && (_hero.Grounded || (GameSettings.PvP.AhssAirReload.Value)))
                _hero.Reload();

            bothGunsHaveBullet = false;
            noBulletsInEitherGun = false;
            gunsHaveABullet = false;
        }

        public override void OnAttack()
        {
            if (!_hero.UseGun)
                ProcessBladeAttack();
            else
                ProcessGunAttack();
        }

        public override void OnAttackRelease()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.UseGun)
                    if (gunsHaveABullet)
                    {
                        if (_hero.Grounded)
                        {
                            if (_hero.LeftGunHasBullet && _hero.RightGunHasBullet)
                            {
                                if (_hero.IsLeftHandHooked)
                                    _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                                else
                                    _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            }
                            else if (_hero.LeftGunHasBullet)
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                            else if (_hero.RightGunHasBullet)
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                        }
                        else if (_hero.LeftGunHasBullet && _hero.RightGunHasBullet)
                        {
                            if (_hero.IsLeftHandHooked)
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;
                            else
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        }
                        else if (_hero.LeftGunHasBullet)
                            _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                        else if (_hero.RightGunHasBullet)
                            _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;

                        if (_hero.LeftGunHasBullet || _hero.RightGunHasBullet)
                            bothGunsHaveBullet = true;
                        else
                            noBulletsInEitherGun = true;
                    }
        }

        private void ProcessBladeAttack()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                bool left = _hero.TargetMoveDirection.x < 0f;
                bool right = _hero.TargetMoveDirection.x > 0f;
                if (_hero.NeedLean)
                {
                    bool rand = Random.value <= 0.5f;

                    if (left)
                        _hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else if (right)
                        _hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                    else if (_hero.LeanLeft)
                        _hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                    else
                        _hero.AttackAnimation = rand ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                }
                else if (left)
                    _hero.AttackAnimation = HeroAnim.ATTACK2;
                else if (right)
                    _hero.AttackAnimation = HeroAnim.ATTACK1;
                else if (_hero.LastHook != null && _hero.LastHook.TryGetComponent<TitanBase>(out var titan))
                {
                    if (titan.Body.Neck != null)
                        _hero.AttackAccordingToTarget(titan.Body.Neck);
                    else
                        skillFailed = true;
                }
                else if ((_hero.HookLeft != null) && (_hero.HookLeft.transform.parent != null))
                {
                    var neck = _hero.HookLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (neck != null)
                        _hero.AttackAccordingToTarget(neck);
                    else
                        _hero.AttackAccordingToMouse();
                }
                else if ((_hero.HookRight != null) && (_hero.HookRight.transform.parent != null))
                {
                    var transform2 = _hero.HookRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                    if (transform2 != null)
                        _hero.AttackAccordingToTarget(transform2);
                    else
                        _hero.AttackAccordingToMouse();
                }
                else
                {
                    var nearestTitan = _hero.FindNearestTitan();
                    if (nearestTitan != null)
                    {
                        var neck = nearestTitan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                        if (neck != null)
                            _hero.AttackAccordingToTarget(neck);
                        else
                            _hero.AttackAccordingToMouse();
                    }
                    else
                        _hero.AttackAccordingToMouse();
                }
            }
        }

        private void ProcessGunAttack()
        {
            if (_hero.LeftGunHasBullet)
            {
                _hero.LeftArmAim = true;
                _hero.RightArmAim = false;
            }
            else
            {
                _hero.LeftArmAim = false;

                if (_hero.RightGunHasBullet)
                    _hero.RightArmAim = true;
                else
                    _hero.RightArmAim = false;
            }
        }

        public override void OnSpecialAttack()
        {
            if (!_hero.UseGun)
                ProcessBladeSpecialAttack();
            else
                ProcessGunSpecialAttack();

            ProcessGun();

            if (_hero.Skill != null)
            {
                if (!_hero.Skill.IsActive)
                {
                    if (!_hero.Skill.Use())
                    {
                        if (_hero.NeedLean)
                        {
                            if (_hero.LeanLeft)
                                _hero.AttackAnimation = (Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                            else
                                _hero.AttackAnimation = (Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                        }
                        else
                            _hero.AttackAnimation = HeroAnim.ATTACK1;

                        _hero.PlayAnimation(_hero.AttackAnimation);
                    }
                }
            }
        }

        public override void OnSpecialAttackRelease()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                if (_hero.UseGun)
                {
                    if (!(_hero.Skill is BombPvpSkill))
                    {
                        if (_hero.LeftGunHasBullet && _hero.RightGunHasBullet)
                        {
                            if (_hero.Grounded)
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH;
                            else
                                _hero.AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH_AIR;

                            bothGunsHaveBullet = true;
                        }
                        else if (!_hero.LeftGunHasBullet && !_hero.RightGunHasBullet)
                            noBulletsInEitherGun = true;
                        else
                            gunsHaveABullet = true;
                    }
                }
            }
        }

        private void ProcessBladeSpecialAttack()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                skillFailed = false;
                if (_hero.SkillCDDuration > 0f)
                    skillFailed = true;
                else
                {
                    _hero.SkillCDDuration = _hero.SkillCDLast;
                    var skillSuccess = !_hero.Skill.Use();
                    if (!skillSuccess)
                        skillFailed = true;
                }
            }
        }
        private void ProcessGunSpecialAttack()
        {
            _hero.LeftArmAim = true;
            _hero.RightArmAim = true;
        }

        private void ProcessGun()
        {
            if (_hero.LeftArmAim || _hero.RightArmAim)
            {
                RaycastHit hit3;
                Ray ray3 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
                if (Physics.Raycast(ray3, out hit3, Hero.HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                    _hero.GunTarget = hit3.point;
            }
        }

        public override void OnReload()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.Animation.IsPlaying(_hero.StandAnimation) || !_hero.Grounded)
                    _hero.Reload();
        }

        public override void OnSalute()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.Animation.IsPlaying(_hero.StandAnimation))
                    _hero.SetState<HumanIdleState>();
        }

        public override void OnItem1() => _hero.ShootFlare(1);
        public override void OnItem2() => _hero.ShootFlare(2);
        public override void OnItem3() => _hero.ShootFlare(3);

        public override void OnMount()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.Horse != null && _hero.IsMounted)
                    _hero.GetOffHorse();
        }
    }
}