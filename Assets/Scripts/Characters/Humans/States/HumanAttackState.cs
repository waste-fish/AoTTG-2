using UnityEngine;
using Assets.Scripts.UI.Input;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanAttackState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                if (!_hero.UseGun)
                    UpdateBlade();
                else
                    UpdateGun();
            }
        }

        private void UpdateBlade()
        {
            if (!_hero.AttackReleased)
            {
                //TODO: Pause the Animation if the player is holding a button
                if (!InputManager.HumanAttack)
                {
                    _hero.SetAnimationSpeed(_hero.CurrentAnimation);
                    _hero.AttackReleased = true;
                }
                else if (_hero.Animation[_hero.AttackAnimation].normalizedTime >= 0.32f && _hero.Animation[_hero.AttackAnimation].speed > 0f)
                {
                    Debug.Log("Trying to freeze");
                    _hero.SetAnimationSpeed(_hero.AttackAnimation, 0f);
                }
            }

            UpdateBladeAttackAnimation();

            if (_hero.Animation[_hero.AttackAnimation].normalizedTime >= 1f)
            {
                if ((_hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_0) || (_hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_1))
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        object[] parameters = new object[] { 5f, 100f };
                        _hero.photonView.RPC(nameof(_hero.NetTauntAttack), PhotonTargets.MasterClient, parameters);
                    }
                    else
                        _hero.NetTauntAttack(5f, 100f);

                    _hero.FalseAttack();
                    _hero.SetState<HumanIdleState>();
                }
                else if (_hero.AttackAnimation == HeroAnim.SPECIAL_ARMIN)
                {
                    if (!PhotonNetwork.isMasterClient)
                        _hero.photonView.RPC(nameof(_hero.NetlaughAttack), PhotonTargets.MasterClient, new object[0]);
                    else
                        _hero.NetlaughAttack();

                    _hero.FalseAttack();
                    _hero.SetState<HumanIdleState>();
                }
                else if (_hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0)
                    _hero.Rigidbody.velocity -= ((Vector3.up * Time.deltaTime) * 30f);
                else
                {
                    _hero.FalseAttack();
                    _hero.SetState<HumanIdleState>();
                }
            }

            if (_hero.Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1) && (_hero.Animation[HeroAnim.SPECIAL_MIKASA_1].normalizedTime >= 1f))
            {
                _hero.FalseAttack();
                _hero.SetState<HumanIdleState>();
            }
        }

        private void UpdateBladeAttackAnimation()
        {
            if ((_hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0) && (_hero.CurrentBladeSta > 0f))
            {
                if (_hero.Animation[_hero.AttackAnimation].normalizedTime >= 0.8f)
                {
                    if (!_hero.checkBoxLeft.IsActive)
                    {
                        _hero.checkBoxLeft.IsActive = true;
                        _hero.Rigidbody.velocity = (-Vector3.up * 30f);
                    }
                    if (!_hero.checkBoxRight.IsActive)
                    {
                        _hero.checkBoxRight.IsActive = true;
                        _hero.slash.Play();
                    }
                }
                else if (_hero.checkBoxLeft.IsActive)
                {
                    _hero.checkBoxLeft.IsActive = false;
                    _hero.checkBoxRight.IsActive = false;
                    _hero.checkBoxLeft.ClearHits();
                    _hero.checkBoxRight.ClearHits();
                }
            }
            else
            {
                float min;
                float max;

                switch (_hero.AttackAnimation)
                {
                    case HeroAnim.SPECIAL_LEVI:
                        min = 0.35f;
                        max = 0.5f;
                        break;
                    case HeroAnim.SPECIAL_PETRA:
                        min = 0.35f;
                        max = 0.48f;
                        break;
                    case HeroAnim.SPECIAL_ARMIN:
                        min = 0.25f;
                        max = 0.35f;
                        break;
                    case HeroAnim.ATTACK4:
                        min = 0.6f;
                        max = 0.9f;
                        break;
                    case HeroAnim.SPECIAL_SASHA:
                        min = -1f;
                        max = -1f;
                        break;
                    default:
                        min = 0.5f;
                        max = 0.85f;
                        break;
                }

                if (_hero.CurrentBladeSta == 0f)
                {
                    max = -1f;
                    min = -1f;
                }

                if ((_hero.Animation[_hero.AttackAnimation].normalizedTime > min) && (_hero.Animation[_hero.AttackAnimation].normalizedTime < max))
                {
                    if (!_hero.checkBoxLeft.IsActive)
                    {
                        _hero.checkBoxLeft.IsActive = true;
                        _hero.slash.Play();
                    }
                    if (!_hero.checkBoxRight.IsActive)
                    {
                        _hero.checkBoxRight.IsActive = true;
                    }
                }
                else if (_hero.checkBoxLeft.IsActive)
                {
                    _hero.checkBoxLeft.IsActive = false;
                    _hero.checkBoxRight.IsActive = false;
                    _hero.checkBoxLeft.ClearHits();
                    _hero.checkBoxRight.ClearHits();
                }
                if ((_hero.AttackLoop > 0) && (_hero.Animation[_hero.AttackAnimation].normalizedTime > max))
                {
                    _hero.AttackLoop--;
                    _hero.PlayAnimationAt(_hero.AttackAnimation, min);
                }
            }
        }

        private void UpdateGun()
        {
            _hero.checkBoxLeft.IsActive = false;
            _hero.checkBoxRight.IsActive = false;
            _hero.transform.rotation = Quaternion.Lerp(_hero.transform.rotation, _hero.GunDummy.transform.rotation, Time.deltaTime * 30f);

            if (!_hero.AttackReleased && (_hero.Animation[_hero.AttackAnimation].normalizedTime > 0.167f))
            {
                GameObject shotGun;
                _hero.AttackReleased = true;
                bool flag7 = false;
                if ((_hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH) || (_hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH_AIR))
                {
                    //Should use AHSSShotgunCollider instead of TriggerColliderWeapon.  
                    //Apply that change when abstracting weapons from this class.
                    //Note, when doing the abstraction, the relationship between the weapon collider and the abstracted weapon class should be carefully considered.
                    _hero.checkBoxLeft.IsActive = true;
                    _hero.checkBoxRight.IsActive = true;
                    flag7 = true;
                    _hero.LeftGunHasBullet = false;
                    _hero.RightGunHasBullet = false;
                    _hero.Rigidbody.AddForce((-_hero.transform.forward * 1000f), ForceMode.Acceleration);
                }
                else
                {
                    if (_hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L || _hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L_AIR)
                    {
                        _hero.checkBoxLeft.IsActive = true;
                        _hero.LeftGunHasBullet = false;
                    }
                    else
                    {
                        _hero.checkBoxRight.IsActive = true;
                        _hero.RightGunHasBullet = false;
                    }
                    _hero.Rigidbody.AddForce((-_hero.transform.forward * 600f), ForceMode.Acceleration);
                }

                _hero.Rigidbody.AddForce((Vector3.up * 200f), ForceMode.Acceleration);

                var prefabName = "FX/shotGun";
                if (flag7)
                    prefabName = "FX/shotGun 1";

                if (_hero.photonView.isMine)
                {
                    shotGun = PhotonNetwork.Instantiate(prefabName, (_hero.transform.position + (_hero.transform.up * 0.8f)) - (_hero.transform.right * 0.1f), _hero.transform.rotation, 0);
                    if (shotGun.GetComponent<EnemyfxIDcontainer>() != null)
                        shotGun.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = _hero.photonView.viewID;
                }
                else
                    Object.Instantiate(Resources.Load<GameObject>(prefabName), ((_hero.transform.position + (_hero.transform.up * 0.8f)) - (_hero.transform.right * 0.1f)), _hero.transform.rotation);
            }

            if (_hero.Animation[_hero.AttackAnimation].normalizedTime >= 1f)
            {
                _hero.FalseAttack();
                _hero.SetState<HumanIdleState>();
                _hero.checkBoxLeft.IsActive = false;
                _hero.checkBoxRight.IsActive = false;
            }

            if (!_hero.Animation.IsPlaying(_hero.AttackAnimation))
            {
                _hero.FalseAttack();
                _hero.SetState<HumanIdleState>();
                _hero.checkBoxLeft.IsActive = false;
                _hero.checkBoxRight.IsActive = false;
            }
        }
    }
}
