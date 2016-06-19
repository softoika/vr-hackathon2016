using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class Slide : Photon.MonoBehaviour
{

	private Image slideImage;
	private Sprite[] slideSprites;
	private int currentIndex;
    PhotonView photonViewSlide;
    // Use this for initialization
    void Start () 
	{
        photonViewSlide = PhotonView.Get(this);
        slideImage = GameObject.Find ("Slide Screen").GetComponent<Image> ();
		slideSprites = Resources.LoadAll<Sprite> ("Sample Slides");
		currentIndex = 0;
		Repaint (slideSprites[currentIndex]);
	}
    void Update() {
        if (Input.GetMouseButton(0))
        {
            //UpChangeSlide();
            photonView.RPC("UpChangeSlide", PhotonTargets.All);
            print("左ボタンが押されている");
        }
        if (Input.GetMouseButton(1))
        {
            //DownChangeSlide();
            photonView.RPC("DownChangeSlide", PhotonTargets.All);
            print("右ボタンが押されている");
        }
    }
    [PunRPC]
    public void UpChangeSlide()
	{
       
        if (currentIndex < slideSprites.Length -1) currentIndex++;
        Repaint(slideSprites [currentIndex]);
	}
    [PunRPC]
    public void DownChangeSlide()
    {
        if (currentIndex > 0) currentIndex--;
        Repaint(slideSprites[currentIndex]);
    }
   
    private void Repaint(Sprite sprite)
	{
		float width = sprite.textureRect.width;
		float height = sprite.textureRect.height;
		// spriteのサイズを反映
		gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2(width, height);
		slideImage.sprite = sprite;
	}

}
