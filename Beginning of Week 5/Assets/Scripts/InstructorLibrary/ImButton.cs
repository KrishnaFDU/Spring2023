using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// immediate mode version of Button
public class ImButton : Button
{
	int _lastFrameClicked;
	PointerEventData.InputButton _button;
	
	
	public override void OnPointerClick( PointerEventData eventData )
	{
		if( this.interactable )
		{
			_lastFrameClicked = Time.frameCount;
			_button = eventData.button;
		}
	}
	
	public bool IsClicked( PointerEventData.InputButton button )
	{
		return _button == button
			&& _lastFrameClicked == Time.frameCount
			;
	}
	
	public bool IsAnyClicked()
	{
		return _lastFrameClicked == Time.frameCount;
	}

	public bool IsLeftClicked() { return IsClicked( PointerEventData.InputButton.Left ); }
	public bool IsRightClicked() { return IsClicked( PointerEventData.InputButton.Right ); }
	public bool IsMiddleClicked() { return IsClicked( PointerEventData.InputButton.Middle ); }
}