using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Entity.Player
{
    public class FadeText : MonoBehaviour
    {
        #region Variables

        private TMP_Text m_Text;
        [SerializeField] private float m_FadeSpeed;
        [SerializeField] private float m_MoveSpeed;

        #endregion

        private void Start()
        {
            m_Text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (m_Text.color.a <= 0)
            {
                Destroy(gameObject);
            }

            Color updatedColor = m_Text.color;
            updatedColor.a -= m_FadeSpeed * Time.deltaTime;
            m_Text.color = updatedColor; // Reduce alpha
            m_Text.rectTransform.localPosition += Vector3.up * m_MoveSpeed * Time.deltaTime;
        }
    }
}