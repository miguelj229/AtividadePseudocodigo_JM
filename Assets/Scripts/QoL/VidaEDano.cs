using UnityEngine;

public class VidaEDano : MonoBehaviour
{
    public int vida = 5;
    
    // 1. Declarando a variável do Animator que estava faltando
    private Animator animator; 
    private bool estaMorto = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ReceberDano(int quantidade)
    {
        if (estaMorto) return; 

        vida -= quantidade;
        Debug.Log(gameObject.name + " recebeu " + quantidade + " de dano. Vida atual: " + vida);
    }

    void Update()
    {
       
        if (vida <= 0 && !estaMorto)
        {
            Morrer();   
        }
    }

    void Morrer()
    {
        estaMorto = true;
        Debug.Log(gameObject.name + " morreu.");
        
        
        if (animator != null)
        {
            animator.SetTrigger("Morte");
        }

       
        Destroy(gameObject, 0.4f);
    }
    
    void OnCollisionEnter2D(Collision2D coliDano)
    {
        if (coliDano.gameObject.CompareTag("Inimigo"))
        {
            ReceberDano(1);
        }
    }
}
