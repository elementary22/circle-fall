using System;
using UnityEngine;

public class AnimatorListener : MonoBehaviour
{
    public Action onComplete;

    public void CompleteAnim()
    {
        onComplete?.Invoke();
    }
}
