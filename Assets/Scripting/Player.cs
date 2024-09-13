using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System.Linq;

public class Player : Object
{
    public List<WeaponData> inventory;
    public int currentWeaponIndex = 0;
    public Transform mainCameraTransform;
    public WeaponDatabase weaponDB;


    public AudioClip[] rockFootsteps;  // Array for rock footstep sounds
    public AudioClip[] grassFootsteps; // Array for grass footstep sounds
    public AudioClip[] metalFootsteps; // Array for metal footstep sounds

    public AudioClip[] rockFootstepsRun;  // Array for rock footstep run sounds
    public AudioClip[] grassFootstepsRun; // Array for grass footstep run sounds
    public AudioClip[] metalFootstepsRun; // Array for metal footstep run sounds

    private readonly string[] surfaceTags = { "Rock", "Grass", "Metal" };


    public AudioSource footstepAudioSource;  // The AudioSource component
    public float footstepIntervalNormal = 0.5f;    // Interval between footsteps
    public float footstepIntervalRunning = 0.0f;    // Interval between footsteps when running, scaled after getting rigidbodyFirstPersonController

    private float footstepTimer = 0f;
    private Rigidbody rb;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;



    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        if (inventory.Count < 2) {
            inventory.Add(weaponDB.weaponList[0]);
        }
        EquipWeapon(inventory[currentWeaponIndex]);

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("Player: Cannot get Rigidbody");
        }

        rigidbodyFirstPersonController = gameObject.GetComponent<RigidbodyFirstPersonController>();
        if (rigidbodyFirstPersonController == null)
        {
            Debug.Log("Player: Cannot get RigidbodyFirstPersonController");
        }


        footstepAudioSource = GetComponent<AudioSource>();
        if (footstepAudioSource == null)
        {
            Debug.Log("Player: Cannot get footstepAudioSource");
        }

        if (rockFootsteps.Length == 0)
        {
            Debug.Log("Player: rockFootsteps list is empty");
        }
        if (grassFootsteps.Length == 0)
        {
            Debug.Log("Player: grassFootsteps list is empty");
        }
        if (metalFootsteps.Length == 0)
        {
            Debug.Log("Player: metalFootsteps list is empty");
        }

        if (rockFootstepsRun.Length == 0)
        {
            Debug.Log("Player: rockFootstepsRun list is empty");
        }
        if (grassFootstepsRun.Length == 0)
        {
            Debug.Log("Player: grassFootstepsRun list is empty");
        }
        if (metalFootstepsRun.Length == 0)
        {
            Debug.Log("Player: metalFootstepsRun list is empty");
        }

        footstepTimer = 0f;
        footstepIntervalRunning = footstepIntervalNormal / rigidbodyFirstPersonController.movementSettings.RunMultiplier;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }

        footstepTimer += Time.deltaTime;
        
        if (rigidbodyFirstPersonController.Velocity.magnitude > 1.0f && !footstepAudioSource.isPlaying)
        {
            if (rigidbodyFirstPersonController.Running && footstepTimer >= footstepIntervalRunning)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
            else if (footstepTimer >= footstepIntervalNormal)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }

    }

    // This method searches upward in the hierarchy until it finds a parent with a valid tag
    private string GetSurfaceTag(Transform child)
    {
        Transform current = child;

        // Traverse up through the hierarchy
        while (current != null)
        {
            // If the current object's tag is one of the surface tags, return it
            if (surfaceTags.Contains(current.tag))
            {
                return current.tag;
            }

            current = current.parent;
        }

        // Return empty if no valid tag is found
        return string.Empty;
    }

    // Play a footstep sound based on the surface the player is standing on
    private void PlayFootstep()
    {
        RaycastHit hit;

        // Raycast downward from the player to detect the surface
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            //string tag = hit.collider.tag;

            // Get the surface tag by searching the hierarchy
            string tag = GetSurfaceTag(hit.collider.transform);
            //Debug.Log("Footstep raycast name: " + hit.collider.name);
            //Debug.Log("Footstep raycast tag: " + hit.collider.tag);

            if (tag == "Rock")
            {
                if (rigidbodyFirstPersonController.Running)
                {
                    PlayRandomFootstep(rockFootstepsRun);
                }
                else
                {
                    PlayRandomFootstep(rockFootsteps);
                }
                
            }
            else if (tag == "Grass")
            {
                if (rigidbodyFirstPersonController.Running)
                {
                    PlayRandomFootstep(grassFootstepsRun);
                }
                else
                {
                    PlayRandomFootstep(grassFootsteps);
                }
            }
            else if (tag == "Metal")
            {
                if (rigidbodyFirstPersonController.Running)
                {
                    PlayRandomFootstep(metalFootstepsRun);
                }
                else
                {
                    PlayRandomFootstep(metalFootsteps);
                }
            }
        }
    }

    // Play a random footstep sound from the array
    private void PlayRandomFootstep(AudioClip[] footstepClips)
    {
        if (footstepClips.Length > 0)
        {
            AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.PlayOneShot(randomClip, 2f);
        }
    }

    void SwitchWeapon()
    {
        if (inventory.Count > 1)
        {
            SaveCurrentWeapon();
            currentWeaponIndex = (currentWeaponIndex + 1) % inventory.Count;
            EquipWeapon(inventory[currentWeaponIndex]);
        }
    }

    void EquipWeapon(WeaponData weapon)
    {
        foreach (Transform child in mainCameraTransform)
        {
            Destroy(child.gameObject);
        }

        GameObject weaponInstance = Instantiate(weapon.weaponPrefab, mainCameraTransform);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity; 

        Gun gun = weaponInstance.GetComponent<Gun>();
        gun.Mag = weapon.Mag;
        gun.Ammo = weapon.Ammo;
    }

    public void AddAmmo()
    {
        if (inventory.Count > 0)
        {
            for(int i = 0; i < inventory.Count; i++){
                for(int j = 0 ; j < weaponDB.weaponList.Count; j++){
                    if (inventory[i].weaponName == weaponDB.weaponList[j].weaponName){
                        inventory[i].Ammo = weaponDB.weaponList[j].Ammo;
                        Gun gun = GameObject.FindObjectOfType<Gun>();
                        gun.Ammo = inventory[currentWeaponIndex].Ammo;
                        Debug.Log("Weapon " + weaponDB.weaponList[j].Ammo +  "Inventory" + inventory[i].Ammo);
                    }
                }
            }
        }
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!inventory.Contains(newWeapon) && inventory.Count < 2)
        {
            inventory.Add(newWeapon);
        }
    }

    void SaveCurrentWeapon(){
        WeaponData currentWeapon = inventory[currentWeaponIndex];
        Gun currentGun = mainCameraTransform.GetComponentInChildren<Gun>();
        if (currentWeapon != null){
            currentWeapon.Mag = currentGun.Mag;
            currentWeapon.Ammo = currentGun.Ammo;
        }
    }
    void OnCollisionEnter(Collision other)
    {
    //    Debug.Log("Coliding with tag " + other.gameObject.tag);
    //    Debug.Log("Coliding with object " + other.gameObject.name);
        if (other.gameObject.tag == "AmmoBox"){
            AddAmmo();
        }
    }

}
