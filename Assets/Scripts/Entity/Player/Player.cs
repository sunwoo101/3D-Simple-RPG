using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Entity.Player
{
    // Require components
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(MovementHandler))]
    [RequireComponent(typeof(AttackHandler))]

    public class Player : Entity
    {
        #region enums

        public enum StateMachine
        {
            Idle,
            Walking,
            Sprinting,
            Jumping,
            Aiming,
            Attacking,
            Death
        }

        #endregion

        #region Variables

        // State machine
        protected StateMachine m_StateMachine;
        protected StateMachine m_StateBuffer; // When doing another action store a state buffer to return to the buffered state after doing the other action

        // References
        [SerializeField] protected PlayerAttributesPersistentScriptableObject m_PlayerAttributesPersistentSO;
        protected InputHandler m_InputHandler;
        protected MovementHandler m_MovementHandler;
        protected CameraController m_CameraController;
        protected AttackHandler m_AttackHandler;
        protected Animator m_Animator;
        protected PlayerHUD m_PlayerHUD;
        [SerializeField] protected GameObject m_CriticalArrowIndicator;
        [SerializeField] protected Image m_DeathDim;
        [SerializeField] protected float m_DeathDimSpeed;

        // Attributes
        [SerializeField] protected float m_AdditionalAttackDamagePerLevel;

        #endregion

        protected void LoadData()
        {
            SaveData data = SaveSystem.Load();

            if (data != null)
            {
                m_PlayerAttributesPersistentSO.m_Level = data.level;
                m_PlayerAttributesPersistentSO.m_Experience = data.experience;
            }
            else
            {
                m_PlayerAttributesPersistentSO.m_Level = 1;
                m_PlayerAttributesPersistentSO.m_Experience = 0;
            }
        }

        internal void SaveData()
        {
            SaveData data = new SaveData(m_PlayerAttributesPersistentSO.m_Level, m_PlayerAttributesPersistentSO.m_Experience);

            SaveSystem.Save(data);
        }

        protected override void Start()
        {
            // Initial state
            m_StateMachine = StateMachine.Idle;
            m_StateBuffer = StateMachine.Idle;

            // References
            m_InputHandler = GetComponent<InputHandler>();
            m_MovementHandler = GetComponent<MovementHandler>();
            m_CameraController = Camera.main.GetComponent<CameraController>();
            m_AttackHandler = GetComponent<AttackHandler>();
            m_Animator = GetComponent<Animator>();
            m_PlayerHUD = GetComponent<PlayerHUD>();

            // Input handler setup
            InputHandler.s_InputMode = InputHandler.InputMode.KBMouse;
            m_InputHandler.Enable();

            // Lock cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Load data
            LoadData();
            m_PlayerHUD.SetLevelText(m_PlayerAttributesPersistentSO.m_Level);
            m_PlayerAttributesPersistentSO.UpdateExperienceRequired();
            m_PlayerHUD.SetExperienceBar(m_PlayerAttributesPersistentSO.m_ExperienceRequiredForLevel, m_PlayerAttributesPersistentSO.m_Experience);

            // HUD setup
            ResetHealth();
            m_PlayerHUD.SetHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_PlayerAttributesPersistentSO.m_Health);
            m_PlayerHUD.SetChargeIndicator(m_EntityBaseAttributesSO.m_BowMaxChargeTime, m_AttackHandler.m_Charge);
            m_CriticalArrowIndicator.SetActive(false);
        }

        protected override void Update()
        {
            m_InputHandler.GetInput();

            m_MovementHandler.GroundCheck();

            switch (m_StateMachine)
            {
                case StateMachine.Idle:
                    IdleState();
                    break;

                case StateMachine.Walking:
                    WalkingState();
                    break;

                case StateMachine.Sprinting:
                    SprintingState();
                    break;

                case StateMachine.Jumping:
                    JumpingState();
                    break;

                case StateMachine.Aiming:
                    AimingState();
                    break;

                case StateMachine.Attacking:
                    AttackingState();
                    break;

                case StateMachine.Death:
                    DeathState();
                    break;

                default:
                    m_StateMachine = StateMachine.Idle;
                    break;
            }

            m_MovementHandler.Gravity();

            m_CameraController.RotateCamera(m_InputHandler.m_Camera, 1);
            m_CameraController.OffsetCamera(transform);

#if UNITY_EDITOR
            DebugStuff();
#endif
        }

        #region State Methods

        protected void IdleState()
        {
            // Sprint
            if (Sprint(m_EntityBaseAttributesSO.m_MovementSpeed, m_EntityBaseAttributesSO.m_SprintSpeedMultiplier))
            {
                m_StateBuffer = m_StateMachine;
                m_StateMachine = StateMachine.Sprinting;
            }
            // Walk
            else if (Walk(m_EntityBaseAttributesSO.m_MovementSpeed))
                m_StateMachine = StateMachine.Walking;

            // Jump
            if (Jump())
            {
                m_StateBuffer = m_StateMachine;
                m_StateMachine = StateMachine.Jumping;
            }

            // Aim
            if (m_InputHandler.m_SecondaryAttack)
            {
                m_StateBuffer = StateMachine.Idle;
                m_StateMachine = StateMachine.Aiming;
            }

            m_Animator.SetBool("Idle", true);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", false);
        }

        protected void WalkingState()
        {
            // Sprint
            if (Sprint(m_EntityBaseAttributesSO.m_MovementSpeed, m_EntityBaseAttributesSO.m_SprintSpeedMultiplier))
            {
                m_StateBuffer = m_StateMachine;
                m_StateMachine = StateMachine.Sprinting;
            }
            // Walk
            else if (!Walk(m_EntityBaseAttributesSO.m_MovementSpeed))
            {
                m_StateMachine = StateMachine.Idle;
            }

            // Jump
            if (Jump())
            {
                m_StateBuffer = m_StateMachine;
                m_StateMachine = StateMachine.Jumping;
            }

            // Aim
            if (m_InputHandler.m_SecondaryAttack)
            {
                m_StateBuffer = StateMachine.Idle;
                m_StateMachine = StateMachine.Aiming;
            }

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", true);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", false);
        }

        protected void SprintingState()
        {
            // Sprint
            if (!Sprint(m_EntityBaseAttributesSO.m_MovementSpeed, m_EntityBaseAttributesSO.m_SprintSpeedMultiplier))
                m_StateMachine = StateMachine.Walking;

            // Jump
            if (Jump())
            {
                m_StateBuffer = m_StateMachine;
                m_StateMachine = StateMachine.Jumping;
            }

            // Aim
            if (m_InputHandler.m_SecondaryAttack)
            {
                m_StateBuffer = StateMachine.Idle;
                m_StateMachine = StateMachine.Aiming;
            }

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", true);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", false);
        }

        protected void JumpingState()
        {
            // Move in air
            if (!Sprint(m_EntityBaseAttributesSO.m_MovementSpeed, m_EntityBaseAttributesSO.m_SprintSpeedMultiplier))
                Walk(m_EntityBaseAttributesSO.m_MovementSpeed);

            // Return to state buffer
            if (m_MovementHandler.IsGrounded())
                m_StateMachine = m_StateBuffer;

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", true);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", false);
        }

        protected void AimingState()
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            transform.forward = forward; // Set player rotation to the direction the player is aiming

            // Attack
            float chargeMultiplier = m_AttackHandler.m_Charge / m_EntityBaseAttributesSO.m_BowMaxChargeTime;
            if (m_InputHandler.m_PrimaryAttack && chargeMultiplier >= m_AttackHandler.m_MinimumChargeMultiplier)
            {
                Attack();
                m_AttackHandler.m_Charge = 0;
                m_PlayerHUD.SetChargeIndicator(m_EntityBaseAttributesSO.m_BowMaxChargeTime, m_AttackHandler.m_Charge);
                m_StateMachine = StateMachine.Attacking;
            }

            // Aim
            if (m_InputHandler.m_SecondaryAttack)
            {
                CameraZoom();

                // Bow charging
                float chargeAmount = Time.deltaTime;
                if (m_EntityBaseAttributesSO.m_BowMaxChargeTime - m_AttackHandler.m_Charge >= chargeAmount)
                {
                    m_AttackHandler.m_Charge += chargeAmount;
                    m_PlayerHUD.SetChargeIndicator(m_EntityBaseAttributesSO.m_BowMaxChargeTime, m_AttackHandler.m_Charge);
                }
                else
                {
                    m_AttackHandler.m_Charge = m_EntityBaseAttributesSO.m_BowMaxChargeTime;
                    m_PlayerHUD.SetChargeIndicator(m_EntityBaseAttributesSO.m_BowMaxChargeTime, m_AttackHandler.m_Charge);
                }
            }
            else
            {
                // Return to state buffer
                if (m_CameraController.ZoomOut())
                {
                    m_AttackHandler.m_Charge = 0;
                    m_PlayerHUD.SetChargeIndicator(m_EntityBaseAttributesSO.m_BowMaxChargeTime, m_AttackHandler.m_Charge);
                    m_StateMachine = StateMachine.Idle;
                }
            }

            if (m_AttackHandler.m_Charge == 1)
            {
                m_CriticalArrowIndicator.SetActive(true);
            }
            else
            {
                m_CriticalArrowIndicator.SetActive(false);
            }

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", true);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", false);
        }

        protected void AttackingState()
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            transform.forward = forward; // Set player rotation to the direction the player is aiming

            m_StateMachine = StateMachine.Aiming;

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", true);
            m_Animator.SetBool("Death", false);
        }

        protected void DeathState()
        {
            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Sprint", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Aim", false);
            m_Animator.SetBool("Attack", false);
            m_Animator.SetBool("Death", true);

            Color color = m_DeathDim.color;
            color.a += m_DeathDimSpeed * Time.deltaTime;
            m_DeathDim.color = color;

            if (color.a >= 1f)
            {
                SaveData();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        #endregion

        #region Actions

        protected bool Jump()
        {
            if (m_MovementHandler.Jump(m_InputHandler.m_Jump, m_EntityBaseAttributesSO.m_JumpHeight))
                return true;
            else
                return false;
        }

        protected bool Walk(float movementSpeed)
        {
            if (m_MovementHandler.Move(m_InputHandler.m_Movement, m_CameraController.transform, movementSpeed, m_InputHandler.m_LockMovement, m_CameraController))
                return true;
            else
                return false;
        }

        protected bool Sprint(float movementSpeed, float movementSpeedMultiplier)
        {
            if (m_InputHandler.m_Sprint && m_MovementHandler.Move(m_InputHandler.m_Movement, m_CameraController.transform, movementSpeed * movementSpeedMultiplier, m_InputHandler.m_LockMovement, m_CameraController))
                return true;
            else
                return false;
        }

        protected bool Attack()
        {
            float chargeMultiplier = m_AttackHandler.m_Charge / m_EntityBaseAttributesSO.m_BowMaxChargeTime;
            if (chargeMultiplier != 1)
            {
                chargeMultiplier /= 2; // Half damage if the charge is not full
            }

            float attackDamage = m_EntityBaseAttributesSO.m_BaseAttackDamage + m_PlayerAttributesPersistentSO.m_Level * m_AdditionalAttackDamagePerLevel;
            float knockback = chargeMultiplier == 1 ? m_EntityBaseAttributesSO.m_BowKnockback : 0; // Apply knockback if bow is fully charged
            if (m_AttackHandler.Attack(m_InputHandler.m_PrimaryAttack, attackDamage * chargeMultiplier, TeamID(), m_CameraController.transform, transform, knockback))
                return true;
            else
                return false;
        }

        protected bool CameraZoom()
        {
            if (m_InputHandler.m_SecondaryAttack)
            {
                m_CameraController.ZoomIn();
                return true;
            }
            else
            {
                m_CameraController.ZoomOut();
                return false;
            }
        }

        #endregion

        #region Debugging

        [SerializeField] private Text m_StateMachineDebugText;
        [SerializeField] private Text m_HealthDebugText;

        private void DebugStuff()
        {
            m_StateMachineDebugText.text = "State: " + m_StateMachine;
            m_HealthDebugText.text = "Health: " + m_PlayerAttributesPersistentSO.m_Health;

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SceneManager.LoadScene(0);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // TakeDamage(10, transform);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                NewGame();
            }

            //Debug.Log(m_StateMachine);
        }

        private void NewGame()
        {
            // Attributes
            m_PlayerAttributesPersistentSO.m_Health = m_EntityBaseAttributesSO.m_BaseHealth;
            m_PlayerAttributesPersistentSO.m_AttackDamage = m_EntityBaseAttributesSO.m_BaseAttackDamage;
            m_PlayerHUD.SetHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_PlayerAttributesPersistentSO.m_Health);
        }

        #endregion

        #region Damage

        public override void TakeDamage(float damage, Transform damageSource)
        {
            // Set knockback
            Enemy.Enemy enemy = damageSource.GetComponent<Enemy.Enemy>();
            if (enemy != null)
            {
                enemy.UpdateKnockbackValue(m_EntityBaseAttributesSO.m_BowKnockback);
            }

            if (m_PlayerAttributesPersistentSO.m_Health <= damage)
            {
                m_PlayerAttributesPersistentSO.m_Health = 0;

                Death();
            }
            else
            {
                m_PlayerAttributesPersistentSO.m_Health -= damage;
            }

            m_PlayerHUD.SetHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_PlayerAttributesPersistentSO.m_Health);
        }

        public override void Death()
        {
            m_StateMachine = StateMachine.Death;
        }

        #endregion

        #region XP

        public void GiveXP(float amount)
        {
            m_PlayerAttributesPersistentSO.m_Experience += amount;
            UpdateXPBar();
        }

        public void UpdateXPBar()
        {
            while (m_PlayerAttributesPersistentSO.m_Experience >= m_PlayerAttributesPersistentSO.m_ExperienceRequiredForLevel)
            {
                m_PlayerAttributesPersistentSO.m_Level++;
                m_PlayerHUD.SetLevelText(m_PlayerAttributesPersistentSO.m_Level);
                m_PlayerAttributesPersistentSO.m_Experience -= m_PlayerAttributesPersistentSO.m_ExperienceRequiredForLevel;
                m_PlayerAttributesPersistentSO.UpdateExperienceRequired();
            }
            m_PlayerHUD.SetExperienceBar(m_PlayerAttributesPersistentSO.m_ExperienceRequiredForLevel, m_PlayerAttributesPersistentSO.m_Experience);
        }

        #endregion

        public void Teleport(Vector3 position)
        {
            m_MovementHandler.m_CharacterController.enabled = false;
            transform.position = position;
            m_MovementHandler.m_CharacterController.enabled = true;
        }

        public void ResetHealth()
        {
            m_PlayerAttributesPersistentSO.m_Health = m_EntityBaseAttributesSO.m_BaseHealth;
            m_PlayerHUD.SetHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_PlayerAttributesPersistentSO.m_Health);
        }
    }
}