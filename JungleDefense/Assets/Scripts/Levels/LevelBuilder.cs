using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private GameObject buildTilePrefab;
    [SerializeField] private GameObject pathTilePrefab;
    [SerializeField] private float tileSize = 1f;

    private readonly List<GameObject> spawnedTiles = new List<GameObject>();
    private readonly List<Vector3> pathPositions = new List<Vector3>();

    public void BuildLevel(LevelData levelData)
    {
        ClearLevel();

        if (!IsLevelDataValid(levelData))
        {
            return;
        }

        for (int y = 0; y < levelData.height; y++)
        {
            string row = levelData.rows[y];

            for (int x = 0; x < levelData.width; x++)
            {
                char cell = row[x];
                Vector3 position = GetCellPosition(levelData, x, y);

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

        EnsurePathManager();
        PathManager.Instance.SetPath(pathPositions);
    }

    public void ClearLevel()
    {
        for (int i = spawnedTiles.Count - 1; i >= 0; i--)
        {
            if (spawnedTiles[i] != null)
            {
                Destroy(spawnedTiles[i]);
            }
        }

        spawnedTiles.Clear();
        pathPositions.Clear();

        if (PathManager.Instance != null)
        {
            PathManager.Instance.ClearPath();
        }
    }

    private Vector3 GetCellPosition(LevelData levelData, int x, int y)
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
        spawnedTiles.Add(tileObject);

        Tile tile = tileObject.GetComponent<Tile>();

        if (tile == null)
        {
            tile = tileObject.AddComponent<Tile>();
        }

        tile.isBuildable = isBuildable;
        tile.isOccupied = false;
    }

    private void EnsurePathManager()
    {
        if (PathManager.Instance != null)
        {
            return;
        }

        new GameObject("PathManager").AddComponent<PathManager>();
    }

    private bool IsLevelDataValid(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned.");
            return false;
        }

        if (levelData.width <= 0 || levelData.height <= 0)
        {
            Debug.LogError("LevelData width and height must be greater than zero.");
            return false;
        }

        if (levelData.rows == null || levelData.rows.Length != levelData.height)
        {
            Debug.LogError("LevelData rows count does not match height.");
            return false;
        }

        for (int y = 0; y < levelData.rows.Length; y++)
        {
            if (string.IsNullOrEmpty(levelData.rows[y]) || levelData.rows[y].Length != levelData.width)
            {
                Debug.LogError($"LevelData row {y} length does not match width.");
                return false;
            }
        }

        return true;
    }
}
