using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

class AssetDownloader : MonoBehaviour
{
    public string GameObjectAssetBundleUrl = "http://localhost:8012/unity/bundles/box/cube.unity3d";
    public string GameObjectAssetName = "cube";
    public string SkyboxAssetBundleUrl = "http://localhost:8012/unity/bundles/skyboxes/skybox.unity3d";
    public string SkyboxAssetName = "sea";
    public string SceneAssetBundleUrl = "http://localhost:8012/unity/bundles/stage_scene/stage.unity3d";
    public string ScenesAssetName = "scene.unity3d";
    public string[] ScenesAssetPaths;

    public GameObject assetInstantiated = null;

    private AssetBundle assetBundle = null;
    

    void OnDisable()
    {
        if (assetBundle != null)
            assetBundle.Unload(true);
    }

    public void DownloadAsset(string assetFileName)
    {
        if (assetInstantiated)
        {
            Destroy(assetInstantiated);
            assetInstantiated = null;
        }

        string assetName = System.IO.Path.GetFileNameWithoutExtension(assetFileName);
        string extension = System.IO.Path.GetExtension(assetFileName);

        if (extension == ".scene")
        {
            StartCoroutine(GetAssetBundle_Scene(assetFileName, assetName));
        }
        else if (extension == ".gobj")
        {
            StartCoroutine(GetAssetBundle_GameObject(assetFileName, assetName));
        }
        else if (extension == ".unity3d")
        {
            StartCoroutine(GetAssetBundle_GameObject(assetFileName, assetName));
        }

        
    }

    public void DownloadGameObject()
    {
        if (assetInstantiated)
        {
            Destroy(assetInstantiated);
            assetInstantiated = null;
        }

        StartCoroutine(GetAssetBundle_GameObject(GameObjectAssetBundleUrl, GameObjectAssetName));
    }

    public void DownloadGameObject(string assetFileName)
    {
        if (assetInstantiated)
        {
            Destroy(assetInstantiated);
            assetInstantiated = null;
        }

        StartCoroutine(GetAssetBundle_GameObject(assetFileName, System.IO.Path.GetFileNameWithoutExtension(assetFileName)));
    }

    public void DownloadSkybox()
    {
        StartCoroutine(GetAssetBundle_Skybox(SkyboxAssetBundleUrl, SkyboxAssetName));
    }

    public void DownloadScenes()
    {
        StartCoroutine(GetAssetBundle_Scene(SceneAssetBundleUrl, ScenesAssetName));
    }


    IEnumerator GetAssetBundle_Skybox(string assetBundleUrl, string assetName)
    {
        UnityWebRequest www = UnityWebRequest.GetAssetBundle(assetBundleUrl);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(www);
            

            Material m = assetBundle.LoadAsset<Material>(assetName);
            m.shader = Shader.Find("Skybox/Cubemap");
            RenderSettings.skybox = m;

            assetBundle.Unload(false);
        }

    }

    IEnumerator GetAssetBundle_GameObject(string assetBundleUrl, string assetName)
    {
        UnityWebRequest www = UnityWebRequest.GetAssetBundle(assetBundleUrl);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(www);
            assetInstantiated = Instantiate(assetBundle.LoadAsset<GameObject>(assetName));
            assetBundle.Unload(false);
        }

    }


    IEnumerator GetAssetBundle_Scene(string assetBundleUrl, string assetName)
    {
        UnityWebRequest www = UnityWebRequest.GetAssetBundle(assetBundleUrl);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(www);
            ScenesAssetPaths = assetBundle.GetAllScenePaths();

            foreach (var scene_name in ScenesAssetPaths)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                    //scene_name,
                    System.IO.Path.GetFileNameWithoutExtension(scene_name),
                    UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            assetBundle.Unload(false);
        }

    }
}