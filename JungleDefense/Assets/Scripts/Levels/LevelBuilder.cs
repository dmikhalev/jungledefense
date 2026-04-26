using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public LevelData levelData;

    public GameObject buildTilePrefab;
    public GameObject pathTilePrefab;

    public float tileSize = 1f;

    private List<Vector3> pathPositions = new List<Vector3>();

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
                    GameObject tile = Instantiate(buildTilePrefab, pos, Quaternion.identity);

                    Tile t = tile.AddComponent<Tile>();
                    t.isBuildable = true;
                }
                else if (c == 'P')
                {
                    GameObject tile = Instantiate(pathTilePrefab, pos, Quaternion.identity);

                    Tile t = tile.AddComponent<Tile>();
                    t.isBuildable = false;

                    pathPositions.Add(pos);
                }
            }
        }

        // создаём объект PathManager если его нет
        if (PathManager.Instance == null)
        {
            new GameObject("PathManager").AddComponent<PathManager>();
        }

        PathManager.Instance.SetPath(pathPositions);
    }
}