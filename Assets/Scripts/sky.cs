using UnityEngine;

public class sky : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    void Update()
    {
        float rotation = Time.time * rotationSpeed;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}
