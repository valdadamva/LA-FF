using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int x, y;
    public bool isMine;
    public int neighborMines;
    public bool isRevealed;
    public bool isFlagged;

    public Button button;
    public Text label;
    public Image flagIcon;
    public GridManager manager;

    public void Setup(int x, int y, GridManager manager)
    {
        this.x = x;
        this.y = y;
        this.manager = manager;
    }

    public void Reveal()
    {
        if (isRevealed || isFlagged) return;

        isRevealed = true;
        button.interactable = false;

        if (isMine)
        {
            label.text = "💣";
            manager.GameOver();
        }
        else
        {
            label.text = neighborMines > 0 ? neighborMines.ToString() : "";
            if (neighborMines == 0)
                manager.FloodFill(x, y);
        }

        manager.CheckWin();
    }

    public void ToggleFlag()
    {
        if (isRevealed) return;

        isFlagged = !isFlagged;
        flagIcon.enabled = isFlagged;
    }
}