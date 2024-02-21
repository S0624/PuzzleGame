using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
	IFade fade;

    [SerializeField, Range(0, 1)]
    public float cutoutRange = 1.0f;
	private float fadeSpeed = 0.02f; 

	void Start ()
	{
        Init ();
        this.cutoutRange = 1.0f;
		fade.Range = cutoutRange;
	}


    void Init ()
	{
        fade = GetComponent<IFade> ();
	}

	//void OnValidate ()
	//{
	//	Init ();
	//	fade.Range = cutoutRange;
	//}

	public void FadeOut()
	{
		cutoutRange -= fadeSpeed;

		if (cutoutRange < 0)
		{
			cutoutRange = 0;
		}
		fade.Range = cutoutRange;


	}
	public void FadeIn()
	{

		cutoutRange += fadeSpeed;
		if (cutoutRange > 1)
		{
			cutoutRange = 1;
		}
		fade.Range = cutoutRange;

	}
	//public void FadeOut (float time)
	//{
	//	FadeOut();
	//}

	//public void FadeIn (float time)
	//{
	//	FadeIn();
	//}
}