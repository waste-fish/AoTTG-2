using System;

namespace Assets.Scripts.Characters.Humans.States
{
    public abstract class BaseHumanState
    {
        [Obsolete]
        public abstract HumanState OldHumanState { get; }
        protected readonly Hero hero;

        protected BaseHumanState(Hero hero)
        {
            this.hero = hero;
        }

        public virtual void OnUpdate()
        {

        }
        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnAttack()
        {

        }
        public virtual void OnAttackRelease()
        {

        }
        public virtual void OnSpecialAttack()
        {

        }
        public virtual void OnSpecialAttackRelease()
        {

        }

        public virtual void OnReload()
        {

        }

        public virtual void OnSalute()
        {

        }

        public virtual void OnItem1()
        {

        }
        public virtual void OnItem2()
        {

        }
        public virtual void OnItem3()
        {

        }

        public virtual void OnMount()
        {

        }
    }
}