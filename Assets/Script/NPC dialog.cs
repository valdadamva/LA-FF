using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // Ссылка на диалоговые реплики

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

