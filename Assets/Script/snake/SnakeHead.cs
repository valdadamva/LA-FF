using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour 
{
    [SerializeField]
    private GameObject yellowBlock;  // Примечание: Убедитесь, что эти префабы назначены в инспекторе Unity.

    [SerializeField]
    private GameObject redBlock;     // Иначе возникнет NullReferenceException при попытке их использовать.

    private Block[] snake = new Block[1000];  // Примечание: Фиксированный размер массива (1000).
                                              // Если змейка может вырасти больше, это вызовет ошибку.
                                              // Лучше использовать List<Block> для динамического размера.

    private int w = 10;  // Ширина поля? Лучше сделать readonly или const, если это константа.
    private int h = 20;  // Высота поля? То же самое — можно добавить модификатор readonly.

    private int size = 4;  // Начальный размер змейки? Можно добавить [SerializeField], чтобы настраивать в редакторе.

    private float delay=0.1f;
    private Vector2 direction =Vector2.up;
    private Block food =new Block();
    
    
    public class Block
    {
        public Vector2 pos = new Vector2();  // Примечание: pos инициализирован по умолчанию (0,0).
        public GameObject ob;                 // ob не инициализирован — может вызвать NullReferenceException.
    }
    
    // Use this for initialization
    void Start()
    {
        Initialize();
        StartCoroutine(MoveRoutine());
    }
    
    // Update is called once per frame
   void Update()
   {
       // Проверка ввода с клавиатуры
       bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up;
       bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down;
       bool leftPressed = Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right;
       bool rightPressed = Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left;
   
       // Изменение направления с приоритетом последней нажатой клавиши
       if (downPressed)
           direction = Vector2.down;
       else if (upPressed)
           direction = Vector2.up;
       else if (leftPressed)
           direction = Vector2.left;
       else if (rightPressed)
           direction = Vector2.right;
   }
    
    void Initialize()
    {
        // Исправлено: Vector2(i, j) вместо Vector2(i, i)
        for (int i = 0; i <= w; i++)
            for (int j = 0; j <= h; j++)
                Instantiate(yellowBlock, new Vector2(i, j), Quaternion.identity);
    
        for (int i = 0; i < size; i++)
            CreateBlockIfNull(i);
        
        food.ob=Instantiate(redBlock, new Vector2(food.pos.x, food.pos.y),Quaternion.identity);
        
        ResetFood();
    }
    
    void CreateBlockIfNull(int index)
    {
        if (snake[index] == null)
        {
            snake[index] = new Block();
            snake[index].ob = Instantiate(redBlock, new Vector2(-100, -100), Quaternion.identity);
            // Примечания:
            // 1. Нет проверки, что redBlock не равен null (может вызвать NullReferenceException)
            // 2. Блок создается в (-100, -100) - возможно, лучше инициализировать в видимой области
            // 3. Не обновляется pos в структуре Block (остается Vector2.zero)
        }
    }
    
    IEnumerator MoveRoutine()
    {
        while(true)
        {
            Move(); // Примечание: Метод Move() не показан в коде
            yield return new WaitForSeconds(delay);
            // Примечания:
            // 1. Переменная delay не объявлена - нужно добавить [SerializeField] private float delay = 0.5f;
            // 2. Нет обработки остановки корутины (например, при Game Over)
            // 3. Бесконечный цикл может вызвать проблемы при перезапуске игры
        }
    }
    void Move()
    {
        // Обновление позиций сегментов змейки (от хвоста к голове)
        for (int i = size; i > 0; i--)
        {
            CreateBlockIfNull(i);      // Примечание: Избыточный вызов - можно вынести за цикл
            CreateBlockIfNull(i - 1);  // Примечание: Двойное создание блоков может снижать производительность
            
            snake[i].pos.x = snake[i - 1].pos.x;
            snake[i].pos.y = snake[i - 1].pos.y;
            // Примечание: Можно заменить одной строкой: snake[i].pos = snake[i - 1].pos;
        }
    
        // Обновление позиции головы
        snake[0].pos += direction;  // Примечание: Переменная direction не объявлена в показанном коде
                                   // Нужно добавить: [SerializeField] private Vector2 direction = Vector2.right;
                  
        if (CheckIfDead())
        {
            RestartGame();
            return;
        }
        CheckEatFood();
        CheckEdge();
        // Обновление позиций GameObject на сцене
        for(int i = 0; i < size; i++)
        {
            // Примечание: Опечатка в оригинале - new Vector2[Snake[i].pos.x] должно быть new Vector2(
            // Также ошибка в регистре Snake vs snake
            snake[i].ob.transform.position = new Vector2(snake[i].pos.x, snake[i].pos.y);
        }
    }
    
    
    void CheckEdge()
    {
        // Проверка правой границы
        if (snake[0].pos.x > w)
        {
            snake[0].pos.x = 0;
        }
        // Проверка верхней границы
        else if (snake[0].pos.y > h)
        {
            snake[0].pos.y = 0;
        }
        // Проверка левой границы
        else if (snake[0].pos.x < 0)
        {
            snake[0].pos.x = w;
        }
        // Проверка нижней границы
        else if (snake[0].pos.y < 0)
        {
            snake[0].pos.y = h;
        }
    }
    
    void CheckEatFood()
    {
        if (snake[0].pos.x==food.pos.x && snake[0].pos.y==food.pos.y)
        {
            size++;
            ResetFood();
        }
        
    }
    
    void ResetFood()
    {
        // Генерация случайной позиции в пределах игрового поля
        food.pos.x = Random.Range(1, w - 1);  // Исключаем самую правую границу
        food.pos.y = Random.Range(1, h - 1);  // Исключаем самую верхнюю границу
        
        // Обновление позиции GameObject на сцене
        if (food.ob != null)
        {
            food.ob.transform.position = new Vector2(food.pos.x, food.pos.y);
        }
        else
        {
            Debug.LogError("Food GameObject is not assigned!");
        }
    }
    
    bool CheckIfDead()
    {
        for (int i=1;i<size;i++)
        {
            if (snake[0].pos.x==snake[i].pos.x && snake[0].pos.y==snake[i].pos.y)
                return true;
        }
        return false;
    }
    
    
    void RestartGame()
    {
        // Оригинальный код без изменений:
        for(int i = 0; i < size; i++)
        {
            Destroy(snake[i].ob);  // Примечание: Нет проверки на null (может вызвать NullReferenceException)
            snake[i] = null;      // Примечание: Обнуление элемента массива
        }
        
        direction = Vector2.up;  // Сброс направления
        size = 4;                // Сброс размера змейки
    }
            
}