using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{
    public enum Estado { Patrulhando, Perseguindo, Atacando, Morto }
    public Estado estadoAtual = Estado.Patrulhando;

    [Header("Atributos")]
    [SerializeField] private int vidaMaxima = 50;
    [SerializeField] private int danoDeAtaque = 10;
    [SerializeField] private float velocidadePatrulha = 2f;
    [SerializeField] private float velocidadePerseguicao = 4f;

    [Header("Deteccao")]
    [SerializeField] private float raioDeDeteccao = 6f;
    [SerializeField] private float raioDeAtaque = 1.2f;
    [SerializeField] private float raioDeDesistencia = 10f;

    [Header("Patrulha")]
    [SerializeField] private Transform[] pontosDePatrulha;
    [SerializeField] private float toleranciaDePonto = 0.3f;

    [Header("Combate")]
    [SerializeField] private float intervaloDeAtaque = 1.5f;
    [SerializeField] private AudioClip somDeAtaque;
    [SerializeField] private AudioClip somDeMorte;

    // Referencias
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audio;
    private Transform jogador;

    // Estado interno

    private int vida;
    private int indicePatrulhaAtual = 0;
    private float tempoDesdeUltimoAtaque = 0f;

    
    private void Awake()
    {
        rb    = GetComponent<Rigidbody2D>();
        anim  = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        vida  = vidaMaxima;

        // Busca o jogador pelo tag (feito uma vez, nao todo frame)
        GameObject jogadorObj = GameObject.FindWithTag("Player");
        if (jogadorObj != null)
            jogador = jogadorObj.transform;
        else
            Debug.LogError("InimigoIA: nenhum objeto com tag Player encontrado.");
    }

    private void FixedUpdate()
    {
        if (estadoAtual == Estado.Morto) return;
        if (jogador == null) return;

        AtualizarEstado();
        ExecutarEstadoAtual();
    }

    private void AtualizarEstado()
    {
        float distanciaAoJogador = Vector2.Distance(
            transform.position, jogador.position);

        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                if (distanciaAoJogador <= raioDeDeteccao)
                    MudarEstado(Estado.Perseguindo);
                break;

            case Estado.Perseguindo:
                if (distanciaAoJogador <= raioDeAtaque)
                    MudarEstado(Estado.Atacando);
                else if (distanciaAoJogador >= raioDeDesistencia)
                    MudarEstado(Estado.Patrulhando);
                break;

            case Estado.Atacando:
                if (distanciaAoJogador > raioDeAtaque)
                    MudarEstado(Estado.Perseguindo);
                break;
        }
    }

    private void ExecutarEstadoAtual()
    {
        switch (estadoAtual)
        {
            case Estado.Patrulhando:  Estado_Patrulhando();   break;
            case Estado.Perseguindo:  Estado_Perseguindo();   break;
            case Estado.Atacando:     Estado_Atacando();      break;
        }
    }

    private void MudarEstado(Estado novoEstado)
    {
        estadoAtual = novoEstado;

        // Atualiza o Animator conforme o estado
        anim.SetBool("Movendo",  novoEstado == Estado.Patrulhando
                                || novoEstado == Estado.Perseguindo);
        anim.SetBool("Atacando", novoEstado == Estado.Atacando);
    }

    void Estado_Patrulhando()
    {
        if (pontosDePatrulha == null || pontosDePatrulha.Length == 0) return;

        Transform ponto = pontosDePatrulha[indicePatrulhaAtual];
        MoverEm(ponto.position, velocidadePatrulha);

        float distAoPonto = Vector2.Distance(transform.position, ponto.position);
        if (distAoPonto <= toleranciaDePonto)
        {
            // Avanca para o proximo ponto (circular)
            indicePatrulhaAtual =
                (indicePatrulhaAtual + 1) % pontosDePatrulha.Length;
        }
    }

    void Estado_Perseguindo()
    {
         MoverEm(jogador.position, velocidadePerseguicao);
    }

    void Estado_Atacando()
    {
       // Para de se mover durante o ataque
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        tempoDesdeUltimoAtaque += Time.fixedDeltaTime;
        if (tempoDesdeUltimoAtaque < intervaloDeAtaque) return;

        tempoDesdeUltimoAtaque = 0f;

        // Aplica dano ao jogador (via interface IDanificavel)
        VidaEDano alvo = jogador.GetComponent<VidaEDano>();
        alvo?.ReceberDano(danoDeAtaque);

        if (somDeAtaque != null)
            audio.PlayOneShot(somDeAtaque);
    }

    void Estado_Morto()
    {
        MudarEstado(Estado.Morto);
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Morte");

        if (somDeMorte != null)
            audio.PlayOneShot(somDeMorte);

        // Remove da lista de inimigos do GameManager
        // GameManager.Instance.RemoverInimigo(gameObject);

        Debug.Log("Inimigo eliminado.");
        Destroy(gameObject, 2f); // aguarda a animacao de morte
    }

    
    // ------ HELPERS ------

    private void MoverEm(Vector2 destino, float velocidade)
    {
        Vector2 direcao = (destino - (Vector2)transform.position).normalized;
        direcao.y = 0f; // ignora diferenca vertical no movimento horizontal

        rb.linearVelocity = new Vector2(
            direcao.x * velocidade,
            rb.linearVelocity.y
            );

        // Vira o inimigo na direcao do movimento
        if (direcao.x > 0.1f)
            transform.localScale = new Vector2(1f, 1f);
        else if (direcao.x < -0.1f)
            transform.localScale = new Vector2(-1f, 1f);
    }


    // ------ INTERFACE IDanificavel ------

    public void ReceberDano(int dano, string origem)
    {
        if (estadoAtual == Estado.Morto) return;
        if (dano <= 0) return;

        vida -= dano;
        Debug.Log($"[Inimigo] Recebeu {dano} de dano de {origem}. Vida atual: {vida}");

        if (vida <= 0)
            Estado_Morto();
    }
    
    public bool EstaVivo() => estadoAtual != Estado.Morto;


    // ------ DEBUG ------
    private void OnDrawGizmosSelected()
    {
        // Raio de deteccao (amarelo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeDeteccao);

        // Raio de ataque (vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeAtaque);

        // Raio de desistencia (cinza)
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, raioDeDesistencia);
    }

}