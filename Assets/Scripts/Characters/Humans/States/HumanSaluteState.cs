using UnityEngine;
using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanSaluteState : BaseHumanState
    {
        public override void OnEnter() => _hero.CrossFade(HeroAnim.SALUTE, 0.1f);

        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                if (_hero.Animation[HeroAnim.SALUTE].normalizedTime >= 1f)
                    _hero.SetState<HumanIdleState>();
            }
        }
    }
}
