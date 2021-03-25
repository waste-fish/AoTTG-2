using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public abstract class BaseHumanState
    {
        protected Hero Hero { get; private set; }
        protected BaseHumanState _previous;

        public static BaseHumanState Create<T>(Hero hero, BaseHumanState previous = null) where T : BaseHumanState, new()
        {
            Debug.Log(previous?.GetType().Name + " -> " + typeof(T).Name);
            return new T { Hero = hero, _previous = previous };
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }

        public virtual void OnAttack() { }
        public virtual void OnAttackRelease() { }
        public virtual void OnSkill() { }
        public virtual void OnSkillRelease() { }
        public virtual void OnReload() { }
        public virtual void OnJump() { }
        public virtual void OnSalute() { }
        public virtual void OnItem1() { }
        public virtual void OnItem2() { }
        public virtual void OnItem3() { }
        public virtual void OnMount() { }
        public virtual void OnDodge() { }
        public virtual void OnGas() { }
    }
}