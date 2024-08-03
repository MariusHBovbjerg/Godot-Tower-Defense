using Godot;
using System;

public partial class EntitySpawner
{
	private Action<Node2D> _entityReady;

	public EntitySpawner(Action<Node2D> entityReady)
	{
		_entityReady = entityReady;
	}

	public void spawnEntityUsingStrategy(IEntityBuilder builder, Texture2D texture, Vector2 scale, ISpawnStrategy strategy)
	{
		var position = strategy.ExecuteStrategy();

		var entity = builder.Build(texture, scale, position);
		_entityReady?.Invoke(entity);
	}
}
