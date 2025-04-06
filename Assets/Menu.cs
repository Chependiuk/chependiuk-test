using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        // Завантажуємо першу сцену в білді (змініть 0 на потрібний індекс)
        SceneManager.LoadScene(1);
    }
    public void GotoMenu()
    {
       
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}