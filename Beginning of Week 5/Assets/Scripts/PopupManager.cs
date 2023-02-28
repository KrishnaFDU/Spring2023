using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    [SerializeField]
	PlayerInput _playerInput = null;

    [SerializeField]
    ConfirmPopup _gameOverPopup = null;

    [SerializeField]
    string _successTitle = "You won!";

    [SerializeField]
	string _failTitle = "You died!";

	[SerializeField]
	string _prompt = "Choose Accept to try again";

    [SerializeField]
    Ship _ship;

    [SerializeField]
    EnemyWaves _enemyWaves;

    void Start()
    {
        _gameOverPopup.Hide();
    }

    void Update()
    {
        if (_gameOverPopup.IsShown())
        {
            if( !_gameOverPopup.UpdatePopup() )
            {
                SceneManager.LoadScene( 0 );
            }
        }
        else
        {
            if (_ship == null)
            {
                _gameOverPopup.Show( _failTitle, _prompt, _playerInput );
            }
            else if (_enemyWaves.AreAllEnemiesDead())
            {
                _gameOverPopup.Show( _successTitle, _prompt, _playerInput );
            }
        }
    }
}
