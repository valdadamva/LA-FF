using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Добавляем правильное пространство имен

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText; // UI-текст для отображения диалога
    public GameObject dialoguePanel; // Панель с текстом
    private Queue<string> sentences = new Queue<string>(); // Очередь для хранения фраз

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}