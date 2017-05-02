using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxManager : MonoBehaviour
{
    public string assetBundleFolderUrl;
    public string assetBundleName;
    public Camera mainCamera;
    public float blendTimeSeconds = 1.0f;
    public Material fadeMaterial;

    public int index = 0;
    public List<Material> skybox;

    public string[] assetNames;

    public AssetBundleManifest assetBundleManifest;
    public UnityEngine.UI.Text screenText;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Missing camera reference");
            enabled = false;
        }


        skybox.Clear();
        if (screenText != null)
            screenText.text = "Loading...";
        StartCoroutine(DownloadMainBundle(assetBundleFolderUrl, assetBundleName));
    }
	
    void OnDisable()
    {
        RenderSettings.skybox = null;
    }

	void Update ()
    {
        //
        // For Windows
        //
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WebGLPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExecuteSkyboxSwitcher();
            }
        }

        //
        // For Google Cardboard
        //
#if UNITY_ANDROID
        if (GvrViewer.Instance.Triggered && !Input.GetKey(KeyCode.LeftAlt))
        {
            Execute();
        }
#endif
    }

    public void ExecuteSkyboxSwitcher()
    {
        StartCoroutine(ExecuteBlend(blendTimeSeconds));
    }

    IEnumerator ExecuteBlend(float seconds)
    {
        index = ++index % skybox.Count;

        float half_seconds = seconds;

        float alpha = fadeMaterial.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / half_seconds)
        {
            Color newColor = new Color(
                fadeMaterial.color.r, 
                fadeMaterial.color.g, 
                fadeMaterial.color.b, 
                Mathf.Lerp(alpha, 1.0f, t));

            fadeMaterial.color = newColor;

            yield return null;
        }

        RenderSettings.skybox = skybox[index];

        yield return null;

        alpha = fadeMaterial.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / half_seconds)
        {
            Color newColor = new Color(
                fadeMaterial.color.r,
                fadeMaterial.color.g,
                fadeMaterial.color.b,
                Mathf.Lerp(alpha, 0.0f, t));

            fadeMaterial.color = newColor;

            yield return null;
        }
    }
    

    private IEnumerator DownloadAssetBundle(string assetBundleUrl)
    {
        //using (UnityWebRequest webRequest = UnityWebRequest.GetAssetBundle(assetBundleUrl, this.assetBundleManifest.GetAssetBundleHash(assetNames[0]), 0))
        using (UnityWebRequest webRequest = UnityWebRequest.GetAssetBundle(assetBundleUrl))
        {
            yield return webRequest.Send();

            if (!webRequest.isError)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

                if (bundle == null)
                {
                    Debug.LogErrorFormat(string.Format("Could not load AssetBundle manifest. URL: {0}", assetBundleUrl));
                }
                else
                {
                    Debug.Log(Time.frameCount + " Time foreach in 1: " + Time.realtimeSinceStartup);
                    foreach (string sub_bundle_name in bundle.GetAllAssetNames())
                    {
                        Material m = bundle.LoadAsset<Material>(sub_bundle_name);
                        m.shader = Shader.Find("Skybox/Cubemap");
                        skybox.Add(m);
                    }
                    Debug.Log(Time.frameCount + " Time foreach in 2: " + Time.realtimeSinceStartup);

                    yield break;
                }
            }
        }
    }


    private IEnumerator DownloadMainBundle(string assetBundleUrl, string assetBundleName)
    {
        string bundleUrl = assetBundleUrl + "/" + assetBundleName;
        using (UnityWebRequest webRequest = UnityWebRequest.GetAssetBundle(bundleUrl))
        {
            yield return webRequest.Send();

            if (!webRequest.isError)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

                if (bundle == null)
                {
                    Debug.LogErrorFormat(string.Format("Could not load AssetBundle manifest. URL: {0}", assetBundleUrl));
                }
                else
                {
                    assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                    assetNames = assetBundleManifest.GetAllAssetBundles();
                }
            }
        }

        Debug.Log(Time.frameCount + " Time manifest: " + Time.realtimeSinceStartup);

        yield return null;

        foreach(string bundleName in assetBundleManifest.GetAllAssetBundles())
        {
            Debug.Log(Time.frameCount + " Time foreach out 1 : " + Time.realtimeSinceStartup);
            yield return StartCoroutine(DownloadAssetBundle(assetBundleFolderUrl + "/" + bundleName));
            Debug.Log(Time.frameCount + " Time foreach out 2 : " + Time.realtimeSinceStartup);
            ExecuteSkyboxSwitcher();
        }

        if (screenText != null)
            screenText.text = "";
    }
}
