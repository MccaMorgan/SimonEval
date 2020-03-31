using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager for the user input virtual button
/// </summary>
public class SimonButton : MonoBehaviour
{
    // Tone that sounds when this button is pressed, as well as when randomly selected in the sequence
    [SerializeField] private AudioClip _buttonTone = null;

    // Reference to the game manager in this scene
    private GameManager _gameManager;

    // The color this button was at start so it can be reset after flashing
    private Color _startingButtonColor;

    // Audio source attached to this game object
    private AudioSource _attachedAudioSource;

    // Image component attached to this object
    private Image _attachedImageComponent;


    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _attachedImageComponent = GetComponent<Image>();
        _startingButtonColor = _attachedImageComponent.color;

        _attachedAudioSource = GetComponent<AudioSource>();
        _attachedAudioSource.clip = _buttonTone;
    }

    /// <summary>
    /// Returns the length of the audio clip for this buttons tone
    /// </summary>
    /// <returns></returns>
    public float ToneLength()
    {
        return _buttonTone.length;
    }

    /// <summary>
    /// Listener for user input on this button
    /// </summary>
    public void ButtonPressedColoredButton()
    {
        PlayTone();
        _gameManager.OnUserInput(this);
    }

    /// <summary>
    /// Play this tone, either on user input or when the game manager calls this when running through the current sequence
    /// </summary>
    public void PlayTone()
    {
        StartCoroutine(FlashButtonLight());
        _attachedAudioSource.PlayOneShot(_buttonTone);
    }

    /// <summary>
    /// Changes the color of the button graphic to represent the button lighting up
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashButtonLight()
    {
        _attachedImageComponent.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        _attachedImageComponent.color = _startingButtonColor;
        yield return null;
    }
}