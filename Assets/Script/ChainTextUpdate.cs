using UnityEngine;
using UnityEngine.UI;
public class ChainTextUpdate : MonoBehaviour
{
    public SpriteRenderer[] _chainImage;
    
    public void ChainImageUpdate(int num, Sprite sprite)
    //public void ChainImageUpdate(int num, int chainNum)
    {
        _chainImage[num].sprite = sprite;
    }
}
