using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager _instance;
    private Transform _uiRoot;
    private Dictionary<string, string> pathDict;        // 路径配置字典
    private Dictionary<string, GameObject> prefabDict;  // 预制体缓存字典
    public Dictionary<string, BasePanel> panelDict;     // 已打开界面的缓存字典
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
            }
            return _uiRoot;
        }
    }
    private UIManager()
    {
        InitDicts();
    }
    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();
        pathDict = new Dictionary<string, string>()
        {
            //StartScene
            {UIConst.StartPanel,"Prefab/Panel/StartScenePanel/StartPanel"},
            {UIConst.NamePanel,"Prefab/Panel/StartScenePanel/NamePanel" },
            {UIConst.RegisterPanel,"Prefab/Panel/StartScenePanel/RegisterPanel" },
            {UIConst.LoadPanel,"Prefab/Panel/StartScenePanel/LoadPanel" },
            //MainScene
            {UIConst.MainPanel,"Prefab/Panel/MainScenePanel/MainPanel" },
            {UIConst.SettingPanel,"Prefab/Panel/MainScenePanel/SettingPanel" },
            {UIConst.EnterPanel,"Prefab/Panel/MainScenePanel/EnterPanel" },
            //GameScene
            {UIConst.GamePanel,"Prefab/Panel/GameScenePanel/GamePanel" },
            {UIConst.WinPanel,"Prefab/Panel/GameScenePanel/WinPanel" }
            // 路径为 Resources 目录下的相对路径，通过 Resources.Load 加载
        };
    }

    public BasePanel GetPanel(string name)
    {
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel))
        {
            return panel;
        }
        return null;
    }

    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel)) // 检查是否已经打开
        {
            Debug.Log("界面已打开：" + name);
            return null;
        }
        string path = "";
        if (!pathDict.TryGetValue(name, out path)) // 检查路径是否已配置
        {
            Debug.Log("界面名称错误，或未配置路径：" + name);
            return null;
        }
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        panel.Openpanel(name);
        return panel;
    }
    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if (!panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面未打开：" + name);
            return false;
        }
        panel.Closepanel();
        return true;
    }
    public bool CheckPanelIsOpen(string name)
    {
        return panelDict.ContainsKey(name);
    }
}
public class UIConst
{
    public const string StartPanel = "StartPanel";

    public const string NamePanel = "NamePanel";

    public const string LoadPanel = "LoadPanel";

    public const string RegisterPanel = "RegisterPanel";

    public const string MainPanel = "MainPanel";

    public const string SettingPanel = "SettingPanel";

    public const string EnterPanel = "EnterPanel";

    public const string GamePanel = "GamePanel";

    public const string WinPanel = "WinPanel";
}
