
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
   public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void loadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    
}
