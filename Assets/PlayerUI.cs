using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    private PlayerController controller;
    private WeaponManager weaponManager;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreboard;

    [SerializeField]
    private Text ammoText;
   
    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    public void Update()
    {
        SetAmmoAmount(weaponManager.currentMagazineSize);

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

    void SetAmmoAmount(int _ammount)
    {
        // remplir le texte avec le nbr de munitions
        ammoText.text = _ammount.ToString();
    }
}
