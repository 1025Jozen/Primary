using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleGame_Watermelon
{
    public class SceneReset : MonoBehaviour
    {
        [SerializeField] KeyCode _resetSceneKey = KeyCode.R;
        void Update()
        {
            if (Input.GetKeyDown(_resetSceneKey)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
