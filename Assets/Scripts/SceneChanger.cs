using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace malvra
{
    /// <summary>
    /// This class handles the scene flow of the application.
    /// The respective scene id is given in build settings in the unity editor.
    /// </summary>
    public class SceneChanger : MonoBehaviour
    {
        private static Stack<int> sceneHistory = new Stack<int>();
        public void LoadPrevious()
        {
            if (sceneHistory.Count > 0)
                SceneManager.LoadScene(sceneHistory.Pop());
        }

        public void LoadSiembraEnCama()
        {
            LoadScene("SiembraEnCamaScene");
        }

        public void LoadHome()
        {
            LoadScene("HomeScene");
        }

        private void LoadScene(string sceneName)
        {
            sceneHistory.Push(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
