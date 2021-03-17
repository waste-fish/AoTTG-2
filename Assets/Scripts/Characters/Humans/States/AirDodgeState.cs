using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public class AirDodgeState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.AirDodge;
        public AirDodgeState(Hero hero) : base(hero) { }

        public override void OnFixedUpdate()
        {
            if (hero.DashTime > 0f)
            {
                hero.DashTime -= Time.fixedDeltaTime;
                if (hero.currentSpeed > hero.OriginVM)
                    hero.Rigidbody.AddForce(((-hero.Rigidbody.velocity * Time.fixedDeltaTime) * 1.7f), ForceMode.VelocityChange);
            }
            else
            {
                hero.DashTime = 0f;
                // State must be set directly, as Idle() will cause the HERO to briefly enter the stand animation mid-air
                hero.SquidState = new IdleState(hero);
            }
        }
    }
}
