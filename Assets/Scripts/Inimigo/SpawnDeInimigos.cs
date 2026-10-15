using UnityEngine;

public class SpawnDeInimigos : MonoBehaviour
{
    public GameObject prefabInimigo;
    public float intervaloSpawn = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnarInimigo), intervaloSpawn, intervaloSpawn);
    }

    void SpawnarInimigo()
    {
        Instantiate(prefabInimigo, transform.position, Quaternion.identity);
    }
}