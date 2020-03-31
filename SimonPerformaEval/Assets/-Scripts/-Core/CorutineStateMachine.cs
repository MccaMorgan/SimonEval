using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// A finite state machine using coroutines that allows start, update, and end logic to be defined in a single method
/// </summary>
public class CoroutineStateMachine : MonoBehaviour 
{
	// Delegate that defines a behaviour state 
	protected delegate IEnumerator BehaviourState(MethodInfo thisMethod);

	// The current state
	protected BehaviourState CurrentState { get; private set; }

	/// The time in seconds since start up that the last state changed occured
	private float _timeOflLastStateChange;
	
	/// <summary>
	/// Change State
	/// </summary>
	protected void ChangeState(BehaviourState newState)
	{
		SyncStateChange(newState);
	}
	
	/// <summary>
	/// Handles state change logic
	/// </summary>
	private void SyncStateChange(BehaviourState newState)
	{
        if(CurrentState != null)
	        StopCoroutine(CurrentState.Method.Name);

        CurrentState = newState;
		_timeOflLastStateChange = Time.time;
		StartCoroutine(		newState(newState.Method)	);
		OnStateChange(CurrentState);
	}

	/// <summary>
	/// How long the current state has been the current state
	/// </summary>
	/// <returns></returns>
	protected float LocalTime()
	{
		return Time.time - _timeOflLastStateChange;
	}

	/// <summary>
	/// When the current state is changed
	/// </summary>
	/// <param name="newState"></param>
	protected virtual void OnStateChange(BehaviourState newState)
	{
		//Intentionally Blank
	}
	
    
	/*
	/// <summary>
	/// An example of how a behaviour state method should look in the derived classes
	/// </summary>
	protected IEnumerator ExampleOnlyDoNotUSe(MethodInfo thisMethod)
	{
		//Code here will be run once when this state becomes the current state
		
		while(CurrentState.Method == thisMethod)
		{
			//Code here will be run every frame while this state remains the current state
			yield return null;
		}
		
		//Code here will be run once when this state stops being the current state
	}
	*/
}