using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy
{
    public class Enemy : Entity
    {
        #region enums

        public enum StateMachine
        {
            Idle,
            Alert,
            Knockback,

            // Slime king states
            Charge,
            Teleport,
            Shockwave,
            SpawnSlime,
        }

        #endregion

        #region Variables

        // State machine
        protected StateMachine m_StateMachine;

        // Attributes
        public float m_Health;
        [SerializeField] protected float m_KnockbackSpeed;

        // AI setup
        [SerializeField] protected float m_AlertRange;

        // References
        protected NavMeshAgent m_NavMeshAgent;
        protected Animator m_Animator;
        protected Transform m_PlayerTransform;
        protected HealthBar m_HealthBar;

        // Loot
        [SerializeField] protected float m_ExperienceDrop;

        #endregion

        protected void Awake()
        {
            m_StateMachine = StateMachine.Idle;
        }

        protected override void Start()
        {
            m_Health = m_EntityBaseAttributesSO.m_BaseHealth;

            // References
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
            m_HealthBar = GetComponent<HealthBar>();
        }

        protected override void Update()
        {
            Debug.Log(m_StateMachine);
            switch (m_StateMachine)
            {
                case StateMachine.Idle:
                    IdleState();
                    break;
                case StateMachine.Alert:
                    AlertState();
                    break;
                case StateMachine.Knockback:
                    KnockbackState();
                    break;
                default:
                    m_StateMachine = StateMachine.Idle;
                    break;
            }
        }

        #region State Methods

        protected virtual void IdleState()
        {
            m_NavMeshAgent.ResetPath();

            if (Vector3.Distance(transform.position, m_PlayerTransform.position) <= m_AlertRange)
            {
                m_StateMachine = StateMachine.Alert;
            }

            m_Animator.SetBool("Idle", true);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Knockback", false);
        }

        protected virtual void AlertState()
        {
            LookAtPlayer();

            m_NavMeshAgent.destination = m_PlayerTransform.position;
            m_NavMeshAgent.speed = m_EntityBaseAttributesSO.m_MovementSpeed;

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Alert", true);
            m_Animator.SetBool("Knockback", false);
        }

        protected void KnockbackState()
        {
            m_NavMeshAgent.ResetPath();

            Knockback();

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Knockback", true);
        }

        #endregion

        protected void Knockback()
        {
            Vector3 direction = transform.forward;
            direction *= -1;

            m_NavMeshAgent.Move(direction * m_KnockbackSpeed * Time.deltaTime);
        }

        protected void LookAtPlayer()
        {
            Vector3 lookPosition = m_PlayerTransform.position;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);
        }

        public void EndTakeDamage()
        {
            TriggerAlert();
        }

        public void TriggerAlert()
        {
            Debug.Log("AlertTriggered");
            m_StateMachine = StateMachine.Alert;
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            Entity entity = collision.transform.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.TeamID() != TeamID())
                {
                    entity.TakeDamage(m_EntityBaseAttributesSO.m_BaseAttackDamage, transform);
                    LookAtPlayer();
                    m_StateMachine = StateMachine.Knockback;
                }
            }
        }

        protected virtual void OnTriggerStay(Collider collision)
        {
            Entity entity = collision.transform.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.TeamID() != TeamID())
                {
                    LookAtPlayer();
                    m_StateMachine = StateMachine.Knockback;
                }
            }
        }

        public void UpdateKnockbackValue(float value)
        {
            m_KnockbackSpeed = value;
        }

        public override void TakeDamage(float damage, Transform damageSource)
        {
            if (m_Health <= damage)
            {
                m_Health = 0;

                // Give xp to player
                Player.Player player = damageSource.GetComponent<Player.Player>();
                if (player != null)
                {
                    player.GiveXP(m_ExperienceDrop);
                }

                Death();
            }
            else
            {
                m_Health -= damage;
                LookAtPlayer();
                m_StateMachine = StateMachine.Knockback;
            }

            m_HealthBar.UpdateHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_Health);
        }

        public override void Death()
        {
            Destroy(gameObject);
        }
    }
}