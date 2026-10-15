using UnityEngine;

public class MovimentoPersonagem : MonoBehaviour
{
// Lógica explicada pelo aluno:
// o jogador anda só na horizontal

    [Tooltip("Velocidade de movimento do personagem")]
    public float velocidade = 5f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float entradaHorizontal = Input.GetAxis("Horizontal");

        Vector3 direcao = new Vector3(entradaHorizontal, 0f);
        transform.Translate(direcao * velocidade * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("Velocidade", Mathf.Abs(entradaHorizontal));
        }
    }
}