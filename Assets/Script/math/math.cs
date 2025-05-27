using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Game15Puzzle3D : MonoBehaviour
{
    [Header("Настройки")]
    public List<Texture> numberTextures; // Текстуры для чисел 1-15
    public Material tileMaterial;       // Базовый материал
    public float tileSize = 1f;        // Размер куба
    public float spacing = 0.1f;       // Расстояние между кубами

    public float time = 0f;

    private int[,] grid = new int[4, 4];
    private GameObject[,] tileObjects = new GameObject[4, 4];
    private Vector2Int emptyPos;
    
    private GUIStyle guiStyle = new GUIStyle();
    
    private bool hasWon = false;
    private float finalTime = 0f;


    void Start()
    {
        // Проверка ресурсов
        if (numberTextures == null || numberTextures.Count < 15)
        {
            Debug.LogError("Не хватает текстур! Нужно 15 текстур.");
            return;
        }

        if (tileMaterial == null)
        {
            Debug.LogError("Не назначен материал!");
            return;
        }

        InitializeGrid();
        CreateVisualTiles(); 
        hasWon = false;        // <--- сбрасываем статус победы
        ShuffleTiles();        // теперь перемешивание будет работать

    }

    void InitializeGrid()
    {
        int num = 1;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                grid[x, y] = (num <= 15) ? num : 0;
                num++;
            }
        }
        emptyPos = new Vector2Int(3, 3);
    }

    void CreateVisualTiles()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                // Создаем куб
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.SetParent(transform);
                
                // Позиционируем
                tile.transform.localPosition = new Vector3(
                    (3-x) * (tileSize + spacing),
                    0,
                    y * (tileSize + spacing)
                );
                
                // Масштабируем
                tile.transform.localScale = Vector3.one * tileSize;
                
                // Назначаем материал и текстуру
                Renderer renderer = tile.GetComponent<Renderer>();
                renderer.material = new Material(tileMaterial);
                
                // Сохраняем ссылку
                tileObjects[x, y] = tile;
                
                // Обновляем текстуру
                UpdateTileVisual(x, y, grid[x, y]);
            }
        }
    }

    void UpdateTileVisual(int x, int y, int num)
    {
        if (tileObjects[x, y] == null) return;

        Renderer renderer = tileObjects[x, y].GetComponent<Renderer>();
        if (renderer == null) return;

        if (num != 0)
        {
            renderer.material.mainTexture = numberTextures[num - 1];
            tileObjects[x, y].SetActive(true);
        }
        else
        {
            tileObjects[x, y].SetActive(false);
        }
    }

    void Update()
    {
        if (!hasWon)
            time += Time.deltaTime;

        if (hasWon && Input.GetKeyDown(KeyCode.Return))
        {
            Restart();
        }

        HandleInput();
    }


    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector2Int.right);
    }
    void TryMoveRaw(Vector2Int dir)
    {
        Vector2Int targetPos = emptyPos + dir;

        if (targetPos.x < 0 || targetPos.x >= 4 || targetPos.y < 0 || targetPos.y >= 4)
            return;

        // Обмен значениями
        grid[emptyPos.x, emptyPos.y] = grid[targetPos.x, targetPos.y];
        grid[targetPos.x, targetPos.y] = 0;

        // Обновляем визуал
        UpdateTileVisual(emptyPos.x, emptyPos.y, grid[emptyPos.x, emptyPos.y]);
        UpdateTileVisual(targetPos.x, targetPos.y, 0);

        emptyPos = targetPos;
    }


    void TryMove(Vector2Int dir)
    {
        if (hasWon) return; // запрет на движение после победы

        Vector2Int targetPos = emptyPos + dir;

        if (targetPos.x < 0 || targetPos.x >= 4 || targetPos.y < 0 || targetPos.y >= 4)
            return;

        // Обмен значениями
        grid[emptyPos.x, emptyPos.y] = grid[targetPos.x, targetPos.y];
        grid[targetPos.x, targetPos.y] = 0;

        // Обновляем визуал
        UpdateTileVisual(emptyPos.x, emptyPos.y, grid[emptyPos.x, emptyPos.y]);
        UpdateTileVisual(targetPos.x, targetPos.y, 0);

        emptyPos = targetPos;

        if (CheckWin() && !hasWon)
        {
            hasWon = true;
            finalTime = time;
            Debug.Log("Победа! Время: " + finalTime.ToString("F2"));
        }
    }


    void ShuffleTiles()
    {
        // Делаем случайные ходы ИЗ РЕШАЕМОЙ ПОЗИЦИИ
        for (int i = 0; i < 200; i++)
        {
            var directions = new List<Vector2Int> { 
                Vector2Int.up, Vector2Int.down, 
                Vector2Int.left, Vector2Int.right 
            };
        
            var validMoves = directions.FindAll(dir => 
                (emptyPos + dir).x >= 0 && (emptyPos + dir).x < 4 && 
                (emptyPos + dir).y >= 0 && (emptyPos + dir).y < 4
            );

            if (validMoves.Count > 0)
            {
                Vector2Int randomDir = validMoves[Random.Range(0, validMoves.Count)];
                TryMoveRaw(randomDir);
            }
        }

        // Если после перемешивания получилась нерешаемая комбинация - делаем ещё ход
        if (!IsSolvable())
        {
            TryMoveRaw(Vector2Int.right); // Простое исправление
        }
    }

    bool IsSolvable()
    {
        int inversions = 0;
        List<int> flatGrid = new List<int>();

        // Преобразуем 2D-массив в 1D-список (игнорируя пустую клетку)
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (grid[x, y] != 0) flatGrid.Add(grid[x, y]);
            }
        }

        // Считаем инверсии
        for (int i = 0; i < flatGrid.Count - 1; i++)
        {
            for (int j = i + 1; j < flatGrid.Count; j++)
            {
                if (flatGrid[i] > flatGrid[j]) inversions++;
            }
        }

        // Строка с пустым местом снизу (1–4)
        int emptyRowFromBottom = emptyPos.y + 1;

        // Условие решаемости для 4×4
        return (inversions + emptyRowFromBottom) % 2 == 0;
    }


    bool CheckWin()
    {
        int num = 1;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 3; x >= 0; x--)
            {
                if (grid[3-x, y] != num % 16)
                {
                    return false;
                }

                num++;
            }
        }
        Debug.Log("Победа! "+ time.ToString());
        return true;
    }


    void Restart()
    {
        InitializeGrid();
        ShuffleTiles();

        for (int x = 0; x < 4; x++)
        for (int y = 0; y < 4; y++)
            UpdateTileVisual(x, y, grid[x, y]);

        hasWon = false;
        time = 0f;
        finalTime = 0f;
    }

    
    void OnGUI()
    {
        if (hasWon)
        {
            guiStyle.fontSize = 30;
            guiStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 500, 50), $"Победа! Время: {finalTime:F2} секунд", guiStyle);
            GUI.Label(new Rect(10, 50, 500, 50), "Нажмите ENTER, чтобы начать заново", guiStyle);
        }
    }

}