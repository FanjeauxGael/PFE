using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Revolver";
    public int damage = 100;
    public float range = 100f;

    public float fireRate = 0f;

    public int magazineSize = 2;

    public float reloadTime = 1.5f;

    public GameObject graphics;
}
