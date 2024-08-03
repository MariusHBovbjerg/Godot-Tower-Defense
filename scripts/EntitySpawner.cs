using Godot;
using System;

public partial class EntitySpawner
{
	public void spawnEntityUsingStrategy(Node parent, IEntityBuilder builder, Texture2D texture, Vector2 scale, ISpawnStrategy strategy)
	{
		var position = strategy.ExecuteStrategy();

		var entity = builder.Build(texture, scale, position);

		parent.AddChild(entity);
	}
}