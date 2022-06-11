using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private bool bouncingBullets;


    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    // Start is called before the first frame update
    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("Pas de caméra renseignée sur le système de tir.");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        } else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    public void CancelShoot()
    {
        CancelInvoke("Shoot");
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    // Fonction appelée sur le serveur lorsque notre joueur tir
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //Fait apparaitre les effets de tir chez trous les clients
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
    }

    [Command]
    void CmdOnRicocheting(Vector3 position, Quaternion quaternion)
    {
        RpcTrailEffect(position, quaternion);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.bounceSound);
    }

    [ClientRpc]
    void RpcTrailEffect(Vector3 position, Quaternion quaternion)
    {
        Instantiate(weaponManager.GetCurrentGraphics().bulletTrail, position, quaternion);
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;

        CmdOnShoot();

        RaycastHit hit;
        Vector3 direction = transform.forward;
        CmdOnRicocheting(cam.transform.position, Quaternion.identity);
        TrailRenderer trail = Instantiate(weaponManager.GetCurrentGraphics().bulletTrail, cam.transform.position, Quaternion.identity);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            

            if (hit.collider.tag == "Player")
            {
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, 0f, false));
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }
            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, currentWeapon.bounceDistance, true));

        }
        else
        {
            StartCoroutine(SpawnTrail(trail, direction * 100, Vector3.zero, currentWeapon.bounceDistance, false));
        }

        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, float bounceDistance, bool madeImpact)
    {
        Vector3 startPosition = trail.transform.position;
        Vector3 direction = (hitPoint - trail.transform.position).normalized;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startingDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * currentWeapon.speed;

            yield return null;
        }

        trail.transform.position = hitPoint;

        if (madeImpact)
        {
            CmdOnRicocheting(hitPoint, Quaternion.LookRotation(hitNormal));

            if (bouncingBullets && bounceDistance > 0) 
            {
                Vector3 bounceDirection = Vector3.Reflect(direction, hitNormal);

                if (Physics.Raycast(hitPoint, bounceDirection, out RaycastHit hit, bounceDistance, mask))
                {
                    if (hit.collider.tag == "Player")
                    {
                        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, 0f, false));
                        CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
                        yield return null;
                    }
                    else
                    {
                        yield return StartCoroutine(SpawnTrail(trail,
                                                               hit.point,
                                                               hit.normal,
                                                               bounceDistance - Vector3.Distance(hit.point, hitPoint),
                                                               true
                                                               ));
                    }
                }
                else
                {
                    yield return StartCoroutine(SpawnTrail(
                        trail,
                        bounceDirection * bounceDistance,
                        Vector3.zero,
                        0,
                        false
                        ));
                }
            }
        }

        Destroy(trail.gameObject, trail.time);
    }


    [Command]
    private void CmdPlayerShot(string playerId, float damage, string sourceID)
    {
        Debug.Log(playerId + " a été touché.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage, sourceID);
    }
}
