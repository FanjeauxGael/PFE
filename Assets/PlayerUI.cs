using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
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

    [SerializeField]
    public GameObject loose;

    [SerializeField]
    public GameObject win;

    public void SetPlayer(Player _player)
    {
        player = _player;
        SetActiveScoreboard(false);
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
            SetActiveScoreboard(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            SetActiveScoreboard(false);
        }

        if (player.kills >= GameManager.instance.matchSettings.nbKill)
        {
            SetActiveScoreboard(true);
            win.SetActive(true);
            StartCoroutine(GameManager.instance.checkKills());
        }
        if ((player.deaths - player.suicide) >= GameManager.instance.matchSettings.nbKill)
        {
            SetActiveScoreboard(true);
            loose.SetActive(true);
            StartCoroutine(GameManager.instance.checkKills());
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    public void SetActiveScoreboard(bool isActive)
    {
        scoreboard.SetActive(isActive);
    }

    void SetAmmoAmount(int _ammount)
    {
        // remplir le texte avec le nbr de munitions
        ammoText.text = _ammount.ToString();
    }
}
