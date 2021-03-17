using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class FillGasState : BaseHumanState
    {
        public override HumanState OldHumanState => HumanState.FillGas;

        public FillGasState(Hero hero) : base(hero) { }

        public override void OnUpdate()
        {
            if (!hero.titanForm && !hero.isCannon)
                if (hero.Animation.IsPlaying(HeroAnim.SUPPLY) && hero.Animation[HeroAnim.SUPPLY].normalizedTime >= 1f)
                {
                    hero.Equipment.Weapon.Resupply();
                    hero.currentBladeSta = hero.totalBladeSta;
                    hero.CurrentGas = hero.totalGas;

                    if (hero.useGun)
                    {
                        hero.leftBulletLeft = hero.rightBulletLeft = hero.BulletMax;
                        hero.rightGunHasBullet = true;
                        hero.leftGunHasBullet = true;
                    }

                    hero.Idle();
                }
        }
    }
}
