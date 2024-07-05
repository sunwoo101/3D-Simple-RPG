using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject m_PausePanel;
        [SerializeField] private string m_MainMenuSceneName;
        private bool m_Paused;

        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_Paused = !m_Paused;

                if (m_Paused)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }

        private void Pause()
        {
            m_PausePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            m_Paused = false;
            m_PausePanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

        public void MainMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(m_MainMenuSceneName);
        }
    }
}