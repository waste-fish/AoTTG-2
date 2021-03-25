using Assets.Scripts.Characters.Titan;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanGrabState : BaseHumanState
    {
        public override void OnFixedUpdate()
        {
            _hero.Rigidbody.AddForce(-_hero.Rigidbody.velocity, ForceMode.VelocityChange);
        }

        public override void OnSpecialAttack()
        {
            if (!_hero.UseGun)
            {
                if (_hero.Skill.CanUseWhileGrabbed)
                {
                    _hero.SkillCDDuration = _hero.SkillCDLast;
                    if (_hero.TitanWhoGrabMe.GetComponent<MindlessTitan>() != null)
                    {
                        _hero.BreakFreeFromGrab();
                        _hero.photonView.RPC(nameof(_hero.NetSetIsGrabbedFalse), PhotonTargets.All, new object[0]);
                        if (PhotonNetwork.isMasterClient)
                            _hero.TitanWhoGrabMe.GetComponent<MindlessTitan>().GrabEscapeRpc();
                        else
                            PhotonView.Find(_hero.TitanWhoGrabMeID).photonView.RPC(nameof(MindlessTitan.GrabEscapeRpc), PhotonTargets.MasterClient, new object[0]);
                        _hero.Skill.Use();
                    }
                }
            }
        }
    }
}
