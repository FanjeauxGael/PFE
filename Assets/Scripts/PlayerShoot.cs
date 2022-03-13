﻿using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

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
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdOnShoot();

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if(hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);
            }

            CmdOnHit(hit.point, hit.normal);
        }

    }

    [Command]
    private void CmdPlayerShot(string playerId, float damage)
    {
        Debug.Log(playerId + " a été touché.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }
}
