using UnityEngine;

public class EventDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnBrickCollected +=
            OnBrickCollected;
    }


    private void OnDisable()
    {
        EventManager.OnBrickCollected -=
            OnBrickCollected;
    }


    private void OnBrickCollected(
        CharacterBase character,
        int stackCount)
    {
        Debug.Log(
            character.gameObject.name +
            " Brick topladı. Stack: " +
            stackCount
        );
    }
}