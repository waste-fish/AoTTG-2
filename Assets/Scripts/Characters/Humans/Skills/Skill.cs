using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.Equipment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Characters.Humans.Skills
{
    public abstract class Skill : ScriptableObject
    {
        protected Hero Hero { get; private set; }

        [JsonIgnore] [SerializeField] private UnityEngine.Sprite sprite;
        [JsonIgnore] public UnityEngine.Sprite Sprite => sprite;

        [SerializeField] protected float _cooldown;
        public float MaxCooldown => _cooldown;
        protected float CurrentCooldown { get; private set; }

        [SerializeField] protected bool _canUseGas = true;
        public bool CanUseGas => _canUseGas;

        [SerializeField] protected bool _breaksGrabState;
        public bool BreaksGrabState => _breaksGrabState;

        public List<EquipmentType> CompatibleEquipmentTypes = new List<EquipmentType>();

        [JsonIgnore] [SerializeField] public AnimationClip[] Animations = new AnimationClip[0];
        protected int CurrentAnimation = 0;

        public string CurrentAnimationName => HeroAnim.SKILL_PREFIX + CurrentAnimation;

        public void Initialize(Hero hero)
        {
            CurrentCooldown = _cooldown;
            Hero = hero;
        }
        public bool IsActive { get; protected set; }
        public abstract bool Use();
        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        // Skills seem to check on Hero State:
        // Grabbed: Jean & Eren
        // Idle: Eren, Marco, Armin, Sasha, Mikasa, Levi, Petra

        // Special skill: bomb, which is used for bomb pvp.

        // Some skills check whether or not the player is on the ground
        // None of the skills currently are working for AHSS
        // AHSS skill would be dual shot
    }
}
