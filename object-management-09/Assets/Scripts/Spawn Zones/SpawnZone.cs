﻿using UnityEngine;

public abstract class SpawnZone : PersistableObject {

	public abstract Vector3 SpawnPoint { get; }

	[System.Serializable]
	public struct SpawnConfiguration {

		public enum MovementDirection {
			Forward,
			Upward,
			Outward,
			Random
		}

		public ShapeFactory[] factories;

		public MovementDirection movementDirection;

		public FloatRange speed;

		public FloatRange angularSpeed;

		public FloatRange scale;

		public ColorRangeHSV color;

		public bool uniformColor;

		public MovementDirection oscillationDirection;

		public FloatRange oscillationAmplitude;

		public FloatRange oscillationFrequency;
	}

	[SerializeField]
	SpawnConfiguration spawnConfig;

	public virtual Shape SpawnShape () {
		int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
		Shape shape = spawnConfig.factories[factoryIndex].GetRandom();

		Transform t = shape.transform;
		t.localPosition = SpawnPoint;
		t.localRotation = Random.rotation;
		t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
		if (spawnConfig.uniformColor) {
			shape.SetColor(spawnConfig.color.RandomInRange);
		}
		else {
			for (int i = 0; i < shape.ColorCount; i++) {
				shape.SetColor(spawnConfig.color.RandomInRange, i);
			}
		}

		float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
		if (angularSpeed != 0f) {
			var rotation = shape.AddBehavior<RotationShapeBehavior>();
			rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
		}

		float speed = spawnConfig.speed.RandomValueInRange;
		if (speed != 0f) {
			//Vector3 direction;
			//switch (spawnConfig.movementDirection) {
			//	case SpawnConfiguration.MovementDirection.Upward:
			//		direction = transform.up;
			//		break;
			//	case SpawnConfiguration.MovementDirection.Outward:
			//		direction = (t.localPosition - transform.position).normalized;
			//		break;
			//	case SpawnConfiguration.MovementDirection.Random:
			//		direction = Random.onUnitSphere;
			//		break;
			//	default:
			//		direction = transform.forward;
			//		break;
			//}
			var movement = shape.AddBehavior<MovementShapeBehavior>();
			movement.Velocity =
				GetDirectionVector(spawnConfig.movementDirection, t) * speed;
		}

		SetupOscillation(shape);
		return shape;
	}

	void SetupOscillation (Shape shape) {
		float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
		float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
		if (amplitude == 0f || frequency == 0f) {
			return;
		}
		var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
		oscillation.Offset = GetDirectionVector(
			spawnConfig.oscillationDirection, shape.transform
		) * amplitude;
		oscillation.Frequency = frequency;
	}

	Vector3 GetDirectionVector (
		SpawnConfiguration.MovementDirection direction, Transform t
	) {
		switch (direction) {
			case SpawnConfiguration.MovementDirection.Upward:
				return transform.up;
			case SpawnConfiguration.MovementDirection.Outward:
				return (t.localPosition - transform.position).normalized;
			case SpawnConfiguration.MovementDirection.Random:
				return Random.onUnitSphere;
			default:
				return transform.forward;
		}
	}
}