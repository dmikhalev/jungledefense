using System.Collections.Generic;

public static class EnemyRegistry
{
    private static readonly List<Enemy> enemies = new List<Enemy>();

    public static IReadOnlyList<Enemy> Enemies => enemies;

    public static void Register(Enemy enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public static void Unregister(Enemy enemy)
    {
        if (enemy != null)
        {
            enemies.Remove(enemy);
        }
    }

    public static void Clear()
    {
        enemies.Clear();
    }
}
