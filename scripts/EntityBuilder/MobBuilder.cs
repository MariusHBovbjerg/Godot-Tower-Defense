using Godot;
using System;

public class MobBuilder : IEntityBuilder
{
	public Node2D Build(Texture2D texture, Vector2 scale, Vector2 position)
	{
		var sprite = new Sprite2D
		{
			Texture = texture,
			Scale = scale
		};

		var collisionshape = new CollisionShape2D
		{
			Shape = new RectangleShape2D
			{
				Size = sprite.Texture.GetSize()
			},
			Scale = scale
		};

		var timer = new Timer
		{
			WaitTime = 0.1f
		};

		return new Mob(sprite, collisionshape, timer, position);
	}
}
