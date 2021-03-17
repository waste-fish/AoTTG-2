using UnityEngine;
using Assets.Scripts.UI.Input;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class AttackState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Attack;

        public AttackState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
            {
                if (!hero.useGun)
                    UpdateBlade();
                else
                    UpdateGun();
            }
        }

        private void UpdateBlade()
        {
            if (!hero.AttackReleased)
            {
                //TODO: Pause the Animation if the player is holding a button
                if (!InputManager.HumanAttack)
                {
                    hero.SetAnimationSpeed(hero.CurrentAnimation);
                    hero.AttackReleased = true;
                }
                else if (hero.Animation[hero.AttackAnimation].normalizedTime >= 0.32f && hero.Animation[hero.AttackAnimation].speed > 0f)
                {
                    Debug.Log("Trying to freeze");
                    hero.SetAnimationSpeed(hero.AttackAnimation, 0f);
                }
            }

            UpdateBladeAttackAnimation();

            if (hero.Animation[hero.AttackAnimation].normalizedTime >= 1f)
            {
                if ((hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_0) || (hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_1))
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        object[] parameters = new object[] { 5f, 100f };
                        hero.photonView.RPC(nameof(hero.NetTauntAttack), PhotonTargets.MasterClient, parameters);
                    }
                    else
                        hero.NetTauntAttack(5f, 100f);

                    hero.FalseAttack();
                    hero.Idle();
                }
                else if (hero.AttackAnimation == HeroAnim.SPECIAL_ARMIN)
                {
                    if (!PhotonNetwork.isMasterClient)
                        hero.photonView.RPC(nameof(hero.NetlaughAttack), PhotonTargets.MasterClient, new object[0]);
                    else
                        hero.NetlaughAttack();

                    hero.FalseAttack();
                    hero.Idle();
                }
                else if (hero.AttackAnimation == HeroAnim.ATTACK3_1)
                    hero.Rigidbody.velocity -= ((Vector3.up * Time.deltaTime) * 30f);
                else
                {
                    hero.FalseAttack();
                    hero.Idle();
                }
            }

            if (hero.Animation.IsPlaying(HeroAnim.ATTACK3_2) && (hero.Animation[HeroAnim.ATTACK3_2].normalizedTime >= 1f))
            {
                hero.FalseAttack();
                hero.Idle();
            }
        }

        private void UpdateBladeAttackAnimation()
        {
            if ((hero.AttackAnimation == HeroAnim.ATTACK3_1) && (hero.currentBladeSta > 0f))
            {
                if (hero.Animation[hero.AttackAnimation].normalizedTime >= 0.8f)
                {
                    if (!hero.checkBoxLeft.IsActive)
                    {
                        hero.checkBoxLeft.IsActive = true;
                        hero.Rigidbody.velocity = (-Vector3.up * 30f);
                    }
                    if (!hero.checkBoxRight.IsActive)
                    {
                        hero.checkBoxRight.IsActive = true;
                        hero.slash.Play();
                    }
                }
                else if (hero.checkBoxLeft.IsActive)
                {
                    hero.checkBoxLeft.IsActive = false;
                    hero.checkBoxRight.IsActive = false;
                    hero.checkBoxLeft.ClearHits();
                    hero.checkBoxRight.ClearHits();
                }
            }
            else
            {
                float min;
                float max;

                switch (hero.AttackAnimation)
                {
                    case HeroAnim.ATTACK5:
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

                if (hero.currentBladeSta == 0f)
                {
                    max = -1f;
                    min = -1f;
                }

                if ((hero.Animation[hero.AttackAnimation].normalizedTime > min) && (hero.Animation[hero.AttackAnimation].normalizedTime < max))
                {
                    if (!hero.checkBoxLeft.IsActive)
                    {
                        hero.checkBoxLeft.IsActive = true;
                        hero.slash.Play();
                    }
                    if (!hero.checkBoxRight.IsActive)
                    {
                        hero.checkBoxRight.IsActive = true;
                    }
                }
                else if (hero.checkBoxLeft.IsActive)
                {
                    hero.checkBoxLeft.IsActive = false;
                    hero.checkBoxRight.IsActive = false;
                    hero.checkBoxLeft.ClearHits();
                    hero.checkBoxRight.ClearHits();
                }
                if ((hero.AttackLoop > 0) && (hero.Animation[hero.AttackAnimation].normalizedTime > max))
                {
                    hero.AttackLoop--;
                    hero.PlayAnimationAt(hero.AttackAnimation, min);
                }
            }
        }

        private void UpdateGun()
        {
            hero.checkBoxLeft.IsActive = false;
            hero.checkBoxRight.IsActive = false;
            hero.transform.rotation = Quaternion.Lerp(hero.transform.rotation, hero.GunDummy.transform.rotation, Time.deltaTime * 30f);

            if (!hero.AttackReleased && (hero.Animation[hero.AttackAnimation].normalizedTime > 0.167f))
            {
                GameObject shotGun;
                hero.AttackReleased = true;
                bool flag7 = false;
                if ((hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH) || (hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH_AIR))
                {
                    //Should use AHSSShotgunCollider instead of TriggerColliderWeapon.  
                    //Apply that change when abstracting weapons from this class.
                    //Note, when doing the abstraction, the relationship between the weapon collider and the abstracted weapon class should be carefully considered.
                    hero.checkBoxLeft.IsActive = true;
                    hero.checkBoxRight.IsActive = true;
                    flag7 = true;
                    hero.leftGunHasBullet = false;
                    hero.rightGunHasBullet = false;
                    hero.Rigidbody.AddForce((-hero.transform.forward * 1000f), ForceMode.Acceleration);
                }
                else
                {
                    if (hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L || hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L_AIR)
                    {
                        hero.checkBoxLeft.IsActive = true;
                        hero.leftGunHasBullet = false;
                    }
                    else
                    {
                        hero.checkBoxRight.IsActive = true;
                        hero.rightGunHasBullet = false;
                    }
                    hero.Rigidbody.AddForce((-hero.transform.forward * 600f), ForceMode.Acceleration);
                }

                hero.Rigidbody.AddForce((Vector3.up * 200f), ForceMode.Acceleration);

                var prefabName = "FX/shotGun";
                if (flag7)
                    prefabName = "FX/shotGun 1";

                if (hero.photonView.isMine)
                {
                    shotGun = PhotonNetwork.Instantiate(prefabName, (hero.transform.position + (hero.transform.up * 0.8f)) - (hero.transform.right * 0.1f), hero.transform.rotation, 0);
                    if (shotGun.GetComponent<EnemyfxIDcontainer>() != null)
                        shotGun.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = hero.photonView.viewID;
                }
                else
                    Object.Instantiate(Resources.Load<GameObject>(prefabName), ((hero.transform.position + (hero.transform.up * 0.8f)) - (hero.transform.right * 0.1f)), hero.transform.rotation);
            }

            if (hero.Animation[hero.AttackAnimation].normalizedTime >= 1f)
            {
                hero.FalseAttack();
                hero.Idle();
                hero.checkBoxLeft.IsActive = false;
                hero.checkBoxRight.IsActive = false;
            }

            if (!hero.Animation.IsPlaying(hero.AttackAnimation))
            {
                hero.FalseAttack();
                hero.Idle();
                hero.checkBoxLeft.IsActive = false;
                hero.checkBoxRight.IsActive = false;
            }
        }
    }
}
