using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
public class TerrainLOD : MonoBehaviour
{

    private Transform MainCam;
    [SerializeField] float RenderDistance;
    public bool isRendered = true;
    public float Distance;
    public Terrain terrain;
    // Start is called before the first frame update
    void Awake()
    {
        MainCam = Camera.main.transform;
        terrain = gameObject.GetComponent<Terrain>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Application.isPlaying)
        {
            CalculateLOD(MainCam);
        }
        else
        {
            CalculateLOD(SceneView.lastActiveSceneView.camera.transform);
        }
        
        
    }

    private void CalculateLOD(Transform camPos)
    {
        Distance = Vector3.Distance(camPos.position, gameObject.transform.position);
        if (Distance > RenderDistance)
        {
            if (isRendered)
            {
                terrain.enabled = false;
                isRendered = false;
            }
            
        }
        if (Distance <= RenderDistance)
        {
            if (!isRendered)
            {
                terrain.enabled = true;
                isRendered = true;
            }
        }
    }

}
