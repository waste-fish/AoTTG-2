using UnityEngine;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanSaluteState : BaseHumanState
    {
        public override void OnEnter() => Hero.CrossFade(HeroAnim.SALUTE, 0.1f);

        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
            {
                if (Hero.Animation[HeroAnim.SALUTE].normalizedTime >= 1f)
                    Hero.SetState<HumanIdleState>();
            }
        }
    }
}
