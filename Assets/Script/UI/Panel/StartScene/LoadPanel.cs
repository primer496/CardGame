using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadPanel : BasePanel
{
    private Transform UIExitButton;
    private Transform UILoadButton;
    private Transform UIAccountInput;
    private Transform UIPasswordInput;


    private string account;
    private string password;
    protected override void Awake()
    {
        UIExitButton = transform.Find("ExitButton");
        UILoadButton = transform.Find("LoadButton");
        UIAccountInput = transform.Find("Account/AccountInput");
        UIPasswordInput = transform.Find("Password/PasswordInput");

        UIExitButton.GetComponent<Button>().onClick.AddListener(OnClickExit);
        UILoadButton.GetComponent<Button>().onClick.AddListener(OnClickLoad);
        UIAccountInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetAccount);
        UIPasswordInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetPassword);

        account = "";
        password = "";
    }
    private void OnClickLoad()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void OnClickExit()
    {
        UIManager.Instance.ClosePanel(UIConst.LoadPanel);
    }
    private void GetAccount(string account)
    {
        Debug.Log("账号:" + account);
    }
    private void GetPassword(string password)
    {
        Debug.Log("密码:" + password);
    }
}
