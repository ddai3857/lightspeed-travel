using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StatusUI : UI
{
    public static StatusUI instance = null;
    VisualElement root;
    Label status;
    Label completeTime;
    Label bestTime;
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

        status = root.Query<Label>("Status"); // Unused unless we want a death screen too
        completeTime = root.Query<Label>("CompleteTime");
        bestTime = root.Query<Label>("BestTime");

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
                SettingsUI.instance.ToggleLevelRestart(false);
            }
            SettingsUI.instance.Toggle(false);
            Toggle(false);
        };
        quit.RegisterCallback<PointerEnterEvent>(MouseEnter);
        quit.RegisterCallback<PointerLeaveEvent>(MouseExit);

        restart.clicked += () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameUI.instance.ResetLevel();
            Toggle(false);
        };
        restart.RegisterCallback<PointerEnterEvent>(MouseEnter);
        restart.RegisterCallback<PointerLeaveEvent>(MouseExit);

        // Dbug M as win button
        InputAction debugAction = InputSystem.actions.FindAction("Debug");
        debugAction.started += (InputAction.CallbackContext context) => {Toggle(true);};

        // Start off by defualt
        root.style.scale = new Vector2(1, 0);
        root.style.display = DisplayStyle.None;
    }

    public void Toggle(bool turnOn)
    {
        if (visualCoroutines.ContainsKey(root) && visualCoroutines[root] != null)
        {
            return;
        }

        if (turnOn)
        {
            GameUI.instance.TogglePause();
            float currTime = GameUI.instance.time;
            Debug.Log(LevelUI.currLevel);
            float oldBestTime = LevelUI.levelTimes[LevelUI.currLevel];
            if (oldBestTime < 0 || oldBestTime > currTime)
            {
                // Save new best time
                LevelUI.levelTimes[LevelUI.currLevel] = currTime;
                oldBestTime = currTime;
            }
            System.TimeSpan span = System.TimeSpan.FromSeconds(currTime);
            System.TimeSpan span2 = System.TimeSpan.FromSeconds(oldBestTime);
            completeTime.text = "Your time was: " + $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
            bestTime.text = "Best Time: " + $"{(int)span2.TotalMinutes:D2}:{span2.Seconds:D2}";

            root.style.display = DisplayStyle.Flex;
            visualCoroutines[root] = StartCoroutine(UIToggle(root, true));
        }
        else
        {
            visualCoroutines[root] = StartCoroutine(UIToggle(root, false));
        }
    }


}
