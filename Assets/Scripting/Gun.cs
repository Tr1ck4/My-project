using System.Collections;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public int Mag;
    public int maxMag;
    public int Ammo;
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public Animator gunAnimator;
    public float impactForce = 30f;
    public float fireRate = 15f;
    private Camera fpsCam;
    private AudioSource audioSource;
    private float nextTime = 0f;
    private bool isReloading = false;
    private TMP_Text mag_text;
    private TMP_Text ammo_text;


    // Update is called once per frame
    void Start(){
        fpsCam = GameObject.FindObjectOfType<Camera>();
        audioSource = gameObject.AddComponent<AudioSource>();
        GameObject gameplayObject = GameObject.Find("Gameplay");

        if (gameplayObject != null)
        {
            TMP_Text[] textComponents = gameplayObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text textComponent in textComponents)
            {
                if (textComponent.name == "Mag")
                {
                    mag_text = textComponent;
                }
                else if (textComponent.name == "Ammo")
                {
                    ammo_text = textComponent;
                }
            }
        }
        else
        {
            Debug.LogError("Gameplay object not found");
        }
    }
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTime){
            nextTime = Time.time +1f/fireRate;
            if(Mag > 0){Shoot();}
            else{ if(!isReloading) StartCoroutine(ReloadCoroutine());}
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading){
            StartCoroutine(ReloadCoroutine());
        }
        mag_text.text = Mag.ToString();
        ammo_text.text = Ammo.ToString();
    }
    void Shoot(){
        audioSource.PlayOneShot(shootClip);
        muzzleFlash.Play();
        Mag-=1;
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)){
            Object target = hit.transform.GetComponent<Object>();
            if (target != null){
                target.TakeDamage(damage);
            }
        }
        if (hit.rigidbody != null){
            hit.rigidbody.AddForce(-hit.normal * impactForce);
        }
    }
    IEnumerator ReloadCoroutine()
    {
        if (Ammo > 0 && Mag < maxMag)
        {
            isReloading = true;
            audioSource.PlayOneShot(reloadClip);
            gunAnimator.Play("reload");
        }
        
        yield return new WaitForSeconds(3.0f);
        if (Ammo > 0 && Mag < maxMag)
        {
            int ammoNeeded = maxMag - Mag;
            
            if (Ammo >= ammoNeeded)
            {
                Mag = maxMag;
                Ammo -= ammoNeeded;
            }
            else
            {
                Mag += Ammo;
                Ammo = 0;
            }
            nextTime += 3.0f;
        }
        isReloading = false;
    }
}
