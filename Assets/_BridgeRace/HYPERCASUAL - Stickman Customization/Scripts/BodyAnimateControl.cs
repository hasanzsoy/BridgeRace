using UnityEngine.UI;
using UnityEngine;

public class BodyAnimateControl : MonoBehaviour
{
    public Animator modelAnimator;
    public Text buttonText;

    private bool _isAnimate = false;


    public void AnimateButtonClick()
    {
        if (!_isAnimate)
        {
            modelAnimator.Play("Walk");
            buttonText.text = "STOP";
        }
        else
        {
            modelAnimator.Play("TPose");
            buttonText.text = "WALK";
        }
        _isAnimate = !_isAnimate;
    }
}