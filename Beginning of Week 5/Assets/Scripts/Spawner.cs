using UnityEngine;


public class Spawner : MonoBehaviour
{
	[SerializeField]
	Transform _prefab;


	public Transform Spawn( Vector3 position, Quaternion rotation )
	{
		return GameObject.Instantiate<Transform>( _prefab, position, rotation, transform );
	}
}