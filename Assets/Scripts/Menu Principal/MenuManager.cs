using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void IniciarJogo()
    {
        SceneManager.LoadScene("Game");
    }

    public void FecharJogo()
    {
        Application.Quit();
        Debug.Log("Jogo Quitado.");
    }
}
