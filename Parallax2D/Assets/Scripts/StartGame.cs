using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGame : MonoBehaviour
{
    public string sceneToLoad;

    public void ChangeSceneNow()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
