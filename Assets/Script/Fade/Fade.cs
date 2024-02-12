using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
	IFade fade;

	void Start ()
	{
        Init ();
        this.cutoutRange = 1.0f;
		fade.Range = cutoutRange;
	}

    [SerializeField, Range(0, 1)]
    public float cutoutRange;

    void Init ()
	{
        fade = GetComponent<IFade> ();
	}

	void OnValidate ()
	{
		Init ();
		fade.Range = cutoutRange;
	}

	private void FadeOut()
	{
		cutoutRange -= 0.02f;
		if (cutoutRange < 0)
		{
			cutoutRange = 0;
		}
		fade.Range = cutoutRange;

	}
	private void FadeIn()
	{
		cutoutRange += 0.02f;
		if (cutoutRange > 1)
		{
			cutoutRange = 1;
		}
		fade.Range = cutoutRange;

	}
	public void FadeOut (float time)
	{
		FadeOut();
	}

	public void FadeIn (float time)
	{
		FadeIn();
	}
}