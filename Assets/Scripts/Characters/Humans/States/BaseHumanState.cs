using UnityEngine;

namespace Assets.Scripts.Characters.Humans.States
{
    public abstract class BaseHumanState
    {
        protected Hero _hero;
        protected BaseHumanState _previous;

        public static BaseHumanState NewState<T>(Hero hero, BaseHumanState previous) where T : BaseHumanState, new()
        {
            Debug.Log(previous?.GetType().Name + " -> " + typeof(T).Name);
            return new T { _hero = hero, _previous = previous };
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }

        public virtual void OnAttack() { }
        public virtual void OnAttackRelease() { }
        public virtual void OnSpecialAttack() { }
        public virtual void OnSpecialAttackRelease() { }
        public virtual void OnReload() { }
        public virtual void OnSalute() { }
        public virtual void OnItem1() { }
        public virtual void OnItem2() { }
        public virtual void OnItem3() { }
        public virtual void OnMount() { }
    }
}