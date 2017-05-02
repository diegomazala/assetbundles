using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSkybox : MonoBehaviour
{
    public Camera mainCamera;
    public float blendTimeSeconds = 1.0f;
    public Material fadeMaterial;

    public int index = 0;
    public Material[] skybox;


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
                Execute();
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

    public void Execute()
    {
        StartCoroutine(ExecuteBlend(blendTimeSeconds));
    }

    IEnumerator ExecuteBlend(float seconds)
    {
        index = ++index % skybox.Length;

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
