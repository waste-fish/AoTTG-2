using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CloudStack : MonoBehaviour
{
    [Header("Volume settings")]
    [SerializeField] int horizontalStackSize;
    [SerializeField] float cloudHeight;
    [SerializeField] float offset;
    [SerializeField] bool autoCalculateOffset = true;
    [Header("Rendering Settings")]
    [SerializeField] int Layer;
    [SerializeField] Material clouds;
    [SerializeField] Mesh quadMesh;
    [Header("Editor Settings")]
    [SerializeField] bool updateClouds = false;

    private Matrix4x4 matrix;
    private Camera camera;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        UpdateSettings(horizontalStackSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateClouds)
        {
            UpdateSettings(horizontalStackSize);
            updateClouds = false;
        }
        RenderClouds();
    }

    public void UpdateSettings(int NewAmmount)
    {
        cloudHeight = transform.position.y;
        clouds.SetFloat("_midYValue", transform.position.y);
        clouds.SetFloat("_cloudHeight", cloudHeight);
        if (autoCalculateOffset)
        {
            offset = cloudHeight / NewAmmount / 4f;
        }
        startPos = transform.position + (Vector3.up * (offset * NewAmmount / 2f));
    }

    public void RenderClouds()
    {
        for(int i = 0; i < horizontalStackSize; i++)
        {
            matrix = Matrix4x4.TRS(startPos - (Vector3.up * offset * i), transform.rotation, transform.localScale);
            Graphics.DrawMesh(quadMesh, matrix, clouds, Layer, camera, 0, null, true, false, false);
        }
    }
}
