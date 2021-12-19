﻿using UnityEngine;

public class CompositeSpawnZone : SpawnZone {

	[SerializeField]
	bool overrideConfig;

	[SerializeField]
	bool sequential;

	[SerializeField]
	SpawnZone[] spawnZones;

	int nextSequentialIndex;

	public override Vector3 SpawnPoint {
		get {
			int index;
			if (sequential) {
				index = nextSequentialIndex++;
				if (nextSequentialIndex >= spawnZones.Length) {
					nextSequentialIndex = 0;
				}
			}
			else {
				index = Random.Range(0, spawnZones.Length);
			}
			return spawnZones[index].SpawnPoint;
		}
	}

	public override void ConfigureSpawn (Shape shape) {
		if (overrideConfig) {
			base.ConfigureSpawn(shape);
		}
		else {
			int index;
			if (sequential) {
				index = nextSequentialIndex++;
				if (nextSequentialIndex >= spawnZones.Length) {
					nextSequentialIndex = 0;
				}
			}
			else {
				index = Random.Range(0, spawnZones.Length);
			}
			spawnZones[index].ConfigureSpawn(shape);
		}
	}

	public override void Save (GameDataWriter writer) {
		writer.Write(nextSequentialIndex);
	}

	public override void Load (GameDataReader reader) {
		nextSequentialIndex = reader.ReadInt();
	}
}