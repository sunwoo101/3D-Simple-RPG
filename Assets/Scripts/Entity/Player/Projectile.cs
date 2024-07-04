using System.Collections;
using System.Collections.Generic;
using Entity;
using Entity.Enemy;
using TMPro;
using UnityEngine;

namespace Entity
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected float m_Speed;
        internal TeamID.ID m_TeamID;
        internal float m_Rotation;
        internal float m_AttackDamage;
        internal Transform m_DamageSource;
        private bool m_EnableMovement;
        [SerializeField] protected GameObject m_DamageTextPrefab;
        internal float m_KnockbackSpeed;

        protected void Start()
        {
            m_EnableMovement = true;
        }

        protected void Update()
        {
            if (m_EnableMovement)
            {
                Move();
            }
        }

        protected void Move()
        {
            transform.position += transform.forward * m_Speed * Time.deltaTime;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            Entity entity = collider.transform.GetComponent<Entity>();

            if (entity != null)
            {
                if (entity.TeamID() != m_TeamID)
                {
                    m_EnableMovement = false;

                    // Set knockback
                    Enemy.Enemy enemy = collider.transform.GetComponent<Enemy.Enemy>();
                    if (enemy != null)
                    {
                        enemy.UpdateKnockbackValue(m_KnockbackSpeed);
                    }

                    // Damage target
                    entity.TakeDamage(m_AttackDamage, m_DamageSource);

                    // Instantiate damage text
                    Transform hitInfo = GameObject.Find("HitInfo").GetComponent<Transform>(); // Find the parent for the hit info text
                    GameObject damageText = Instantiate(m_DamageTextPrefab, hitInfo); // Instantiate and store a reference
                    RectTransform damageTextTransform = damageText.GetComponent<RectTransform>(); // Get reference to rect transform
                    Vector3 point = collider.ClosestPoint(transform.position); // Get collision point
                    point = Camera.main.WorldToScreenPoint(point); // Convert to screen position

                    // Generate a random offset
                    float xRandomOffset = Random.Range(20.0f, 50.0f);
                    float randomDirection = Random.Range(0, 2);
                    randomDirection = randomDirection == 0 ? -1 : 1;
                    point.x += xRandomOffset * randomDirection;
                    point.y += Random.Range(-50.0f, 50.0f);
                    damageTextTransform.position = point;

                    // Set damage text
                    damageText.GetComponent<TMP_Text>().text = m_AttackDamage.ToString();

                    Destroy(gameObject);
                }
            }
            else
            {
                // Prevent projectile from colliding with spawners
                if (!collider.CompareTag("IgnoreProjectile"))
                {
                    m_EnableMovement = false;

                    // Stick to environment
                    StickToEnvironment();//collider.transform);
                }
            }
        }

        protected void StickToEnvironment()//Transform hit)
        {
            // transform.SetParent(hit);
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().detectCollisions = false;
        }
    }
}