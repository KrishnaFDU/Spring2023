using UnityEngine;
using UnityEngine.InputSystem;


public class Ship : MiniStateMachine<Ship.State>
{
	public enum State
	{
		Playing,
		Exploding,
		Die,
		COUNT
	}
	public readonly int StateCount = (int)State.COUNT;


	[SerializeField]
	PlayerInput _playerInput = null;

	[SerializeField]
	Spawner _bulletSpawner = null;

	[SerializeField]
	ParticleSystem _explosion = null;

	[SerializeField]
	float _speed = 5.0f;

	[SerializeField]
	float _shotsPerSecond = 10.0f;

	[SerializeField]
	float _bulletThrust = 10.0f;

	[SerializeField]
	Transform[] _bulletEmitPoint = null;

	[SerializeField]
	float _timeToShrink = 1f;

	[SerializeField]
	float _timeToDie = 2f;

	
	InputAction _steeringInput;
	InputAction _shootInput;
	float _shotDelay;
	float _lastShotTime;
	int _bulletEmitIndex;
	float _deathStart;
	SpriteRenderer _spriteRenderer;


	void Start()
	{
		InitializeFSM( State.Playing, StateCount );

		_steeringInput = _playerInput.actions[Constant.Input.Steer];
		_shootInput = _playerInput.actions[Constant.Input.Shoot];
		_shotDelay = 1.0f / _shotsPerSecond;
		_lastShotTime = 0.0f;
		_bulletEmitIndex = 0;
		_deathStart = 0f;

		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		UpdateFSM();
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if( (State)GetCurrentStateIndex() == State.Playing )
		{
			// check to see if we hit an enemy
			SpaceInvaderBehaviour e = c.transform.GetComponent<SpaceInvaderBehaviour>();
			if( e != null )
			{
				TransitionTo( State.Exploding );
			}
		}
	}

	State OnUpdatePlaying()
	{
		Vector2 steering = _steeringInput.ReadValue<Vector2>();
		Vector3 delta = _speed * steering * Time.deltaTime;
		transform.position = transform.position + delta;

		if( _shootInput.ReadValue<float>() == 1.0f )
		{
			float timeDelta = Time.time - _lastShotTime;
			if( timeDelta >= _shotDelay )
			{
				Transform t = _bulletEmitPoint[_bulletEmitIndex];
				Transform bullet = _bulletSpawner.Spawn( t.position, t.rotation );
				Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
				bulletBody.AddForce( transform.up * _bulletThrust );

				_bulletEmitIndex = (_bulletEmitIndex + 1) % _bulletEmitPoint.Length;
				_lastShotTime = Time.time;
			}
		}

		return State.Playing;
	}

	void OnEnterExploding()
	{
		_deathStart = Time.time;
		_explosion.Simulate(0f, true, true);
		_explosion.Play();
	}

	State OnUpdateExploding()
	{
		float timeSinceDeath = Time.time - _deathStart;
		if( timeSinceDeath >= _timeToDie )
		{
			return State.Die;
		}

		float animateAlpha = Mathf.InverseLerp( 0f, _timeToShrink, timeSinceDeath );

		float scale = 1.0f - animateAlpha;
		_spriteRenderer.transform.localScale = new Vector3( scale, scale, scale );

		return State.Exploding;
	}

	void OnEnterDie()
	{
		GameObject.Destroy(this);
	}

	State OnUpdateDie()
	{
		return State.Die;
	}
}