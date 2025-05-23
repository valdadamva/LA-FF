using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour 
{
    [SerializeField] private GameObject yellowBlock;
    [SerializeField] private GameObject redBlock;
    [SerializeField] private GameObject FoodBlock;

    private Block[] snake = new Block[1000];
    private int w = 10;
    private int h = 20;
    private int size = 4;
    private float delay = 0.1f;
    private Vector2 direction = Vector2.up;
    private Block food = new Block();

    public bool isGameOver = false;
    private GUIStyle guiStyle = new GUIStyle();

    private List<GameObject> yellowBlocks = new List<GameObject>(); // ДОБАВЛЕНО

    public class Block
    {
        public Vector2 pos = new Vector2();
        public GameObject ob;
    }

    public void Start()
    {
        Initialize();
        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        if (!isGameOver)
        {
            bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up;
            bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down;
            bool leftPressed = Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right;
            bool rightPressed = Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left;

            if (downPressed)
                direction = Vector2.down;
            else if (upPressed)
                direction = Vector2.up;
            else if (leftPressed)
                direction = Vector2.left;
            else if (rightPressed)
                direction = Vector2.right;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return)) // Enter
            {
                direction = Vector2.up;
                size = 4;
                isGameOver = false;
                Initialize();
            }
        }
    }

    void Initialize()
    {
        // Очищаем поле перед созданием новых блоков
        yellowBlocks.Clear();

        // Создание фона
        for (int i = 0; i <= w; i++)
        {
            for (int j = 0; j <= h; j++)
            {
                GameObject block = Instantiate(yellowBlock, new Vector3(i, j, -2397), Quaternion.identity);
                yellowBlocks.Add(block); // ДОБАВЛЕНО
            }
        }

        for (int i = 0; i < size; i++)
            CreateBlockIfNull(i);

        food.ob = Instantiate(FoodBlock, new Vector3(food.pos.x, food.pos.y, -2397), Quaternion.identity);
        ResetFood();
    }

    void CreateBlockIfNull(int index)
    {
        if (snake[index] == null)
        {
            snake[index] = new Block();
            snake[index].ob = Instantiate(redBlock, new Vector3(-100, -100, -2397), Quaternion.identity);
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (!isGameOver)
            {
                Move();
            }
            yield return new WaitForSeconds(delay);
        }
    }

    void Move()
    {
        for (int i = size; i > 0; i--)
        {
            CreateBlockIfNull(i);
            CreateBlockIfNull(i - 1);
            snake[i].pos = snake[i - 1].pos;
        }

        snake[0].pos += direction;

        if (CheckIfDead())
        {
            RestartGame();
            return;
        }

        CheckEatFood();
        CheckEdge();

        for (int i = 0; i < size; i++)
        {
            snake[i].ob.transform.position = new Vector3(snake[i].pos.x, snake[i].pos.y, -2397);
        }
    }

    void CheckEdge()
    {
        if (snake[0].pos.x > w)
            snake[0].pos.x = 0;
        else if (snake[0].pos.y > h)
            snake[0].pos.y = 0;
        else if (snake[0].pos.x < 0)
            snake[0].pos.x = w;
        else if (snake[0].pos.y < 0)
            snake[0].pos.y = h;
    }

    void CheckEatFood()
    {
        if (snake[0].pos.x == food.pos.x && snake[0].pos.y == food.pos.y)
        {
            size++;
            ResetFood();
        }
    }

    void ResetFood()
    {
        food.pos.x = Random.Range(1, w - 1);
        food.pos.y = Random.Range(1, h - 1);

        if (food.ob != null)
        {
            food.ob.transform.position = new Vector3(food.pos.x, food.pos.y, -2397);
        }
        else
        {
            Debug.LogError("Food GameObject is not assigned!");
        }
    }

    bool CheckIfDead()
    {
        for (int i = 1; i < size; i++)
        {
            if (snake[0].pos == snake[i].pos)
                return true;
        }
        return false;
    }

    public void RestartGame()
    {
        isGameOver = true;

        // Удаление сегментов змейки
        for (int i = 0; i < size; i++)
        {
            if (snake[i] != null && snake[i].ob != null)
                Destroy(snake[i].ob);
            snake[i] = null;
        }

        // Удаление еды
        if (food.ob != null)
        {
            Destroy(food.ob);
            food.ob = null;
        }
    }

    public void DestroyAll()
    {
        // Удаление всех блоков змейки
        for (int i = 0; i < snake.Length; i++)
        {
            if (snake[i] != null && snake[i].ob != null)
            {
                Destroy(snake[i].ob);
                snake[i] = null;
            }
        }

        // Удаление еды
        if (food != null && food.ob != null)
        {
            Destroy(food.ob);
            food.ob = null;
        }

        // Удаление всех желтых блоков
        foreach (var block in yellowBlocks)
        {
            if (block != null)
                Destroy(block);
        }
        yellowBlocks.Clear();

        // Сброс состояния
        size = 4;
        isGameOver = true;
    }

    void OnGUI()
    {
        if (isGameOver)
        {
            guiStyle.fontSize = 30;
            guiStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 500, 50), $"Game Over! Size: {size}", guiStyle);
            GUI.Label(new Rect(10, 50, 500, 50), "Press ENTER to restart", guiStyle);
        }
    }
}
