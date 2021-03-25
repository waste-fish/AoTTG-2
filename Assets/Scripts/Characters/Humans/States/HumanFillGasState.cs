using Assets.Scripts.Characters.Humans.Constants;

namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanFillGasState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (_hero.Animation.IsPlaying(HeroAnim.SUPPLY) && _hero.Animation[HeroAnim.SUPPLY].normalizedTime >= 1f)
                {
                    _hero.Equipment.Weapon.Resupply();
                    _hero.CurrentBladeSta = _hero.TotalBladeSta;
                    _hero.CurrentGas = _hero.TotalGas;

                    if (_hero.UseGun)
                    {
                        _hero.LeftBulletRemaining = _hero.RightBulletRemaining = _hero.BulletMax;
                        _hero.RightGunHasBullet = true;
                        _hero.LeftGunHasBullet = true;
                    }

                    _hero.SetState<HumanIdleState>();
                }
        }
    }
}
