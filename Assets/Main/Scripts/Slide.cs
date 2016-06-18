using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class Slide : MonoBehaviour {

	private Image slideImage;
	private Sprite[] slideSprites;
	private int currentIndex;

	// Use this for initialization
	void Start () 
	{
		slideImage = GameObject.Find (gameObject.name).GetComponent<Image> ();
		slideSprites = Resources.LoadAll<Sprite> ("Sample Slides");
		currentIndex = 0;
		Repaint (slideSprites[currentIndex]);
	}

	public void ChangeSlide()
	{
		if (++currentIndex == slideSprites.Length) currentIndex = 0;
		Repaint (slideSprites [currentIndex]);
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
