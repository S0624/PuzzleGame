using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputState : MonoBehaviour
{
    private TestInputManager[] m_PadControl;
    private int _index = 0;
    private Vector2[] _move;
    void Start()
    {
        m_PadControl = Gamepad.all.Select(pad =>
        {
            var control = new TestInputManager();
            control.devices = new UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>(new[] { pad });
            control.Enable();
            return control;
        }).ToArray();
        _move = new Vector2[m_PadControl.Length];
    }


    void Update()
    {
        //Debug.Log(InputStateUpdate());
        InputStateUpdate();
    }
    public Vector2 InputStateUpdate()
    {
        int index = 0;
        foreach (var control in m_PadControl)
        {
            var move = control.Piece.Move.ReadValue<Vector2>();
            if (move != Vector2.zero)
            {
                //Debug.Log($"move {index}:{move}");
                //return move;
            }
            //if (control.Piece.Attack.triggered)
            //{
            //    //Debug.Log($"attack {index}");
            //}
            SetInputKeyDate(index, move);
            ++index;
        }
        return Vector2.zero;
    }
    private void SetInputKeyDate(int index, Vector2 move)
    {
        _index = index;
        _move[index] = move;
    }
    //public Vector2 GetInputKeyDate(int index)
    public Vector2 GetInputKeyDate(int index)
    {
        return _move[index];
        //Debug.Log(index);
        //if (index == _index)
        //{
        //    return _move;
        //}
        //else
        //{
        //    return Vector2.zero;
        //}
    }
}

