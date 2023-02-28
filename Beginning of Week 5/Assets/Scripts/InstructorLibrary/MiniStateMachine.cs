using System;
using System.Reflection;

using UnityEngine;

// The type T must be an enum. Each enumerated name in the
// enum should be used once and only once, and there should
// be no gaps in the enum.
//
// For each Name in the enum, the child class must implement:
// T OnUpdateName();
// 
// In addition, the client may optionally implement:
// void OnEnterName();
// void OnExitName();

public class MiniStateMachine<T> : MonoBehaviour
	where T : struct
{
	delegate void InitFunc();
	delegate T UpdateFunc();

	InitFunc[] _onEnter;
	InitFunc[] _onExit;
	UpdateFunc[] _onUpdate;

	int _currentStateIndex;


	// Child interface

	protected int GetCurrentStateIndex() { return _currentStateIndex; }

	protected void InitializeFSM( T startState, int stateCount )
	{
		_onEnter = new InitFunc[stateCount];
		_onExit = new InitFunc[stateCount];
		_onUpdate = new UpdateFunc[stateCount];

		string[] enumNames = Enum.GetNames( typeof(T) );
		MethodInfo[] method = GetType().GetMethods( BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly );

		// This is a dumb N-squared algo, but N should be small, and this should only be called during init
		for( int i = 0; i < stateCount; i++ )
		{
			string e = enumNames[i];
			string enterName = "OnEnter" + e;
			string exitName = "OnExit" + e;
			string updateName = "OnUpdate" + e;

			for( int j = 0; j < method.Length; j++ )
			{
				MethodInfo m = method[j];
				string name = m.Name;

				if( name == updateName )
				{
					_onUpdate[i] = (UpdateFunc)Delegate.CreateDelegate( typeof(UpdateFunc), this, m );
				}
				else if( name == enterName )
				{
					_onEnter[i] = (InitFunc)Delegate.CreateDelegate( typeof(InitFunc), this, m );
				}
				else if( name == exitName )
				{
					_onExit[i] = (InitFunc)Delegate.CreateDelegate( typeof(InitFunc), this, m );
				}
			}

			// check functions:
			// - updates must exist
			// - inits do not, if they are missing fill in with NullInit()
			if( _onUpdate[i] == null )
			{
				Debug.LogError( $"Missing update function '{updateName}', please make sure it is defined in the child class." );
			}

			if( _onEnter[i] == null )
			{
				_onEnter[i] = NullInit;
			}

			if( _onExit[i] == null )
			{
				_onExit[i] = NullInit;
			}
		}

		_currentStateIndex = Convert.ToInt32( startState );
		_onEnter[_currentStateIndex]();
	}

	protected void UpdateFSM()
	{
		// this structure means that there is no frame delay between states,
		// all states will be ticked in the same frame if there is a transition.
		// However, the programmer must avoid a transition loop, or this code will hang.
		bool needsUpdate = true;
		while( needsUpdate )
		{
			T next = _onUpdate[_currentStateIndex]();
			needsUpdate = TransitionTo( next );
		}
	}

	protected bool TransitionTo( T nextState )
	{
		int nextStateIndex = Convert.ToInt32( nextState );
		if( nextStateIndex == _currentStateIndex )
		{
			return false;
		}

		_onExit[_currentStateIndex]();
		_onEnter[nextStateIndex]();
		_currentStateIndex = nextStateIndex;
		return true;
	}


	// private

	void NullInit() {}
}