using UnityEngine;
using UnityEngine.UI;
public class ImageTextUpdate : MonoBehaviour
{
    public SpriteRenderer[] _spriteImage;
    public Image _image;
    public Sprite[] _textImage;
    public void ChainImageUpdate(int num, Sprite sprite)
    {
        _spriteImage[num].sprite = sprite;
    }
    public void UIImageUpdate(int num = 0)
    {
        _image.sprite = _textImage[num];
    }
}
