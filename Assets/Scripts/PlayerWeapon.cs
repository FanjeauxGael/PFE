using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Revolver";
    public int damage = 100;
    public float range = 100f;
    public float speed = 0f;

    public float fireRate = 0f;

    public int magazineSize = 2;

    public float reloadTime = 1.5f;

    public float bounceDistance = 10f;

    public GameObject graphics;

    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip bounceSound;
}
