using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxManagerUI : MonoBehaviour
{
    public SkyboxManager skyboxMngr;

    public UnityEngine.UI.Text screenText;

    public UnityEngine.UI.InputField uiBundleFolder;
    public UnityEngine.UI.InputField uiBundleName;
    public UnityEngine.UI.Button uiReloadBundle;

    public UnityEngine.UI.Button buttonNumberTemplate;
    public UnityEngine.UI.LayoutGroup buttonLayout;

    public void ShowHide()
    {
        uiBundleFolder.gameObject.SetActive(!uiBundleFolder.gameObject.activeSelf);
        uiBundleName.gameObject.SetActive(!uiBundleName.gameObject.activeSelf);
        uiReloadBundle.gameObject.SetActive(!uiReloadBundle.gameObject.activeSelf);
    }

    public void SetAssetBundleFolder(string bundle_folder)
    {
        skyboxMngr.AssetBundleFolderUrl = bundle_folder;
    }

    public void SetAssetBundleName(string bundle_name)
    {
        skyboxMngr.AssetBundleName = bundle_name;
    }

    public void Reload()
    {
        if (screenText != null)
            screenText.text = "Loading...";

        ClearButtons();
        skyboxMngr.Reload();
    }

    public void NextSkybox()
    {
        skyboxMngr.NextSkybox();
    }

    public void PrevSkybox()
    {
        skyboxMngr.PrevSkybox();
    }

    public void GotoSkybox(int index)
    {
        skyboxMngr.GotoSkybox(index);
    }

    void Start()
    {
        ShowHide();
    }
	
    void Update()
    {
        //
        // For Windows
        //
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WebGLPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftControl))
            {
                ShowHide();
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextSkybox();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PrevSkybox();
            }
            else
            {
                for (int i = 0; i < 10; ++i)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        GotoSkybox(i);
                    }
                }
            }
        }
    }

    public void AddSkyboxButton(int index)
    {
        UnityEngine.UI.Button new_button = Instantiate<UnityEngine.UI.Button>(buttonNumberTemplate);
        new_button.transform.SetParent(buttonLayout.transform);
        new_button.GetComponentInChildren<UnityEngine.UI.Text>().text = index.ToString();
        new_button.onClick.AddListener(() => GotoSkybox(index));
    }

    public void ClearButtons()
    {
        UnityEngine.UI.Button[] children = buttonLayout.GetComponentsInChildren<UnityEngine.UI.Button>();
        for (int i=0; i<children.Length; ++i)
        {
            Destroy(children[i]);
        }
    }

}
