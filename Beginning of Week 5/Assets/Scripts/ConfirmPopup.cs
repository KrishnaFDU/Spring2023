using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using TMPro;


public class ConfirmPopup : MonoBehaviour
{
	[SerializeField]
	TMP_Text _title = null;

	[SerializeField]
	TMP_Text _prompt = null;

	[SerializeField]
	ImButton _acceptButton = null;


	InputAction _acceptAction = null;


	public void Show( string title, string prompt, PlayerInput playerInput )
	{
		_title.text = title;
		_prompt.text = prompt;
		gameObject.SetActive(true);
		_acceptAction = playerInput.actions[Constant.Input.Accept];
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public bool IsShown() { return gameObject.activeSelf; }

	// returns true if popup still needs updating
	public bool UpdatePopup()
	{
		if(  _acceptButton.IsAnyClicked()
		  || _acceptAction.ReadValue<float>() == 1.0f
		  )
		{
			Hide();
			return false;
		}

		return true;
	}
}