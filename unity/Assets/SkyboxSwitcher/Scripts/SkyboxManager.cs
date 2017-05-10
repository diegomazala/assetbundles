using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxManager : MonoBehaviour
{
    
    public string AssetBundleFolderUrl;
    public string AssetBundleName;
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

        StartCoroutine(DownloadSkyboxAssetBundle(AssetBundleFolderUrl, AssetBundleName));
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


    IEnumerator DownloadSkyboxAssetBundle(string assetBundleFolderUrl, string assetBundleName)
    {
        string assetBundleUrl = assetBundleFolderUrl + "/" + assetBundleName;
        UnityWebRequest www = UnityWebRequest.GetAssetBundle(assetBundleUrl);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            this.assetNames = bundle.GetAllAssetNames();
            yield return null;

            if (bundle == null)
            {
                Debug.LogErrorFormat(string.Format("Could not load AssetBundle manifest. URL: {0}", assetBundleUrl));
                yield break;
            }
            else
            {
                assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                assetNames = assetBundleManifest.GetAllAssetBundles();
                yield return null;


                foreach (string sub_bundle_name in assetBundleManifest.GetAllAssetBundles())
                {
                    yield return StartCoroutine(DownloadSkyboxMaterialAssetBundle(assetBundleFolderUrl + "/" + sub_bundle_name));
                    ExecuteSkyboxSwitcher();
                }

                bundle.Unload(false);
                yield break;
            }

        }

    }



    private IEnumerator DownloadSkyboxMaterialAssetBundle(string assetBundleUrl)
    {
        using (UnityWebRequest www = UnityWebRequest.GetAssetBundle(assetBundleUrl))
        {
            yield return www.Send();

            if (!www.isError)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                if (bundle == null)
                {
                    Debug.LogErrorFormat(string.Format("Could not load AssetBundle manifest. URL: {0}", assetBundleUrl));
                }
                else
                {
                    foreach (string sub_bundle_name in bundle.GetAllAssetNames())
                    {
                        Material m = bundle.LoadAsset<Material>(sub_bundle_name);
                        m.shader = Shader.Find("Skybox/Cubemap");
                        skybox.Add(m);

                    }
                    bundle.Unload(false);
                    yield break;
                }
            }
        }
    }

}
