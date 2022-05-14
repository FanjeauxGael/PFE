using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    private PlayerController controller;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreboard;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }
}
