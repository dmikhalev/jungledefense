using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public LevelData levelData;

    public GameObject buildTilePrefab;
    public GameObject pathTilePrefab;

    public float tileSize = 1f;

    void Start()
    {
        BuildLevel();
    }

    void BuildLevel()
    {
        for (int y = 0; y < levelData.height; y++)
        {
            string row = levelData.rows[y];

            for (int x = 0; x < levelData.width; x++)
            {
                char c = row[x];

                float offsetX = (levelData.width - 1) / 2f;
                float offsetY = (levelData.height - 1) / 2f;

                Vector3 pos = new Vector3(
                    (x - offsetX) * tileSize,
                    (offsetY - y) * tileSize,
                    0
                );

                if (c == '1')
                {
                    Instantiate(buildTilePrefab, pos, Quaternion.identity);
                }
                else if (c == 'P')
                {
                    Instantiate(pathTilePrefab, pos, Quaternion.identity);
                }
            }
        }
    }
}