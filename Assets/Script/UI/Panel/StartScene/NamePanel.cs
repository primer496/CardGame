using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NamePanel : BasePanel
{
    private Transform UICreateButton;
    private Transform UINameInput;

    private string Name;

    protected override void Awake()
    {
        UICreateButton = transform.Find("ConfrimButton");
        UINameInput = transform.Find("NameInput");

        UICreateButton.GetComponent<Button>().onClick.AddListener(OnClickConfirm);
        UINameInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetName);

        Name = "";
    }
    private void OnClickConfirm()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void GetName(string Name)
    {
        Debug.Log("姓名"+Name);
    }
}
