using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerUI))]
public class Player : NetworkBehaviour
{
    private bool _isDead = false;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private float maxHealth = 1f;

    [SyncVar]
    private float currentHealth;

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    private bool[] wasEnabledOnStart;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    [SerializeField]
    private AudioClip deathSound;

    public void Setup()
    {
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBoradcastNewPlayerSetup();
    }

    [Command(requiresAuthority = false)]
    private void CmdBoradcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }

        SetDefaults();
    }

    public void showScoreboard()
    {
        GetComponent<PlayerSetup>().getPlayerUIPrefab().GetComponent<PlayerUI>().SetActiveScoreboard(true);
    }
    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Réactive les gameobjects du joueur lors de la mort
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Apparition dy système de particules de respawn
        GameObject _gfxIns = Instantiate(spawnEffect, new Vector3(transform.position.x, transform.position.y - 15 , transform.position.z), Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 20, spawnPoint.position.z);
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        Setup();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(999, "Joueur");
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string sourceID)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        Debug.Log(transform.name + " a maintenant : " + currentHealth + " points de vie.");

        if(currentHealth <= 0)
        {
            Die(sourceID);
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(deathSound);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if(sourcePlayer != null && sourcePlayer.name != transform.name)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(transform.name, sourcePlayer.name);
            
        }

        deaths++;

        // Désactive les components du joueur lros de la mort
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
            if (disableOnDeath[i] is PlayerShoot)
            {
                ((PlayerShoot)disableOnDeath[i]).CancelShoot();
            }
        }


        // Désactive les gameobjects du joueur lors de la mort
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // Désactive le collider du joueur
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Apparition dy système de particules de mort
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        GameManager.instance.checkKills();

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + "a été éliminé.");
      
        StartCoroutine(Respawn());

    }
}
