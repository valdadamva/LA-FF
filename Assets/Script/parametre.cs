using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : NetworkBehaviour
{
    public GameObject infoCanvas; // Canvas с UI
    public Slider valueSlider; // Слайдер от 0 до 100
    public Text valueText; // Отображение значения слайдера

    public InputField nameInput, hobbyInput, ageInput, genderInput; // Поля ввода

    [SyncVar(hook = nameof(OnNameChanged))] private string playerName = "Алекс";
    [SyncVar(hook = nameof(OnHobbyChanged))] private string playerHobby = "Геймдев";
    [SyncVar(hook = nameof(OnAgeChanged))] private string playerAge = "25";
    [SyncVar(hook = nameof(OnGenderChanged))] private string playerGender = "Мужской";
    [SyncVar(hook = nameof(OnSliderChanged))] private float playerValue = 50f;

    private bool isCanvasVisible = false; // Статус отображения Canvas

    void Start()
    {
        infoCanvas.SetActive(false); // Изначально Canvas скрыт

        valueSlider.minValue = 0;
        valueSlider.maxValue = 100;
        valueSlider.onValueChanged.AddListener(delegate { CmdSetSlider(valueSlider.value); });

        // Только локальный игрок может изменять данные
        if (isLocalPlayer)
        {
            nameInput.onEndEdit.AddListener(delegate { CmdSetName(nameInput.text); });
            hobbyInput.onEndEdit.AddListener(delegate { CmdSetHobby(hobbyInput.text); });
            ageInput.onEndEdit.AddListener(delegate { CmdSetAge(ageInput.text); });
            genderInput.onEndEdit.AddListener(delegate { CmdSetGender(genderInput.text); });
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E)) // Если нажата клавиша E
        {
            ToggleCanvas();
        }
    }

    void ToggleCanvas()
    {
        isCanvasVisible = !isCanvasVisible;
        infoCanvas.SetActive(isCanvasVisible);
    }

    // === Команды для сервера ===	
    [Command]
    void CmdSetName(string newName) { playerName = newName; }
    [Command]
    void CmdSetHobby(string newHobby) { playerHobby = newHobby; }
    [Command]
    void CmdSetAge(string newAge) { playerAge = newAge; }
    [Command]
    void CmdSetGender(string newGender) { playerGender = newGender; }
    [Command]
    void CmdSetSlider(float newValue) { playerValue = newValue; }

    // === Хуки для обновления UI ===
    void OnNameChanged(string oldName, string newName) { nameInput.text = newName; }
    void OnHobbyChanged(string oldHobby, string newHobby) { hobbyInput.text = newHobby; }
    void OnAgeChanged(string oldAge, string newAge) { ageInput.text = newAge; }
    void OnGenderChanged(string oldGender, string newGender) { genderInput.text = newGender; }
    void OnSliderChanged(float oldValue, float newValue) 
    { 
        valueSlider.value = newValue; 
        valueText.text = "Значение: " + Mathf.RoundToInt(newValue);
    }
}
