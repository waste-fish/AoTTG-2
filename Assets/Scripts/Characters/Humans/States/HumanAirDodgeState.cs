using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanAirDodgeState : BaseHumanState
    {
        public override void OnFixedUpdate()
        {
            if (Hero.DashTime > 0f)
            {
                Hero.DashTime -= Time.fixedDeltaTime;
                if (Hero.CurrentSpeed > Hero.OriginVM)
                    Hero.Rigidbody.AddForce((-Hero.Rigidbody.velocity * Time.fixedDeltaTime) * 1.7f, ForceMode.VelocityChange);
            }
            else
            {
                Hero.DashTime = 0f;
                Hero.SetState<HumanIdleState>(true);
            }
        }
    }
}
