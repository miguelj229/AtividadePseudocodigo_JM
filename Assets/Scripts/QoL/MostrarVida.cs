using UnityEngine;
using TMPro;

public class MostrarVida : MonoBehaviour
{
    public TextMeshProUGUI textoVida, textoPont, textoTempo;
    public VidaEDano vida;
    public ColetaDeItens pontuacoes;
    public Temporizador timer;
    private Animator animator;
    void Start()
    {
        vida = Object.FindAnyObjectByType<VidaEDano>();
        pontuacoes = Object.FindAnyObjectByType<ColetaDeItens>();
        timer = Object.FindAnyObjectByType<Temporizador>();
    }
    void Update()
    {
        textoVida.text = vida.vida.ToString("0");
        textoPont.text = pontuacoes.pontuacao.ToString("0");
        textoTempo.text = timer.tempoRestante.ToString("60");

    }
}
