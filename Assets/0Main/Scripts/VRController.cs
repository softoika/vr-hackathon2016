using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRController: Photon.MonoBehaviour
{

    private LineRenderer lineRenderer;
    private SteamVR_TrackedObject trackedObject;
    private Slide slide;
    private Image slideImage;
    private Sprite[] slideSprites;
    private int currentIndex;
    PhotonView photonView;
    void Start()
    {
        photonView = PhotonView.Get(this);
        lineRenderer = GetComponent<LineRenderer>();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        SlideInit();
    }

    void SlideInit()
    {
        slideImage = GameObject.Find("Slide Screen").GetComponent<Image>();
        slideSprites = Resources.LoadAll<Sprite>("Sample Slides");
        currentIndex = 0;
        Repaint(slideSprites[currentIndex]);
        photonView.RPC("ReserStop", PhotonTargets.All);
    }
    [PunRPC]
    public void UpChangeSlide()
    {
        if (currentIndex < slideSprites.Length - 1) currentIndex++;
        Repaint(slideSprites[currentIndex]);
    }
    [PunRPC]
    public void DownChangeSlide()
    {
        if (currentIndex > 0) currentIndex--;
        Repaint(slideSprites[currentIndex]);
    }
    [PunRPC]
    public void ReserStart() {
        lineRenderer.enabled = true;
    }
    [PunRPC]
    public void ReserStop()
    {
        lineRenderer.enabled = false;
    }
    [PunRPC]
    private void Repaint(Sprite sprite)
    {
        float width = sprite.textureRect.width;
        float height = sprite.textureRect.height;
        // spriteのサイズを反映
        GameObject.Find("Slide Screen").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        slideImage.sprite = sprite;
    }

    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObject.index);
        
        // トリガーを押している間レーザーを出す
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            //lineRenderer.enabled = true;
            photonView.RPC("ReserStart", PhotonTargets.All);
        }
        else
        {
            //lineRenderer.enabled = false;
            photonView.RPC("ReserStop", PhotonTargets.All);
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Vector2 position = device.GetAxis();
            Debug.Log("x: " + position.x + " y: " + position.y);
            if (position.x > 0.3)
            {
                photonView.RPC("UpChangeSlide", PhotonTargets.All);
            }
            if (position.x <= -0.3) {
                photonView.RPC("DownChangeSlide", PhotonTargets.All);
            } 
        }

        /*
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Vector2 position = device.GetAxis();
            Debug.Log("x: " + position.x + " y: " + position.y);
            if (position.x > 0.3) slide.UpChangeSlide();
            if (position.x <= -0.3) slide.DownChangeSlide();
        }
        
        
        // スライドページ送り
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            slide.ChangeSlide();
        }*/
    }
}