using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public float JumpForce;
    public TMP_Text survivalTimeText;
    public SpikeGenerator spikeGenerator;  // ссылка на генератор шипов

    private float time = 0f;
    private bool isGameOver = false;
    private bool isGrounded = false;

    private Rigidbody RB;
    private Vector3 startPosition;

    void Awake()
    {
        RB = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isGameOver)
        {
            time += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                RB.linearVelocity = Vector3.zero;
                RB.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartMiniGame();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("sole"))
        {
            isGrounded = true;
        }
    }

    public void EndGame()
    {
        isGameOver = true;

        if (survivalTimeText != null)
        {
            survivalTimeText.gameObject.SetActive(true);

            float note = time / 3f;
            int score = Mathf.Clamp((int)note, 0, 20);
            survivalTimeText.text = $"Ta note : {score}/20\nAppuie sur ENTER pour recommencer";
        }
    }

    void RestartMiniGame()
    {
        // Сброс состояния игрока
        isGameOver = false;
        time = 0f;

        // Сброс позиции и физики
        transform.position = startPosition;
        RB.linearVelocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;

        // Сброс UI
        if (survivalTimeText != null)
        {
            survivalTimeText.gameObject.SetActive(false);
        }

        // Сброс генератора шипов
        if (spikeGenerator != null)
        {
            spikeGenerator.Restart();
        }

        isGrounded = true;
    }
}
