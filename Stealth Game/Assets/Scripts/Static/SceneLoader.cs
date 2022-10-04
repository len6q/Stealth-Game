using UnityEngine.SceneManagement;

public class SceneLoader
{
    public static void Load(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
}
