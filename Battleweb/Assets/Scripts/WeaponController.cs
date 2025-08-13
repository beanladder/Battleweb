using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 600f; // Rounds per minute
    public float damage = 30f;
    public float range = 100f;
    public int magazineSize = 30;
    public float reloadTime = 2.5f;
    
    [Header("Recoil Pattern")]
    public AnimationCurve recoilPatternX;
    public AnimationCurve recoilPatternY;
    public float recoilMultiplier = 1f;
    public float recoilRecoverySpeed = 5f;
    
    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public Transform firePoint;
    
    private WeaponAnimator weaponAnimator;
    private AudioSource audioSource;
    private Camera playerCamera;
    
    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    private int shotsFired = 0;
    private Vector2 currentRecoil;
    private Vector2 targetRecoil;
    
    void Start()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
        audioSource = GetComponent<AudioSource>();
        playerCamera = Camera.main;
        
        currentAmmo = magazineSize;
        
        // Initialize recoil curves if not set
        if (recoilPatternX.keys.Length == 0)
        {
            recoilPatternX = AnimationCurve.Linear(0, 0, 1, 0.5f);
        }
        if (recoilPatternY.keys.Length == 0)
        {
            recoilPatternY = AnimationCurve.Linear(0, 0, 1, 1f);
        }
    }
    
    void Update()
    {
        HandleInput();
        HandleRecoilRecovery();
    }
    
    void HandleInput()
    {
        if (isReloading) return;
        
        // Auto fire
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                Fire();
            }
            else
            {
                PlayEmptySound();
            }
        }
        
        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartReload();
        }
    }
    
    void Fire()
    {
        nextTimeToFire = Time.time + 60f / fireRate;
        currentAmmo--;
        shotsFired++;
        
        // Trigger weapon animation recoil
        if (weaponAnimator != null)
        {
            weaponAnimator.TriggerRecoil();
        }
        
        // Apply camera recoil
        ApplyCameraRecoil();
        
        // Raycast for hit detection
        PerformRaycast();
        
        // Effects
        PlayFireEffects();
        
        // Auto reload when empty
        if (currentAmmo <= 0)
        {
            StartReload();
        }
    }
    
    void ApplyCameraRecoil()
    {
        float recoilProgress = (float)shotsFired / magazineSize;
        
        float recoilX = recoilPatternX.Evaluate(recoilProgress) * recoilMultiplier;
        float recoilY = recoilPatternY.Evaluate(recoilProgress) * recoilMultiplier;
        
        // Add some randomness
        recoilX += Random.Range(-0.1f, 0.1f);
        recoilY += Random.Range(-0.05f, 0.1f);
        
        targetRecoil += new Vector2(recoilX, recoilY);
        
        // Apply recoil to camera
        if (playerCamera != null)
        {
            Transform cameraTransform = playerCamera.transform;
            cameraTransform.localRotation *= Quaternion.Euler(-recoilY, recoilX, 0);
        }
    }
    
    void HandleRecoilRecovery()
    {
        if (!Input.GetMouseButton(0))
        {
            shotsFired = Mathf.Max(0, shotsFired - 1);
            targetRecoil = Vector2.Lerp(targetRecoil, Vector2.zero, Time.deltaTime * recoilRecoverySpeed);
        }
    }
    
    void PerformRaycast()
    {
        if (playerCamera == null) return;
        
        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, range))
        {
            // Handle hit logic here
            Debug.Log($"Hit: {hit.collider.name} at distance: {hit.distance}");
            
            // You can add damage dealing logic here
            // Example: hit.collider.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }
    
    void PlayFireEffects()
    {
        // Muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        // Fire sound
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
    
    void PlayEmptySound()
    {
        if (audioSource != null && emptySound != null)
        {
            audioSource.PlayOneShot(emptySound);
        }
    }
    
    void StartReload()
    {
        if (isReloading || currentAmmo >= magazineSize) return;
        
        isReloading = true;
        shotsFired = 0; // Reset recoil pattern
        
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        Invoke(nameof(FinishReload), reloadTime);
    }
    
    void FinishReload()
    {
        currentAmmo = magazineSize;
        isReloading = false;
    }
    
    // Public getters for UI
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
}