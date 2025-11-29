using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;  // <-- Needed for Play Again

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    [Header("Joystick Control")]
    public FixedJoystick fixedJoystick;

    [Header("UI Buttons")]
    public GameObject playAgainButton;
    public GameObject quitButton;

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);

        // Hide buttons at start
        playAgainButton.SetActive(false);
        quitButton.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winTextObject.SetActive(true);
            ShowEndButtons();
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        Vector3 joyMovement =
            Vector3.forward * fixedJoystick.Vertical +
            Vector3.right * fixedJoystick.Horizontal;

        Vector3 finalMovement = movement + joyMovement;

        rb.AddForce(finalMovement * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);

            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";

            ShowEndButtons();
        }
    }

    // Show UI buttons when game ends
    void ShowEndButtons()
    {
        playAgainButton.SetActive(true);
        quitButton.SetActive(true);
    }

    // Called by Play Again button
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called by Quit button
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops play mode
#else
        Application.Quit(); // quits app on Android or PC build
#endif
    }
}
