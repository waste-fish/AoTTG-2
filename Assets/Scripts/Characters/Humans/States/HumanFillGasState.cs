using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanFillGasState : BaseHumanState
    {
        public override bool HasMovementControl => false;

        public override void OnUpdate()
        {
            if (!Hero.TitanForm && !Hero.IsCannon)
                if (Hero.Animation.IsPlaying(HeroAnim.SUPPLY) && Hero.Animation[HeroAnim.SUPPLY].normalizedTime >= 1f)
                {
                    Hero.Equipment.Weapon.Resupply();
                    Hero.CurrentBladeSta = Hero.TotalBladeSta;
                    Hero.CurrentGas = Hero.TotalGas;

                    if (Hero.UseGun)
                    {
                        Hero.LeftBulletRemaining = Hero.RightBulletRemaining = Hero.BulletMax;
                        Hero.RightGunHasBullet = true;
                        Hero.LeftGunHasBullet = true;
                    }

                    Hero.SetState<HumanIdleState>();
                }
        }
    }
}
