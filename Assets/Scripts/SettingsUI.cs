using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsUI : UI
{

    public static SettingsUI instance = null;
    VisualElement root;
    Slider sound;
    Button quit;
    Button restart;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        sound = root.Query<Slider>("Sound");
        quit = root.Query<Button>("Quit");
        restart = root.Query<Button>("Restart");


        quit.clicked += () =>
        {
            if (SceneManager.GetActiveScene().buildIndex != 0) 
            {
                SceneManager.LoadScene(0); // Go to title if not there
                InputSystem.actions.FindActionMap("Player").Disable();
                GameUI.instance.ResetLevel();
                GameUI.instance.Toggle(false);
                ToggleLevelRestart(false);
            }
            Toggle(false);
        };
        quit.RegisterCallback<PointerEnterEvent>(MouseEnter);
        quit.RegisterCallback<PointerLeaveEvent>(MouseExit);

        restart.clicked += () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameUI.instance.ResetLevel();
        };
        restart.RegisterCallback<PointerEnterEvent>(MouseEnter);
        restart.RegisterCallback<PointerLeaveEvent>(MouseExit);


        // Start off by defualt
        root.style.scale = new Vector2(1, 0);
        root.style.display = DisplayStyle.None;
        ToggleLevelRestart(false);
    }

    public void Toggle(bool turnOn)
    {
        if (visualCoroutines.ContainsKey(root) && visualCoroutines[root] != null)
        {
            return;
        }

        Debug.Log("Turning on here" + turnOn);
        if (turnOn)
        {
            root.style.display = DisplayStyle.Flex;
            visualCoroutines[root] = StartCoroutine(UIToggle(root, true));
        }
        else
        {
            visualCoroutines[root] = StartCoroutine(UIToggle(root, false));
        }
    }

    public void ToggleLevelRestart(bool toggleOn)
    {
        if (toggleOn)
        {
            restart.SetEnabled(true);
            restart.text = "Restart";

        }
        else
        {
            restart.SetEnabled(false);
            restart.text = "";
        }
    }
}
