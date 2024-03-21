using DG.Tweening;
using UnityEngine;
// HACK テスト用の移動実装

public class SphereMove : MonoBehaviour
{
    // スクリプトの取得.
    [SerializeField] private InputState _inputManager;
    // 何Pか指定する.
    public int _playerIndex;
    [SerializeField] private GameObject[] _ghostSphere;

    // フィールドの情報を受け取るための変数
    public FieldData _fieldObject; 
    // 色の情報を受け取るための変数
    public SphereColorManager _colorManager;
    // 移動速度.
    //private float _speed;
    //private float currentSpeed;
    // 移動情報
    // HACK たすけて！！！！！
    private const int _borad_Height = 13 - 2;
    public Vector2Int _spherePos = new Vector2Int(3, _borad_Height);
    // 仮で戻す位置を覚えておく変数
    private Vector3 _spherePostemp;
    // 時間を図る変数(数秒たつと自動で落下させるために使用)
    private int _timer = 0;
    // ボタンを押し続けているか図る(これがないと一瞬で移動してしまうため)
    private int _inputframe = 0;
    // 色の番号
    private int[] _colorNum = new int[2];
    // 移動用のスコア
    private int _moveScore = 0;
    // 再生成用のフラグ.
    private bool _isReGenereteSpher = false;
    // どの向きに回転させるか（回転処理に使用）
    private Vector2Int[] _sphereDirection = new Vector2Int[(int)Direction.max];
    // 何回ボタンを押されたかを図るために使用する変数
    private int _direction = 0;
    // サウンドの取得
    private SoundManager _soundManager;
    public bool _isRegeneration = false;
    private float _goshtalpha = 0.8f;
    // Start is called before the first frame update
    // 初期化処理
    void Start()
    {
        //SphereInit();

        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    // キューブの初期化処理.
    public void SphereInit()
    {
        this.transform.position = Vector3.zero;
        // 自分のポジションから
        // これをしないといろんな位置に行ってしまうので、フィールドに合わせてキューブの位置を初期化する
        this.transform.position = transform.position + transform.position + _fieldObject.fieldPos(_spherePos);

        _spherePostemp = this.transform.position;

        _sphereDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        _sphereDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _sphereDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _sphereDirection[(int)Direction.Right] = new Vector2Int(1, 0);
    }

    // 移動や回転したときの処理.
    public void SphereUpdate()
    {
        // キーの入力情報取得
        _inputManager.GetInputPlayerPadNum(_playerIndex);
        //if(_playerIndex < 0)
        //{
        //    this.gameObject.SetActive(false);
        //}

        SphereDestory();
        // 方向キーの入力取得
        // 下左右に動かす
        if (!_fieldObject.IsGameOver())
        {
            // フィールドの情報の更新処理.
            if (!_fieldObject.IsFieldUpdate())
            {
                // 本来はクイック移動はない
                Vector2 moveInput = _inputManager.GetInputMoveDate();
                // 急降下で下に落とす.
                if (moveInput.y > 0)
                {
                    if (_inputManager.DGetInputWasPressData())
                    {
                        Installation();
                    }
                }
                MoveState();
                RotationState();
            }
        }
        GhostSphereUpdate();
        //#endif
    }
    // 時間で落下したときの処理.
    public void FreeFallUpdate()
    {
        _timer++;
        //// 下左右に動かす

        // 下にキューブがあったら進まないようにしたい
        bool isSphereMove = false;
        Vector2Int checkPos = new Vector2Int(0, _borad_Height);

        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x,
                (int)child.transform.position.y);
            //if (checkPos.y > pos.y)
            {
                checkPos = pos;
            }
            checkPos = new Vector2Int(checkPos.x, checkPos.y - (int)this.transform.position.y) + _spherePos;
            isSphereMove = _fieldObject.IsNextSphereY(checkPos, 0);
            if (isSphereMove)
            {
                break;
            }
        }
        if (!_fieldObject.IsFieldUpdate())
        {
            if (_timer > 60 * 1.2 && isSphereMove)
            {
                Installation();
            }

            // HACK 時間での落下処理(仮)
            if (_timer > 60 * 1.2)
            {
                //_cubePos.y--;
                SpherePos(0, -1);
                _timer = 0;
            }
        }

    }

    private void MoveState()
    {
        Vector2 moveInput = _inputManager.GetInputMoveDate();
        if (_inputManager.IsMovePressed())
        {
            _inputframe++;
        }

        // HACK 斜めってどうやんねん(？？？？？？)
        // 下.
        if (moveInput.y < 0)
        {
            bool isMove  = false;
            if (SphereMoveState())
            {
                _moveScore++;
                Vector2Int checkPos = new Vector2Int(0, _borad_Height);

                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x,
                        (int)child.transform.position.y);
                    //if (checkPos.y > pos.y)
                    {
                        checkPos = pos;
                    }
                    checkPos = new Vector2Int(checkPos.x, checkPos.y - (int)this.transform.position.y) + _spherePos;
                    
                    isMove = _fieldObject.IsNextSphereY(checkPos, 0);
                    if(isMove)
                    {
                        break;
                    }
                }
                
                if (!isMove)
                {
                    //_cubePos.y--;
                    _timer = 0;
                    SpherePos(0, -1);
                }
                else
                {
                    // 下を押されたら落下時間を無視
                    _timer = (int)(60 * 1.2f);
                }
                _inputframe = 0;
            }
        }

        // 右.
        if (moveInput.x > 0)
        {
            if (SphereMoveState())
            {
                Vector2Int checkPos = new Vector2Int(0, 0);

                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _spherePos;
                    if (checkPos.x <= pos.x)
                    {
                        checkPos = pos;
                    }

                }
                if (!_fieldObject.IsNextSphereX(checkPos, 1))
                {
                    _soundManager.SEPlay(SoundSEData.MoveSE);
                    //_cubePos.x++;
                    SpherePos(1, 0);
                }
                _inputframe = 0;
            }
        }
        // 左.
        else if (moveInput.x < 0)
        {
            if (SphereMoveState())
            {
                Vector2Int checkPos = new Vector2Int(7, 0);
                foreach (Transform child in this.transform)
                {
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _spherePos;
                    if (checkPos.x >= pos.x)
                    {
                        checkPos = pos;
                    }
                }
                if (!_fieldObject.IsNextSphereX(checkPos, -1))
                {
                    _soundManager.SEPlay(SoundSEData.MoveSE);
                    //_cubePos.x--;
                    SpherePos(-1, 0);
                }
                _inputframe = 0;
            }
        }
    }

    // 設置するときの処理
    private void Installation()
    {
        if (!_fieldObject.IsGameOver())
        {
            // 子オブジェクトの番号を取得する用に使用
            int childcount = 0;
            // スコアを初期化する.
            _moveScore = 0;
            foreach (Transform child in this.transform)
            {
                // 子オブジェクトに対する処理をここに書く
                //Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, (int)child.transform.position.y - (int)this.transform.position.y) + _cubePos;
                Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x + _spherePos.x,
                    (int)child.transform.position.y - (int)this.transform.position.y);


                // HACK テスト用.
                // こいつが悪さをしている
                // こいつはforで置ける一番低い場所を取ってしまうので0からいくとそうなる（説明下手だな？）
                pos = _fieldObject.SteepDescent(pos, _direction);
                _colorNum[childcount] = _colorManager.GetComponent<SphereColorManager>().GetColorNumber(child.name,childcount);
                //Debug.Log(child.position + "wa"+ child.name +"  " + pos.y + " " + _colorNum);
                //Debug.Log("ここは" + pos);
                
                _fieldObject.IsNormalSphere(pos, _colorNum[childcount]);
                childcount++;
            }
            // HACK 余りにも力業オブ力業.
            this.transform.position = new Vector3(0,-10,0);
            // 再生成のフラグを立てる.
            _isReGenereteSpher = true;
        }
    }
    /// <summary>
    /// ゴーストの更新処理
    /// </summary>
    private void GhostSphereUpdate()
    {
        if (!_fieldObject.IsGameOver())
        {
            if (_isReGenereteSpher)
            {
                for (int i = 0; i < _ghostSphere.Length; i++)
                {
                    _ghostSphere[i].transform.position = new Vector3(0, -10, 0);
                }
                return;
            }
            // 子オブジェクトの番号を取得する用に使用
            int childcount = 0;
            foreach (Transform child in this.transform)
            {
                // 位置の更新
                Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x + _spherePos.x,
                    (int)child.transform.position.y - (int)this.transform.position.y);
                pos = _fieldObject.SteepDescent(pos);
                // HACK 強制的に位置を変更している。
                if (_direction == (int)Direction.Down)
                {
                    if (childcount != 0)
                    {
                        pos.y = pos.y - _sphereDirection[_direction].y;
                    }

                }
                // ゴーストの場所変更
                _ghostSphere[childcount].transform.localPosition = new Vector3(pos.x, pos.y, 0);
                // ゴーストのカラー変更処理
                GhostSphereColor(child,childcount);


                childcount++;
            }
        }
    }
    /// <summary>
    /// ゴーストのカラー処理
    /// </summary>
    private void GhostSphereColor(Transform child,int childcount)
    {
        // 色の取得
        _colorNum[childcount] = _colorManager.GetComponent<SphereColorManager>().GetColorNumber(child.name, childcount);
        var ghostcolor = _colorManager.GetComponent<SphereColorManager>().GetColor(child.name, childcount, _colorNum[childcount]);
        // ゴーストのアルファ値を半透明にする
        ghostcolor.a = _goshtalpha;
        // ゴーストのカラーの取得
        _ghostSphere[childcount].GetComponent<Renderer>().material.color = ghostcolor;
    }
    // スフィアを設置したときの処理.
    public void InstallationProcess(bool isChain, FieldData controller)
    ///private void SphereReGenerete()
    {
        if (isChain && _isReGenereteSpher)
        {
            // お邪魔を落とす.
            controller.DisturbanceFall();
            // お邪魔が落下中だったら次のスフィアを生成しない.
            if (controller.MoveObstacleSphere()) return;
            _isRegeneration = true;
            controller.IsSetReset();
            // スフィアを再生成する.
            SphereReGenerete();
        }
    }
    // スフィアの再生成.
    public void SphereReGenerete()
    {
        _spherePos = new Vector2Int(3, _borad_Height);
        this.transform.position = _spherePostemp;
        // 回転の角度をもとに戻してあげる.
        _direction = 0;
        SphereRotation();
        // HACK うーん・・・とりあえず色替えはできてるけどバグが発生してるぅ
        _colorManager.GetComponent<SphereColorManager>().ColorChenge(this.gameObject.name);
        _isReGenereteSpher = false;
    }
    // ゲームオーバーだったら消す.(テスト)
    private void SphereDestory()
    {
        if (_fieldObject.IsGameOver())
        {
            Destroy(gameObject);
        }
    }

    public void SpherePos(int x = 0, int y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
        _spherePos.x += x;
        _spherePos.y += y;
    }
    // キューブの移動状態.
    private bool SphereMoveState()
    {
        if (_inputframe > 8 || _inputManager.DGetInputWasPressData())
        {
            return true;
        }
        return false;
    }
    private void RotationState()
    {
        // 右回り
        if (_inputManager.GetInputRotaDate(RotaState.right))
        {
            _soundManager.SEPlay(SoundSEData.Rota);
            if (!IsRotaChenck()) return;
            _direction++;
            if (_direction > 3)
            {
                _direction = 0;
            }
            // HACK 回転処理 ﾋｨ.........
            SphereRotation();
        }
        // 左まわり
        else if (_inputManager.GetInputRotaDate(RotaState.left))
        {
            _soundManager.SEPlay(SoundSEData.Rota);
            if (!IsRotaChenck()) return;
            _direction--;
            if (_direction < 0)
            {
                _direction = 3;
            }
            // HACK 回転処理 ﾋｨ.........
            SphereRotation();
        }

    }
    /// <summary>
    /// 回転可能かどうか
    /// </summary>
    private bool IsRotaChenck()
    {
        foreach (Transform child in this.transform)
        {
            child.transform.position =
                new Vector3(_sphereDirection[_direction].x + transform.position.x, _sphereDirection[_direction].y + transform.position.y, 0);
            break;
        }
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x,
                (int)child.transform.position.y - (int)this.transform.position.y) + _spherePos;
            if (!_fieldObject.IsSphereRota(pos, _direction)) return false;

        }
        return true;
    }
    // 回転処理.
    private void SphereRotation()
    {
        foreach (Transform child in this.transform)
        {
            child.transform.position =
                new Vector3(_sphereDirection[_direction].x + transform.position.x, _sphereDirection[_direction].y + transform.position.y, 0);
            break;
        }

        Vector2Int checkPos = new Vector2Int(0, 0);
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 
                (int)child.transform.position.y - (int)this.transform.position.y) + _spherePos;

            // HACK 壁に貫通しないように処理
            //Debug.Log(child.name + pos);
            checkPos = _fieldObject.MoveRotaCheck(pos, _sphereDirection[_direction]);
            //_cubePos += checkPos;
            SpherePos(checkPos.x, checkPos.y);
        }
    }
    // 仮実装
    public int MoveScore()
    {
        return _moveScore;
    }
    public void SetScore()
    {
        _moveScore = 0;
    }
}
