using UnityEngine;
using Random = System.Random;
using Math = System.Math;


public class EnemyWaves : MonoBehaviour
{
	[SerializeField]
	Camera _camera = null;

	[SerializeField]
	Spawner _enemySpawner = null;

	[SerializeField]
	GameObject _enemyPrefab = null;

	[SerializeField]
	int _enemiesToSpawn = 20;

	[SerializeField]
	float _minSpawnDelay = 0.5f;

	[SerializeField]
	float _maxSpawnDelay = 2.0f;

	[SerializeField]
	int _seed = 46346734;


	Random _random;
	float _nextSpawnTime = 0.0f;
	float _spriteSize = 0.0f;
	int _enemiesSpawned = 0;

	public bool AreAllEnemiesDead()
	{
		return _enemiesToSpawn == _enemiesSpawned && GetComponentsInChildren<SpaceInvaderBehaviour>().Length == 0;
	}

	void Start()
	{
		_random = new Random( _seed );

		_nextSpawnTime = UpdateSpawnTime( _random, _minSpawnDelay, _maxSpawnDelay );

		SpriteRenderer sr = _enemyPrefab.GetComponentInChildren<SpriteRenderer>();
		Vector3 extents = sr.sprite.bounds.extents;
		_spriteSize = 2.0f * Math.Max( extents.x, extents.y );
	}

	void Update()
	{
		if( _enemiesToSpawn == _enemiesSpawned || Time.time < _nextSpawnTime)
		{
			return;
		}

		// spawn and reset timer
		Vector2 position = PositionAboveScreen( _random, _camera, _spriteSize );
		_enemySpawner.Spawn( position, Quaternion.identity );
		_nextSpawnTime = UpdateSpawnTime( _random, _minSpawnDelay, _maxSpawnDelay );
		++_enemiesSpawned;
	}


	// PRIVATE

	static Vector2 PositionAboveScreen( Random r, Camera camera, float enemySize )
	{
		Vector2 topLeft = camera.GetTopLeft();
		Vector2 width = new Vector2( camera.GetWidth(), 0.0f );
		Vector2 topRight = topLeft + width;

		float alpha = (float)r.NextDouble();
		Vector2 topEdgePosition = Vector2.Lerp( topLeft, topRight, alpha );

		Vector2 offsetHeight = new Vector2( 0.0f, enemySize );
		return topEdgePosition + offsetHeight;
	}

	static float UpdateSpawnTime( Random r, float min, float max )
	{
		float alpha = (float)r.NextDouble();
		float nextTime = Time.time + Mathf.Lerp( min, max, alpha );
		return nextTime;
	}
}