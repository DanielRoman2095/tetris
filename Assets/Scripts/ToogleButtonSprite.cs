using UnityEngine;
using UnityEngine.UI;

public class ToogleButtonSprite : MonoBehaviour
{
    [SerializeField]
    Sprite fistSprite;
    [SerializeField]
    Sprite SecondSprite;

    [SerializeField]
    Image button;

    public bool temp;
    public void ToogleSprite()
    {
        if(temp)
            button.sprite = fistSprite;
        else
            button.sprite = SecondSprite;

        temp = !temp;
    }

}
