using UnityEngine;

[System.Serializable]
public class Dialogue  : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] sentences ; // Массив фраз NPC
}

