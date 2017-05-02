using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AssetDownloader : MonoBehaviour
{
    public string url;
    GameObject waterCraftGO;
    public Text loadingText;
    public Transform spawnPos;

    IEnumerator LoadBundle(string watercraftName)
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        //Begin download
        WWW www = WWW.LoadFromCacheOrDownload(url, 0);
        yield return www;

        //Load the downloaded bundle
        AssetBundle bundle = www.assetBundle;

        //Load an asset from the loaded bundle
        AssetBundleRequest bundleRequest = bundle.LoadAssetAsync(watercraftName, typeof(GameObject));
        yield return bundleRequest;

        //get object
        GameObject obj = bundleRequest.asset as GameObject;

        waterCraftGO = Instantiate(obj, spawnPos.position, Quaternion.identity) as GameObject;
        loadingText.text = "";

        bundle.Unload(false);
        www.Dispose();
    }

    public void Load(string watercraftName)
    {
        if (waterCraftGO)
        {
            Destroy(waterCraftGO);
        }

        loadingText.text = "Loading...";
        StartCoroutine(LoadBundle(watercraftName));
    }
}