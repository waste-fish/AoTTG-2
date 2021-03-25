namespace Assets.Scripts.Characters.Humans.States
{
    public class HumanSlideState : BaseHumanState
    {
        public override void OnUpdate()
        {
            if (!_hero.TitanForm && !_hero.IsCannon)
                if (!_hero.Grounded)
                    _hero.SetState<HumanIdleState>(true);
        }
    }
}
