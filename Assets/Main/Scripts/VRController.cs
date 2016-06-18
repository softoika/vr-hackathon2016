using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRController: MonoBehaviour
{
    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        SteamVR_TrackedObject trackedObject = GetComponent<SteamVR_TrackedObject>();
        var device = SteamVR_Controller.Input((int)trackedObject.index);
  
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("トリガーを深く引いている");
            lineRenderer.enabled = true;
        }
        else
        {
           lineRenderer.enabled = false;
        }
 
    }
}