using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform gridParent;
    public int width = 5;
    public int height = 7;
    public int mineCount = 5;

    private Cell[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Cell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject obj = Instantiate(cellPrefab, gridParent);
                Cell cell = obj.GetComponent<Cell>();
                cell.Setup(x, y, this);
                grid[y, x] = cell;
            }
        }

        PlaceMines();
        CalculateNeighbors();
    }

    void PlaceMines()
    {
        int placed = 0;
        while (placed < mineCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (!grid[y, x].isMine)
            {
                grid[y, x].isMine = true;
                placed++;
            }
        }
    }

    void CalculateNeighbors()
    {
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            if (grid[y, x].isMine) continue;

            int count = 0;
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && grid[ny, nx].isMine)
                    count++;
            }

            grid[y, x].neighborMines = count;
        }
    }

    public void FloodFill(int x, int y)
    {
        for (int dy = -1; dy <= 1; dy++)
        for (int dx = -1; dx <= 1; dx++)
        {
            int nx = x + dx, ny = y + dy;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                Cell cell = grid[ny, nx];
                if (!cell.isRevealed && !cell.isMine)
                    cell.Reveal();
            }
        }
    }

    public void CheckWin()
    {
        int revealed = 0;
        foreach (Cell c in grid)
            if (c.isRevealed) revealed++;

        if (revealed == width * height - mineCount)
        {
            Debug.Log("Victory!");
        }
    }

    public void GameOver()
    {
        foreach (Cell c in grid)
        {
            if (c.isMine)
            {
                c.label.text = "💣";
                c.button.interactable = false;
            }
        }

        Debug.Log("Game Over");
    }
}
