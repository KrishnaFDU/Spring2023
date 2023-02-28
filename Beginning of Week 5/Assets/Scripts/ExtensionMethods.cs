using UnityEngine;


static public class ExtensionMethods
{
	public static float GetHeight( this Camera camera )
	{
		return 2.0f * camera.orthographicSize;
	}
	
	static public float GetWidth( this Camera camera )
	{
		return 2.0f * camera.orthographicSize * camera.aspect;
	}
	
	static public Vector2 GetTopLeft( this Camera camera )
	{
		float halfHeight = camera.orthographicSize;
		float halfWidth = halfHeight * camera.aspect;
		Vector2 p = camera.transform.position;
		return new Vector2( p.x - halfWidth, p.y + halfHeight );
	}
}