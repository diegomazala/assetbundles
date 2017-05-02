using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLoad : MonoBehaviour {

    public string url;
    public Material[] materials;
    public Texture[] textures;

    IEnumerator Start()
    {
        WWW www = new WWW(url);
        yield return www;

        textures = www.assetBundle.LoadAllAssets<Texture>();

        int min = Mathf.Min(textures.Length, materials.Length);
        Debug.Log(min);

        for (int i=0; i< min; ++i)
        {
            materials[i].mainTexture = textures[i];
        }

        foreach (var s in www.assetBundle.GetAllAssetNames())
        {
            Debug.Log(s);
        }
            

        // Get the designated main asset and instantiate it.
        //Instantiate(www.assetBundle.mainAsset);
    }

   
}
