using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class GameCameraFog : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _startDistance = 5f;
    [SerializeField, Min(0.01f)] private float _endDistance = 65f;

    private Camera _camera;
    private bool _hasSavedFog;
    private bool _savedFogEnabled;
    private FogMode _savedFogMode;
    private float _savedFogStartDistance;
    private float _savedFogEndDistance;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        RenderPipelineManager.beginCameraRendering += HandleBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += HandleEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= HandleEndCameraRendering;
        RestoreFog();
    }

    private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera != _camera || camera.cameraType != CameraType.Game || _hasSavedFog)
        {
            return;
        }

        _savedFogEnabled = RenderSettings.fog;
        _savedFogMode = RenderSettings.fogMode;
        _savedFogStartDistance = RenderSettings.fogStartDistance;
        _savedFogEndDistance = RenderSettings.fogEndDistance;
        _hasSavedFog = true;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = Mathf.Max(0f, _startDistance);
        RenderSettings.fogEndDistance = Mathf.Max(_endDistance, RenderSettings.fogStartDistance + 0.01f);
    }

    private void HandleEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == _camera)
        {
            RestoreFog();
        }
    }

    private void RestoreFog()
    {
        if (!_hasSavedFog)
        {
            return;
        }

        RenderSettings.fog = _savedFogEnabled;
        RenderSettings.fogMode = _savedFogMode;
        RenderSettings.fogStartDistance = _savedFogStartDistance;
        RenderSettings.fogEndDistance = _savedFogEndDistance;
        _hasSavedFog = false;
    }
}
