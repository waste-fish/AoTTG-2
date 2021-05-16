using Assets.Scripts.Characters.Titan;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanGrabState : BaseHumanState
    {
        public override bool CancelFixedUpdate => true;
        public override bool HasMovementControl => false;

        public override void OnFixedUpdate()
        {
            Hero.Rigidbody.AddForce(-Hero.Rigidbody.velocity, ForceMode.VelocityChange);
        }

        public override void OnSkill()
        {
            if (!Hero.UseGun)
            {
                if (Hero.Skill.BreaksGrabState)
                {
                    Hero.SkillCDDuration = Hero.SkillCDLast;
                    if (Hero.TitanWhoGrabMe.GetComponent<MindlessTitan>() != null)
                    {
                        Hero.BreakFreeFromGrab();
                        Hero.photonView.RPC(nameof(Hero.NetSetIsGrabbedFalse), PhotonTargets.All, new object[0]);
                        if (PhotonNetwork.isMasterClient)
                            Hero.TitanWhoGrabMe.GetComponent<MindlessTitan>().GrabEscapeRpc();
                        else
                            PhotonView.Find(Hero.TitanWhoGrabMeID).photonView.RPC(nameof(MindlessTitan.GrabEscapeRpc), PhotonTargets.MasterClient, new object[0]);
                        Hero.Skill.Use();
                    }
                }
            }
        }
    }
}
