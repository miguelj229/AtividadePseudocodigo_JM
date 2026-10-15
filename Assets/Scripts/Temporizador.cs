using UnityEngine;
public class Temporizador : MonoBehaviour
{
    public float tempoRestante = 60;
    void Update()
    {
        tempoRestante -= Time.deltaTime;
        if (tempoRestante <= 0)
        {
            tempoRestante = 0;
            TerminarJogo();
        }
    }
    void TerminarJogo()
    {
        Debug.Log("Tempo esgotado! Fim de jogo.");
    }
}
