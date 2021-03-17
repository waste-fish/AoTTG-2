using Assets.Scripts.Characters.Titan;

namespace Assets.Scripts.Characters.Humans.States
{
    public class GrabState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.Grab;

        public GrabState(Hero hero) : base(hero) { }

        public override void OnSpecialAttack()
        {
            if (!hero.useGun)
            {
                if (hero.Skill.CanUseWhileGrabbed)
                {
                    hero.skillCDDuration = hero.skillCDLast;
                    if (hero.TitanWhoGrabMe.GetComponent<MindlessTitan>() != null)
                    {
                        hero.BreakFreeFromGrab();
                        hero.photonView.RPC(nameof(hero.NetSetIsGrabbedFalse), PhotonTargets.All, new object[0]);
                        if (PhotonNetwork.isMasterClient)
                            hero.TitanWhoGrabMe.GetComponent<MindlessTitan>().GrabEscapeRpc();
                        else
                            PhotonView.Find(hero.TitanWhoGrabMeID).photonView.RPC(nameof(MindlessTitan.GrabEscapeRpc), PhotonTargets.MasterClient, new object[0]);
                        hero.Skill.Use();
                    }
                }
            }
        }
    }
}
