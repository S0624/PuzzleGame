using UnityEngine;
// HACK テスト用の移動実装

public class TestMove : MonoBehaviour
{
    // スクリプトの取得.
    [SerializeField] private InputState _inputManager;
    // 何Pか指定する(テスト).
    [SerializeField] private int _playerIndex;

    // フィールドの情報を受け取るための変数
    public TestController _fieldObject; 
    // 色の情報を受け取るための変数
    private GameObject _colorManager;
    // 移動速度.
    //private float _speed;
    //private float currentSpeed;
    // 移動情報
    // HACK たすけて！！！！！
    private const int _borad_Height = 13 - 1;
    private Vector2Int _cubePos = new Vector2Int(3, _borad_Height);
    // 仮で戻す位置を覚えておく変数
    private Vector3 _cubePostemp;
    // 時間を図る変数(数秒たつと自動で落下させるために使用)
    private int _timer = 0;
    // ボタンを押し続けているか図る(これがないと一瞬で移動してしまうため)
    private int _inputframe = 0;
    // 色の番号
    private int _colorNum;
    // 移動用のスコア
    private int _moveScore = 0;

    // どの向きに回転させるか（回転処理に使用）
    private Vector2Int[] _cubeDirection = new Vector2Int[(int)Direction.max];
    // 何回ボタンを押されたかを図るために使用する変数
    private int _direction = 0;

    // Start is called before the first frame update
    // 初期化処理
    void Start()
    {
        _colorManager = GameObject.Find("ColorManager");
        //this.transform.position = transform.position + new Vector3(_cubePos.x, _cubePos.y, 0);
        this.transform.position = Vector3.zero;
        // 自分のポジションから
        // これをしないといろんな位置に行ってしまうので、フィールドに合わせてキューブの位置を初期化する
        this.transform.position = transform.position +transform.position + _fieldObject.fieldPos(_cubePos);
        
        //this.transform.position = new Vector3(_cubePos.x, _cubePos.y, 0);
        _cubePostemp = this.transform.position;

        //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
        _cubeDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        _cubeDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _cubeDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _cubeDirection[(int)Direction.Right] = new Vector2Int(1, 0);
    }

    // 移動したときの処理.
    public void RotaUpdate()
    {
        // キーの入力情報取得
        _inputManager.GetInputPlayerPadNum(_playerIndex);
        CubeDestory();
        // 方向キーの入力取得
        // 下左右に動かす
        if (!_fieldObject.IsGameOver())
        {
            // HACK こいつが悪さをしている
            if (!_fieldObject.IsFieldUpdate())
            {
                MoveState();
            }
            // HACK 雑に回転処理を実装(お試し)
            // 右回り
            if (_inputManager.GetInputRotaDate(RotaState.right))
            {
                _direction++;
                if (_direction >= 4)
                {
                    _direction = 0;
                }
                // HACK 回転処理 ﾋｨ.........
                CubeRotation();
            }
            // 左まわり
            else if (_inputManager.GetInputRotaDate(RotaState.left))
            {
                _direction--;
                if (_direction < 0)
                {
                    _direction = 3;
                }
                // HACK 回転処理 ﾋｨ.........
                CubeRotation();
            }

            // 本来はクイック移動はない
            //#if true
            Vector2 moveInput = _inputManager.GetInputMoveDate();
            // 急降下で下に落とす(DEBUG機能).
            if (moveInput.y > 0)
            {
                if (_inputManager.DGetInputWasPressData())
                {
                    Installation();
                }
            }
        }
        //#endif
    }
    // 回転したときの処理.
    public void MoveUpdate()
    {
        _timer++;
        //// 下左右に動かす

        // 下にキューブがあったら進まないようにしたい
        bool isTestMove = false;
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
            checkPos = new Vector2Int(checkPos.x, checkPos.y - (int)this.transform.position.y) + _cubePos;

            isTestMove = _fieldObject.IsNextCubeY(checkPos, 0);
            if (isTestMove)
            {
                break;
            }
        }
        if (!_fieldObject.IsFieldUpdate())
        {
            if (_timer > 60 * 1.2 && isTestMove)
            {
                Installation();
            }

            // HACK 時間での落下処理(仮)
            if (_timer > 60 * 1.2)
            {
                //_cubePos.y--;
                CubePos(0, -1);
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
            bool isTestMove  = false;
            if (CubeMoveState())
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
                    checkPos = new Vector2Int(checkPos.x, checkPos.y - (int)this.transform.position.y) + _cubePos;
                    
                    isTestMove = _fieldObject.IsNextCubeY(checkPos, 0);
                    if(isTestMove)
                    {
                        break;
                    }
                }
                
                if (!isTestMove)
                {
                    //_cubePos.y--;
                    _timer = 0;
                    CubePos(0, -1);
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
            if (CubeMoveState())
            {
                Vector2Int checkPos = new Vector2Int(0, 0);

                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _cubePos;
                    if (checkPos.x < pos.x)
                    {
                        checkPos = pos;
                    }

                }
                if (!_fieldObject.IsNextCubeX(checkPos, 1))
                {
                    //_cubePos.x++;
                    CubePos(1, 0);
                }
                _inputframe = 0;
            }
        }
        // 左.
        else if (moveInput.x < 0)
        {
            if (CubeMoveState())
            {
                Vector2Int checkPos = new Vector2Int(7, 0);
                foreach (Transform child in this.transform)
                {
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _cubePos;
                    if (checkPos.x >= pos.x)
                    {
                        checkPos = pos;
                    }
                }
                if (!_fieldObject.IsNextCubeX(checkPos, -1))
                {
                    //_cubePos.x--;
                    CubePos(-1, 0);
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
            int childcount = 0;
            _moveScore = 0;
            //_fieldObject.GetComponent<TestController>().GetInstallation(true);
            foreach (Transform child in this.transform)
            {
                // 子オブジェクトに対する処理をここに書く
                //Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, (int)child.transform.position.y - (int)this.transform.position.y) + _cubePos;
                Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x + _cubePos.x,
                    (int)child.transform.position.y - (int)this.transform.position.y);

                //Debug.Log(pos);

                // HACK テスト用.
                // こいつが悪さをしている
                // こいつはforで置ける一番低い場所を取ってしまうので0からいくとそうなる（説明下手だな？）
                pos = _fieldObject.SteepDescent(pos, _direction);
                _colorNum = _colorManager.GetComponent<TestColorManager>().GetColorNumber(child.name,childcount);
                //Debug.Log(child.position + "wa"+ child.name +"  " + pos.y + " " + _colorNum);
                _fieldObject.IsSetCube(pos, _colorNum);
                childcount++;
            }
            //_fieldObject.GetComponent<TestController>().IsFieldUpdate();
            CubeReGenerete();
            //_fieldObject.GetComponent<TestController>().GetInstallation(false);
        }
    }
    // キューブの再生成.
    private void CubeReGenerete()
    {
        _cubePos = new Vector2Int(3, _borad_Height);
        this.transform.position = _cubePostemp;
        // 回転の角度をもとに戻してあげる.
        _direction = 0;
        CubeRotation();
        // HACK うーん・・・とりあえず色替えはできてるけどバグが発生してるぅ
        _colorManager.GetComponent<TestColorManager>().ColorChenge();

    }

    // ゲームオーバーだったら消す.(テスト)
    private void CubeDestory()
    {
        if (_fieldObject.IsGameOver())
        {
            Destroy(gameObject);
        }
    }

    private void CubePos(int x = 0, int y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
        _cubePos.x += x;
        _cubePos.y += y;
    }
    // キューブの移動状態.
    private bool CubeMoveState()
    {
        if (_inputframe > 10 || _inputManager.DGetInputWasPressData())
        {
            return true;
        }
        return false;
    }
    // 回転処理.
    private void CubeRotation()
    {
        foreach (Transform child in this.transform)
        {
            child.transform.position =
                new Vector3(_cubeDirection[_direction].x + transform.position.x, _cubeDirection[_direction].y + transform.position.y, 0);
            break;
        }

        Vector2Int checkPos = new Vector2Int(0, 0);
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 
                (int)child.transform.position.y - (int)this.transform.position.y) + _cubePos;

            // HACK 壁に貫通しないように処理
            //Debug.Log(child.name + pos);
            checkPos = _fieldObject.MoveRotaCheck(pos, _cubeDirection[_direction]);
            //_cubePos += checkPos;
            CubePos(checkPos.x, checkPos.y);
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
