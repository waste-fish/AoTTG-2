using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanAirDodgeState : BaseHumanState
    {
        public override void OnFixedUpdate()
        {
            if (_hero.DashTime > 0f)
            {
                _hero.DashTime -= Time.fixedDeltaTime;
                if (_hero.CurrentSpeed > _hero.OriginVM)
                    _hero.Rigidbody.AddForce((-_hero.Rigidbody.velocity * Time.fixedDeltaTime) * 1.7f, ForceMode.VelocityChange);
            }
            else
            {
                _hero.DashTime = 0f;
                _hero.SetState<HumanIdleState>(true);
            }
        }
    }
}
