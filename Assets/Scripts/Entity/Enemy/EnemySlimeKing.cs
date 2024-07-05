using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Enemy
{
    public class EnemySlimeKing : Enemy
    {
        #region Variables

        [SerializeField] protected float m_ChargeSpeed;
        protected Vector3 m_SideStepDirection;
        [SerializeField] protected float m_SideStepDurationSeconds;
        protected float m_SideStepTicks;
        [SerializeField] protected BoxCollider m_TeleportBounds;
        [SerializeField] protected float m_TeleportDurationSeconds;
        protected float m_TeleportStepTicks;
        [SerializeField] protected List<GameObject> m_SlimeMinionPrefabs;
        [SerializeField] protected BoxCollider m_SlimeMinionSpawnBounds;
        [SerializeField] protected int m_SlimeMinionSpawnCount;
        protected bool m_SpanwedSlimes;
        [SerializeField] protected GameObject m_BossTitle;

        protected bool m_BattleEnabled = false;

        #endregion

        protected override void Update()
        {
            if (!m_BattleEnabled)
                return;

            switch (m_StateMachine)
            {
                case StateMachine.Idle:
                    IdleState();
                    break;
                case StateMachine.Alert:
                    AlertState();
                    break;
                case StateMachine.Charge:
                    ChargeState();
                    break;
                case StateMachine.Teleport:
                    TeleportState();
                    break;
                case StateMachine.SpawnSlime:
                    SpawnSlimeState();
                    break;
                default:
                    m_StateMachine = StateMachine.Idle;
                    break;
            }
        }

        protected override void IdleState() // Switch to a random state
        {
            m_NavMeshAgent.ResetPath();

            m_Animator.SetBool("Idle", true);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Knockback", false);
            m_Animator.SetBool("SpawnMinions", false);

            int randomInt = Random.Range(0, 3);

            switch (randomInt)
            {
                case 0:
                    m_StateMachine = StateMachine.Alert;
                    break;
                case 1:
                    m_StateMachine = StateMachine.Charge;
                    break;
                case 2:
                    m_TeleportStepTicks = 0;
                    m_StateMachine = StateMachine.Teleport;
                    break;
                default:
                    m_StateMachine = StateMachine.Idle;
                    break;
            }
        }

        protected override void AlertState() // Side stepping
        {
            m_NavMeshAgent.ResetPath();

            LookAtPlayer();

            if (m_SideStepTicks == 0)
                GenerateRandomSideStepDirection();

            m_NavMeshAgent.Move(m_SideStepDirection * m_EntityBaseAttributesSO.m_MovementSpeed * Time.deltaTime);

            if (m_SideStepTicks >= m_SideStepDurationSeconds)
            {
                m_SideStepTicks = 0;
                m_StateMachine = StateMachine.Charge;
            }

            m_SideStepTicks += Time.deltaTime;

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Alert", true);
            m_Animator.SetBool("Knockback", false);
            m_Animator.SetBool("SpawnMinions", false);
        }

        protected void ChargeState() // Charge at player
        {
            m_NavMeshAgent.ResetPath();

            LookAtPlayer();

            Vector3 direction = transform.forward;

            m_NavMeshAgent.Move(direction * m_ChargeSpeed * Time.deltaTime);

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Alert", true);
            m_Animator.SetBool("Knockback", false);
            m_Animator.SetBool("SpawnMinions", false);
        }

        protected void TeleportState() // Teleport to random position
        {
            m_NavMeshAgent.ResetPath();

            LookAtPlayer();

            if (m_TeleportStepTicks == 0)
            {
                transform.position = GenerateRandomPosition(m_TeleportBounds);
            }

            if (m_TeleportStepTicks >= m_TeleportDurationSeconds)
            {
                m_TeleportStepTicks = 0;

                int randomInt = Random.Range(0, 3);

                switch (randomInt)
                {
                    case 0:
                        m_StateMachine = StateMachine.Alert;
                        break;
                    case 1:
                        m_StateMachine = StateMachine.Charge;
                        break;
                    case 2:
                        m_SpanwedSlimes = false;
                        m_StateMachine = StateMachine.SpawnSlime;
                        break;
                    default:
                        m_StateMachine = StateMachine.Idle;
                        break;
                }
            }

            m_TeleportStepTicks += Time.deltaTime;

            m_Animator.SetBool("Idle", true);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Knockback", false);
            m_Animator.SetBool("SpawnMinions", false);
        }

        protected void SpawnSlimeState() // Spawn slime minions
        {
            m_NavMeshAgent.ResetPath();

            m_Animator.SetBool("Idle", false);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Knockback", false);
            m_Animator.SetBool("SpawnMinions", true);

            if (!m_SpanwedSlimes)
            {
                for (int i = 0; i < m_SlimeMinionSpawnCount; i++)
                {
                    int randomIndex = Random.Range(0, m_SlimeMinionPrefabs.Count);

                    GameObject instance = Instantiate(m_SlimeMinionPrefabs[randomIndex], GenerateRandomPosition(m_SlimeMinionSpawnBounds), Quaternion.identity);
                    instance.GetComponent<Enemy>().TriggerAlert();
                }
            }

            m_SpanwedSlimes = true;
        }

        protected Vector3 GenerateRandomPosition(BoxCollider boxCollider)
        {
            Vector3 boxSize = boxCollider.size;
            Vector3 boxCentre = boxCollider.center;

            Vector3 minBounds = boxCentre - boxSize * 0.5f;
            Vector3 maxBounds = boxCentre + boxSize * 0.5f;

            float teleportX = Random.Range(minBounds.x, maxBounds.x);
            float teleportY = transform.position.y;
            float teleportZ = Random.Range(minBounds.z, maxBounds.z);

            Vector3 randomPosition = transform.position + new Vector3(teleportX, teleportY, teleportZ);

            return randomPosition;
        }

        protected void GenerateRandomSideStepDirection()
        {
            // -1 or 1
            int randomDirection = Random.Range(0, 2);
            randomDirection = randomDirection == 0 ? -1 : 1;

            m_SideStepDirection = transform.right * randomDirection;
        }

        protected override void OnTriggerEnter(Collider collision)
        {
            Entity entity = collision.transform.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.TeamID() != TeamID())
                {
                    entity.TakeDamage(m_EntityBaseAttributesSO.m_BaseAttackDamage, transform);
                    LookAtPlayer();

                    if (m_StateMachine == StateMachine.Charge)
                    {
                        m_TeleportStepTicks = 0;
                        m_StateMachine = StateMachine.Teleport;
                    }
                    else
                    {
                        m_SideStepTicks = 0;
                        m_StateMachine = StateMachine.Alert;
                    }
                }
            }
        }

        protected override void OnTriggerStay(Collider collision)
        {
            Entity entity = collision.transform.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.TeamID() != TeamID())
                {
                    if (m_StateMachine == StateMachine.Charge)
                    {
                        m_TeleportStepTicks = 0;
                        m_StateMachine = StateMachine.Teleport;
                    }
                    else if (m_StateMachine != StateMachine.Teleport)
                    {
                        m_SideStepTicks = 0;
                        m_StateMachine = StateMachine.Alert;
                    }
                }
            }
        }

        public override void TakeDamage(float damage, Transform damageSource)
        {
            if (!m_BattleEnabled)
            {
                m_BossTitle.SetActive(true);
                m_BattleEnabled = true;
            }

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

                if (m_StateMachine == StateMachine.Charge || m_StateMachine == StateMachine.Alert)
                {
                    m_TeleportStepTicks = 0;
                    m_StateMachine = StateMachine.Teleport;
                }
                else if (m_StateMachine == StateMachine.SpawnSlime)
                {
                    m_StateMachine = StateMachine.Alert;
                }
            }

            m_HealthBar.UpdateHealthBar(m_EntityBaseAttributesSO.m_BaseHealth, m_Health);
        }
    }
}