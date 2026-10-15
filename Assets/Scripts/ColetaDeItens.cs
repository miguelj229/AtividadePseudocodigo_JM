using UnityEngine;

public class ColetaDeItens : MonoBehaviour
{
    // Marque o Collider do item como "Is Trigger" no Inspector
    public int pontuacao = 0;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Item"))
        {
            pontuacao += 10;
            Debug.Log("Pontuação atual: " + pontuacao);
            Destroy(outro.gameObject);
        }
    }
}
