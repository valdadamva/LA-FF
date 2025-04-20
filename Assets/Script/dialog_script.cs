using UnityEngine;
using DialogueEditor;

[RequireComponent(typeof(Collider))]
public class ConversationTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCConversation myConversation;
    [SerializeField] private FirstPersonLook cameraLook;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    [SerializeField] private bool lockMovementDuringDialogue = true;

    private bool isSubscribed = false;

    void Start()
    {
        if (cameraLook == null)
            cameraLook = FindFirstObjectByType<FirstPersonLook>();

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (ConversationManager.Instance == null)
        {
            Debug.LogError("ConversationManager не найден!");
            enabled = false;
        }

        if (!isSubscribed)
        {
            ConversationManager.OnConversationStarted += OnDialogueStarted;
            ConversationManager.OnConversationEnded += OnDialogueEnded;
            isSubscribed = true;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(interactionKey) && !ConversationManager.Instance.IsConversationActive)
        {
            ConversationManager.Instance.StartConversation(myConversation);
        }
    }

    private void OnDialogueStarted()
    {
        Debug.Log("Диалог начался — блокируем управление");

        if (cameraLook != null)
        {
            cameraLook.SetCameraEnabled(false);
            cameraLook.SetCursorState(true);
        }

        if (lockMovementDuringDialogue && playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }
    }

    private void OnDialogueEnded()
    {
        Debug.Log("Диалог завершён — возвращаем управление");

        if (cameraLook != null)
        {
            cameraLook.SetCameraEnabled(true);
            cameraLook.SetCursorState(false);
        }

        if (lockMovementDuringDialogue && playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }
    }

    private void OnDestroy()
    {
        if (isSubscribed)
        {
            ConversationManager.OnConversationStarted -= OnDialogueStarted;
            ConversationManager.OnConversationEnded -= OnDialogueEnded;
        }
    }
}
