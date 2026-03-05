using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SaveurSavanteResetter : MonoBehaviour
{
    [Header("Input Action Reference for Reset")]
    public InputActionReference resetAction;

    void OnEnable()
    {
        if (resetAction != null)
        {
            resetAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (resetAction != null)
        {
            resetAction.action.Disable();
        }
    }

    void Update()
    {
        if (resetAction != null && resetAction.action.triggered)
        {
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}