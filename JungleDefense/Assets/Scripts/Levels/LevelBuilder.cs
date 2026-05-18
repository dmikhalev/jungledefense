using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private GameObject buildTilePrefab;
    [SerializeField] private GameObject pathTilePrefab;
    [SerializeField] private float tileSize = 1f;

    private readonly List<Vector3> pathPositions = new List<Vector3>();

    private void Start()
    {
        BuildLevel();
    }

    private void BuildLevel()
    {
        if (!IsLevelDataValid())
        {
            return;
        }

        pathPositions.Clear();

        for (int y = 0; y < levelData.height; y++)
        {
            string row = levelData.rows[y];

            for (int x = 0; x < levelData.width; x++)
            {
                char cell = row[x];

                Vector3 position = GetCellPosition(x, y);

                if (cell == '1')
                {
                    CreateTile(buildTilePrefab, position, true);
                }
                else if (cell == 'P')
                {
                    CreateTile(pathTilePrefab, position, false);
                    pathPositions.Add(position);
                }
            }
        }

        if (PathManager.Instance == null)
        {
            new GameObject("PathManager").AddComponent<PathManager>();
        }

        PathManager.Instance.SetPath(pathPositions);
    }

    private Vector3 GetCellPosition(int x, int y)
    {
        float offsetX = (levelData.width - 1) / 2f;
        float offsetY = (levelData.height - 1) / 2f;

        return new Vector3(
            (x - offsetX) * tileSize,
            (offsetY - y) * tileSize,
            0f
        );
    }

    private void CreateTile(GameObject prefab, Vector3 position, bool isBuildable)
    {
        if (prefab == null)
        {
            Debug.LogError("Tile prefab is not assigned.");
            return;
        }

        GameObject tileObject = Instantiate(prefab, position, Quaternion.identity);
        Tile tile = tileObject.GetComponent<Tile>();

        if (tile == null)
        {
            tile = tileObject.AddComponent<Tile>();
        }

        tile.isBuildable = isBuildable;
        tile.isOccupied = false;
    }

    private bool IsLevelDataValid()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned.");
            return false;
        }

        if (levelData.rows == null || levelData.rows.Length != levelData.height)
        {
            Debug.LogError("LevelData rows count does not match height.");
            return false;
        }

        for (int y = 0; y < levelData.rows.Length; y++)
        {
            if (levelData.rows[y].Length != levelData.width)
            {
                Debug.LogError($"LevelData row {y} length does not match width.");
                return false;
            }
        }

        return true;
    }
}
