using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelUI : UI
{

    VisualElement root;
    List<Button> levels = new List<Button>();
    public static List<float> levelTimes = Enumerable.Repeat(-1f, 6).ToList();
    public static int currLevel = -1;
    Button back;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        for (int i = 0; i < 6; i++)
        {
            string name = "Level" + (i + 1);
            levels.Add(root.Query<Button>(name));
            levels[i].RegisterCallback<PointerEnterEvent>(MouseEnter);
            levels[i].RegisterCallback<PointerLeaveEvent>(MouseExit);
    
            // Show best time
            Label levelText = root.Query<Label>(name + "Text");
            if (levelTimes[i] < 0)
            {
                levelText.text = "XX:XX";
            }
            else
            {
                TimeSpan span = TimeSpan.FromSeconds(levelTimes[i]);
                levelText.text = $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
            }

            int curr = i;
            if (SceneUtility.GetBuildIndexByScenePath(name) >= 0)
            {
                levels[i].clicked += () =>
                {
                    if (SceneManager.GetActiveScene().buildIndex != 0) 
                    {
                        SceneManager.LoadScene(name); // Go to level

                        SettingsUI.instance.ToggleLevelRestart(true);
                        GameUI.instance.Toggle(true);
                        InputSystem.actions.FindActionMap("Player").Enable();

                        currLevel = curr;
                    }
                };
            }
        }


        back = root.Query<Button>("Back");
        back.clicked += () =>
        {
            SceneManager.LoadScene(0);
        };

        back.RegisterCallback<PointerEnterEvent>(MouseEnter);
        back.RegisterCallback<PointerLeaveEvent>(MouseExit);
    }
}
