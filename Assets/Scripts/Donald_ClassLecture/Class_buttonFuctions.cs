using UnityEngine;

public class Class_buttonFuctions : MonoBehaviour
{
    public void resume()
    {
        Class_gameManager.instance.stateUnpause();
    }
}
