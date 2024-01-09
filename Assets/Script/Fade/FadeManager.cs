using UnityEngine;

public class FadeManager : MonoBehaviour
{
    // �t�F�[�h�L�����o�X�̎擾.
    [SerializeField] private Fade _fade;
    // �t�F�[�h�̃t���O.
    public bool _isFade = false;

    private void FixedUpdate()
    {
        // ������Ă�����t�F�[�h�C��.
        if (_isFade)
        {
            _fade.FadeIn(1.0f);
        }
        // ������Ă��Ȃ�������t�F�[�h�A�E�g.
        else
        {
            _fade.FadeOut(1.0f);
        }
    }
}
