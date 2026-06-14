using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected new string name;//覆盖自带的name定义
    protected virtual void Awake()
    {

    }
    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public virtual void Openpanel(string name)
    {
        this.name = name;
        SetActive(true);
    }
    public virtual void Closepanel()
    {
        isRemove = true;
        SetActive(false);
        Destroy(gameObject);
        if (UIManager.Instance.panelDict.ContainsKey(name))
        {
            UIManager.Instance.panelDict.Remove(name);
        }
    }
}
