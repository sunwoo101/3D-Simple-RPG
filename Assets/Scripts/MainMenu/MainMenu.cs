using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string m_GameSceneName;

        #endregion

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Play()
        {
            SceneManager.LoadScene(m_GameSceneName);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}