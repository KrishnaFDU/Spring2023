using UnityEngine;
using UnityEngine.InputSystem;


public class Ship : MonoBehaviour
{
	[SerializeField]
	Camera _camera = null;

	[SerializeField]
	PlayerInput _playerInput = null;

	[SerializeField]
	float _speed = 5.0f;

	InputAction _steeringInput;
	Vector3 _spriteHalfSize;
	Vector2 _cameraHalfSize;
	
	void Start()
	{
		_steeringInput 	= _playerInput.actions["SteerShip"];
		_spriteHalfSize = GetComponent<SpriteRenderer>().sprite.bounds.extents;
		_spriteHalfSize.Scale(transform.localScale);
		_cameraHalfSize = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
	}

	void Update()
	{
		Vector2 steering = _steeringInput.ReadValue<Vector2>();
		Vector3 delta = _speed * steering * Time.deltaTime;
		Vector2 newPosition = transform.position + delta;

		if (Mathf.Abs(newPosition.x) > (_cameraHalfSize.x - _spriteHalfSize.x))
		{
			newPosition.x = transform.position.x;
		}

		if (Mathf.Abs(newPosition.y) > (_cameraHalfSize.y - _spriteHalfSize.y))
		{
			newPosition.y = transform.position.y;
		}
		
		transform.position = newPosition;
	}
}