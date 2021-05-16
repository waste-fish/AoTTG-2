using UnityEngine;
using Assets.Scripts.UI.Input;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanAttackState : BaseHumanState
    {
        public override bool HasMovementControl => false;

        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                if (!Hero.UseGun)
                    UpdateBlade();
                else
                    UpdateGun();
            }
        }

        private void UpdateBlade()
        {
            //if (!Hero.AttackReleased)
            //{
            //    //TODO: Pause the Animation if the player is holding a button
            //    if (!InputManager.HumanAttack)
            //    {
            //        Hero.SetAnimationSpeed(Hero.CurrentAnimation);
            //        Hero.AttackReleased = true;
            //    }
            //    else if (Hero.Animation[Hero.AttackAnimation].normalizedTime >= 0.32f && Hero.Animation[Hero.AttackAnimation].speed > 0f)
            //    {
            //        Debug.Log("Trying to freeze");
            //        Hero.SetAnimationSpeed(Hero.AttackAnimation, 0f);
            //    }
            //}

            UpdateBladeAttackAnimation();

            if (Hero.Animation[Hero.AttackAnimation].normalizedTime >= 1f)
            {
                if ((Hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_0) || (Hero.AttackAnimation == HeroAnim.SPECIAL_MARCO_1))
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        object[] parameters = new object[] { 5f, 100f };
                        Hero.photonView.RPC(nameof(Hero.NetTauntAttack), PhotonTargets.MasterClient, parameters);
                    }
                    else
                        Hero.NetTauntAttack(5f, 100f);

                    Hero.FalseAttack();
                    Hero.SetState<HumanIdleState>();
                }
                else if (Hero.AttackAnimation == HeroAnim.SPECIAL_ARMIN)
                {
                    if (!PhotonNetwork.isMasterClient)
                        Hero.photonView.RPC(nameof(Hero.NetlaughAttack), PhotonTargets.MasterClient, new object[0]);
                    else
                        Hero.NetlaughAttack();

                    Hero.FalseAttack();
                    Hero.SetState<HumanIdleState>();
                }
                else if (Hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0)
                    Hero.Rigidbody.velocity -= ((Vector3.up * Time.deltaTime) * 30f);
                else
                {
                    Hero.FalseAttack();
                    Hero.SetState<HumanIdleState>();
                }
            }

            if (Hero.Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1) && (Hero.Animation[HeroAnim.SPECIAL_MIKASA_1].normalizedTime >= 1f))
            {
                Hero.FalseAttack();
                Hero.SetState<HumanIdleState>();
            }
        }

        private void UpdateBladeAttackAnimation()
        {
            if ((Hero.AttackAnimation == HeroAnim.SPECIAL_MIKASA_0) && (Hero.CurrentBladeSta > 0f))
            {
                if (Hero.Animation[Hero.AttackAnimation].normalizedTime >= 0.8f)
                {
                    if (!Hero.checkBoxLeft.IsActive)
                    {
                        Hero.checkBoxLeft.IsActive = true;
                        Hero.Rigidbody.velocity = (-Vector3.up * 30f);
                    }
                    if (!Hero.checkBoxRight.IsActive)
                    {
                        Hero.checkBoxRight.IsActive = true;
                        Hero.slash.Play();
                    }
                }
                else if (Hero.checkBoxLeft.IsActive)
                {
                    Hero.checkBoxLeft.IsActive = false;
                    Hero.checkBoxRight.IsActive = false;
                    Hero.checkBoxLeft.ClearHits();
                    Hero.checkBoxRight.ClearHits();
                }
            }
            else
            {
                float min;
                float max;

                switch (Hero.AttackAnimation)
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

                if (Hero.CurrentBladeSta == 0f)
                {
                    max = -1f;
                    min = -1f;
                }

                if ((Hero.Animation[Hero.AttackAnimation].normalizedTime > min) && (Hero.Animation[Hero.AttackAnimation].normalizedTime < max))
                {
                    if (!Hero.checkBoxLeft.IsActive)
                    {
                        Hero.checkBoxLeft.IsActive = true;
                        Hero.slash.Play();
                    }
                    if (!Hero.checkBoxRight.IsActive)
                    {
                        Hero.checkBoxRight.IsActive = true;
                    }
                }
                else if (Hero.checkBoxLeft.IsActive)
                {
                    Hero.checkBoxLeft.IsActive = false;
                    Hero.checkBoxRight.IsActive = false;
                    Hero.checkBoxLeft.ClearHits();
                    Hero.checkBoxRight.ClearHits();
                }
                if ((Hero.AttackLoop > 0) && (Hero.Animation[Hero.AttackAnimation].normalizedTime > max))
                {
                    Hero.AttackLoop--;
                    Hero.PlayAnimationAt(Hero.AttackAnimation, min);
                }
            }
        }

        private void UpdateGun()
        {
            Hero.checkBoxLeft.IsActive = false;
            Hero.checkBoxRight.IsActive = false;
            Hero.transform.rotation = Quaternion.Lerp(Hero.transform.rotation, Hero.GunDummy.transform.rotation, Time.deltaTime * 30f);

            if (/*!Hero.AttackReleased && */(Hero.Animation[Hero.AttackAnimation].normalizedTime > 0.167f))
            {
                GameObject shotGun;
                //Hero.AttackReleased = true;
                bool flag7 = false;
                if ((Hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH) || (Hero.AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH_AIR))
                {
                    //Should use AHSSShotgunCollider instead of TriggerColliderWeapon.  
                    //Apply that change when abstracting weapons from this class.
                    //Note, when doing the abstraction, the relationship between the weapon collider and the abstracted weapon class should be carefully considered.
                    Hero.checkBoxLeft.IsActive = true;
                    Hero.checkBoxRight.IsActive = true;
                    flag7 = true;
                    Hero.LeftGunHasBullet = false;
                    Hero.RightGunHasBullet = false;
                    Hero.Rigidbody.AddForce((-Hero.transform.forward * 1000f), ForceMode.Acceleration);
                }
                else
                {
                    if (Hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L || Hero.AttackAnimation == HeroAnim.AHSS_SHOOT_L_AIR)
                    {
                        Hero.checkBoxLeft.IsActive = true;
                        Hero.LeftGunHasBullet = false;
                    }
                    else
                    {
                        Hero.checkBoxRight.IsActive = true;
                        Hero.RightGunHasBullet = false;
                    }
                    Hero.Rigidbody.AddForce((-Hero.transform.forward * 600f), ForceMode.Acceleration);
                }

                Hero.Rigidbody.AddForce((Vector3.up * 200f), ForceMode.Acceleration);

                var prefabName = "FX/shotGun";
                if (flag7)
                    prefabName = "FX/shotGun 1";

                if (Hero.photonView.isMine)
                {
                    shotGun = PhotonNetwork.Instantiate(prefabName, (Hero.transform.position + (Hero.transform.up * 0.8f)) - (Hero.transform.right * 0.1f), Hero.transform.rotation, 0);
                    if (shotGun.GetComponent<EnemyfxIDcontainer>() != null)
                        shotGun.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = Hero.photonView.viewID;
                }
                else
                    Object.Instantiate(Resources.Load<GameObject>(prefabName), ((Hero.transform.position + (Hero.transform.up * 0.8f)) - (Hero.transform.right * 0.1f)), Hero.transform.rotation);
            }

            if (Hero.Animation[Hero.AttackAnimation].normalizedTime >= 1f)
            {
                Hero.FalseAttack();
                Hero.SetState<HumanIdleState>();
                Hero.checkBoxLeft.IsActive = false;
                Hero.checkBoxRight.IsActive = false;
            }

            if (!Hero.Animation.IsPlaying(Hero.AttackAnimation))
            {
                Hero.FalseAttack();
                Hero.SetState<HumanIdleState>();
                Hero.checkBoxLeft.IsActive = false;
                Hero.checkBoxRight.IsActive = false;
            }
        }

        public override void OnFixedUpdate()
        {
            if (Hero.IsGrounded)

            if (Hero.AttackAnimation == HeroAnim.SPECIAL_LEVI)
            {
                if ((Hero.Animation[Hero.AttackAnimation].normalizedTime > 0.4f) && (Hero.Animation[Hero.AttackAnimation].normalizedTime < 0.61f))
                {
                    Hero.Rigidbody.AddForce((Hero.transform.forward * 200f));
                }
            }
            else if (Hero.Animation.IsPlaying(HeroAnim.ATTACK1) || Hero.Animation.IsPlaying(HeroAnim.ATTACK2))
            {
                Hero.Rigidbody.AddForce((Hero.transform.forward * 200f));
            }
        }
    }
}
