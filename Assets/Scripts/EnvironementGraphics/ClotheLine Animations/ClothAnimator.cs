using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothAnimator : MonoBehaviour
{
    private float mSize = 0.0f;
    private SkinnedMeshRenderer render;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        Scale();
    }

    void Scale()
    {
        for(int i=0; i < 249; i++)
        {
            if (mSize >= 100.0f)
            {
                mSize = 0.0f;
            }
            render.SetBlendShapeWeight(i, mSize);
        }
        mSize++;
    }
}
