using System.Collections;
using System.Collections.Generic;
using Entity.Player;
using UnityEngine;

namespace World
{
    public class SlimeKingChallenge : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform m_PlayerTeleportPosition;
        [SerializeField] private GameObject m_SlimeKing;

        #endregion

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<Player>().Teleport(m_PlayerTeleportPosition.position);
                m_SlimeKing.SetActive(true);

                // Slime king title UI
            }
        }
    }
}