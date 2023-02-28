using UnityEngine;

using Math = System.Math;


public class SpaceInvaderBehaviour : MiniStateMachine<SpaceInvaderBehaviour.State>
{
	public enum State
	{
		MoveDown,
		MoveHorizontal,
		Exploding,
		Die,
		COUNT
	}
	public readonly int StateCount = (int)State.COUNT;


	[SerializeField]
	float _speed = 3f;

	[SerializeField]
	float _timeToShrink = 1f;

	[SerializeField]
	float _timeToDie = 2f;


	float _spriteSize = 0f;
	Camera _mainCamera = null;
	float _targetHorizontal = 0f;
	float _targetHeight = 0f;
	float _leftSide = 0f;
	float _rightSide = 0f;
	float _bottomEdge = 0f;
	float _deathStart = 0f;
	SpriteRenderer _spriteRenderer = null;
	ParticleSystem _particle = null;


	void Start()
	{
		_particle = GetComponentInChildren<ParticleSystem>();
		_particle.Stop();
		
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		Vector3 extents = _spriteRenderer.sprite.bounds.extents;
		_spriteSize = 2.0f * Math.Max( extents.x, extents.y );

		_mainCamera = Camera.main;

		float halfSpriteSize = _spriteSize * 0.5f;

		_leftSide = _mainCamera.GetTopLeft().x + halfSpriteSize;
		_rightSide = _leftSide + _mainCamera.GetWidth() - _spriteSize;

		_targetHorizontal = _rightSide;
		_targetHeight = _mainCamera.GetTopLeft().y - halfSpriteSize;

		_bottomEdge = _mainCamera.GetTopLeft().y - _mainCamera.GetHeight() - _spriteSize;

		InitializeFSM( State.MoveDown, StateCount );
	}

	void Update()
	{
		UpdateFSM();
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if( (State)GetCurrentStateIndex() != State.Exploding )
		{
			// check to see if we hit a bullet
			Bullet b = c.transform.GetComponent<Bullet>();
			if( b != null )
			{
				Destroy( b.gameObject );
				TransitionTo( State.Exploding );
			}
		}
	}


	// STATES

	State OnUpdateMoveDown()
	{
		float heightDelta = Mathf.Abs( transform.position.y - _targetHeight );
		float speedDelta = _speed * Time.deltaTime;

		if( heightDelta > speedDelta )
		{
			// normal movement, no transition
			Vector3 delta = new Vector3( 0f, -speedDelta, 0f );
			transform.position = transform.position + delta;
			return State.MoveDown;
		}

		// set position to target height, and transition out
		transform.position = new Vector3( transform.position.x, _targetHeight, transform.position.z );
		return State.MoveHorizontal;
	}

	void OnExitMoveDown()
	{
		_targetHeight = transform.position.y - _spriteSize;
	}

	State OnUpdateMoveHorizontal()
	{
		float rawDelta = _targetHorizontal - transform.position.x;
		float distanceDelta = Mathf.Abs( rawDelta );
		float signDelta = Mathf.Sign( rawDelta );
		float speedDelta = _speed * Time.deltaTime;

		if( distanceDelta > speedDelta )
		{
			// normal movement, no transition
			Vector3 delta = new Vector3( signDelta * speedDelta, 0f, 0f );
			transform.position = transform.position + delta;
			return State.MoveHorizontal;
		}

		// set position to target horizontal, and transition out
		transform.position = new Vector3( _targetHorizontal, transform.position.y, transform.position.z );

		if( transform.position.y < _bottomEdge )
		{
			return State.Die;
		}
		return State.MoveDown;
	}

	void OnExitMoveHorizontal()
	{
		if( _targetHorizontal == _leftSide )
		{
			_targetHorizontal = _rightSide;
		}
		else
		{
			_targetHorizontal = _leftSide;
		}
	}

	void OnEnterExploding()
	{
		_deathStart = Time.time;
		_particle.Simulate(0f, true, true);
		_particle.Play();
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

	State OnUpdateDie()
	{
		Destroy( gameObject );
		return State.Die;
	}
}