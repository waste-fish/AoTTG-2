namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanChangeBladeState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
            {
                _hero.Equipment.Weapon.Reload();
                if (_hero.Animation[_hero.ReloadAnimation].normalizedTime >= 1f)
                    _hero.SetState<HumanIdleState>();
            }
        }
    }
}
