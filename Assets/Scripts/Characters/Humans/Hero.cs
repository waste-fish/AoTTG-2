using Assets.Scripts.Characters.Humans.Constants;
using Assets.Scripts.Characters.Humans.Customization;
using Assets.Scripts.Characters.Humans.Equipment;
using Assets.Scripts.Characters.Humans.Skills;
using Assets.Scripts.Characters.Humans.States;
using Assets.Scripts.Characters.Titan;
using Assets.Scripts.Constants;
using Assets.Scripts.Gamemode.Options;
using Assets.Scripts.Serialization;
using Assets.Scripts.Services;
using Assets.Scripts.Settings;
using Assets.Scripts.UI.InGame.HUD;
using Assets.Scripts.UI.Input;
using Assets.Scripts.Input;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toorah.ScriptableVariables;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Characters.Humans
{
    public class Hero : Human
    {
        #region Events
        public static event Action<Hero> OnSpawnClient;
        public static event Action<Hero> OnDeathClient;
        public static event Action<Hero, Entity> OnKillClient;
        public static event Action<Hero, Entity> OnDealDamageClient;
        public static event Action<Hero> OnUseSkillClient;
        public static event Action<Hero, float> OnUseGasClient;
        public static event Action<Hero> OnLandClient;

        public event Action OnSpawn;
        public event Action OnDeath;
        public event Action<Entity> OnKill;
        public event Action<Entity> OnDealDamage;
        public event Action OnUseSkill;
        public event Action<float> OnUseGas;
        public static event Action OnLand;
        #endregion

        private InputMap inputMap;

        public CharacterPrefabs Prefabs;
        public EquipmentType EquipmentType;

        public const float HOOK_RAYCAST_MAX_DISTANCE = 1000f;

        #region Properties
        public Equipment.Equipment Equipment { get; private set; }
        public Skill Skill { get; private set; }
        public BaseHumanState State { get; private set; }

        private bool AlmostSingleHook { get; set; }
        public string AttackAnimation { get; set; }
        public int AttackLoop { get; set; }
        private GameObject BadGuy { get; set; }
        public bool BladesThrown { get; set; }
        public float BombCD { get; set; }
        public bool BombImmune { get; set; }
        public float BombRadius { get; set; }
        public float BombSpeed { get; set; }
        public float BombTime { get; set; }
        public float BombTimeMax { get; set; }
        public float BuffTime { get; private set; }
        public int BulletMax { get; private set; } = 7;
        public Bullet HookLeft { get; private set; }
        public Bullet HookRight { get; private set; }
        public Dictionary<string, Image> cachedSprites { get; set; }
        public float CameraMultiplier { get; set; }
        public TriggerColliderWeapon checkBoxLeft;
        public TriggerColliderWeapon checkBoxRight;
        public string CurrentAnimation { get; set; }
        public float CurrentBladeSta { get; set; } = 100f;
        public BUFF CurrentBuff { get; private set; }
        public Camera CurrentCamera { get; set; }
        public IN_GAME_MAIN_CAMERA CurrentInGameCamera { get; set; }
        public float CurrentGas { get; set; } = 100f;
        public float CurrentSpeed { get; set; }
        public Vector3 CurrentV { get; set; }
        private bool DashD { get; set; }
        public Vector3 DashDirection { get; set; }
        private bool DashL { get; set; }
        private bool DashR { get; set; }
        public float DashTime { get; set; }
        private bool DashU { get; set; }
        private Vector3 DashV { get; set; }
        public bool Detonate { get; set; }
        private float DTapTime { get; set; } = -1f;
        private bool RightHookHold { get; set; }
        private ErenTitan ErenTitan { get; set; }
        public float FacingDirection { get; set; }
        private Transform ForearmL { get; set; }
        private Transform ForearmR { get; set; }
        private float Gravity { get; set; } = 20f;
        public bool IsGrounded { get; set; }
        public GameObject GunDummy { get; private set; }
        public Vector3 GunTarget { get; set; }
        private Transform HandL { get; set; }
        private Transform HandR { get; set; }
        private bool HasDied { get; set; }
        public bool HasSpawn { get; set; }
        private bool HookedBySomeone { get; set; } = true;
        public GameObject hookRefL1;
        public GameObject hookRefL2;
        public GameObject hookRefR1;
        public GameObject hookRefR2;
        private bool HookSomeone { get; set; }
        private GameObject HookTarget { get; set; }
        private float Invincible { get; set; } = 3f; // Time when you cannot be harmed after spawning
        public bool IsCannon { get; set; }
        private bool IsLaunchLeft { get; set; }
        private bool IsLaunchRight { get; set; }
        public bool IsLeftHandHooked { get; private set; }
        public bool IsMounted { get; private set; }
        public bool IsPhotonCamera { get; set; }
        private bool IsRightHandHooked { get; set; }
        public float JumpHeight { get; set; } = 2f;
        public Transform LastHook { get; set; }
        private float LaunchElapsedTimeL { get; set; }
        private float LaunchElapsedTimeR { get; set; }
        private Vector3 LaunchForce { get; set; }
        public Vector3 LaunchPointLeft { get; private set; }
        public Vector3 LaunchPointRight { get; private set; }
        public bool LeanLeft { get; private set; }
        public bool LeftArmAim { get; set; }
        /*
    public XWeaponTrail leftbladetrail;
    public XWeaponTrail leftbladetrail2;
    */
        [Obsolete("Should be within AHSS.cs")]
        public int LeftBulletRemaining { get; set; } = 7;
        public bool LeftGunHasBullet { get; set; } = true;
        private float RTapTime { get; set; } = -1f;
        private float LTapTime { get; set; } = -1f;
        public GameObject Maincamera { get; set; }
        public float MaxVelocityChange { get; set; } = 10f;
        public AudioSource meatDie;
        public Bomb MyBomb { get; set; }
        public GameObject MyCannon { get; set; }
        public Transform MyCannonBase { get; set; }
        public Transform MyCannonPlayer { get; set; }
        public CannonPropRegion MyCannonRegion { get; set; }
        public Horse Horse { get; private set; }
        [Obsolete("Old method of using player names")]
        public GameObject MyNetWorkName { get; set; }
        public float MyScale { get; set; } = 1f;
        public int MyTeam { get; set; } = 1;
        public List<MindlessTitan> myTitans;
        public bool NeedLean { get; private set; }
        private Quaternion OldHeadRotation { get; set; }
        public float OriginVM { get; private set; }
        private bool LeftHookHold { get; set; }
        public string ReloadAnimation { get; set; } = string.Empty;
        public bool RightArmAim { get; set; }

        [Obsolete("Should be within AHSS.cs")]
        public int RightBulletRemaining { get; set; } = 7;
        public bool RightGunHasBullet { get; set; } = true;
        public AudioSource rope;
        private GameObject SkillCD { get; set; }
        public float SkillCDDuration { get; set; }
        public float SkillCDLast { get; set; }
        public float SkillCDLastCannon { get; set; }
        public AudioSource slash;
        public AudioSource slashHit;

        [Header("Particles")]
        [SerializeField] private ParticleSystem particle_Smoke_3dmg;
        private ParticleSystem.EmissionModule smoke_3dmg_em;
        [SerializeField] private ParticleSystem particle_Sparks;
        public ParticleSystem.EmissionModule SparksEM;

        public float Speed { get; set; } = 10f;
        public GameObject SpeedFX { get; set; }
        public GameObject SpeedFX1 { get; set; }
        public bool Spinning { get; set; }
        public string StandAnimation { get; private set; } = HeroAnim.STAND;
        private Quaternion TargetHeadRotation { get; set; }
        public Quaternion TargetRotation { get; set; }
        public Vector3 TargetV { get; set; }
        public bool TitanForm { get; set; }
        public GameObject TitanWhoGrabMe { get; private set; }
        public int TitanWhoGrabMeID { get; private set; }
        public float TotalBladeSta { get; set; } = 100f;
        public float TotalGas { get; set; } = 100f;
        private Transform UpperarmL { get; set; }
        private Transform UpperarmR { get; set; }
        private float UseGasSpeed { get; set; } = 0.2f;
        public bool UseGun { get; set; }
        private float UTapTime { get; set; } = -1f;
        private bool WallJump { get; set; }
        private float WallRunTime { get; set; }

        public bool IsGrabbed => State is HumanGrabState;
        public bool IsInvincible => (Invincible > 0f);

        #endregion

        // Hero 2.0
        public Animation Animation { get; protected set; }
        public Rigidbody Rigidbody { get; protected set; }
        public SmoothSyncMovement SmoothSync { get; protected set; }

        [SerializeField] private StringVariable bombMainPath;

        public Vector2 TargetMoveDirection { get; private set; }

        #region SetState
        public void SetState<T>(bool skipOnEnter = false) where T : BaseHumanState, new()
        {
            if ((State is HumanAirDodgeState) || (State is HumanGroundDodgeState))
                DashTime = 0f;

            State?.OnExit();
            State = BaseHumanState.Create<T>(this, State);
            if (!skipOnEnter)
                State?.OnEnter();
        }
        #endregion

        #region Input
        private void RegisterInputs()
        {
            inputMap = new InputMap();
            inputMap.Enable();

            inputMap.Human.Move.performed += OnMoveInput;
            inputMap.Human.Move.canceled += OnMoveInput;
            inputMap.Human.Gas.performed += OnGasInput;
            inputMap.Human.Jump.performed += OnJumpInput;
            inputMap.Human.Attack.performed += OnAttackInput;
            inputMap.Human.Attack.performed += OnAttackReleaseInput;
            inputMap.Human.SpecialAttack.performed += OnSkillInput;
            inputMap.Human.SpecialAttack.performed += OnSkillReleaseInput;
            inputMap.Human.Item1.performed += OnItem1Input;
            inputMap.Human.Item2.performed += OnItem2Input;
            inputMap.Human.Item3.performed += OnItem3Input;
            inputMap.UI.Restart.performed += OnRestartInput;
        }
        private void DeregisterInputs()
        {
            if (inputMap == null)
                return;

            inputMap.Disable();

            inputMap.Human.Move.performed -= OnMoveInput;
            inputMap.Human.Move.canceled -= OnMoveInput;
            inputMap.Human.Gas.performed -= OnGasInput;
            inputMap.Human.Jump.performed -= OnJumpInput;
            inputMap.Human.Attack.performed -= OnAttackInput;
            inputMap.Human.SpecialAttack.performed -= OnSkillInput;
            inputMap.Human.Item1.performed -= OnItem1Input;
            inputMap.Human.Item2.performed -= OnItem2Input;
            inputMap.Human.Item3.performed -= OnItem3Input;
            inputMap.UI.Restart.performed -= OnRestartInput;
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            if (IN_GAME_MAIN_CAMERA.isTyping)
                return;

            TargetMoveDirection = context.ReadValue<Vector2>();
        }

        private void RunStateInput(Action stateAction)
        {
            if (HasDied || !photonView.isMine || IN_GAME_MAIN_CAMERA.isTyping || MenuManager.IsAnyMenuOpen)
                return;

            stateAction?.Invoke();
        }

        private void OnGasInput(InputAction.CallbackContext context) => RunStateInput(State.OnGas);

        private void OnJumpInput(InputAction.CallbackContext context) => RunStateInput(State.OnJump);

        private void OnAttackInput(InputAction.CallbackContext context) => RunStateInput(State.OnAttack);
        private void OnAttackReleaseInput(InputAction.CallbackContext context) => RunStateInput(State.OnAttackRelease);

        private void OnSkillInput(InputAction.CallbackContext context) => RunStateInput(State.OnSkill);
        private void OnSkillReleaseInput(InputAction.CallbackContext context) => RunStateInput(State.OnSkillRelease);

        private void OnItem1Input(InputAction.CallbackContext context) => RunStateInput(State.OnItem1);
        private void OnItem2Input(InputAction.CallbackContext context) => RunStateInput(State.OnItem2);
        private void OnItem3Input(InputAction.CallbackContext context) => RunStateInput(State.OnItem3);

        private void OnRestartInput(InputAction.CallbackContext context)
        {
            if (!PhotonNetwork.offlineMode)
                Suicide();
        }
        #endregion

        #region Apply Buff
        public void ApplyBuff(BUFF buff, float time)
        {
            CurrentBuff = buff;
            BuffTime = time;
        }
        #endregion

        #region Initialization
        protected override void Awake()
        {
            base.Awake();

            SetState<HumanIdleState>(true);

            Animation = GetComponent<Animation>();
            Rigidbody = GetComponent<Rigidbody>();
            SmoothSync = GetComponent<SmoothSyncMovement>();

            CurrentCamera = Camera.main;
            Rigidbody.freezeRotation = true;
            HandL = Body.hand_L;
            HandR = Body.hand_R;
            ForearmL = Body.forearm_L;
            ForearmR = Body.forearm_R;
            UpperarmL = Body.upper_arm_L;
            UpperarmR = Body.upper_arm_R;
            Equipment = gameObject.AddComponent<Equipment.Equipment>();
            Faction = Service.Faction.GetHumanity();
            Service.Entity.Register(this);

            CustomAnimationSpeed();
        }

        private void Start()
        {
            gameObject.AddComponent<PlayerInteractable>();
            SetHorse();

            SparksEM = particle_Sparks.emission;
            smoke_3dmg_em = particle_Smoke_3dmg.emission;

            transform.localScale = new Vector3(MyScale, MyScale, MyScale);
            FacingDirection = transform.rotation.eulerAngles.y;
            TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
            smoke_3dmg_em.enabled = false;
            SparksEM.enabled = false;

            if (PhotonNetwork.isMasterClient)
            {
                int iD = photonView.owner.ID;
                if (FengGameManagerMKII.heroHash.ContainsKey(iD))
                {
                    FengGameManagerMKII.heroHash[iD] = this;
                }
                else
                {
                    FengGameManagerMKII.heroHash.Add(iD, this);
                }

                RegisterInputs();
            }

            OnSpawn?.Invoke();

            if (photonView.isMine)
            {
                OnSpawnClient?.Invoke(this);
                SmoothSync.PhotonCamera = true;
                photonView.RPC(nameof(SetMyPhotonCamera), PhotonTargets.OthersBuffered,
                    new object[] { PlayerPrefs.GetFloat("cameraDistance") + 0.3f });
            }

            if (!photonView.isMine)
            {
                gameObject.layer = Layers.NetworkObject.ToLayer();
                if (IN_GAME_MAIN_CAMERA.dayLight == DayLight.Night)
                {
                    GameObject obj3 = Instantiate(Resources.Load<GameObject>("flashlight"));
                    obj3.transform.parent = transform;
                    obj3.transform.position = transform.position + Vector3.up;
                    obj3.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
                }
                Destroy(checkBoxLeft);
                Destroy(checkBoxRight);

                HasSpawn = true;
            }
            else
            {
                CurrentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
                CurrentInGameCamera = CurrentCamera.GetComponent<IN_GAME_MAIN_CAMERA>();

                HasSpawn = true;
                StartCoroutine(ReloadSky());
                BombImmune = false;
                if (GameSettings.PvP.Bomb.Value)
                {
                    BombImmune = true;
                    StartCoroutine(StopImmunity())
                        ;
                }
            }
        }
        #endregion

        public void Update()
        {
            #region Beginning stuff
            // Upon spawning, we cannot be damaged for 3s
            if (Invincible > 0f)
                Invincible -= Time.deltaTime;

            if (HasDied)
                return;

            if (TitanForm && (ErenTitan != null))
            {
                transform.position = ErenTitan.Body.Neck.position;
                SmoothSync.disabled = true;
            }
            else if (IsCannon && (MyCannon != null))
            {
                UpdateCannon();
                SmoothSync.disabled = true;
            }

            if (!photonView.isMine)
                return;

            if (MyCannonRegion != null)
            {
                Service.Ui.SetMessage(LabelPosition.Center, "Press 'Cannon Mount' key to use Cannon.");
                if (InputManager.KeyDown(InputCannon.Mount))
                {
                    MyCannonRegion.photonView.RPC(nameof(CannonPropRegion.RequestControlRPC), PhotonTargets.MasterClient, new object[] { photonView.viewID });
                }
            }
            #endregion

            #region SKILL STUFF 1
            UpdateSkill();
            #endregion

            if (!TitanForm && !IsCannon)
                State.OnUpdate();
        }

        void UpdateSkill()
        {
            if (Skill == null)
                return;

            if (Skill.IsActive)
                Skill.OnUpdate();
        }

        public Vector3 Zero;
        public void FixedUpdate()
        {
            if (!photonView.isMine)
                return;

            ActiveHooks();

            if (!TitanForm && !IsCannon)
            {
                CurrentSpeed = Rigidbody.velocity.magnitude;

                if (!((Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1) || Animation.IsPlaying(HeroAnim.SPECIAL_LEVI)) || Animation.IsPlaying(HeroAnim.SPECIAL_PETRA)))
                    Rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, TargetRotation, Time.deltaTime * 6f);

                UpdateGrounded();

                if (Skill.IsActive)
                    Skill.OnFixedUpdate();

                UpdateHookedSomeone();
                UpdateHookedBySomeone();

                State.OnFixedUpdate();

                var velocity = Rigidbody.velocity;
                var force = Zero - velocity;
                force.x = Mathf.Clamp(force.x, -MaxVelocityChange, MaxVelocityChange);
                force.z = Mathf.Clamp(force.z, -MaxVelocityChange, MaxVelocityChange);
                force.y = 0f;
                if (Animation.IsPlaying(HeroAnim.JUMP) && (Animation[HeroAnim.JUMP].normalizedTime > 0.18f))
                {
                    force.y += 8f;
                }
                if ((Animation.IsPlaying(HeroAnim.HORSE_GET_ON) && (Animation[HeroAnim.HORSE_GET_ON].normalizedTime > 0.18f)) && (Animation[HeroAnim.HORSE_GET_ON].normalizedTime < 1f))
                {
                    force = new Vector3(-Rigidbody.velocity.x, 6f, -Rigidbody.velocity.z);
                    var distance = Vector3.Distance(Horse.transform.position, transform.position);
                    var num9 = (0.6f * Gravity * distance) / 12f;
                    var vector7 = Horse.transform.position - transform.position;
                    force += (num9 * vector7.normalized);
                }
                if (!(State is HumanAttackState) || !UseGun)
                {
                    Rigidbody.AddForce(force, ForceMode.VelocityChange);
                    Rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, FacingDirection, 0f), Time.deltaTime * 10f);
                }
            }
        }

        private void UpdateHookedSomeone()
        {
            if (!HookSomeone)
                return;

            if (HookTarget != null)
            {
                Vector3 vector2 = HookTarget.transform.position - transform.position;
                float magnitude = vector2.magnitude;
                if (magnitude > 2f)
                    Rigidbody.AddForce((((vector2.normalized * Mathf.Pow(magnitude, 0.15f)) * 30f) - (Rigidbody.velocity * 0.95f)), ForceMode.VelocityChange);
            }
            else
                HookSomeone = false;
        }
        private void UpdateHookedBySomeone()
        {
            if (!HookedBySomeone || BadGuy == null)
                return;

            if (BadGuy != null)
            {
                Vector3 vector3 = BadGuy.transform.position - transform.position;
                float f = vector3.magnitude;
                if (f > 5f)
                    Rigidbody.AddForce(((vector3.normalized * Mathf.Pow(f, 0.15f)) * 0.2f), ForceMode.Impulse);
            }
            else
                HookedBySomeone = false;
        }

        private void ActiveHooks()
        {
            var cancel = !(State is HumanIdleState) && (Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_0) || Animation.IsPlaying(HeroAnim.SPECIAL_LEVI) || (Animation.IsPlaying(HeroAnim.SPECIAL_PETRA) || State is HumanGrabState));

            if (InputManager.HumanHookLeft && !cancel)
            {
                if (HookLeft != null)
                    LeftHookHold = true;
                else
                {
                    var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                    LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                    if (Physics.Raycast(ray, out var hitInfo, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                        LaunchLeftRope(hitInfo.distance, hitInfo.point, true);
                    else
                        LaunchLeftRope(HOOK_RAYCAST_MAX_DISTANCE, ray.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), true);

                    rope.Play();
                }
            }
            else
                LeftHookHold = false;


            if (InputManager.HumanHookRight && !cancel)
            {
                if (HookRight != null)
                    RightHookHold = true;
                else
                {
                    var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                    LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                    if (Physics.Raycast(ray, out var hitInfo, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                        LaunchRightRope(hitInfo.distance, hitInfo.point, true);
                    else
                        LaunchRightRope(HOOK_RAYCAST_MAX_DISTANCE, ray.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), true);

                    rope.Play();
                }
            }
            else
                RightHookHold = false;

            if (InputManager.HumanHookBoth && !cancel)
            {
                LeftHookHold = true;
                RightHookHold = true;
                if ((HookLeft == null) && (HookRight == null))
                {
                    var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                    LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                    if (Physics.Raycast(ray, out var hitInfo, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                    {
                        LaunchLeftRope(hitInfo.distance, hitInfo.point, false);
                        LaunchRightRope(hitInfo.distance, hitInfo.point, false);
                    }
                    else
                    {
                        LaunchLeftRope(HOOK_RAYCAST_MAX_DISTANCE, ray.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), false);
                        LaunchRightRope(HOOK_RAYCAST_MAX_DISTANCE, ray.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), false);
                    }

                    rope.Play();
                }
            }
        }
        private void UpdateGrounded()
        {
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
            if (Physics.Raycast(gameObject.transform.position + (Vector3.up * 0.1f), -Vector3.up, (float) 0.3f, mask.value))
            {
                if (!IsGrounded)
                {
                    if (!(State is HumanAttackState))
                    {
                        if (TargetMoveDirection == Vector2.zero && HookLeft == null && HookRight == null && !(State is HumanFillGasState))
                        {
                            SetState<HumanLandState>();
                            CrossFade(HeroAnim.DASH_LAND, 0.01f);
                        }
                        else
                        {
                            if ((Rigidbody.velocity.x * Rigidbody.velocity.x) + (Rigidbody.velocity.z * Rigidbody.velocity.z) > (Speed * Speed * 1.5f)
                                && !(State is HumanFillGasState))
                            {
                                SetState<HumanSlideState>();
                                CrossFade(HeroAnim.SLIDE, 0.05f);
                                FacingDirection = Mathf.Atan2(Rigidbody.velocity.x, Rigidbody.velocity.z) * Mathf.Rad2Deg;
                                TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                                SparksEM.enabled = true;
                            }
                        }
                    }

                    OnLand?.Invoke();
                    if (photonView.isMine)
                        OnLandClient?.Invoke(this);

                    Zero = Rigidbody.velocity;
                }

                IsGrounded = true;
            }
            else
                IsGrounded = false;
        }

        public void SquidLateUpdate()
        {

        }

        #region OldUpdate
        public void OldUpdate()
        {
            #region Beginning stuff
            // Upon spawning, we cannot be damaged for 3s
            if (Invincible > 0f)
            {
                Invincible -= Time.deltaTime;
            }

            if (HasDied)
                return;

            if (TitanForm && (ErenTitan != null))
            {
                transform.position = ErenTitan.Body.Neck.position;
                SmoothSync.disabled = true;
            }
            else if (IsCannon && (MyCannon != null))
            {
                UpdateCannon();
                SmoothSync.disabled = true;
            }

            if (!photonView.isMine)
                return;

            if (MyCannonRegion != null)
            {
                Service.Ui.SetMessage(LabelPosition.Center, "Press 'Cannon Mount' key to use Cannon.");
                if (InputManager.KeyDown(InputCannon.Mount))
                {
                    MyCannonRegion.photonView.RPC(nameof(CannonPropRegion.RequestControlRPC), PhotonTargets.MasterClient, new object[] { photonView.viewID });
                }
            }
            #endregion

            #region SKILL STUFF 1
            // SKILL STUFF 1
            if (Skill != null)
            {
                if (Skill.IsActive)
                {
                    Skill.OnUpdate();
                }
                else if (InputManager.KeyDown(InputHuman.AttackSpecial))
                {
                    if (!Skill.Use() && State is HumanIdleState)
                    {
                        if (NeedLean)
                        {
                            if (LeanLeft)
                            {
                                AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                            }
                            else
                            {
                                AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                            }
                        }
                        else
                        {
                            AttackAnimation = HeroAnim.ATTACK1;
                        }
                        PlayAnimation(AttackAnimation);
                    }
                }
            }
            #endregion

            #region GRAB STUFF 1
            if ((State is HumanGrabState) && !UseGun)
            {
                if (Skill is ErenSkill)
                {
                    if (InputManager.KeyDown(InputHuman.AttackSpecial))
                    {
                        bool flag2 = false;
                        if ((SkillCDDuration > 0f) || flag2)
                        {
                            flag2 = true;
                        }
                        else
                        {
                            SkillCDDuration = SkillCDLast;
                            if (TitanWhoGrabMe.GetComponent<MindlessTitan>() != null)
                            {
                                BreakFreeFromGrab();
                                photonView.RPC(nameof(NetSetIsGrabbedFalse), PhotonTargets.All, new object[0]);
                                if (PhotonNetwork.isMasterClient)
                                {
                                    TitanWhoGrabMe.GetComponent<MindlessTitan>().GrabEscapeRpc();
                                }
                                else
                                {
                                    PhotonView.Find(TitanWhoGrabMeID).photonView.RPC(nameof(MindlessTitan.GrabEscapeRpc), PhotonTargets.MasterClient, new object[0]);
                                }
                                Transform();
                            }
                        }
                    }
                }
            }
            #endregion

            else if (!TitanForm && !IsCannon)
            {
                bool isBothHooksPressed;
                bool isRightHookPressed;
                bool isLeftHookPressed;
                BufferUpdate();
                UpdateExt();
                if (!IsGrounded && !(State is HumanAirDodgeState))
                {
                    if (InputManager.Settings.GasBurstDoubleTap)
                        CheckDashDoubleTap();
                    else
                        CheckDashRebind();

                    if (DashD)
                    {
                        DashD = false;
                        Dash(0f, -1f);
                        return;
                    }

                    if (DashU)
                    {
                        DashU = false;
                        Dash(0f, 1f);
                        return;
                    }

                    if (DashL)
                    {
                        DashL = false;
                        Dash(-1f, 0f);
                        return;
                    }

                    if (DashR)
                    {
                        DashR = false;
                        Dash(1f, 0f);
                        return;
                    }
                }

                #region Idle or Slide while grounded
                if (IsGrounded && ((State is HumanIdleState) || (State is HumanSlideState)))
                {
                    if (!((!InputManager.KeyDown(InputHuman.Jump) || Animation.IsPlaying(HeroAnim.JUMP)) || Animation.IsPlaying(HeroAnim.HORSE_GET_ON)))
                    {
                        SetState<HumanIdleState>();
                        CrossFade(HeroAnim.JUMP, 0.1f);
                        SparksEM.enabled = false;
                    }
                    if (!((!InputManager.KeyDown(InputHorse.Mount) || Animation.IsPlaying(HeroAnim.JUMP)) || Animation.IsPlaying(HeroAnim.HORSE_GET_ON)) && (((Horse != null) && !IsMounted) && (Vector3.Distance(Horse.transform.position, transform.position) < 15f)))
                    {
                        GetOnHorse();
                    }
                    if (!((!InputManager.KeyDown(InputHuman.Dodge) || Animation.IsPlaying(HeroAnim.JUMP)) || Animation.IsPlaying(HeroAnim.HORSE_GET_ON)))
                    {
                        Dodge(false);
                        return;
                    }
                }
                #endregion

                #region Idle
                if (State is HumanIdleState)
                {
                    #region Items
                    if (!MenuManager.IsAnyMenuOpen)
                    {
                        if (InputManager.KeyDown(InputHuman.Item1))
                        {
                            ShootFlare(1);
                        }
                        if (InputManager.KeyDown(InputHuman.Item2))
                        {
                            ShootFlare(2);
                        }
                        if (InputManager.KeyDown(InputHuman.Item3))
                        {
                            ShootFlare(3);
                        }
                    }
                    #endregion

                    #region Restart
                    if (InputManager.KeyDown(InputUi.Restart))
                    {
                        if (!PhotonNetwork.offlineMode)
                        {
                            Suicide();
                        }
                    }
                    #endregion

                    #region Mount
                    if (((Horse != null) && IsMounted) && InputManager.KeyDown(InputHorse.Mount))
                    {
                        GetOffHorse();
                    }
                    #endregion

                    #region Reload
                    if (((Animation.IsPlaying(StandAnimation) || !IsGrounded) && InputManager.KeyDown(InputHuman.Reload)) && ((!UseGun || (GameSettings.PvP.AhssAirReload.Value)) || IsGrounded))
                    {
                        Reload();
                        return;
                    }
                    #endregion

                    #region Salute
                    if (Animation.IsPlaying(StandAnimation) && InputManager.KeyDown(InputHuman.Salute))
                    {
                        SetState<HumanSaluteState>();
                        return;
                    }
                    #endregion

                    if ((!IsMounted && (InputManager.KeyDown(InputHuman.Attack) || InputManager.KeyDown(InputHuman.AttackSpecial))) && !UseGun)
                    {
                        bool flag3 = false;
                        #region Special
                        if (InputManager.KeyDown(InputHuman.AttackSpecial))
                        {
                            if ((SkillCDDuration > 0f) || flag3)
                            {
                                flag3 = true;
                            }
                            else
                            {
                                SkillCDDuration = SkillCDLast;
                                //TODO: Eren Skill
                                if (Skill is ErenSkill)
                                {
                                    Transform();
                                    return;
                                }
                                //TODO: Marco Skill
                                if (Skill is MarcoSkill)
                                {
                                    if (IsGrounded)
                                    {
                                        AttackAnimation = (UnityEngine.Random.Range(0, 2) != 0) ? HeroAnim.SPECIAL_MARCO_1 : HeroAnim.SPECIAL_MARCO_0;
                                        PlayAnimation(AttackAnimation);
                                    }
                                    else
                                    {
                                        flag3 = true;
                                        SkillCDDuration = 0f;
                                    }
                                }
                                //TODO: Armin Skill
                                else if (Skill is ArminSkill)
                                {
                                    if (IsGrounded)
                                    {
                                        AttackAnimation = HeroAnim.SPECIAL_ARMIN;
                                        PlayAnimation(HeroAnim.SPECIAL_ARMIN);
                                    }
                                    else
                                    {
                                        flag3 = true;
                                        SkillCDDuration = 0f;
                                    }
                                }
                                //TODO: Sasha Skill
                                else if (Skill is SashaSkill)
                                {
                                    if (IsGrounded)
                                    {
                                        AttackAnimation = HeroAnim.SPECIAL_SASHA;
                                        PlayAnimation(HeroAnim.SPECIAL_SASHA);
                                        CurrentBuff = BUFF.SpeedUp;
                                        BuffTime = 10f;
                                    }
                                    else
                                    {
                                        flag3 = true;
                                        SkillCDDuration = 0f;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Attack
                        else if (InputManager.KeyDown(InputHuman.Attack))
                        {
                            if (NeedLean)
                            {
                                if (InputManager.Key(InputHuman.Left))
                                {
                                    AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                                }
                                else if (InputManager.Key(InputHuman.Right))
                                {
                                    AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                                }
                                else if (LeanLeft)
                                {
                                    AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_L1 : HeroAnim.ATTACK1_HOOK_L2;
                                }
                                else
                                {
                                    AttackAnimation = (UnityEngine.Random.Range(0, 100) >= 50) ? HeroAnim.ATTACK1_HOOK_R1 : HeroAnim.ATTACK1_HOOK_R2;
                                }
                            }
                            else if (InputManager.Key(InputHuman.Left))
                            {
                                AttackAnimation = HeroAnim.ATTACK2;
                            }
                            else if (InputManager.Key(InputHuman.Right))
                            {
                                AttackAnimation = HeroAnim.ATTACK1;
                            }
                            else if (LastHook != null && LastHook.TryGetComponent<TitanBase>(out var titan))
                            {
                                if (titan.Body.Neck != null)
                                {
                                    AttackAccordingToTarget(titan.Body.Neck);
                                }
                                else
                                {
                                    flag3 = true;
                                }
                            }
                            else if ((HookLeft != null) && (HookLeft.transform.parent != null))
                            {
                                Transform a = HookLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (a != null)
                                {
                                    AttackAccordingToTarget(a);
                                }
                                else
                                {
                                    AttackAccordingToMouse();
                                }
                            }
                            else if ((HookRight != null) && (HookRight.transform.parent != null))
                            {
                                Transform transform2 = HookRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (transform2 != null)
                                {
                                    AttackAccordingToTarget(transform2);
                                }
                                else
                                {
                                    AttackAccordingToMouse();
                                }
                            }
                            else
                            {
                                GameObject obj2 = FindNearestTitan();
                                if (obj2 != null)
                                {
                                    Transform transform3 = obj2.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                    if (transform3 != null)
                                    {
                                        AttackAccordingToTarget(transform3);
                                    }
                                    else
                                    {
                                        AttackAccordingToMouse();
                                    }
                                }
                                else
                                {
                                    AttackAccordingToMouse();
                                }
                            }
                        }
                        #endregion
                        #region flag3
                        if (!flag3)
                        {
                            checkBoxLeft.ClearHits();
                            checkBoxRight.ClearHits();
                            if (IsGrounded)
                            {
                                Rigidbody.AddForce((gameObject.transform.forward * 200f));
                            }
                            PlayAnimation(AttackAnimation);
                            Animation[AttackAnimation].time = 0f;
                            SetState<HumanAttackState>();
                            //if ((IsGrounded || (AttackAnimation == HeroAnim.SPECIAL_MIKASA_0)) || ((AttackAnimation == HeroAnim.SPECIAL_LEVI) || (AttackAnimation == HeroAnim.SPECIAL_PETRA)))
                            //{
                            //    AttackReleased = true;
                            //}
                            //else
                            //{
                            //    AttackReleased = false;
                            //}
                            SparksEM.enabled = false;
                        }
                        #endregion
                    }
                    if (UseGun)
                    {
                        #region Gun Special Attack
                        if (InputManager.Key(InputHuman.AttackSpecial))
                        {
                            LeftArmAim = true;
                            RightArmAim = true;
                        }
                        #endregion
                        #region Gun Attack
                        else if (InputManager.Key(InputHuman.Attack))
                        {
                            if (LeftGunHasBullet)
                            {
                                LeftArmAim = true;
                                RightArmAim = false;
                            }
                            else
                            {
                                LeftArmAim = false;
                                if (RightGunHasBullet)
                                {
                                    RightArmAim = true;
                                }
                                else
                                {
                                    RightArmAim = false;
                                }
                            }
                        }
                        #endregion
                        #region ???
                        else
                        {
                            LeftArmAim = false;
                            RightArmAim = false;
                        }
                        #endregion

                        #region Gun
                        if (LeftArmAim || RightArmAim)
                        {
                            RaycastHit hit3;
                            Ray ray3 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
                            if (Physics.Raycast(ray3, out hit3, 1E+07f, mask.value))
                            {
                                GunTarget = hit3.point;
                            }
                        }
                        #endregion
                        #region flag4-6
                        bool flag4 = false;
                        bool flag5 = false;
                        bool flag6 = false;
                        #endregion
                        //TODO: AHSS skill dual shot
                        #region Attack Special Up
                        if (InputManager.KeyUp(InputHuman.AttackSpecial) && (!(Skill is BombPvpSkill)))
                        {
                            if (LeftGunHasBullet && RightGunHasBullet)
                            {
                                if (IsGrounded)
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH;
                                }
                                else
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_BOTH_AIR;
                                }
                                flag4 = true;
                            }
                            else if (!(LeftGunHasBullet || RightGunHasBullet))
                            {
                                flag5 = true;
                            }
                            else
                            {
                                flag6 = true;
                            }
                        }
                        #endregion
                        #region Attack Up
                        if (flag6 || InputManager.KeyUp(InputHuman.Attack))
                        {
                            if (IsGrounded)
                            {
                                if (LeftGunHasBullet && RightGunHasBullet)
                                {
                                    if (IsLeftHandHooked)
                                    {
                                        AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                                    }
                                    else
                                    {
                                        AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                                    }
                                }
                                else if (LeftGunHasBullet)
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_L;
                                }
                                else if (RightGunHasBullet)
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_R;
                                }
                            }
                            else if (LeftGunHasBullet && RightGunHasBullet)
                            {
                                if (IsLeftHandHooked)
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;
                                }
                                else
                                {
                                    AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                                }
                            }
                            else if (LeftGunHasBullet)
                            {
                                AttackAnimation = HeroAnim.AHSS_SHOOT_L_AIR;
                            }
                            else if (RightGunHasBullet)
                            {
                                AttackAnimation = HeroAnim.AHSS_SHOOT_R_AIR;
                            }
                            if (LeftGunHasBullet || RightGunHasBullet)
                            {
                                flag4 = true;
                            }
                            else
                            {
                                flag5 = true;
                            }
                        }
                        #endregion
                        #region flag4
                        if (flag4)
                        {
                            SetState<HumanAttackState>();
                            CrossFade(AttackAnimation, 0.05f);
                            GunDummy.transform.position = transform.position;
                            GunDummy.transform.rotation = transform.rotation;
                            GunDummy.transform.LookAt(GunTarget);
                            //AttackReleased = false;
                            FacingDirection = GunDummy.transform.rotation.eulerAngles.y;
                            TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                        }
                        else if (flag5 && (IsGrounded || (GameSettings.PvP.AhssAirReload.Value)))
                        {
                            Reload();
                        }
                        #endregion
                    }
                }
                #endregion
                #region Attack
                else if (State is HumanAttackState)
                {
                    if (!UseGun)
                    {
                        #region Holding Attack
                        //if (!AttackReleased)
                        //{
                        //    //TODO: Pause the Animation if the player is holding a button
                        //    if (!InputManager.HumanAttack)
                        //    {
                        //        SetAnimationSpeed(CurrentAnimation);
                        //        AttackReleased = true;
                        //    }
                        //    else if (Animation[AttackAnimation].normalizedTime >= 0.32f && Animation[AttackAnimation].speed > 0f)
                        //    {
                        //        Debug.Log("Trying to freeze");
                        //        SetAnimationSpeed(AttackAnimation, 0f);
                        //    }
                        //}
                        #endregion
                        #region Attack Animations
                        if ((AttackAnimation == HeroAnim.SPECIAL_MIKASA_0) && (CurrentBladeSta > 0f))
                        {
                            if (Animation[AttackAnimation].normalizedTime >= 0.8f)
                            {
                                if (!checkBoxLeft.IsActive)
                                {
                                    checkBoxLeft.IsActive = true;
                                    if (((int) FengGameManagerMKII.settings[0x5c]) == 0)
                                    {
                                        /*
                                                leftbladetrail2.Activate();
                                                rightbladetrail2.Activate();
                                                leftbladetrail.Activate();
                                                rightbladetrail.Activate();
                                                */
                                    }
                                    Rigidbody.velocity = (-Vector3.up * 30f);
                                }
                                if (!checkBoxRight.IsActive)
                                {
                                    checkBoxRight.IsActive = true;
                                    slash.Play();
                                }
                            }
                            else if (checkBoxLeft.IsActive)
                            {
                                checkBoxLeft.IsActive = false;
                                checkBoxRight.IsActive = false;
                                checkBoxLeft.ClearHits();
                                checkBoxRight.ClearHits();
                                /*
                                        leftbladetrail.StopSmoothly(0.1f);
                                        rightbladetrail.StopSmoothly(0.1f);
                                        leftbladetrail2.StopSmoothly(0.1f);
                                        rightbladetrail2.StopSmoothly(0.1f);
                                        */
                            }
                        }
                        else
                        {
                            float num;
                            float num2;
                            if (CurrentBladeSta == 0f)
                            {
                                num = -1f;
                                num2 = -1f;
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_LEVI)
                            {
                                num2 = 0.35f;
                                num = 0.5f;
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_PETRA)
                            {
                                num2 = 0.35f;
                                num = 0.48f;
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_ARMIN)
                            {
                                num2 = 0.25f;
                                num = 0.35f;
                            }
                            else if (AttackAnimation == HeroAnim.ATTACK4)
                            {
                                num2 = 0.6f;
                                num = 0.9f;
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_SASHA)
                            {
                                num = -1f;
                                num2 = -1f;
                            }
                            else
                            {
                                num2 = 0.5f;
                                num = 0.85f;
                            }
                            if ((Animation[AttackAnimation].normalizedTime > num2) && (Animation[AttackAnimation].normalizedTime < num))
                            {
                                if (!checkBoxLeft.IsActive)
                                {
                                    checkBoxLeft.IsActive = true;
                                    slash.Play();
                                    if (((int) FengGameManagerMKII.settings[0x5c]) == 0)
                                    {
                                        //leftbladetrail2.Activate();
                                        //rightbladetrail2.Activate();
                                        //leftbladetrail.Activate();
                                        //rightbladetrail.Activate();
                                    }
                                }
                                if (!checkBoxRight.IsActive)
                                {
                                    checkBoxRight.IsActive = true;
                                }
                            }
                            else if (checkBoxLeft.IsActive)
                            {
                                checkBoxLeft.IsActive = false;
                                checkBoxRight.IsActive = false;
                                checkBoxLeft.ClearHits();
                                checkBoxRight.ClearHits();
                                //leftbladetrail2.StopSmoothly(0.1f);
                                //rightbladetrail2.StopSmoothly(0.1f);
                                //leftbladetrail.StopSmoothly(0.1f);
                                //rightbladetrail.StopSmoothly(0.1f);
                            }
                            if ((AttackLoop > 0) && (Animation[AttackAnimation].normalizedTime > num))
                            {
                                AttackLoop--;
                                PlayAnimationAt(AttackAnimation, num2);
                            }
                        }
                        #endregion
                        #region Marco Anim
                        if (Animation[AttackAnimation].normalizedTime >= 1f)
                        {
                            if ((AttackAnimation == HeroAnim.SPECIAL_MARCO_0) || (AttackAnimation == HeroAnim.SPECIAL_MARCO_1))
                            {
                                if (!PhotonNetwork.isMasterClient)
                                {
                                    object[] parameters = new object[] { 5f, 100f };
                                    photonView.RPC(nameof(NetTauntAttack), PhotonTargets.MasterClient, parameters);
                                }
                                else
                                {
                                    NetTauntAttack(5f, 100f);
                                }
                                FalseAttack();
                                SetState<HumanIdleState>();
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_ARMIN)
                            {
                                if (!PhotonNetwork.isMasterClient)
                                {
                                    photonView.RPC(nameof(NetlaughAttack), PhotonTargets.MasterClient, new object[0]);
                                }
                                else
                                {
                                    NetlaughAttack();
                                }
                                FalseAttack();
                                SetState<HumanIdleState>();
                            }
                            else if (AttackAnimation == HeroAnim.SPECIAL_MIKASA_0)
                            {
                                Rigidbody.velocity -= ((Vector3.up * Time.deltaTime) * 30f);
                            }
                            else
                            {
                                FalseAttack();
                                SetState<HumanIdleState>();
                            }
                        }
                        #endregion
                        if (Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1) && (Animation[HeroAnim.SPECIAL_MIKASA_1].normalizedTime >= 1f))
                        {
                            FalseAttack();
                            SetState<HumanIdleState>();
                        }
                    }
                    #region Gun
                    else
                    {
                        checkBoxLeft.IsActive = false;
                        checkBoxRight.IsActive = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, GunDummy.transform.rotation, Time.deltaTime * 30f);
                        if (/*!AttackReleased &&*/ (Animation[AttackAnimation].normalizedTime > 0.167f))
                        {
                            GameObject obj4;
                            //AttackReleased = true;
                            bool flag7 = false;
                            if ((AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH) || (AttackAnimation == HeroAnim.AHSS_SHOOT_BOTH_AIR))
                            {
                                //Should use AHSSShotgunCollider instead of TriggerColliderWeapon.  
                                //Apply that change when abstracting weapons from this class.
                                //Note, when doing the abstraction, the relationship between the weapon collider and the abstracted weapon class should be carefully considered.
                                checkBoxLeft.IsActive = true;
                                checkBoxRight.IsActive = true;
                                flag7 = true;
                                LeftGunHasBullet = false;
                                RightGunHasBullet = false;
                                Rigidbody.AddForce((-transform.forward * 1000f), ForceMode.Acceleration);
                            }
                            else
                            {
                                if ((AttackAnimation == HeroAnim.AHSS_SHOOT_L) || (AttackAnimation == HeroAnim.AHSS_SHOOT_L_AIR))
                                {
                                    checkBoxLeft.IsActive = true;
                                    LeftGunHasBullet = false;
                                }
                                else
                                {
                                    checkBoxRight.IsActive = true;
                                    RightGunHasBullet = false;
                                }
                                Rigidbody.AddForce((-transform.forward * 600f), ForceMode.Acceleration);
                            }
                            Rigidbody.AddForce((Vector3.up * 200f), ForceMode.Acceleration);
                            string prefabName = "FX/shotGun";
                            if (flag7)
                            {
                                prefabName = "FX/shotGun 1";
                            }
                            if (photonView.isMine)
                            {
                                obj4 = PhotonNetwork.Instantiate(prefabName, ((transform.position + (transform.up * 0.8f)) - (transform.right * 0.1f)), transform.rotation, 0);
                                if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                                {
                                    obj4.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = photonView.viewID;
                                }
                            }
                            else
                            {
                                obj4 = Instantiate(Resources.Load<GameObject>(prefabName), ((transform.position + (transform.up * 0.8f)) - (transform.right * 0.1f)), transform.rotation);
                            }
                        }
                        if (Animation[AttackAnimation].normalizedTime >= 1f)
                        {
                            FalseAttack();
                            SetState<HumanIdleState>();
                            checkBoxLeft.IsActive = false;
                            checkBoxRight.IsActive = false;
                        }
                        if (!Animation.IsPlaying(AttackAnimation))
                        {
                            FalseAttack();
                            SetState<HumanIdleState>();
                            checkBoxLeft.IsActive = false;
                            checkBoxRight.IsActive = false;
                        }
                    }
                    #endregion
                }
                #endregion
                #region ChangeBlade
                else if (State is HumanChangeBladeState)
                {
                    Equipment.Weapon.Reload();
                    if (Animation[ReloadAnimation].normalizedTime >= 1f)
                        SetState<HumanIdleState>();
                }
                #endregion
                #region Salute
                else if (State is HumanSaluteState)
                {
                    if (Animation[HeroAnim.SALUTE].normalizedTime >= 1f)
                        SetState<HumanIdleState>();
                }
                #endregion
                #region GroundDodge
                else if (State is HumanGroundDodgeState)
                {
                    if (Animation.IsPlaying(HeroAnim.DODGE))
                    {
                        if (!(IsGrounded || (Animation[HeroAnim.DODGE].normalizedTime <= 0.6f)))
                            SetState<HumanIdleState>();

                        if (Animation[HeroAnim.DODGE].normalizedTime >= 1f)
                            SetState<HumanIdleState>();
                    }
                }
                #endregion
                #region Land
                else if (State is HumanLandState)
                {
                    if (Animation.IsPlaying(HeroAnim.DASH_LAND) && (Animation[HeroAnim.DASH_LAND].normalizedTime >= 1f))
                        SetState<HumanIdleState>();
                }
                #endregion
                #region FillGas
                else if (State is HumanFillGasState)
                {
                    if (Animation.IsPlaying(HeroAnim.SUPPLY) && Animation[HeroAnim.SUPPLY].normalizedTime >= 1f)
                    {
                        Equipment.Weapon.Resupply();
                        CurrentBladeSta = TotalBladeSta;
                        CurrentGas = TotalGas;
                        if (UseGun)
                        {
                            LeftBulletRemaining = RightBulletRemaining = BulletMax;
                            RightGunHasBullet = true;
                            LeftGunHasBullet = true;
                        }

                        SetState<HumanIdleState>();
                    }
                }
                #endregion
                #region Slide
                else if (State is HumanSlideState)
                {
                    if (!IsGrounded)
                        SetState<HumanIdleState>();
                }
                #endregion
                #region AirDodge
                else if (State is HumanAirDodgeState)
                {
                    if (DashTime > 0f)
                    {
                        DashTime -= Time.deltaTime;
                        if (CurrentSpeed > OriginVM)
                            Rigidbody.AddForce(((-Rigidbody.velocity * Time.deltaTime) * 1.7f), ForceMode.VelocityChange);
                    }
                    else
                    {
                        DashTime = 0f;
                        // State must be set directly, as Idle() will cause the HERO to briefly enter the stand animation mid-air
                        SetState<HumanIdleState>(true);
                    }
                }
                #endregion

                #region dumb stuff
                if (InputManager.Key(InputHuman.HookLeft))
                    isLeftHookPressed = true;
                else
                    isLeftHookPressed = false;
                #endregion

                //TODO: Properly refactor these if statements

                // Attack 3_1 = Mikasa part 1
                // Attack 3_2 = Mikasa part 2
                // Attack 5 = Levi spin
                // special_petra = Petra skill

                // If leftHookPressed
                // (Using HeroAnim.ATTACK3_1 OR Attack5 OR Petra OR Grabbed) AND NOT IDLE
                // 

                #region Other Junk
                if (!(isLeftHookPressed ? (((Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_0) || Animation.IsPlaying(HeroAnim.SPECIAL_LEVI)) || (Animation.IsPlaying(HeroAnim.SPECIAL_PETRA) || (State is HumanGrabState))) ? !(State is HumanIdleState) : false) : true))

                {
                    if (HookLeft != null)
                    {
                        LeftHookHold = true;
                    }
                    else
                    {
                        RaycastHit hit4;
                        Ray ray4 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                        LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                        if (Physics.Raycast(ray4, out hit4, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                        {
                            LaunchLeftRope(hit4.distance, hit4.point, true);
                        }
                        else
                        {
                            LaunchLeftRope(HOOK_RAYCAST_MAX_DISTANCE, ray4.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), true);
                        }
                        rope.Play();
                    }
                }
                else
                {
                    LeftHookHold = false;
                }
                if (InputManager.Key(InputHuman.HookRight))
                {
                    isRightHookPressed = true;
                }
                else
                {
                    isRightHookPressed = false;
                }
                if (!(isRightHookPressed ? (((Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_0) || Animation.IsPlaying(HeroAnim.SPECIAL_LEVI)) || (Animation.IsPlaying(HeroAnim.SPECIAL_PETRA) || (State is HumanGrabState))) ? !(State is HumanIdleState) : false) : true))
                {
                    if (HookRight != null)
                    {
                        RightHookHold = true;
                    }
                    else
                    {
                        RaycastHit hit5;
                        Ray ray5 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                        LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                        if (Physics.Raycast(ray5, out hit5, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                        {
                            LaunchRightRope(hit5.distance, hit5.point, true);
                        }
                        else
                        {
                            LaunchRightRope(HOOK_RAYCAST_MAX_DISTANCE, ray5.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), true);
                        }
                        rope.Play();
                    }
                }
                else
                {
                    RightHookHold = false;
                }
                if (InputManager.Key(InputHuman.HookBoth))
                {
                    isBothHooksPressed = true;
                }
                else
                {
                    isBothHooksPressed = false;
                }
                if (!(isBothHooksPressed ? (((Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_0) || Animation.IsPlaying(HeroAnim.SPECIAL_LEVI)) || (Animation.IsPlaying(HeroAnim.SPECIAL_PETRA) || (State is HumanGrabState))) ? !(State is HumanIdleState) : false) : true))
                {
                    LeftHookHold = true;
                    RightHookHold = true;
                    if ((HookLeft == null) && (HookRight == null))
                    {
                        RaycastHit hit6;
                        Ray ray6 = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                        LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                        if (Physics.Raycast(ray6, out hit6, HOOK_RAYCAST_MAX_DISTANCE, mask.value))
                        {
                            LaunchLeftRope(hit6.distance, hit6.point, false);
                            LaunchRightRope(hit6.distance, hit6.point, false);
                        }
                        else
                        {
                            LaunchLeftRope(HOOK_RAYCAST_MAX_DISTANCE, ray6.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), false);
                            LaunchRightRope(HOOK_RAYCAST_MAX_DISTANCE, ray6.GetPoint(HOOK_RAYCAST_MAX_DISTANCE), false);
                        }
                        rope.Play();
                    }
                }
                #endregion
            }
        }
        #endregion

        #region OldLateUpdate
        public void LateUpdate()
        {
            #region Network? TODO
            if ((MyNetWorkName != null))
            {
                if (TitanForm && (ErenTitan != null))
                {
                    MyNetWorkName.transform.localPosition = ((Vector3.up * Screen.height) * 2f);
                }
                Vector3 start = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);

                LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                if ((Vector3.Angle(Maincamera.transform.forward, start - Maincamera.transform.position) > 90f) || Physics.Linecast(start, Maincamera.transform.position, mask))
                {
                    MyNetWorkName.transform.localPosition = ((Vector3.up * Screen.height) * 2f);
                }
                else
                {
                    Vector2 vector2 = Maincamera.GetComponent<Camera>().WorldToScreenPoint(start);
                    MyNetWorkName.transform.localPosition = new Vector3((float) ((int) (vector2.x - (Screen.width * 0.5f))), (float) ((int) (vector2.y - (Screen.height * 0.5f))), 0f);
                }
            }
            #endregion
            if (!TitanForm && !IsCannon)
            {
                if (InputManager.Settings.CameraTilt && (photonView.isMine))
                {
                    Quaternion quaternion2;
                    Vector3 zero = Vector3.zero;
                    Vector3 position = Vector3.zero;
                    if ((IsLaunchLeft && (HookLeft != null)) && HookLeft.isHooked())
                    {
                        zero = HookLeft.transform.position;
                    }
                    if ((IsLaunchRight && (HookRight != null)) && HookRight.isHooked())
                    {
                        position = HookRight.transform.position;
                    }
                    Vector3 vector5 = Vector3.zero;
                    if ((zero.magnitude != 0f) && (position.magnitude == 0f))
                    {
                        vector5 = zero;
                    }
                    else if ((zero.magnitude == 0f) && (position.magnitude != 0f))
                    {
                        vector5 = position;
                    }
                    else if ((zero.magnitude != 0f) && (position.magnitude != 0f))
                    {
                        vector5 = ((zero + position) * 0.5f);
                    }
                    Vector3 from = Vector3.Project(vector5 - transform.position, Maincamera.transform.up);
                    Vector3 vector7 = Vector3.Project(vector5 - transform.position, Maincamera.transform.right);
                    if (vector5.magnitude > 0f)
                    {
                        Vector3 to = from + vector7;
                        float num = Vector3.Angle(vector5 - transform.position, Rigidbody.velocity) * 0.005f;
                        Vector3 vector9 = Maincamera.transform.right + vector7.normalized;
                        quaternion2 = Quaternion.Euler(Maincamera.transform.rotation.eulerAngles.x, Maincamera.transform.rotation.eulerAngles.y, (vector9.magnitude >= 1f) ? (-Vector3.Angle(from, to) * num) : (Vector3.Angle(from, to) * num));
                    }
                    else
                    {
                        quaternion2 = Quaternion.Euler(Maincamera.transform.rotation.eulerAngles.x, Maincamera.transform.rotation.eulerAngles.y, 0f);
                    }
                    Maincamera.transform.rotation = Quaternion.Lerp(Maincamera.transform.rotation, quaternion2, Time.deltaTime * 2f);
                }
                if ((State is HumanGrabState) && (TitanWhoGrabMe != null))
                {
                    if (TitanWhoGrabMe.TryGetComponent<MindlessTitan>(out var mindlessTitan))
                    {
                        transform.position = mindlessTitan.grabTF.transform.position;
                        transform.rotation = mindlessTitan.grabTF.transform.rotation;
                    }
                    else if (TitanWhoGrabMe.TryGetComponent<FemaleTitan>(out var femaleTitan))
                    {
                        transform.position = femaleTitan.grabTF.transform.position;
                        transform.rotation = femaleTitan.grabTF.transform.rotation;
                    }
                }
                if (UseGun)
                {
                    if (LeftArmAim || RightArmAim)
                    {
                        Vector3 vector10 = GunTarget - transform.position;
                        float current = -Mathf.Atan2(vector10.z, vector10.x) * Mathf.Rad2Deg;
                        float num3 = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
                        HeadMovement();
                        if ((!IsLeftHandHooked && LeftArmAim) && ((num3 < 40f) && (num3 > -90f)))
                        {
                            LeftArmAimTo(GunTarget);
                        }
                        if ((!IsRightHandHooked && RightArmAim) && ((num3 > -40f) && (num3 < 90f)))
                        {
                            RightArmAimTo(GunTarget);
                        }
                    }
                    else if (!IsGrounded)
                    {
                        HandL.localRotation = Quaternion.Euler(90f, 0f, 0f);
                        HandR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                    }
                    if (IsLeftHandHooked && (HookLeft != null))
                    {
                        LeftArmAimTo(HookLeft.transform.position);
                    }
                    if (IsRightHandHooked && (HookRight != null))
                    {
                        RightArmAimTo(HookRight.transform.position);
                    }
                }
                SetHookedPplDirection();
                BodyLean();
            }
        }
        #endregion

        #region OldFixedUpdate
        private void OldFixedUpdate()
        {
            if (!photonView.isMine) return;
            if ((!TitanForm && !IsCannon) && (!IN_GAME_MAIN_CAMERA.isPausing))
            {
                #region CurrentSpeed
                CurrentSpeed = Rigidbody.velocity.magnitude;
                #endregion
                #region Grab
                if (State is HumanGrabState)
                {
                    Rigidbody.AddForce(-Rigidbody.velocity, ForceMode.VelocityChange);
                }
                #endregion
                else
                {
                    #region Grounded
                    LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();
                    if (Physics.Raycast(gameObject.transform.position + ((Vector3.up * 0.1f)), -Vector3.up, (float) 0.3f, mask.value))
                    {
                        if (!IsGrounded)
                        {
                            OnLand?.Invoke();
                        }
                        IsGrounded = true;
                    }
                    else
                    {
                        IsGrounded = false;
                    }
                    #endregion

                    #region Skill
                    if (Skill.IsActive)
                    {
                        Skill.OnFixedUpdate();
                    }
                    #endregion

                    #region HookSomeone/BySomeone
                    if (HookSomeone)
                    {
                        if (HookTarget != null)
                        {
                            Vector3 vector2 = HookTarget.transform.position - transform.position;
                            float magnitude = vector2.magnitude;
                            if (magnitude > 2f)
                            {
                                Rigidbody.AddForce((((vector2.normalized * Mathf.Pow(magnitude, 0.15f)) * 30f) - (Rigidbody.velocity * 0.95f)), ForceMode.VelocityChange);
                            }
                        }
                        else
                        {
                            HookSomeone = false;
                        }
                    }
                    else if (HookedBySomeone && (BadGuy != null))
                    {
                        if (BadGuy != null)
                        {
                            Vector3 vector3 = BadGuy.transform.position - transform.position;
                            float f = vector3.magnitude;
                            if (f > 5f)
                            {
                                Rigidbody.AddForce(((vector3.normalized * Mathf.Pow(f, 0.15f)) * 0.2f), ForceMode.Impulse);
                            }
                        }
                        else
                        {
                            HookedBySomeone = false;
                        }
                    }
                    #endregion

                    #region Read Direction Keys
                    float x = 0f;
                    float z = 0f;
                    if (!IN_GAME_MAIN_CAMERA.isTyping)
                    {
                        if (InputManager.Key(InputHuman.Forward))
                        {
                            z = 1f;
                        }
                        else if (InputManager.Key(InputHuman.Backward))
                        {
                            z = -1f;
                        }
                        else
                        {
                            z = 0f;
                        }
                        if (InputManager.Key(InputHuman.Left))
                        {
                            x = -1f;
                        }
                        else if (InputManager.Key(InputHuman.Right))
                        {
                            x = 1f;
                        }
                        else
                        {
                            x = 0f;
                        }
                    }
                    #endregion

                    if (IsGrounded)
                    {
                        Vector3 vector7;
                        Vector3 zero = Vector3.zero;
                        #region Attack
                        if (State is HumanAttackState)
                        {
                            if (AttackAnimation == HeroAnim.SPECIAL_LEVI)
                            {
                                if ((Animation[AttackAnimation].normalizedTime > 0.4f) && (Animation[AttackAnimation].normalizedTime < 0.61f))
                                {
                                    Rigidbody.AddForce((gameObject.transform.forward * 200f));
                                }
                            }
                            else if (Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1))
                            {
                                zero = Vector3.zero;
                            }
                            else if (Animation.IsPlaying(HeroAnim.ATTACK1) || Animation.IsPlaying(HeroAnim.ATTACK2))
                            {
                                Rigidbody.AddForce((gameObject.transform.forward * 200f));
                            }
                            if (Animation.IsPlaying(HeroAnim.SPECIAL_MIKASA_1))
                            {
                                zero = Vector3.zero;
                            }
                        }
                        #endregion
                        #region JustGrounded
                        //if (JustGrounded)
                        //{
                        //    //TODO: AttackAnimation conditions appear to be useless
                        //    if (!(State is HumanAttackState) || (((AttackAnimation != HeroAnim.SPECIAL_MIKASA_0) && (AttackAnimation != HeroAnim.SPECIAL_LEVI)) && (AttackAnimation != HeroAnim.SPECIAL_PETRA)))
                        //    {
                        //        if (((!(State is HumanAttackState) && (x == 0f)) && ((z == 0f) && (HookLeft == null))) && ((HookRight == null) && !(State is HumanFillGasState)))
                        //        {
                        //            SetState<HumanLandState>();
                        //            CrossFade(HeroAnim.DASH_LAND, 0.01f);
                        //        }
                        //        else
                        //        {
                        //            AttackReleased = true;
                        //            if ((!(State is HumanAttackState) && (((Rigidbody.velocity.x * Rigidbody.velocity.x) + (Rigidbody.velocity.z * Rigidbody.velocity.z)) > ((Speed * Speed) * 1.5f))) && !(State is HumanFillGasState))
                        //            {
                        //                SetState<HumanSlideState>();
                        //                CrossFade(HeroAnim.SLIDE, 0.05f);
                        //                FacingDirection = Mathf.Atan2(Rigidbody.velocity.x, Rigidbody.velocity.z) * Mathf.Rad2Deg;
                        //                TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                        //                SparksEM.enabled = true;
                        //            }
                        //        }
                        //    }
                        //    JustGrounded = false;
                        //    zero = Rigidbody.velocity;
                        //}
                        #endregion
                        #region Ground Dodge
                        if (State is HumanGroundDodgeState)
                        {
                            if ((Animation[HeroAnim.DODGE].normalizedTime >= 0.2f) && (Animation[HeroAnim.DODGE].normalizedTime < 0.8f))
                            {
                                zero = ((-transform.forward * 2.4f) * Speed);
                            }
                            if (Animation[HeroAnim.DODGE].normalizedTime > 0.8f)
                            {
                                zero = (Rigidbody.velocity * 0.9f);
                            }
                        }
                        #endregion
                        #region Idle
                        else if (State is HumanIdleState)
                        {
                            Vector3 vector8 = new Vector3(x, 0f, z);
                            float resultAngle = GetGlobalFacingDirection(x, z);
                            zero = GetGlobaleFacingVector3(resultAngle);
                            float num6 = (vector8.magnitude <= 0.95f) ? ((vector8.magnitude >= 0.25f) ? vector8.magnitude : 0f) : 1f;
                            zero = (zero * num6);
                            zero = (zero * Speed);
                            if ((BuffTime > 0f) && (CurrentBuff == BUFF.SpeedUp))
                            {
                                zero = (zero * 4f);
                            }
                            if ((x != 0f) || (z != 0f))
                            {
                                if (((!Animation.IsPlaying(HeroAnim.RUN_1) && !Animation.IsPlaying(HeroAnim.JUMP)) && !Animation.IsPlaying(HeroAnim.RUN_SASHA)) && (!Animation.IsPlaying(HeroAnim.HORSE_GET_ON) || (Animation[HeroAnim.HORSE_GET_ON].normalizedTime >= 0.5f)))
                                {
                                    if ((BuffTime > 0f) && (CurrentBuff == BUFF.SpeedUp))
                                    {
                                        CrossFade(HeroAnim.RUN_SASHA, 0.1f);
                                    }
                                    else
                                    {
                                        CrossFade(HeroAnim.RUN_1, 0.1f);
                                    }
                                }
                            }
                            else
                            {
                                if (!(((Animation.IsPlaying(StandAnimation) || (State is HumanLandState)) || (Animation.IsPlaying(HeroAnim.JUMP) || Animation.IsPlaying(HeroAnim.HORSE_GET_ON))) || Animation.IsPlaying(HeroAnim.GRABBED)))
                                {
                                    CrossFade(StandAnimation, 0.1f);
                                    zero = (zero * 0f);
                                }
                                resultAngle = -874f;
                            }
                            if (resultAngle != -874f)
                            {
                                FacingDirection = resultAngle;
                                TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                            }
                        }
                        #endregion
                        #region Land
                        else if (State is HumanLandState)
                        {
                            zero = (Rigidbody.velocity * 0.96f);
                        }
                        #endregion
                        #region Slide
                        else if (State is HumanSlideState)
                        {
                            zero = (Rigidbody.velocity * 0.99f);
                            if (CurrentSpeed < (Speed * 1.2f))
                            {
                                SetState<HumanIdleState>();
                                SparksEM.enabled = false;
                            }
                        }
                        #endregion
                        #region Other
                        Vector3 velocity = Rigidbody.velocity;
                        Vector3 force = zero - velocity;
                        force.x = Mathf.Clamp(force.x, -MaxVelocityChange, MaxVelocityChange);
                        force.z = Mathf.Clamp(force.z, -MaxVelocityChange, MaxVelocityChange);
                        force.y = 0f;
                        if (Animation.IsPlaying(HeroAnim.JUMP) && (Animation[HeroAnim.JUMP].normalizedTime > 0.18f))
                        {
                            force.y += 8f;
                        }
                        if ((Animation.IsPlaying(HeroAnim.HORSE_GET_ON) && (Animation[HeroAnim.HORSE_GET_ON].normalizedTime > 0.18f)) && (Animation[HeroAnim.HORSE_GET_ON].normalizedTime < 1f))
                        {
                            force = new Vector3(-Rigidbody.velocity.x, 6f, -Rigidbody.velocity.z);
                            var distance = Vector3.Distance(Horse.transform.position, transform.position);
                            var num9 = (0.6f * Gravity * distance) / 12f;
                            vector7 = Horse.transform.position - transform.position;
                            force += (num9 * vector7.normalized);
                        }
                        if (!(State is HumanAttackState) || !UseGun)
                        {
                            Rigidbody.AddForce(force, ForceMode.VelocityChange);
                            Rigidbody.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0f, FacingDirection, 0f), Time.deltaTime * 10f);
                        }
                        #endregion
                    }

                    #region flags 2-4
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    IsLeftHandHooked = false;
                    IsRightHandHooked = false;
                    if (IsLaunchLeft)
                    {
                        if ((HookLeft != null) && HookLeft.isHooked())
                        {
                            IsLeftHandHooked = true;
                            Vector3 to = HookLeft.transform.position - transform.position;
                            to.Normalize();
                            to = (to * 10f);
                            if (!IsLaunchRight)
                            {
                                to = (to * 2f);
                            }
                            if ((Vector3.Angle(Rigidbody.velocity, to) > 90f) && InputManager.Key(InputHuman.Jump))
                            {
                                flag3 = true;
                                flag2 = true;
                            }
                            if (!flag3)
                            {
                                Rigidbody.AddForce(to);
                                if (Vector3.Angle(Rigidbody.velocity, to) > 90f)
                                {
                                    Rigidbody.AddForce((-Rigidbody.velocity * 2f), ForceMode.Acceleration);
                                }
                            }
                        }
                        LaunchElapsedTimeL += Time.deltaTime;
                        if (LeftHookHold && (CurrentGas > 0f))
                        {
                            UseGas(UseGasSpeed * Time.deltaTime);
                        }
                        else if (LaunchElapsedTimeL > 0.3f)
                        {
                            IsLaunchLeft = false;
                            if (HookLeft != null)
                            {
                                HookLeft.disable();
                                ReleaseIfIHookSb();
                                HookLeft = null;
                                flag3 = false;
                            }
                        }
                    }

                    if (IsLaunchRight)
                    {
                        if ((HookRight != null) && HookRight.isHooked())
                        {
                            IsRightHandHooked = true;
                            Vector3 vector5 = HookRight.transform.position - transform.position;
                            vector5.Normalize();
                            vector5 = (vector5 * 10f);
                            if (!IsLaunchLeft)
                            {
                                vector5 = (vector5 * 2f);
                            }
                            if ((Vector3.Angle(Rigidbody.velocity, vector5) > 90f) && InputManager.Key(InputHuman.Jump))
                            {
                                flag4 = true;
                                flag2 = true;
                            }
                            if (!flag4)
                            {
                                Rigidbody.AddForce(vector5);
                                if (Vector3.Angle(Rigidbody.velocity, vector5) > 90f)
                                {
                                    Rigidbody.AddForce((-Rigidbody.velocity * 2f), ForceMode.Acceleration);
                                }
                            }
                        }
                        LaunchElapsedTimeR += Time.deltaTime;
                        if (RightHookHold && (CurrentGas > 0f))
                        {
                            UseGas(UseGasSpeed * Time.deltaTime);
                        }
                        else if (LaunchElapsedTimeR > 0.3f)
                        {
                            IsLaunchRight = false;
                            if (HookRight != null)
                            {
                                HookRight.disable();
                                ReleaseIfIHookSb();
                                HookRight = null;
                                flag4 = false;
                            }
                        }
                    }

                    if (!IsGrounded)
                    {
                        if (SparksEM.enabled)
                        {
                            SparksEM.enabled = false;
                        }
                        if ((Horse && (Animation.IsPlaying(HeroAnim.HORSE_GET_ON) || Animation.IsPlaying(HeroAnim.AIR_FALL))) && ((Rigidbody.velocity.y < 0f) && (Vector3.Distance(Horse.transform.position + Vector3.up * 1.65f, transform.position) < 0.5f)))
                        {
                            transform.position = Horse.transform.position + Vector3.up * 1.65f;
                            transform.rotation = Horse.transform.rotation;
                            IsMounted = true;
                            CrossFade(HeroAnim.HORSE_IDLE, 0.1f);
                            Horse.Mount();
                        }
                        if (!(((((!(State is HumanIdleState) || Animation.IsPlaying(HeroAnim.DASH)) ||
                            (Animation.IsPlaying(HeroAnim.WALL_RUN) || Animation.IsPlaying(HeroAnim.TO_ROOF))) ||
                            ((Animation.IsPlaying(HeroAnim.HORSE_GET_ON) || Animation.IsPlaying(HeroAnim.HORSE_GET_OFF)) || (Animation.IsPlaying(HeroAnim.AIR_RELEASE) || IsMounted))) ||
                            ((Animation.IsPlaying(HeroAnim.AIR_HOOK_L_JUST) && (Animation[HeroAnim.AIR_HOOK_L_JUST].normalizedTime < 1f)) ||
                            (Animation.IsPlaying(HeroAnim.AIR_HOOK_R_JUST) && (Animation[HeroAnim.AIR_HOOK_R_JUST].normalizedTime < 1f)))) ? (Animation[HeroAnim.DASH].normalizedTime < 0.99f) : false))
                        {
                            if (((!IsLeftHandHooked && !IsRightHandHooked) && ((Animation.IsPlaying(HeroAnim.AIR_HOOK_L) || Animation.IsPlaying(HeroAnim.AIR_HOOK_R)) || Animation.IsPlaying(HeroAnim.AIR_HOOK))) && (Rigidbody.velocity.y > 20f))
                            {
                                Animation.CrossFade(HeroAnim.AIR_RELEASE);
                            }
                            else
                            {
                                bool flag5 = (Mathf.Abs(Rigidbody.velocity.x) + Mathf.Abs(Rigidbody.velocity.z)) > 25f;
                                bool flag6 = Rigidbody.velocity.y < 0f;
                                if (!flag5)
                                {
                                    if (flag6)
                                    {
                                        if (!Animation.IsPlaying(HeroAnim.AIR_FALL))
                                        {
                                            CrossFade(HeroAnim.AIR_FALL, 0.2f);
                                        }
                                    }
                                    else if (!Animation.IsPlaying(HeroAnim.AIR_RISE))
                                    {
                                        CrossFade(HeroAnim.AIR_RISE, 0.2f);
                                    }
                                }
                                else if (!IsLeftHandHooked && !IsRightHandHooked)
                                {
                                    float current = -Mathf.Atan2(Rigidbody.velocity.z, Rigidbody.velocity.x) * Mathf.Rad2Deg;
                                    float num11 = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
                                    if (Mathf.Abs(num11) < 45f)
                                    {
                                        if (!Animation.IsPlaying(HeroAnim.AIR2))
                                        {
                                            CrossFade(HeroAnim.AIR2, 0.2f);
                                        }
                                    }
                                    else if ((num11 < 135f) && (num11 > 0f))
                                    {
                                        if (!Animation.IsPlaying(HeroAnim.AIR2_RIGHT))
                                        {
                                            CrossFade(HeroAnim.AIR2_RIGHT, 0.2f);
                                        }
                                    }
                                    else if ((num11 > -135f) && (num11 < 0f))
                                    {
                                        if (!Animation.IsPlaying(HeroAnim.AIR2_LEFT))
                                        {
                                            CrossFade(HeroAnim.AIR2_LEFT, 0.2f);
                                        }
                                    }
                                    else if (!Animation.IsPlaying(HeroAnim.AIR2_BACKWARD))
                                    {
                                        CrossFade(HeroAnim.AIR2_BACKWARD, 0.2f);
                                    }
                                }

                                else if (!IsRightHandHooked)
                                {
                                    TryCrossFade(Equipment.Weapon.HookForwardLeft, 0.1f);
                                }
                                else if (!IsLeftHandHooked)
                                {
                                    TryCrossFade(Equipment.Weapon.HookForwardRight, 0.1f);
                                }
                                else if (!Animation.IsPlaying(Equipment.Weapon.HookForward))
                                {
                                    TryCrossFade(Equipment.Weapon.HookForward, 0.1f);
                                }
                            }
                        }
                        if (((State is HumanIdleState) && Animation.IsPlaying(HeroAnim.AIR_RELEASE)) && (Animation[HeroAnim.AIR_RELEASE].normalizedTime >= 1f))
                        {
                            CrossFade(HeroAnim.AIR_RISE, 0.2f);
                        }
                        if (Animation.IsPlaying(HeroAnim.HORSE_GET_OFF) && (Animation[HeroAnim.HORSE_GET_OFF].normalizedTime >= 1f))
                        {
                            CrossFade(HeroAnim.AIR_RISE, 0.2f);
                        }
                        if (Animation.IsPlaying(HeroAnim.TO_ROOF))
                        {
                            if (Animation[HeroAnim.TO_ROOF].normalizedTime < 0.22f)
                            {
                                Rigidbody.velocity = Vector3.zero;
                                Rigidbody.AddForce(new Vector3(0f, Gravity * Rigidbody.mass, 0f));
                            }
                            else
                            {
                                if (!WallJump)
                                {
                                    WallJump = true;
                                    Rigidbody.AddForce((Vector3.up * 8f), ForceMode.Impulse);
                                }
                                Rigidbody.AddForce((transform.forward * 0.05f), ForceMode.Impulse);
                            }
                            if (Animation[HeroAnim.TO_ROOF].normalizedTime >= 1f)
                            {
                                PlayAnimation(HeroAnim.AIR_RISE);
                            }
                        }
                        else if (!((((!(State is HumanIdleState) || !IsPressDirectionTowardsHero(x, z)) ||
                                     (InputManager.Key(InputHuman.Jump) ||
                                      InputManager.Key(InputHuman.HookLeft))) ||
                                    ((InputManager.Key(InputHuman.HookRight) ||
                                      InputManager.Key(InputHuman.HookBoth)) ||
                                     (!IsFrontGrounded() || Animation.IsPlaying(HeroAnim.WALL_RUN)))) ||
                                   Animation.IsPlaying(HeroAnim.DODGE)))
                        {
                            CrossFade(HeroAnim.WALL_RUN, 0.1f);
                            WallRunTime = 0f;
                        }
                        else if (Animation.IsPlaying(HeroAnim.WALL_RUN))
                        {
                            Rigidbody.AddForce(((Vector3.up * Speed)) - Rigidbody.velocity, ForceMode.VelocityChange);
                            WallRunTime += Time.deltaTime;
                            if ((WallRunTime > 1f) || ((z == 0f) && (x == 0f)))
                            {
                                Rigidbody.AddForce(((-transform.forward * Speed) * 0.75f), ForceMode.Impulse);
                                Dodge(true);
                            }
                            else if (!IsUpFrontGrounded())
                            {
                                WallJump = false;
                                CrossFade(HeroAnim.TO_ROOF, 0.1f);
                            }
                            else if (!IsFrontGrounded())
                            {
                                CrossFade(HeroAnim.AIR_FALL, 0.1f);
                            }
                        }
                        // If we are using these skills, then we cannot use gas force
                        else if ((!Animation.IsPlaying(HeroAnim.SPECIAL_LEVI) && !Animation.IsPlaying(HeroAnim.SPECIAL_PETRA)) && (!Animation.IsPlaying(HeroAnim.DASH) && !Animation.IsPlaying(HeroAnim.JUMP)))
                        {
                            Vector3 vector11 = new Vector3(x, 0f, z);
                            float num12 = GetGlobalFacingDirection(x, z);
                            Vector3 vector12 = GetGlobaleFacingVector3(num12);
                            float num13 = (vector11.magnitude <= 0.95f) ? ((vector11.magnitude >= 0.25f) ? vector11.magnitude : 0f) : 1f;
                            vector12 = (vector12 * num13);
                            //TODO: ACL
                            vector12 = (vector12 * ((/*(float)setup.myCostume.stat.ACL) */ 125f / 10f) * 2f));
                            if ((x == 0f) && (z == 0f))
                            {
                                if (State is HumanAttackState)
                                {
                                    vector12 = (vector12 * 0f);
                                }
                                num12 = -874f;
                            }
                            if (num12 != -874f)
                            {
                                FacingDirection = num12;
                                TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                            }
                            if (((!flag3 && !flag4) && (!IsMounted && InputManager.Key(InputHuman.Jump))) && (CurrentGas > 0f))
                            {
                                if ((x != 0f) || (z != 0f))
                                {
                                    Rigidbody.AddForce(vector12, ForceMode.Acceleration);
                                }
                                else
                                {
                                    Rigidbody.AddForce((transform.forward * vector12.magnitude), ForceMode.Acceleration);
                                }
                                flag2 = true;
                            }
                        }
                        if ((Animation.IsPlaying(HeroAnim.AIR_FALL) && (CurrentSpeed < 0.2f)) && IsFrontGrounded())
                        {
                            CrossFade(HeroAnim.ON_WALL, 0.3f);
                        }
                    }

                    Spinning = false;
                    if (flag3 && flag4)
                    {
                        float num14 = CurrentSpeed + 0.1f;
                        AddRightForce();
                        Vector3 vector13 = (((HookRight.transform.position + HookLeft.transform.position) * 0.5f)) - transform.position;
                        float num15 = 0f;
                        if (InputManager.Key(InputHuman.ReelIn))
                        {
                            num15 = -1f;
                        }
                        else if (InputManager.Key(InputHuman.ReelOut))
                        {
                            num15 = 1f;
                        }
                        else
                        {
                            num15 = UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 5555f;
                        }
                        num15 = Mathf.Clamp(num15, -0.8f, 0.8f);
                        float num16 = 1f + num15;
                        Vector3 vector14 = Vector3.RotateTowards(vector13, Rigidbody.velocity, 1.53938f * num16, 1.53938f * num16);
                        vector14.Normalize();
                        Spinning = true;
                        Rigidbody.velocity = (vector14 * num14);
                    }
                    else if (flag3)
                    {
                        float num17 = CurrentSpeed + 0.1f;
                        AddRightForce();
                        Vector3 vector15 = HookLeft.transform.position - transform.position;
                        float num18 = 0f;
                        if (InputManager.Key(InputHuman.ReelIn))
                        {
                            num18 = -1f;
                        }
                        else if (InputManager.Key(InputHuman.ReelOut))
                        {
                            num18 = 1f;
                        }
                        else
                        {
                            num18 = UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 5555f;
                        }
                        num18 = Mathf.Clamp(num18, -0.8f, 0.8f);
                        float num19 = 1f + num18;
                        Vector3 vector16 = Vector3.RotateTowards(vector15, Rigidbody.velocity, 1.53938f * num19, 1.53938f * num19);
                        vector16.Normalize();
                        Spinning = true;
                        Rigidbody.velocity = (vector16 * num17);
                    }
                    else if (flag4)
                    {
                        float num20 = CurrentSpeed + 0.1f;
                        AddRightForce();
                        Vector3 vector17 = HookRight.transform.position - transform.position;
                        float num21 = 0f;
                        if (InputManager.Key(InputHuman.ReelIn))
                        {
                            num21 = -1f;
                        }
                        else if (InputManager.Key(InputHuman.ReelOut))
                        {
                            num21 = 1f;
                        }
                        else
                        {
                            num21 = UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 5555f;
                        }
                        num21 = Mathf.Clamp(num21, -0.8f, 0.8f);
                        float num22 = 1f + num21;
                        Vector3 vector18 = Vector3.RotateTowards(vector17, Rigidbody.velocity, 1.53938f * num22, 1.53938f * num22);
                        vector18.Normalize();
                        Spinning = true;
                        Rigidbody.velocity = (vector18 * num20);
                    }
                    bool flag7 = false;
                    if ((HookLeft != null) || (HookRight != null))
                    {
                        if (((HookLeft != null) && (HookLeft.transform.position.y > gameObject.transform.position.y)) && (IsLaunchLeft && HookLeft.isHooked()))
                        {
                            flag7 = true;
                        }
                        if (((HookRight != null) && (HookRight.transform.position.y > gameObject.transform.position.y)) && (IsLaunchRight && HookRight.isHooked()))
                        {
                            flag7 = true;
                        }
                    }
                    if (flag7)
                    {
                        Rigidbody.AddForce(new Vector3(0f, -10f * Rigidbody.mass, 0f));
                    }
                    else
                    {
                        Rigidbody.AddForce(new Vector3(0f, -Gravity * Rigidbody.mass, 0f));
                    }

                    if (CurrentSpeed > 10f)
                    {
                        CurrentCamera.fieldOfView = Mathf.Lerp(CurrentCamera.fieldOfView, Mathf.Min((float) 100f, (float) (CurrentSpeed + 40f)), 0.1f);
                    }
                    else
                    {
                        CurrentCamera.fieldOfView = Mathf.Lerp(CurrentCamera.fieldOfView, 50f, 0.1f);
                    }
                    if (flag2)
                    {
                        UseGas(UseGasSpeed * Time.deltaTime);
                        if (!smoke_3dmg_em.enabled && photonView.isMine)
                        {
                            object[] parameters = new object[] { true };
                            photonView.RPC(nameof(Net3DMGSMOKE), PhotonTargets.Others, parameters);
                        }
                        smoke_3dmg_em.enabled = true;
                    }
                    else
                    {
                        if (smoke_3dmg_em.enabled && photonView.isMine)
                        {
                            object[] objArray3 = new object[] { false };
                            photonView.RPC(nameof(Net3DMGSMOKE), PhotonTargets.Others, objArray3);
                        }
                        smoke_3dmg_em.enabled = false;
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region OnDestroy
        protected override void OnDestroy()
        {
            DeregisterInputs();

            base.OnDestroy();
            if (MyNetWorkName != null)
            {
                Destroy(MyNetWorkName);
            }
            if (GunDummy != null)
            {
                Destroy(GunDummy);
            }
            ReleaseIfIHookSb();

        }
        #endregion

        #region Other
        public void Initialize(CharacterPreset preset)
        {
            //TODO: Remove hack
            var manager = GetComponent<CustomizationManager>();
            if (preset == null)
            {
                preset = manager.Presets.First();
            }

            preset.Apply(this, manager.Prefabs);
            Skill = preset.CurrentBuild.Skill;
            Skill.Initialize(this);

            EquipmentType = preset.CurrentBuild.Equipment;
            Equipment.Initialize();

            if (EquipmentType == EquipmentType.Ahss)
            {
                StandAnimation = HeroAnim.AHSS_STAND_GUN;
                UseGun = true;
                GunDummy = new GameObject
                {
                    name = "GunDummy"
                };
                GunDummy.transform.position = transform.position;
                GunDummy.transform.rotation = transform.rotation;
            }

            if (photonView.isMine)
            {
                //TODO: If this is a default preset, find a more efficient way
                var config = JsonConvert.SerializeObject(preset, Formatting.Indented, new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new ColorJsonConverter() },
                    ContractResolver = new JsonIgnoreResolver() // This will stop Images (on skills) from being serialized
                });
                photonView.RPC(nameof(InitializeRpc), PhotonTargets.OthersBuffered, config);
            }

            EntityService.Register(this);
        }

        [PunRPC]
        public void InitializeRpc(string characterPreset, PhotonMessageInfo info)
        {
            if (photonView.isMine)
            {
                //TODO: HandLe Abusive RPC
                return;
            }

            if (info.sender.ID == photonView.ownerId)
            {
                Initialize(JsonConvert.DeserializeObject<CharacterPreset>(characterPreset, new ColorJsonConverter()));
            }
        }

        public override void OnHit(Entity attacker, int damage)
        {
            //TODO: 160 HERO OnHit logic
            //if (!isInvincible() && _state != HERO_STATE.Grab)
            //    markDie();
        }

        #region Animation

        public void SetAnimationSpeed(string animationName, float animationSpeed = 1f)
        {
            Animation[animationName].speed = animationSpeed;
            if (!photonView.isMine) return;

            photonView.RPC(nameof(SetAnimationSpeedRpc), PhotonTargets.Others, animationName, animationSpeed);
        }

        [PunRPC]
        private void SetAnimationSpeedRpc(string animationName, float animationSpeed, PhotonMessageInfo info)
        {
            if (info.sender.ID == photonView.owner.ID)
            {
                Animation[animationName].speed = animationSpeed;
            }
        }

        public void CrossFade(string newAnimation, float fadeLength = 0.1f)
        {
            Debug.Log("Crossfading to: " + newAnimation);
            if (string.IsNullOrWhiteSpace(newAnimation))
                return;

            if (Animation.IsPlaying(newAnimation))
                return;

            if (!photonView.isMine)
                return;

            CurrentAnimation = newAnimation;
            Animation.CrossFade(newAnimation, fadeLength);
            photonView.RPC(nameof(CrossFadeRpc), PhotonTargets.Others, newAnimation, fadeLength);
        }

        [PunRPC]
        protected void CrossFadeRpc(string newAnimation, float fadeLength, PhotonMessageInfo info)
        {
            if (info.sender.ID == photonView.owner.ID)
            {
                CurrentAnimation = newAnimation;
                Animation.CrossFade(newAnimation, fadeLength);
            }
        }

        public void TryCrossFade(string animationName, float time)
        {
            if (!Animation.IsPlaying(animationName))
            {
                CrossFade(animationName, time);
            }
        }

        private void CustomAnimationSpeed()
        {
            Animation[HeroAnim.SPECIAL_LEVI].speed = 1.85f;
            Animation[HeroAnim.CHANGE_BLADE].speed = 1.2f;
            Animation[HeroAnim.AIR_RELEASE].speed = 0.6f;
            Animation[HeroAnim.CHANGE_BLADE_AIR].speed = 0.8f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_BOTH].speed = 0.38f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_BOTH_AIR].speed = 0.5f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_L].speed = 0.4f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_L_AIR].speed = 0.5f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_R].speed = 0.4f;
            Animation[HeroAnim.AHSS_GUN_RELOAD_R_AIR].speed = 0.5f;
        }

        [PunRPC]
        public void NetPlayAnimation(string aniName)
        {
            CurrentAnimation = aniName;
            if (Animation != null)
            {
                Animation.Play(aniName);
            }
        }

        [PunRPC]
        private void NetPlayAnimationAt(string aniName, float normalizedTime)
        {
            CurrentAnimation = aniName;
            if (Animation != null)
            {
                Animation.Play(aniName);
                Animation[aniName].normalizedTime = normalizedTime;
            }
        }

        public void PlayAnimation(string aniName)
        {
            CurrentAnimation = aniName;
            Debug.Log("Animation: " + aniName);
            Animation.Play(aniName);
            if (PhotonNetwork.connected && photonView.isMine)
            {
                object[] parameters = new object[] { aniName };
                photonView.RPC(nameof(NetPlayAnimation), PhotonTargets.Others, parameters);
            }
        }

        public void PlayAnimationAt(string aniName, float normalizedTime)
        {
            CurrentAnimation = aniName;
            Animation.Play(aniName);
            Animation[aniName].normalizedTime = normalizedTime;
            if (PhotonNetwork.connected && photonView.isMine)
            {
                object[] parameters = new object[] { aniName, normalizedTime };
                photonView.RPC(nameof(NetPlayAnimationAt), PhotonTargets.Others, parameters);
            }
        }

        #endregion

        public void AttackAccordingToMouse()
        {
            if (UnityEngine.Input.mousePosition.x < (Screen.width * 0.5))
            {
                AttackAnimation = HeroAnim.ATTACK2;
            }
            else
            {
                AttackAnimation = HeroAnim.ATTACK1;
            }
        }

        public void AttackAccordingToTarget(Transform a)
        {
            Vector3 vector = a.position - transform.position;
            float current = -Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;
            float f = -Mathf.DeltaAngle(current, transform.rotation.eulerAngles.y - 90f);
            if (((Mathf.Abs(f) < 90f) && (vector.magnitude < 6f)) && ((a.position.y <= (transform.position.y + 2f)) && (a.position.y >= (transform.position.y - 5f))))
            {
                AttackAnimation = HeroAnim.ATTACK4;
            }
            else if (f > 0f)
            {
                AttackAnimation = HeroAnim.ATTACK1;
            }
            else
            {
                AttackAnimation = HeroAnim.ATTACK2;
            }
        }

        public void BackToHuman()
        {
            SmoothSync.disabled = false;
            Rigidbody.velocity = Vector3.zero;
            TitanForm = false;
            BreakFreeFromGrab();
            FalseAttack();
            SkillCDDuration = SkillCDLast;
            CurrentInGameCamera.SetMainObject(gameObject, true, false);
            photonView.RPC(nameof(BackToHumanRPC), PhotonTargets.Others, new object[0]);
        }

        [PunRPC]
        private void BackToHumanRPC()
        {
            TitanForm = false;
            ErenTitan = null;
            SmoothSync.disabled = false;
        }

        [PunRPC]
        public void BadGuyReleaseMe()
        {
            HookedBySomeone = false;
            BadGuy = null;
        }

        [PunRPC]
        public void BlowAway(Vector3 force)
        {
            if (photonView.isMine)
            {
                Rigidbody.AddForce(force, ForceMode.Impulse);
                transform.LookAt(transform.position);
            }
        }

        private void BodyLean()
        {
            if (photonView.isMine)
            {
                float z = 0f;
                NeedLean = false;
                if ((!UseGun && (State is HumanAttackState)) && ((AttackAnimation != HeroAnim.SPECIAL_MIKASA_0) && (AttackAnimation != HeroAnim.SPECIAL_MIKASA_1)))
                {
                    float y = Rigidbody.velocity.y;
                    float x = Rigidbody.velocity.x;
                    float num4 = Rigidbody.velocity.z;
                    float num5 = Mathf.Sqrt((x * x) + (num4 * num4));
                    float num6 = Mathf.Atan2(y, num5) * Mathf.Rad2Deg;
                    TargetRotation = Quaternion.Euler(-num6 * (1f - (Vector3.Angle(Rigidbody.velocity, transform.forward) / 90f)), FacingDirection, 0f);
                    if ((IsLeftHandHooked && (HookLeft != null)) || (IsRightHandHooked && (HookRight != null)))
                    {
                        transform.rotation = TargetRotation;
                    }
                }
                else
                {
                    if ((IsLeftHandHooked && (HookLeft != null)) && (IsRightHandHooked && (HookRight != null)))
                    {
                        if (AlmostSingleHook)
                        {
                            NeedLean = true;
                            z = GetLeanAngle(HookRight.transform.position, true);
                        }
                    }
                    else if (IsLeftHandHooked && (HookLeft != null))
                    {
                        NeedLean = true;
                        z = GetLeanAngle(HookLeft.transform.position, true);
                    }
                    else if (IsRightHandHooked && (HookRight != null))
                    {
                        NeedLean = true;
                        z = GetLeanAngle(HookRight.transform.position, false);
                    }
                    if (NeedLean)
                    {
                        float a = 0f;
                        if (!UseGun && (State is HumanAttackState))
                        {
                            a = CurrentSpeed * 0.1f;
                            a = Mathf.Min(a, 20f);
                        }
                        TargetRotation = Quaternion.Euler(-a, FacingDirection, z);
                    }
                    else if (State is HumanAttackState)
                    {
                        TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                    }
                }
            }
        }

        public void BombInit()
        {
            //skillIDHUD = skillId.ToString();
            //skillCDDuration = skillCDLast;
            //if (GameSettings.PvP.Bomb == true)
            //{
            //    int num = (int) FengGameManagerMKII.settings[250];
            //    int num2 = (int) FengGameManagerMKII.settings[251];
            //    int num3 = (int) FengGameManagerMKII.settings[252];
            //    int num4 = (int) FengGameManagerMKII.settings[253];
            //    if ((num < 0) || (num > 10))
            //    {
            //        num = 5;
            //        FengGameManagerMKII.settings[250] = 5;
            //    }
            //    if ((num2 < 0) || (num2 > 10))
            //    {
            //        num2 = 5;
            //        FengGameManagerMKII.settings[0xfb] = 5;
            //    }
            //    if ((num3 < 0) || (num3 > 10))
            //    {
            //        num3 = 5;
            //        FengGameManagerMKII.settings[0xfc] = 5;
            //    }
            //    if ((num4 < 0) || (num4 > 10))
            //    {
            //        num4 = 5;
            //        FengGameManagerMKII.settings[0xfd] = 5;
            //    }
            //    if ((((num + num2) + num3) + num4) > 20)
            //    {
            //        num = 5;
            //        num2 = 5;
            //        num3 = 5;
            //        num4 = 5;
            //        FengGameManagerMKII.settings[250] = 5;
            //        FengGameManagerMKII.settings[0xfb] = 5;
            //        FengGameManagerMKII.settings[0xfc] = 5;
            //        FengGameManagerMKII.settings[0xfd] = 5;
            //    }
            //    bombTimeMax = ((num2 * 60f) + 200f) / ((num3 * 60f) + 200f);
            //    bombRadius = (num * 4f) + 20f;
            //    bombCD = (num4 * -0.4f) + 5f;
            //    bombSpeed = (num3 * 60f) + 200f;
            //    ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
            //    propertiesToSet.Add(PhotonPlayerProperty.RCBombR, (float) FengGameManagerMKII.settings[0xf6]);
            //    propertiesToSet.Add(PhotonPlayerProperty.RCBombG, (float) FengGameManagerMKII.settings[0xf7]);
            //    propertiesToSet.Add(PhotonPlayerProperty.RCBombB, (float) FengGameManagerMKII.settings[0xf8]);
            //    propertiesToSet.Add(PhotonPlayerProperty.RCBombA, (float) FengGameManagerMKII.settings[0xf9]);
            //    propertiesToSet.Add(PhotonPlayerProperty.RCBombRadius, bombRadius);
            //    PhotonNetwork.player.SetCustomProperties(propertiesToSet);
            //    skillId = SkillId.bomb;
            //    skillIDHUD = SkillId.armin.ToString();
            //    skillCDLast = bombCD;
            //    skillCDDuration = 10f;
            //    if (Service.Time.GetRoundTime() > 10f)
            //    {
            //        skillCDDuration = 5f;
            //    }
            //}
        }

        private void BreakApart(Vector3 v, bool isBite)
        {
            //TODO: Implement Character Break Apart with the characters materials
            return;
        }

        private void BufferUpdate()
        {
            if (BuffTime > 0f)
            {
                BuffTime -= Time.deltaTime;
                if (BuffTime <= 0f)
                {
                    BuffTime = 0f;
                    if ((CurrentBuff == BUFF.SpeedUp) && Animation.IsPlaying(HeroAnim.RUN_SASHA))
                    {
                        CrossFade(HeroAnim.RUN_1, 0.1f);
                    }
                    CurrentBuff = BUFF.NoBuff;
                }
            }
        }

        public void Reload()
        {
            if ((!UseGun || IsGrounded) || GameSettings.PvP.AhssAirReload.Value)
            {
                SetState<HumanChangeBladeState>();
                BladesThrown = false;
                Equipment.Weapon.PlayReloadAnimation();
            }
        }

        private void CheckDashDoubleTap()
        {
            if (UTapTime >= 0f)
            {
                UTapTime += Time.deltaTime;
                if (UTapTime > 0.2f)
                {
                    UTapTime = -1f;
                }
            }
            if (DTapTime >= 0f)
            {
                DTapTime += Time.deltaTime;
                if (DTapTime > 0.2f)
                {
                    DTapTime = -1f;
                }
            }
            if (LTapTime >= 0f)
            {
                LTapTime += Time.deltaTime;
                if (LTapTime > 0.2f)
                {
                    LTapTime = -1f;
                }
            }
            if (RTapTime >= 0f)
            {
                RTapTime += Time.deltaTime;
                if (RTapTime > 0.2f)
                {
                    RTapTime = -1f;
                }
            }
            if (InputManager.KeyDown(InputHuman.Forward))
            {
                if (UTapTime == -1f)
                {
                    UTapTime = 0f;
                }
                if (UTapTime != 0f)
                {
                    DashU = true;
                }
            }
            if (InputManager.KeyDown(InputHuman.Backward))
            {
                if (DTapTime == -1f)
                {
                    DTapTime = 0f;
                }
                if (DTapTime != 0f)
                {
                    DashD = true;
                }
            }
            if (InputManager.KeyDown(InputHuman.Left))
            {
                if (LTapTime == -1f)
                {
                    LTapTime = 0f;
                }
                if (LTapTime != 0f)
                {
                    DashL = true;
                }
            }
            if (InputManager.KeyDown(InputHuman.Right))
            {
                if (RTapTime == -1f)
                {
                    RTapTime = 0f;
                }
                if (RTapTime != 0f)
                {
                    DashR = true;
                }
            }
        }

        private void CheckDashRebind()
        {
            if (InputManager.Key(InputHuman.GasBurst))
            {
                if (InputManager.Key(InputHuman.Forward))
                {
                    DashU = true;
                }
                else if (InputManager.Key(InputHuman.Backward))
                {
                    DashD = true;
                }
                else if (InputManager.Key(InputHuman.Left))
                {
                    DashL = true;
                }
                else if (InputManager.Key(InputHuman.Right))
                {
                    DashR = true;
                }
            }
        }

        public void CheckTitan()
        {
            int count;
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer() | Layers.PlayerAttackBox.ToLayer();

            RaycastHit[] hitArray = Physics.RaycastAll(ray, 180f, mask.value);
            var raycastHits = new List<RaycastHit>();
            List<MindlessTitan> mindlessTitans = new List<MindlessTitan>();
            for (count = 0; count < hitArray.Length; count++)
            {
                RaycastHit item = hitArray[count];
                raycastHits.Add(item);
            }
            raycastHits.Sort((x, y) => x.distance.CompareTo(y.distance));
            float num2 = 180f;
            for (count = 0; count < raycastHits.Count; count++)
            {
                RaycastHit hit2 = raycastHits[count];
                GameObject gameObject = hit2.collider.gameObject;
                if (gameObject.layer == 0x10)
                {
                    if (gameObject.name.Contains("PlayerCollisionDetection") && ((hit2 = raycastHits[count]).distance < num2))
                    {
                        num2 -= 60f;
                        if (num2 <= 60f)
                        {
                            count = raycastHits.Count;
                        }
                        MindlessTitan component = gameObject.GetComponentInParent<MindlessTitan>();
                        if (component != null)
                        {
                            mindlessTitans.Add(component);
                        }
                    }
                }
                else
                {
                    count = raycastHits.Count;
                }
            }
            for (count = 0; count < myTitans.Count; count++)
            {
                MindlessTitan titan2 = myTitans[count];
                if (!mindlessTitans.Contains(titan2))
                {
                    titan2.IsLooked = false;
                }
            }
            for (count = 0; count < mindlessTitans.Count; count++)
            {
                MindlessTitan titan3 = mindlessTitans[count];
                titan3.IsLooked = true;
            }
            myTitans = mindlessTitans;
        }

        private void Dash(float horizontal, float vertical)
        {
            if (((DashTime <= 0f) && (CurrentGas > 0f)) && !IsMounted)
            {
                UseGas(TotalGas * 0.04f);
                FacingDirection = GetGlobalFacingDirection(horizontal, vertical);
                DashV = GetGlobaleFacingVector3(FacingDirection);
                OriginVM = CurrentSpeed;
                Quaternion quaternion = Quaternion.Euler(0f, FacingDirection, 0f);
                Rigidbody.rotation = quaternion;
                TargetRotation = quaternion;
                PhotonNetwork.Instantiate("FX/boost_smoke", transform.position, transform.rotation, 0);
                DashTime = 0.5f;
                CrossFade(HeroAnim.DASH, 0.1f);
                Animation[HeroAnim.DASH].time = 0.1f;
                SetState<HumanAirDodgeState>();
                FalseAttack();
                Rigidbody.AddForce((DashV * 40f), ForceMode.VelocityChange);
            }
        }

        public void Die(Vector3 v, bool isBite)
        {
            if (Invincible <= 0f)
            {
                if (TitanForm && (ErenTitan != null))
                {
                    ErenTitan.lifeTime = 0.1f;
                }
                if (HookLeft != null)
                {
                    HookLeft.removeMe();
                }
                if (HookRight != null)
                {
                    HookRight.removeMe();
                }
                meatDie.Play();
                if ((photonView.isMine) && !UseGun)
                {
                    /*
                leftbladetrail.Deactivate();
                rightbladetrail.Deactivate();
                leftbladetrail2.Deactivate();
                rightbladetrail2.Deactivate();
                */
                }
                BreakApart(v, isBite);
                CurrentInGameCamera.gameOver = true;
                FalseAttack();
                HasDied = true;
                Transform audioDie = transform.Find("audio_die");
                audioDie.parent = null;
                audioDie.GetComponent<AudioSource>().Play();

                var propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.deaths, (int) PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.deaths] + 1);
                photonView.owner.SetCustomProperties(propertiesToSet);

                if (PlayerPrefs.HasKey("EnableSS") && (PlayerPrefs.GetInt("EnableSS") == 1))
                {
                    CurrentInGameCamera.StartSnapShot2(audioDie.position, 0, null, 0.02f);
                }
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        public void Dodge(bool offTheWall = false)
        {
            if (((!InputManager.Key(InputHorse.Mount) || !Horse) || IsMounted) || (Vector3.Distance(Horse.transform.position, transform.position) >= 15f))
            {
                SetState<HumanGroundDodgeState>();
                if (!offTheWall)
                {
                    float num3 = GetGlobalFacingDirection(TargetMoveDirection.x, TargetMoveDirection.y);
                    if (TargetMoveDirection.magnitude > 0f)
                    {
                        FacingDirection = num3 + 180f;
                        TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
                    }
                    CrossFade(HeroAnim.DODGE, 0.1f);
                }
                else
                {
                    PlayAnimation(HeroAnim.DODGE);
                    PlayAnimationAt(HeroAnim.DODGE, 0.2f);
                }
                SparksEM.enabled = false;
            }
        }

        public void Transform()
        {
            SkillCDDuration = SkillCDLast;
            if (HookLeft != null)
            {
                HookLeft.removeMe();
            }
            if (HookRight != null)
            {
                HookRight.removeMe();
            }
            ErenTitan = PhotonNetwork.Instantiate("ErenTitan", transform.position, transform.rotation, 0).GetComponent<ErenTitan>();
            ErenTitan.realBody = gameObject;

            CurrentInGameCamera.FlashBlind();
            CurrentInGameCamera.SetMainObject(ErenTitan.gameObject, true, false);
            ErenTitan.born();
            ErenTitan.Rigidbody.velocity = Rigidbody.velocity;
            Rigidbody.velocity = Vector3.zero;
            transform.position = ErenTitan.Body.Neck.position;
            TitanForm = true;
            object[] parameters = new object[] { ErenTitan.gameObject.GetPhotonView().viewID };
            photonView.RPC(nameof(WhoIsMyErenTitan), PhotonTargets.Others, parameters);
            if ((smoke_3dmg_em.enabled && photonView.isMine))
            {
                object[] objArray2 = new object[] { false };
                photonView.RPC(nameof(Net3DMGSMOKE), PhotonTargets.Others, objArray2);
            }
            smoke_3dmg_em.enabled = false;
        }

        public void FalseAttack()
        {
            if (UseGun)
            {
                //if (AttackReleased)
                //    return;

                SetAnimationSpeed(CurrentAnimation);
                //AttackReleased = true;
                return;
            }
            if (photonView.isMine)
            {
                checkBoxLeft.IsActive = false;
                checkBoxRight.IsActive = false;
                checkBoxLeft.ClearHits();
                checkBoxRight.ClearHits();
            }

            AttackLoop = 0;
            //if (!AttackReleased)
            //{
            //    SetAnimationSpeed(CurrentAnimation);
            //    AttackReleased = true;
            //}
        }

        public void FillGas()
        {
            CurrentGas = TotalGas;
        }

        public GameObject FindNearestTitan()
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titan");
            GameObject obj2 = null;
            float positiveInfinity = float.PositiveInfinity;
            Vector3 position = transform.position;
            foreach (GameObject obj3 in objArray)
            {
                Vector3 vector2 = obj3.transform.position - position;
                float sqrMagnitude = vector2.sqrMagnitude;
                if (sqrMagnitude < positiveInfinity)
                {
                    obj2 = obj3;
                    positiveInfinity = sqrMagnitude;
                }
            }
            return obj2;
        }

        [Obsolete("Does this do something?")]
        //Hotfix for Issue 97.
        private void AddRightForce()
        {
            //Whereas this may not be completely accurate to AoTTG, it is very close. Further balancing required in the future.
            Rigidbody.AddForce(Rigidbody.velocity * 0.00f, ForceMode.Acceleration);
        }


        public Vector3 GetGlobaleFacingVector3(float resultAngle)
        {
            float num = -resultAngle + 90f;
            float x = Mathf.Cos(num * Mathf.Deg2Rad);
            return new Vector3(x, 0f, Mathf.Sin(num * Mathf.Deg2Rad));
        }

        public float GetGlobalFacingDirection(float horizontal, float vertical)
        {
            if ((vertical == 0f) && (horizontal == 0f))
            {
                return transform.rotation.eulerAngles.y;
            }
            float y = CurrentCamera.transform.rotation.eulerAngles.y;
            float num2 = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            num2 = -num2 + 90f;
            return (y + num2);
        }

        private float GetLeanAngle(Vector3 p, bool left)
        {
            if (!UseGun && (State is HumanAttackState))
            {
                return 0f;
            }
            float num = p.y - transform.position.y;
            float num2 = Vector3.Distance(p, transform.position);
            float a = Mathf.Acos(num / num2) * Mathf.Rad2Deg;
            a *= 0.1f;
            a *= 1f + Mathf.Pow(Rigidbody.velocity.magnitude, 0.2f);
            Vector3 vector3 = p - transform.position;
            float current = Mathf.Atan2(vector3.x, vector3.z) * Mathf.Rad2Deg;
            float target = Mathf.Atan2(Rigidbody.velocity.x, Rigidbody.velocity.z) * Mathf.Rad2Deg;
            float num6 = Mathf.DeltaAngle(current, target);
            a += Mathf.Abs((float) (num6 * 0.5f));
            if (!(State is HumanAttackState))
            {
                a = Mathf.Min(a, 80f);
            }
            if (num6 > 0f)
            {
                LeanLeft = true;
            }
            else
            {
                LeanLeft = false;
            }
            if (UseGun)
            {
                return (a * ((num6 >= 0f) ? ((float) 1) : ((float) (-1))));
            }
            float num7 = 0f;
            if ((left && (num6 < 0f)) || (!left && (num6 > 0f)))
            {
                num7 = 0.1f;
            }
            else
            {
                num7 = 0.5f;
            }
            return (a * ((num6 >= 0f) ? num7 : -num7));
        }

        public void GetOffHorse()
        {
            PlayAnimation(HeroAnim.HORSE_GET_OFF);
            Rigidbody.AddForce((((Vector3.up * 10f) - (transform.forward * 2f)) - (transform.right * 1f)), ForceMode.VelocityChange);
            Unmounted();
        }

        public void GetOnHorse()
        {
            PlayAnimation(HeroAnim.HORSE_GET_ON);
            FacingDirection = Horse.transform.rotation.eulerAngles.y;
            TargetRotation = Quaternion.Euler(0f, FacingDirection, 0f);
        }

        public void GetSupply()
        {
            if ((Animation.IsPlaying(StandAnimation)
                 || Animation.IsPlaying(HeroAnim.RUN_1)
                 || Animation.IsPlaying(HeroAnim.RUN_SASHA))
                && (CurrentBladeSta != TotalBladeSta || CurrentGas != TotalGas || Equipment.Weapon.CanReload))
            {
                SetState<HumanFillGasState>();
                CrossFade(HeroAnim.SUPPLY, 0.1f);
            }
        }

        public void Grabbed(GameObject titan, bool leftHand)
        {
            if (IsMounted)
            {
                Unmounted();
            }
            SetState<HumanGrabState>();
            GetComponent<CapsuleCollider>().isTrigger = true;
            FalseAttack();
            TitanWhoGrabMe = titan;
            if (TitanForm && (ErenTitan != null))
            {
                ErenTitan.lifeTime = 0.1f;
            }
            smoke_3dmg_em.enabled = false;
            SparksEM.enabled = false;
        }

        public bool HasDiedOrInvincible()
        {
            return (HasDied || IsInvincible);
        }

        private void HeadMovement()
        {
            Transform neck = Body.neck;
            Transform head = Body.head;
            float x = Mathf.Sqrt(((GunTarget.x - head.position.x) * (GunTarget.x - head.position.x)) + ((GunTarget.z - head.position.z) * (GunTarget.z - head.position.z)));
            TargetHeadRotation = head.rotation;
            Vector3 vector5 = GunTarget - head.position;
            float current = -Mathf.Atan2(vector5.z, vector5.x) * Mathf.Rad2Deg;
            float num3 = -Mathf.DeltaAngle(current, head.rotation.eulerAngles.y - 90f);
            num3 = Mathf.Clamp(num3, -40f, 40f);
            float y = neck.position.y - GunTarget.y;
            float num5 = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            num5 = Mathf.Clamp(num5, -40f, 30f);
            TargetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + num5, head.rotation.eulerAngles.y + num3, head.rotation.eulerAngles.z);
            OldHeadRotation = Quaternion.Lerp(OldHeadRotation, TargetHeadRotation, Time.deltaTime * 60f);
            head.rotation = OldHeadRotation;
        }

        public void HookedByHuman(int hooker, Vector3 hookPosition)
        {
            object[] parameters = new object[] { hooker, hookPosition };
            photonView.RPC(nameof(RPCHookedByHuman), photonView.owner, parameters);
        }

        [PunRPC]
        public void HookFail()
        {
            HookTarget = null;
            HookSomeone = false;
        }

        public void HookToHuman(GameObject target, Vector3 hookPosition)
        {
            ReleaseIfIHookSb();
            HookTarget = target;
            HookSomeone = true;
            if (target.GetComponent<Hero>() != null)
            {
                target.GetComponent<Hero>().HookedByHuman(photonView.viewID, hookPosition);
            }
            LaunchForce = hookPosition - transform.position;
            float num = Mathf.Pow(LaunchForce.magnitude, 0.1f);
            if (IsGrounded)
            {
                Rigidbody.AddForce((Vector3.up * Mathf.Min((float) (LaunchForce.magnitude * 0.2f), (float) 10f)), ForceMode.Impulse);
            }
            Rigidbody.AddForce(((LaunchForce * num) * 0.1f), ForceMode.Impulse);
        }

        private bool IsFrontGrounded()
        {
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

            return Physics.Raycast(gameObject.transform.position + ((gameObject.transform.up * 1f)), gameObject.transform.forward, (float) 1f, mask.value);
        }

        private bool IsPressDirectionTowardsHero(float h, float v)
        {
            if ((h == 0f) && (v == 0f))
            {
                return false;
            }
            return (Mathf.Abs(Mathf.DeltaAngle(GetGlobalFacingDirection(h, v), transform.rotation.eulerAngles.y)) < 45f);
        }

        private bool IsUpFrontGrounded()
        {
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

            return Physics.Raycast(gameObject.transform.position + ((gameObject.transform.up * 3f)), gameObject.transform.forward, (float) 1.2f, mask.value);
        }

        public void Launch(Vector3 des, bool left = true, bool leviMode = false)
        {
            if (IsMounted)
            {
                Unmounted();
            }
            if (!(State is HumanAttackState))
            {
                SetState<HumanIdleState>();
            }
            Vector3 vector = des - transform.position;
            if (left)
            {
                LaunchPointLeft = des;
            }
            else
            {
                LaunchPointRight = des;
            }
            vector.Normalize();
            vector = (vector * 20f);
            if (((HookLeft != null) && (HookRight != null)) && (HookLeft.isHooked() && HookRight.isHooked()))
            {
                vector = (vector * 0.8f);
            }
            if (!Animation.IsPlaying(HeroAnim.SPECIAL_LEVI) && !Animation.IsPlaying(HeroAnim.SPECIAL_PETRA))
            {
                leviMode = false;
            }
            else
            {
                leviMode = true;
            }
            if (!leviMode)
            {
                FalseAttack();
                SetState<HumanIdleState>();
                if (UseGun)
                {
                    CrossFade(HeroAnim.AHSS_HOOK_FORWARD_BOTH, 0.1f);
                }
                else if (left && !IsRightHandHooked)
                {
                    CrossFade(HeroAnim.AIR_HOOK_L_JUST, 0.1f);
                }
                else if (!left && !IsLeftHandHooked)
                {
                    CrossFade(HeroAnim.AIR_HOOK_R_JUST, 0.1f);
                }
                else
                {
                    CrossFade(HeroAnim.DASH, 0.1f);
                    Animation[HeroAnim.DASH].time = 0f;
                }
            }
            if (left)
            {
                IsLaunchLeft = true;
            }
            if (!left)
            {
                IsLaunchRight = true;
            }
            LaunchForce = vector;
            if (!leviMode)
            {
                if (vector.y < 30f)
                {
                    LaunchForce += (Vector3.up * (30f - vector.y));
                }
                if (des.y >= transform.position.y)
                {
                    LaunchForce += ((Vector3.up * (des.y - transform.position.y)) * 10f);
                }
                Rigidbody.AddForce(LaunchForce);
            }
            FacingDirection = Mathf.Atan2(LaunchForce.x, LaunchForce.z) * Mathf.Rad2Deg;
            Quaternion quaternion = Quaternion.Euler(0f, FacingDirection, 0f);
            gameObject.transform.rotation = quaternion;
            Rigidbody.rotation = quaternion;
            TargetRotation = quaternion;
            if (left)
            {
                LaunchElapsedTimeL = 0f;
            }
            else
            {
                LaunchElapsedTimeR = 0f;
            }
            if (leviMode)
            {
                LaunchElapsedTimeR = -100f;
            }
            if (Animation.IsPlaying(HeroAnim.SPECIAL_PETRA))
            {
                LaunchElapsedTimeR = -100f;
                LaunchElapsedTimeL = -100f;
                if (HookRight != null)
                {
                    HookRight.disable();
                    ReleaseIfIHookSb();
                }
                if (HookLeft != null)
                {
                    HookLeft.disable();
                    ReleaseIfIHookSb();
                }
            }
            SparksEM.enabled = false;
        }

        public void LaunchLeftRope(float distance, Vector3 point, bool single, int mode = 0)
        {
            if (CurrentGas != 0f)
            {
                UseGas(UseGasSpeed);
                HookLeft = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0).GetComponent<Bullet>();
                GameObject obj2 = !UseGun ? hookRefL1 : hookRefL2;
                string str = !UseGun ? "hookRefL1" : "hookRefL2";
                HookLeft.transform.position = obj2.transform.position;
                float num = !single ? ((distance <= 50f) ? (distance * 0.05f) : (distance * 0.3f)) : 0f;
                Vector3 vector = (point - ((transform.right * num))) - HookLeft.transform.position;
                vector.Normalize();
                if (mode == 1)
                {
                    HookLeft.launch((vector * 3f), Rigidbody.velocity, str, true, gameObject, true);
                }
                else
                {
                    HookLeft.launch((vector * 3f), Rigidbody.velocity, str, true, gameObject, false);
                }
                LaunchPointLeft = Vector3.zero;
            }
        }

        public void LaunchRightRope(float distance, Vector3 point, bool single, int mode = 0)
        {
            if (CurrentGas == 0f)
                return;

            UseGas(UseGasSpeed);
            HookRight = PhotonNetwork.Instantiate("hook", transform.position, transform.rotation, 0).GetComponent<Bullet>();
            GameObject obj2 = !UseGun ? hookRefR1 : hookRefR2;
            string str = !UseGun ? "hookRefR1" : "hookRefR2";
            HookRight.transform.position = obj2.transform.position;
            float num = !single ? ((distance <= 50f) ? (distance * 0.05f) : (distance * 0.3f)) : 0f;
            Vector3 vector = (point + ((transform.right * num))) - HookRight.transform.position;
            vector.Normalize();

            if (mode == 1)
                HookRight.launch((vector * 5f), Rigidbody.velocity, str, false, gameObject, true);
            else
                HookRight.launch((vector * 3f), Rigidbody.velocity, str, false, gameObject, false);

            LaunchPointRight = Vector3.zero;
        }

        private void LeftArmAimTo(Vector3 target)
        {
            float y = target.x - UpperarmL.transform.position.x;
            float num2 = target.y - UpperarmL.transform.position.y;
            float x = target.z - UpperarmL.transform.position.z;
            float num4 = Mathf.Sqrt((y * y) + (x * x));
            HandL.localRotation = Quaternion.Euler(90f, 0f, 0f);
            ForearmL.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            UpperarmL.rotation = Quaternion.Euler(0f, 90f + (Mathf.Atan2(y, x) * Mathf.Rad2Deg), -Mathf.Atan2(num2, num4) * Mathf.Rad2Deg);
        }

        public void MarkDie()
        {
            HasDied = true;
            SetState<HumanDieState>();
        }

        [PunRPC]
        private void Net3DMGSMOKE(bool ifON)
        {
            if (particle_Smoke_3dmg == null)
                return;

            smoke_3dmg_em.enabled = ifON;
        }

        [PunRPC]
        public void NetDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true, PhotonMessageInfo info = new PhotonMessageInfo())
        {
            if (photonView.isMine && (GameSettings.Gamemode.GamemodeType != GamemodeType.TitanRush))
            {
                if (FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
                {
                    photonView.RPC(nameof(BackToHumanRPC), PhotonTargets.Others, new object[0]);
                    return;
                }

                if (!info.sender.isLocal && !info.sender.isMasterClient)
                {
                    if ((info.sender.CustomProperties[PhotonPlayerProperty.name] == null) || (info.sender.CustomProperties[PhotonPlayerProperty.isTitan] == null))
                        FengGameManagerMKII.instance.chatRoom.AddMessage("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID.ToString() + "</color>");
                    else if (viewID < 0)
                    {
                        if (titanName == "")
                            FengGameManagerMKII.instance.chatRoom.AddMessage("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID.ToString() + " (possibly valid).</color>");
                        else
                            FengGameManagerMKII.instance.chatRoom.AddMessage("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID.ToString() + "</color>");
                    }
                    else if (PhotonView.Find(viewID) == null)
                        FengGameManagerMKII.instance.chatRoom.AddMessage("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID.ToString() + "</color>");
                    else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                        FengGameManagerMKII.instance.chatRoom.AddMessage("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID.ToString() + "</color>");
                }
            }

            if (PhotonNetwork.isMasterClient)
            {
                int iD = photonView.owner.ID;

                if (FengGameManagerMKII.heroHash.ContainsKey(iD))
                    FengGameManagerMKII.heroHash.Remove(iD);
            }

            if (photonView.isMine)
            {
                var vector = (Vector3.up * 5000f);

                if (MyBomb != null)
                    MyBomb.destroyMe();

                if (MyCannon != null)
                    PhotonNetwork.Destroy(MyCannon);

                if (TitanForm && (ErenTitan != null))
                    ErenTitan.lifeTime = 0.1f;

                if (SkillCD != null)
                    SkillCD.transform.localPosition = vector;

            }

            if (HookLeft != null)
                HookLeft.removeMe();

            if (HookRight != null)
                HookRight.removeMe();

            meatDie.Play();
            FalseAttack();
            BreakApart(v, isBite);
            if (photonView.isMine)
            {
                CurrentInGameCamera.SetSpectorMode(false);
                CurrentInGameCamera.gameOver = true;
                FengGameManagerMKII.instance.myRespawnTime = 0f;
            }
            HasDied = true;
            var audioDie = transform.Find("audio_die");
            if (audioDie != null)
            {
                audioDie.parent = null;
                audioDie.GetComponent<AudioSource>().Play();
            }
            SmoothSync.disabled = true;
            if (photonView.isMine)
            {
                PhotonNetwork.RemoveRPCs(photonView);
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.dead, true);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.deaths, RCextensions.returnIntFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.deaths]) + 1);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                if (viewID != -1)
                {
                    PhotonView view2 = PhotonView.Find(viewID);
                    if (view2 != null)
                    {
                        FengGameManagerMKII.instance.sendKillInfo(killByTitan, $"[{info.sender.ID.ToString().Color("ffc000")}] {RCextensions.returnStringFromObject(view2.owner.CustomProperties[PhotonPlayerProperty.name])}", false, RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
                        propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                        propertiesToSet.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(view2.owner.CustomProperties[PhotonPlayerProperty.kills]) + 1);
                        view2.owner.SetCustomProperties(propertiesToSet);
                    }
                }
                else
                {
                    FengGameManagerMKII.instance.sendKillInfo(!(titanName == string.Empty), $"[{info.sender.ID.ToString().Color("ffc000")}] {titanName}", false, RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
                }
            }
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }

        [PunRPC]
        public void NetDie2(int viewID = -1, string titanName = "", PhotonMessageInfo info = new PhotonMessageInfo())
        {
            GameObject obj2;
            if ((photonView.isMine) && (GameSettings.Gamemode.GamemodeType != GamemodeType.TitanRush))
            {
                if (FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
                {
                    photonView.RPC(nameof(BackToHumanRPC), PhotonTargets.Others, new object[0]);
                    return;
                }

                if (!info.sender.IsLocal && !info.sender.IsMasterClient)
                {
                    if ((info.sender.CustomProperties[PhotonPlayerProperty.name] == null) || (info.sender.CustomProperties[PhotonPlayerProperty.isTitan] == null))
                        FengGameManagerMKII.instance.chatRoom.AddMessage($"Unusual Kill from ID {info.sender.ID}".Color("FFCC00"));
                    else if (viewID < 0)
                    {
                        if (titanName == "")
                            FengGameManagerMKII.instance.chatRoom.AddMessage($"Unusual Kill from ID {info.sender.ID} (possibly valid).".Color("FFCC00"));
                        else if (GameSettings.PvP.Bomb.Value && (!GameSettings.PvP.Cannons.Value))
                            FengGameManagerMKII.instance.chatRoom.AddMessage($"Unusual Kill from ID {info.sender.ID}".Color("FFCC00"));
                    }
                    else if (PhotonView.Find(viewID) == null)
                        FengGameManagerMKII.instance.chatRoom.AddMessage($"Unusual Kill from ID {info.sender.ID}".Color("FFCC00"));
                    else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
                        FengGameManagerMKII.instance.chatRoom.AddMessage($"Unusual Kill from ID {info.sender.ID}".Color("FFCC00"));
                }
            }
            if (photonView.isMine)
            {
                Vector3 vector = (Vector3.up * 5000f);

                if (MyBomb != null)
                    MyBomb.destroyMe();

                if (MyCannon != null)
                    PhotonNetwork.Destroy(MyCannon);

                PhotonNetwork.RemoveRPCs(photonView);

                if (TitanForm && (ErenTitan != null))
                    ErenTitan.lifeTime = 0.1f;

                if (SkillCD != null)
                    SkillCD.transform.localPosition = vector;
            }
            meatDie.Play();
            if (HookLeft != null)
                HookLeft.removeMe();

            if (HookRight != null)
                HookRight.removeMe();

            Transform audioDie = transform.Find("audio_die");
            audioDie.parent = null;
            audioDie.GetComponent<AudioSource>().Play();
            if (photonView.isMine)
            {
                CurrentInGameCamera.SetMainObject(null, true, false);
                CurrentInGameCamera.SetSpectorMode(true);
                CurrentInGameCamera.gameOver = true;
                FengGameManagerMKII.instance.myRespawnTime = 0f;
            }
            FalseAttack();
            HasDied = true;
            SmoothSync.disabled = true;
            if (photonView.isMine)
            {
                PhotonNetwork.RemoveRPCs(photonView);
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.dead, true);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.deaths, ((int) PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.deaths]) + 1);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                if (viewID != -1)
                {
                    PhotonView view2 = PhotonView.Find(viewID);
                    if (view2 != null)
                    {
                        FengGameManagerMKII.instance.sendKillInfo(true,
                            $"{info.sender.ID.ToString().Color("ffc000")} " +
                            $"{RCextensions.returnStringFromObject(view2.owner.CustomProperties[PhotonPlayerProperty.name])}", false,
                            RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
                        propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                        propertiesToSet.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(view2.owner.CustomProperties[PhotonPlayerProperty.kills]) + 1);
                        view2.owner.SetCustomProperties(propertiesToSet);
                    }
                }
                else
                    FengGameManagerMKII.instance.sendKillInfo(true, $"{info.sender.ID.ToString().Color("ffc000")} {titanName}",
                        false, RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
            }

            if (photonView.isMine)
                obj2 = PhotonNetwork.Instantiate("hitMeat2", audioDie.position, Quaternion.Euler(270f, 0f, 0f), 0);
            else
                obj2 = Instantiate(Resources.Load<GameObject>("hitMeat2"));

            obj2.transform.position = audioDie.position;
            if (photonView.isMine)
                PhotonNetwork.Destroy(photonView);

            if (PhotonNetwork.isMasterClient)
            {
                int iD = photonView.owner.ID;
                if (FengGameManagerMKII.heroHash.ContainsKey(iD))
                    FengGameManagerMKII.heroHash.Remove(iD);
            }
        }

        public void NetDieLocal(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
        {
            if (photonView.isMine)
            {
                Vector3 vector = (Vector3.up * 5000f);
                if (TitanForm && (ErenTitan != null))
                    ErenTitan.lifeTime = 0.1f;

                if (MyBomb != null)
                    MyBomb.destroyMe();

                if (MyCannon != null)
                    PhotonNetwork.Destroy(MyCannon);

                if (SkillCD != null)
                    SkillCD.transform.localPosition = vector;
            }

            if (HookLeft != null)
                HookLeft.removeMe();

            if (HookRight != null)
                HookRight.removeMe();

            meatDie.Play();
            FalseAttack();
            BreakApart(v, isBite);
            if (photonView.isMine)
            {
                CurrentInGameCamera.SetSpectorMode(false);
                CurrentInGameCamera.gameOver = true;
                FengGameManagerMKII.instance.myRespawnTime = 0f;
            }
            HasDied = true;
            Transform audioDie = transform.Find("audio_die");
            audioDie.parent = null;
            audioDie.GetComponent<AudioSource>().Play();
            SmoothSync.disabled = true;
            if (photonView.isMine)
            {
                PhotonNetwork.RemoveRPCs(photonView);
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.dead, true);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add(PhotonPlayerProperty.deaths, RCextensions.returnIntFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.deaths]) + 1);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                if (viewID != -1)
                {
                    var view = PhotonView.Find(viewID);
                    if (view != null)
                    {
                        FengGameManagerMKII.instance.sendKillInfo(killByTitan,
                            RCextensions.returnStringFromObject(view.owner.CustomProperties[PhotonPlayerProperty.name]),
                            false, RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
                        propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                        propertiesToSet.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(view.owner.CustomProperties[PhotonPlayerProperty.kills]) + 1);
                        view.owner.SetCustomProperties(propertiesToSet);
                    }
                }
                else
                    FengGameManagerMKII.instance.sendKillInfo(!(titanName == string.Empty), titanName,
                        false, RCextensions.returnStringFromObject(PhotonNetwork.player.CustomProperties[PhotonPlayerProperty.name]), 0);
            }
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(photonView);
            }
            if (PhotonNetwork.isMasterClient)
            {
                int iD = photonView.owner.ID;
                if (FengGameManagerMKII.heroHash.ContainsKey(iD))
                {
                    FengGameManagerMKII.heroHash.Remove(iD);
                }
            }
        }

        [PunRPC]
        public void NetGrabbed(int id, bool leftHand)
        {
            TitanWhoGrabMeID = id;
            NetPlayAnimation("grabbed");
            Grabbed(PhotonView.Find(id).gameObject, leftHand);
        }

        [PunRPC]
        public void NetlaughAttack()
        {
            throw new NotImplementedException("Titan laugh attack is not implemented yet");
        }

        [PunRPC]
        public void NetSetIsGrabbedFalse() => SetState<HumanIdleState>();

        [PunRPC]
        public void NetTauntAttack(float tauntTime, float distance = 100f)
        {
            throw new NotImplementedException("Titan taunt behavior is not yet implemented");
        }

        [PunRPC]
        public void NetUngrabbed()
        {
            BreakFreeFromGrab();
            NetPlayAnimation(StandAnimation);
            FalseAttack();
        }

        public void ReleaseIfIHookSb()
        {
            if (!HookSomeone || HookTarget == null)
                return;

            HookTarget.GetPhotonView().RPC(nameof(BadGuyReleaseMe), HookTarget.GetPhotonView().owner, new object[0]);
            HookTarget = null;
            HookSomeone = false;
        }

        public IEnumerator ReloadSky()
        {
            yield return new WaitForSeconds(0.5f);

            if ((FengGameManagerMKII.skyMaterial != null) && (Camera.main.GetComponent<Skybox>().material != FengGameManagerMKII.skyMaterial))
                Camera.main.GetComponent<Skybox>().material = FengGameManagerMKII.skyMaterial;
        }

        public void ResetAnimationSpeed()
        {
            var enumerator = Animation.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current as AnimationState;
                    if (current != null)
                        current.speed = 1f;
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            CustomAnimationSpeed();
        }

        [PunRPC]
        public void ReturnFromCannon(PhotonMessageInfo info)
        {
            if (info.sender != photonView.owner)
                return;

            IsCannon = false;
            SmoothSync.disabled = false;
        }

        private void RightArmAimTo(Vector3 target)
        {
            var y = target.x - UpperarmR.transform.position.x;
            var num2 = target.y - UpperarmR.transform.position.y;
            var x = target.z - UpperarmR.transform.position.z;
            var num4 = Mathf.Sqrt((y * y) + (x * x));
            HandR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            ForearmR.localRotation = Quaternion.Euler(90f, 0f, 0f);
            UpperarmR.rotation = Quaternion.Euler(180f, 90f + (Mathf.Atan2(y, x) * Mathf.Rad2Deg), Mathf.Atan2(num2, num4) * Mathf.Rad2Deg);
        }

        [PunRPC]
        private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
        {
            HookedBySomeone = true;
            BadGuy = PhotonView.Find(hooker).gameObject;
            if (Vector3.Distance(hookPosition, transform.position) < 15f)
            {
                LaunchForce = PhotonView.Find(hooker).gameObject.transform.position - transform.position;
                Rigidbody.AddForce((-Rigidbody.velocity * 0.9f), ForceMode.VelocityChange);
                var num = Mathf.Pow(LaunchForce.magnitude, 0.1f);

                if (IsGrounded)
                    Rigidbody.AddForce((Vector3.up * Mathf.Min(LaunchForce.magnitude * 0.2f, 10f)), ForceMode.Impulse);

                Rigidbody.AddForce(((LaunchForce * num) * 0.1f), ForceMode.Impulse);
                if (!(State is HumanGrabState))
                {
                    DashTime = 1f;
                    CrossFade(HeroAnim.DASH, 0.05f);
                    Animation[HeroAnim.DASH].time = 0.1f;
                    SetState<HumanAirDodgeState>();
                    FalseAttack();
                    FacingDirection = Mathf.Atan2(LaunchForce.x, LaunchForce.z) * Mathf.Rad2Deg;
                    Quaternion quaternion = Quaternion.Euler(0f, FacingDirection, 0f);
                    gameObject.transform.rotation = quaternion;
                    Rigidbody.rotation = quaternion;
                    TargetRotation = quaternion;
                }
            }
            else
            {
                HookedBySomeone = false;
                BadGuy = null;
                PhotonView.Find(hooker).RPC(nameof(HookFail), PhotonView.Find(hooker).owner, new object[0]);
            }
        }

        private void SetHookedPplDirection()
        {
            AlmostSingleHook = false;
            if (IsRightHandHooked && IsLeftHandHooked)
            {
                if ((HookLeft != null) && (HookRight != null))
                {
                    Vector3 normal = HookLeft.transform.position - HookRight.transform.position;
                    if (normal.sqrMagnitude < 4f)
                    {
                        Vector3 vector2 = (((HookLeft.transform.position + HookRight.transform.position) * 0.5f)) - transform.position;
                        FacingDirection = Mathf.Atan2(vector2.x, vector2.z) * Mathf.Rad2Deg;
                        if (UseGun && !(State is HumanAttackState))
                        {
                            var current = -Mathf.Atan2(Rigidbody.velocity.z, Rigidbody.velocity.x) * Mathf.Rad2Deg;
                            var target = -Mathf.Atan2(vector2.z, vector2.x) * Mathf.Rad2Deg;
                            var num3 = -Mathf.DeltaAngle(current, target);
                            FacingDirection += num3;
                        }
                        AlmostSingleHook = true;
                    }
                    else
                    {
                        var to = transform.position - HookLeft.transform.position;
                        var vector6 = transform.position - HookRight.transform.position;
                        var vector7 = ((HookLeft.transform.position + HookRight.transform.position) * 0.5f);
                        var from = transform.position - vector7;
                        if ((Vector3.Angle(from, to) < 30f) && (Vector3.Angle(from, vector6) < 30f))
                        {
                            AlmostSingleHook = true;
                            var vector9 = vector7 - transform.position;
                            FacingDirection = Mathf.Atan2(vector9.x, vector9.z) * Mathf.Rad2Deg;
                        }
                        else
                        {
                            AlmostSingleHook = false;
                            var forward = transform.forward;
                            Vector3.OrthoNormalize(ref normal, ref forward);
                            FacingDirection = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
                            var num4 = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;
                            if (Mathf.DeltaAngle(num4, FacingDirection) > 0f)
                            {
                                FacingDirection += 180f;
                            }
                        }
                    }
                }
            }
            else
            {
                AlmostSingleHook = true;
                Vector3 zero;
                if (IsRightHandHooked && (HookRight != null))
                    zero = HookRight.transform.position - transform.position;
                else
                {
                    if (!IsLeftHandHooked || (HookLeft == null))
                        return;

                    zero = HookLeft.transform.position - transform.position;
                }
                FacingDirection = Mathf.Atan2(zero.x, zero.z) * Mathf.Rad2Deg;
                if (!(State is HumanAttackState))
                {
                    var num6 = -Mathf.Atan2(Rigidbody.velocity.z, Rigidbody.velocity.x) * Mathf.Rad2Deg;
                    var num7 = -Mathf.Atan2(zero.z, zero.x) * Mathf.Rad2Deg;
                    var num8 = -Mathf.DeltaAngle(num6, num7);
                    if (UseGun)
                        FacingDirection += num8;
                    else
                    {
                        float num9;
                        if ((IsLeftHandHooked && (num8 < 0f)) || (IsRightHandHooked && (num8 > 0f)))
                            num9 = -0.1f;
                        else
                            num9 = 0.1f;

                        FacingDirection += num8 * num9;
                    }
                }
            }
        }

        [PunRPC]
        public void SetMyCannon(int viewID, PhotonMessageInfo info)
        {
            if (info.sender != photonView.owner)
                return;

            var view = PhotonView.Find(viewID);
            if (view == null)
                return;

            MyCannon = view.gameObject;
            if (MyCannon == null)
                return;

            MyCannonBase = MyCannon.transform;
            MyCannonPlayer = MyCannonBase.Find("PlayerPoint");
            IsCannon = true;
        }

        [PunRPC]
        public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
        {
            if (photonView.owner != info.sender)
                return;

            CameraMultiplier = offset;
            SmoothSync.PhotonCamera = true;
            IsPhotonCamera = true;
        }

        [PunRPC]
        private void SetMyTeam(int val)
        {
            MyTeam = val;
            checkBoxLeft.myTeam = val;
            checkBoxRight.myTeam = val;

            if (!PhotonNetwork.isMasterClient)
                return;

            object[] objArray;
            //TODO: Sync these upon gamemode syncSettings
            if (GameSettings.PvP.Mode == PvpMode.AhssVsBlades)
            {
                var num = 0;
                if (photonView.owner.CustomProperties[PhotonPlayerProperty.RCteam] != null)
                    num = RCextensions.returnIntFromObject(photonView.owner.CustomProperties[PhotonPlayerProperty.RCteam]);

                if (val != num)
                {
                    objArray = new object[] { num };
                    photonView.RPC(nameof(SetMyTeam), PhotonTargets.AllBuffered, objArray);
                }
            }
            else if (GameSettings.PvP.Mode == PvpMode.FreeForAll && (val != photonView.owner.ID))
            {
                objArray = new object[] { photonView.owner.ID };
                photonView.RPC(nameof(SetMyTeam), PhotonTargets.AllBuffered, objArray);
            }
        }

        public void SetTeam(int team)
        {
            if (!photonView.isMine)
            {
                SetMyTeam(team);
                return;
            }

            var parameters = new object[] { team };
            photonView.RPC(nameof(SetMyTeam), PhotonTargets.AllBuffered, parameters);
            var propertiesToSet = new ExitGames.Client.Photon.Hashtable
            {
                { PhotonPlayerProperty.team, team }
            };
            PhotonNetwork.player.SetCustomProperties(propertiesToSet);
        }

        public void ShootFlare(int type)
        {
            var flare = Service.Inventory.GetItems<Items.Flare>()[type - 1];
            flare.Use(this);
        }

        [PunRPC]
        public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
        {
            if (!info.sender.isMasterClient || !photonView.isMine || MyCannon)
                return;

            if (Horse && IsMounted)
                GetOffHorse();

            SetState<HumanIdleState>();

            if (HookLeft)
                HookLeft.removeMe();

            if (HookRight)
                HookRight.removeMe();

            if (smoke_3dmg_em.enabled && photonView.isMine)
            {
                var parameters = new object[] { false };
                photonView.RPC(nameof(Net3DMGSMOKE), PhotonTargets.Others, parameters);
            }
            smoke_3dmg_em.enabled = false;
            Rigidbody.velocity = Vector3.zero;
            var strArray = settings.Split(new char[] { ',' });

            if (strArray.Length > 15)
                MyCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1],
                    new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])),
                    new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[0x10]), Convert.ToSingle(strArray[0x11]), Convert.ToSingle(strArray[0x12])), 0);
            else
                MyCannon = PhotonNetwork.Instantiate("RCAsset/" + strArray[1],
                    new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])),
                    new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])), 0);

            MyCannonBase = MyCannon.transform;
            MyCannonPlayer = MyCannon.transform.Find("PlayerPoint");
            IsCannon = true;
            MyCannon.GetComponent<Cannon>().myHero = this;
            MyCannonRegion = null;
            CurrentInGameCamera.SetMainObject(MyCannon.transform.Find("Barrel").Find("FiringPoint").gameObject, true, false);
            CurrentCamera.fieldOfView = 55f;
            photonView.RPC(nameof(SetMyCannon), PhotonTargets.OthersBuffered, new object[] { MyCannon.GetPhotonView().viewID });
            SkillCDLastCannon = SkillCDLast;
            SkillCDLast = 3.5f;
            SkillCDDuration = 3.5f;
        }

        public void SetHorse()
        {
            if (!photonView.isMine)
                return;

            if (GameSettings.Horse.Enabled.Value && Horse == null)
            {
                var position = transform.position + Vector3.up * 5f;
                var rotation = transform.rotation;
                Horse = Horse.Create(this, position, rotation);
            }

            if (!GameSettings.Horse.Enabled.Value && Horse != null)
                PhotonNetwork.Destroy(Horse);
        }

        public IEnumerator StopImmunity()
        {
            yield return new WaitForSeconds(5f);
            BombImmune = false;
        }

        private void Suicide()
        {
            NetDieLocal((Rigidbody.velocity * 50f), false, -1, string.Empty, true);
            FengGameManagerMKII.instance.needChooseSide = true;
        }

        public void BreakFreeFromGrab()
        {
            FacingDirection = 0f;
            TargetRotation = Quaternion.Euler(0f, 0f, 0f);
            transform.parent = null;
            GetComponent<CapsuleCollider>().isTrigger = false;
            SetState<HumanIdleState>(true);
            photonView.RPC(nameof(NetSetIsGrabbedFalse), PhotonTargets.All, new object[0]);
            if (PhotonNetwork.isMasterClient)
                TitanWhoGrabMe.GetComponent<MindlessTitan>().GrabEscapeRpc();
            else
                PhotonView.Find(TitanWhoGrabMeID).RPC(nameof(MindlessTitan.GrabEscapeRpc), PhotonTargets.MasterClient, new object[0]);
        }

        private void Unmounted()
        {
            Horse.GetComponent<Horse>().Unmount();
            IsMounted = false;
        }


        public void UpdateCannon()
        {
            transform.position = MyCannonPlayer.position;
            transform.rotation = MyCannonBase.rotation;
        }

        public void UpdateExt()
        {
            if (!(Skill is BombPvpSkill))
                return;

            if (InputManager.KeyDown(InputHuman.AttackSpecial) && (SkillCDDuration <= 0f))
            {
                if (!((MyBomb == null) || MyBomb.disabled))
                    MyBomb.Explode(BombRadius);

                Detonate = false;
                SkillCDDuration = BombCD;
                var hitInfo = new RaycastHit();
                var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

                CurrentV = transform.position;
                TargetV = CurrentV + ((Vector3.forward * 200f));

                if (Physics.Raycast(ray, out hitInfo, 1000000f, mask.value))
                    TargetV = hitInfo.point;

                var vector = Vector3.Normalize(TargetV - CurrentV);
                var obj2 = PhotonNetwork.Instantiate(bombMainPath, CurrentV + ((vector * 4f)), new Quaternion(0f, 0f, 0f, 1f), 0);
                obj2.GetComponent<Rigidbody>().velocity = (vector * BombSpeed);
                MyBomb = obj2.GetComponent<Bomb>();
                BombTime = 0f;
            }
            else if ((MyBomb != null) && !MyBomb.disabled)
            {
                BombTime += Time.deltaTime;
                var flag2 = false;

                if (InputManager.KeyUp(InputHuman.AttackSpecial))
                    Detonate = true;
                else if (InputManager.KeyDown(InputHuman.AttackSpecial) && Detonate)
                {
                    Detonate = false;
                    flag2 = true;
                }

                if (BombTime >= BombTimeMax)
                    flag2 = true;

                if (flag2)
                {
                    MyBomb.Explode(BombRadius);
                    Detonate = false;
                }
            }
        }

        private void UseGas(float amount)
        {
            CurrentGas = Mathf.MoveTowards(CurrentGas, 0f, amount);

            OnUseGas?.Invoke(CurrentGas / TotalGas);
            if (photonView.isMine)
                OnUseGasClient?.Invoke(this, CurrentGas / TotalGas);
        }

        [PunRPC]
        private void WhoIsMyErenTitan(int id)
        {
            ErenTitan = PhotonView.Find(id).gameObject.GetComponent<ErenTitan>();
            TitanForm = true;
        }
        #endregion

        private void OnGUI()
        {
            var style = new GUIStyle { fontSize = 50, richText = true };
            GUI.TextField(new Rect(100f, 100f, 300f, 100f), $"<color=white>Anim:\t {CurrentAnimation}\nState:\t {State.GetType().Name}</color>", style);
        }
    }
}