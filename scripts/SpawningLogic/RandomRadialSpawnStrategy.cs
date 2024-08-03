using Godot;

public class RandomRadialSpawnStrategy : ISpawnStrategy
{
    private Vector2 _centerPoint;
    private float _radius;

    public RandomRadialSpawnStrategy(Vector2 centerPoint, float radius)
    {
        _centerPoint = centerPoint;
        _radius = radius;
    }

    public Vector2 ExecuteStrategy()
    {
        var angle = GD.RandRange(0, 360);

        var enemyX = _centerPoint.X + _radius * Mathf.Cos(angle);

        var enemyY = _centerPoint.Y + _radius * Mathf.Sin(angle);

        return new Vector2(enemyX, enemyY);
    }
}