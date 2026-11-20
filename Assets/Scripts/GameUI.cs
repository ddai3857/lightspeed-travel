using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    public static GameUI instance = null;
    VisualElement root;
    Label timer;
    public float time = 0;
    bool isOn = false;
    bool isPaused = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Reuse this instance
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        timer = root.Query<Label>("Timer");

        //Make P the pause button (opens settings and pauses game)
        InputAction pauseAction = InputSystem.actions.FindAction("Pause");
        pauseAction.started += (InputAction.CallbackContext context) => {TogglePause();};

        //Start toggle off until level starts
        Toggle(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (isOn && !isPaused)
        {
            time += Time.deltaTime;
        }

        System.TimeSpan span = System.TimeSpan.FromSeconds(time);
        timer.text = $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
    }

    public void TogglePause()
    {
        isPaused = !isPaused; 

        SettingsUI.instance.Toggle(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused) // Pause game
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
    public void ResetLevel()
    {
        TogglePause();
        time = 0;
    }

    public void Toggle(bool toggleOn)
    {
        if (toggleOn)
        {
            root.style.display = DisplayStyle.Flex;
        }
        else
        {
            root.style.display = DisplayStyle.None;
        }
        isOn = toggleOn;
    }
}
