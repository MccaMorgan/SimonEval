using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game loop and game state
/// </summary>
public class GameManager : CoroutineStateMachine
{
    // How much time should pass between the user entering the correct sequence, and the sequence repeating
    [SerializeField] private float _sequenceRepeatDelay = 0;

    // How much time should pass between tones when playing the sequence to the player
    [SerializeField] private float _toneInterval = 0;

    // The four gameplay buttons
    [SerializeField] private List<SimonButton> _allButtons = new List<SimonButton>();

    // Parent for all PreGameState UI
    [SerializeField] private RectTransform _preGameUiPanel = null;

    // Each time a new button is randomly chosen, a reference to the button component is added to this list in order of the sequence
    private List<SimonButton> _currentSequence = new List<SimonButton>();

    // The number of buttons the player has pressed so far while attempting to match the sequence this round
    private int _userInputsThisRound = 0;

    // Is the game ready to for the players inputs to be tested against the sequence
    private bool _inputReady = false;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        ChangeState(PreGameState);
    }

    /// <summary>
    /// Listener for user pressing the start button
    /// </summary>
    public void ButtonPressedStart()
    {
        ChangeState(PlaySequence);
    }

    /// <summary>
    /// Listener for button inputs
    /// </summary>
    public void OnUserInput(SimonButton userInputtedButtonComponent)
    {
        if (CurrentState != WaitForUserInput || !_inputReady)
        {
            // A button was pressed before the sequence started or finished playing, considered a game over
            RestartGame();
        }

        if (_currentSequence.Count <= 0)
            return;
        
        if (_currentSequence[_userInputsThisRound] == userInputtedButtonComponent)
        {
            _userInputsThisRound++;
        }
        else
        {
            // Incorrect, game over
            RestartGame();
        }
    }

    /// <summary>
    /// This state waits for the player to press start
    /// </summary>
    private IEnumerator PreGameState(MethodInfo thisMethod)
    {
        _preGameUiPanel.gameObject.SetActive(true);

        while (CurrentState.Method == thisMethod)
        {
            yield return null;
        }

        _preGameUiPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// This state plays the current sequence to the player
    /// </summary>
    private IEnumerator PlaySequence(MethodInfo thisMethod)
    {
        // Selects a button at random and adds a reference to its SimonButton component to the list
        _currentSequence.Add(_allButtons[Random.Range(0, _allButtons.Count)]);
        yield return new WaitForSeconds(_sequenceRepeatDelay);

        while (CurrentState.Method == thisMethod)
        {
            foreach (SimonButton currentSequenceElement in _currentSequence)
            {
                currentSequenceElement.PlayTone();
                yield return new WaitForSeconds(currentSequenceElement.ToneLength() + _toneInterval);
                ChangeState(WaitForUserInput);
            }

            yield return null;
        }
    }

    /// <summary>
    /// This state watches for the users input and changes states when the player has matched the entire sequence 
    /// </summary>
    private IEnumerator WaitForUserInput(MethodInfo thisMethod)
    {
        _inputReady = true;

        while (CurrentState.Method == thisMethod)
        {
            if (_userInputsThisRound >= _currentSequence.Count)
            {
                _userInputsThisRound = 0;
                ChangeState(PlaySequence);
            }

            yield return null;
        }

        _inputReady = false;
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}