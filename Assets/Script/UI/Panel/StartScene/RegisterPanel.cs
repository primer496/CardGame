using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private Transform UIExitButton;
    private Transform UIConfirmButton;
    private Transform UIAccountInput;
    private Transform UIPasswordInput;
    private Transform UIPasswordRepeatInput;

    private string Account;
    private string Password;
    private string PasswordRepeat;
    override protected void Awake()
    {
        UIExitButton = transform.Find("ExitButton");
        UIConfirmButton = transform.Find("RegisterConfirm");
        UIAccountInput = transform.Find("Account/AccountInput");
        UIPasswordInput = transform.Find("Password/PasswordInput");
        UIPasswordRepeatInput = transform.Find("Repeat/RepeatInput");

        UIExitButton.GetComponent<Button>().onClick.AddListener(OnclickExit);
        UIConfirmButton.GetComponent<Button>().onClick.AddListener(OnclickConfirm);
        UIAccountInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetAccount);
        UIPasswordInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetPassword);
        UIPasswordRepeatInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(GetRepeat);

        Account = "";
        Password = "";
        PasswordRepeat = "";
    }
    private void OnclickExit()
    {
        UIManager.Instance.ClosePanel(UIConst.RegisterPanel);
    }
    private void OnclickConfirm()
    {
        // 在这里检查密码重复输入是否正确
        UIManager.Instance.ClosePanel(UIConst.RegisterPanel);
    }
    private void GetAccount(string content)
    {
        Debug.Log("账户：" + content);
    }
    private void GetPassword(string content) 
    {
        Debug.Log("密码：" + content);
    }
    private void GetRepeat(string content)
    {
        Debug.Log("重复：" + content);
    }
}
