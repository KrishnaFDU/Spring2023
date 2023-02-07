using UnityEngine;
using UnityEngine.InputSystem;


public class Ship : MonoBehaviour
{
	[SerializeField]
	PlayerInput playerInput = null;

	[SerializeField]
	float speed = 5.0f;


	InputAction _steeringInput;


	void Start()
	{
		_steeringInput = playerInput.actions["SteerShip"];
	}

	void Update()
	{
		Vector2 steering = _steeringInput.ReadValue<Vector2>();
		Vector3 delta = speed * steering * Time.deltaTime;
		transform.position = transform.position + delta;
	}
}