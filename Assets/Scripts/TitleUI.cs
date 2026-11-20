using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleUI : UI
{
    VisualElement root;
    Button start;
    Button settings;
    Button quit;
    Label title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        title = root.Query<Label>("Title");
        title.text = "Light<color=#ADD8E6>speed</color> Travel";

        start = root.Query<Button>("Start");
        settings = root.Query<Button>("Settings");
        quit = root.Query<Button>("Quit");

        start.clicked += () =>
        {
            SceneManager.LoadScene("LevelSelect");
        };
        settings.clicked += () =>
        {
            SettingsUI.instance.Toggle(true);
        };
        quit.clicked += () =>
        {
            Application.Quit();
        };

        start.RegisterCallback<PointerEnterEvent>(MouseEnter);
        start.RegisterCallback<PointerLeaveEvent>(MouseExit);
        settings.RegisterCallback<PointerEnterEvent>(MouseEnter);
        settings.RegisterCallback<PointerLeaveEvent>(MouseExit);
        quit.RegisterCallback<PointerEnterEvent>(MouseEnter);
        quit.RegisterCallback<PointerLeaveEvent>(MouseExit);

        // Default don't use player inputs outside of levels
        InputSystem.actions.FindActionMap("Player").Disable();
    }
}
