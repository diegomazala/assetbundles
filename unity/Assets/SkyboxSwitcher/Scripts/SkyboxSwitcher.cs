using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public float blendTimeSeconds = 1.0f;
    public Material fadeMaterial;

    public int index = 0;
    public List<Material> skybox;


    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Missing camera reference");
            enabled = false;
        }
    }

    void OnDisable()
    {
        RenderSettings.skybox = null;
    }


    public void NextSkybox()
    {
        index = ++index % skybox.Count;
        StartCoroutine(ExecuteBlend(blendTimeSeconds));
    }

    public void PrevSkybox()
    {
        --index;
        if (index < 0)
            index = skybox.Count - 1;
        StartCoroutine(ExecuteBlend(blendTimeSeconds));
    }

    public void GotoSkybox(int _index)
    {
        index = _index % skybox.Count;
        StartCoroutine(ExecuteBlend(blendTimeSeconds));
    }

    IEnumerator ExecuteBlend(float seconds)
    {
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
    
}
