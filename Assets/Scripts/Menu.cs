using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayGame() {
        //播放下一个场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void QuitGame() {
        Application.Quit();
    }

    public void UIEnable() {
        GameObject.Find("Canvs/UI").SetActive(true);
    }
}
