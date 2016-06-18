using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRController: MonoBehaviour
{
    [SerializeField] private GameObject slideScreen;

    private LineRenderer lineRenderer;
    private SteamVR_TrackedObject trackedObject;
    private Slide slide;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        slide = slideScreen.GetComponent<Slide>();
    }

    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObject.index);
        
        // トリガーを押している間レーザーを出す
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            lineRenderer.enabled = true;
        }
        else
        {
           lineRenderer.enabled = false;
        }
 
        // スライドページ送り
        if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            slide.ChangeSlide();
        }
    }
}