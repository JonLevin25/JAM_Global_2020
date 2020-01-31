using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private Button gameOverButton;
    [SerializeField] private int restartSceneIndex;

    // Start is called before the first frame update
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameOverButton.gameObject);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(restartSceneIndex);
    }
}
