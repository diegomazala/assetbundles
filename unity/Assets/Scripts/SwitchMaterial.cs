using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMaterial : MonoBehaviour
{
    public Camera mainCamera;
    public float blendTimeSeconds = 1.0f;
    public Material fadeMaterial;

    public int index = 0;
    public Renderer sphere;
    public Material[] skybox;

    private bool hasGvrViewer = false;

    void Start ()
    {
        hasGvrViewer = (GvrViewer.Instance != null);
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
        if (hasGvrViewer && GvrViewer.Instance.Triggered && !Input.GetKey(KeyCode.LeftAlt))
        {
            Execute();
        }
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

        sphere.material = skybox[index];

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
