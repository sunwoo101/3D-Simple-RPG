using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Enemy
{
    public class Spawner : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject m_EnemyPrefab;
        [SerializeField] private LayerMask m_LayerMask;
        private GameObject m_SpawnedEnemy;

        #endregion

        private void OnTriggerEnter(Collider collider)
        {
            Player.Player player = collider.GetComponent<Player.Player>();

            if (player != null)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            if (m_SpawnedEnemy == null)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(transform.position, -transform.up, out raycastHit, 50.0f))
                {
                    m_SpawnedEnemy = Instantiate(m_EnemyPrefab, raycastHit.point, Quaternion.identity);
                }
            }
        }
    }
}