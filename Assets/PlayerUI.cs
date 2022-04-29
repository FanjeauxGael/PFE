using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    private PlayerController controller;

    [SerializeField]
    private GameObject pauseMenu;

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }
}
