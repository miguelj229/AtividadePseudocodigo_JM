using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pulo : MonoBehaviour
{
// Lógica explicada pelo aluno:
// o jogador aperta espaço e pula uma vez, não duas, nem três

    public float forcaPulo = 5f;
    private bool noChao = false;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && noChao || (Input.GetKeyDown(KeyCode.W)) && noChao)
        {
            rb.AddForce(Vector3.up * forcaPulo, ForceMode2D.Impulse);
            noChao = false;

            if (animator != null)
            {
                animator.SetTrigger("Pular");
            }
        }

        if (animator != null)
        {
            animator.SetBool("EstaNoChao", noChao);
        }
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Chao"))
        {
            noChao = true;
        }
    }

    void OnCollisionExit2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Chao"))
        {
            noChao = false;
        }
    }
}