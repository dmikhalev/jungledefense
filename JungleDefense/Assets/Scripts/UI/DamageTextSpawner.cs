using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingDamageText damageTextPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.6f, 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Spawn(Vector3 position, int damage)
    {
        if (damageTextPrefab == null || damage <= 0)
        {
            return;
        }

        FloatingDamageText text = Instantiate(
            damageTextPrefab,
            position + spawnOffset,
            Quaternion.identity
        );

        text.Init(damage);
    }
}
