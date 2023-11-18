using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Dropdown scene_name;
    public TMP_InputField path_field;
    public TMP_InputField start_field;
    public TMP_InputField end_field;

    public Button start_button;

    void Start()
    {
        start_button.onClick.AddListener(() => start_scene());
        if(GlobalVariables.path != null)
        {
            path_field.text = GlobalVariables.path;
        }
    }


    private void start_scene()
    {
        GlobalVariables.path = path_field.text;
        GlobalVariables.start_from = int.Parse(start_field.text);
        GlobalVariables.end_at = int.Parse(end_field.text);

        string scene_name_text = scene_name.options[scene_name.value].text;

        if (scene_name_text == "scene_1")
            SceneManager.LoadScene(1);
        else if (scene_name_text == "scene_2")
            SceneManager.LoadScene(2);
        else if (scene_name_text == "scene_3")
            SceneManager.LoadScene(3);
        else if (scene_name_text == "scene_4")
            SceneManager.LoadScene(4);

    }

}
