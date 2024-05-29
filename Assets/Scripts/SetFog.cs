using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class SetFog : MonoBehaviour
{
    private float previousFogStart;
    private float previousFogEnd;
    private string camName;

    public float fog_val_start;
    public float fog_val_end;

    private void OnEnable()
    {
        camName = GetComponent<Camera>().name;
        previousFogStart = RenderSettings.fogStartDistance;
        previousFogEnd = RenderSettings.fogEndDistance;
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }

    void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera.name == camName)
        {
            RenderSettings.fogStartDistance = fog_val_start;
            RenderSettings.fogEndDistance = fog_val_end;
        }
    }

    void EndRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera.name == camName)
        {
            RenderSettings.fogStartDistance = previousFogStart;
            RenderSettings.fogEndDistance = previousFogEnd;
        }
    }
}