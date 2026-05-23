using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingDamageText damageTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(Vector3 position, int damage)
    {
        if (damageTextPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = position + new Vector3(0f, 0.6f, 0f);

        FloatingDamageText text = Instantiate(
            damageTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        text.Init(damage);
    }
}