using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 魚のアニメーションの管理スクリプト
public class FishAnimController : MonoBehaviour
{
    // オブジェクトの取得
    public GameObject _fish;
	private Animator _animator;
	private List<string> _animationList = new List<string>
											{   "Attack",
												"Bounce",
												"Clicked",
												"Death",
												"Eat",
												"Fear",
												"Fly",
												"Hit",
												"Idle_A", "Idle_B", "Idle_C",
												"Jump",
												"Roll",
												"Run",
												"Sit",
												"Spin/Splash",
												"Swim",
												"Walk"
											};
	private List<string> _shapekeyList = new List<string>
											{   "Eyes_Annoyed",
												"Eyes_Blink",
												"Eyes_Cry",
												"Eyes_Dead",
												"Eyes_Excited",
												"Eyes_Happy",
												"Eyes_LookDown",
												"Eyes_LookIn",
												"Eyes_LookOut",
												"Eyes_LookUp",
												"Eyes_Rabid",
												"Eyes_Sad",
												"Eyes_Shrink",
												"Eyes_Sleep",
												"Eyes_Spin",
												"Eyes_Squint",
												"Eyes_Trauma",
												"Sweat_L",
												"Sweat_R",
												"Teardrop_L",
												"Teardrop_R"
											};
	private Vector3 _addMove = new Vector3(-0.02f, 0.01f, 0.0f);
	private Vector3 _add = new Vector3(0.0f, 0.0f, 0.0f);
	private int _addTimer = 0;
	// Start is called before the first frame update
	void Start()
    {
		int randX = Random.Range(0, 5);
		int randY = Random.Range(-7, 0);
		transform.position += new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
		_animator = _fish.GetComponent<Animator>();
		if (_animator != null)
		{
			// アニメーションの初期化設定
			FishAnimInit();
			FishSwimFailure();
		}
	}
	// Update is called once per frame
	void Update()
	{
		FishUpdate();
	}
	// 魚のアニメーションの初期化処理.
	private void FishAnimInit()
    {
		// ランダムに目のアニメーションを設定する
		int eyeRandom = Random.Range(0, _shapekeyList.Count);
		_animator.Play(_shapekeyList[eyeRandom].ToString());
	}
	// 泳ぐのに失敗した魚の設定
	private void FishSwimFailure()
    {
		// 低確率で泳ぐの失敗した魚を出現させる
		int fishAnimRandom = Random.Range(0, _animationList.Count);
		//fishAnimRandom = 8;
		if (fishAnimRandom == 8)
		//if (fishAnimRandom == 12)
		{
			_animator.Play(_animationList[fishAnimRandom].ToString());
			//_animator.Play(_shapekeyList[3].ToString());

		}
	}
	// 更新処理
	private void FishUpdate()
    {
		_addTimer++;
		if (_addTimer > 60)
		{
			_add += _addMove;
			_addTimer = 0;

		}
		_fish.transform.position += _add;
	}
	public bool IsDestory()
    {
		if(this.transform.position.x < -20)
        {
			return true;
        }
		return false;
    }
}
