using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [Header("References")]
    public FPSController fpsController;
    public Transform weaponTransform;
    
    [Header("Sway Settings")]
    public float swayAmount = 0.02f;
    public float swayMaxAmount = 0.06f;
    public float swaySmoothness = 6f;
    
    [Header("Bob Settings")]
    public float bobAmount = 0.05f;
    public float bobSpeed = 14f;
    public float runBobMultiplier = 1.5f;
    public float crouchBobMultiplier = 0.5f;
    
    [Header("Breathing Settings")]
    public float breathingAmount = 0.01f;
    public float breathingSpeed = 1.5f;
    
    [Header("Recoil Settings")]
    public float recoilAmount = 0.3f;
    public float recoilSpeed = 10f;
    public float recoilReturnSpeed = 5f;
    
    [Header("Aim Settings")]
    public Vector3 aimPosition = new Vector3(0, -0.2f, 0.3f);
    public float aimSpeed = 8f;
    
    private Vector3 initialPosition;
    private Vector3 swayPosition;
    private float bobTimer;
    private float breathingTimer;
    private Vector3 recoilOffset;
    private bool isAiming;
    private bool isRecoiling;
    
    void Start()
    {
        if (weaponTransform == null)
            weaponTransform = transform;
            
        initialPosition = weaponTransform.localPosition;
    }
    
    void Update()
    {
        HandleInput();
        CalculateSway();
        CalculateBob();
        CalculateBreathing();
        CalculateRecoil();
        ApplyAllEffects();
    }
    
    void HandleInput()
    {
        isAiming = Input.GetMouseButton(1); // Right mouse button for aiming
        
        if (Input.GetMouseButtonDown(0)) // Left mouse button for shooting
        {
            TriggerRecoil();
        }
    }
    
    void CalculateSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Calculate sway based on mouse movement
        float swayX = Mathf.Clamp(-mouseX * swayAmount, -swayMaxAmount, swayMaxAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -swayMaxAmount, swayMaxAmount);
        
        Vector3 targetSwayPosition = new Vector3(swayX, swayY, 0);
        swayPosition = Vector3.Lerp(swayPosition, targetSwayPosition, Time.deltaTime * swaySmoothness);
    }
    
    void CalculateBob()
    {
        if (fpsController == null) return;
        
        float bobMultiplier = 1f;
        
        if (fpsController.IsMoving && fpsController.IsGrounded)
        {
            if (fpsController.IsRunning)
                bobMultiplier = runBobMultiplier;
            else if (fpsController.IsCrouching)
                bobMultiplier = crouchBobMultiplier;
                
            bobTimer += Time.deltaTime * bobSpeed * bobMultiplier;
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * 2f);
        }
    }
    
    void CalculateBreathing()
    {
        breathingTimer += Time.deltaTime * breathingSpeed;
    }
    
    void CalculateRecoil()
    {
        if (isRecoiling)
        {
            recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
            
            if (recoilOffset.magnitude < 0.01f)
            {
                recoilOffset = Vector3.zero;
                isRecoiling = false;
            }
        }
    }
    
    void ApplyAllEffects()
    {
        Vector3 finalPosition = initialPosition;
        
        // Apply sway
        finalPosition += swayPosition;
        
        // Apply bob
        if (fpsController != null && fpsController.IsMoving && fpsController.IsGrounded)
        {
            finalPosition.y += Mathf.Sin(bobTimer) * bobAmount;
            finalPosition.x += Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;
        }
        
        // Apply breathing (subtle idle movement)
        if (fpsController == null || !fpsController.IsMoving)
        {
            finalPosition.y += Mathf.Sin(breathingTimer) * breathingAmount;
        }
        
        // Apply recoil
        finalPosition += recoilOffset;
        
        // Apply aiming
        if (isAiming)
        {
            finalPosition = Vector3.Lerp(finalPosition, initialPosition + aimPosition, Time.deltaTime * aimSpeed);
        }
        
        weaponTransform.localPosition = finalPosition;
    }
    
    public void TriggerRecoil()
    {
        isRecoiling = true;
        
        // Random recoil pattern
        float recoilX = Random.Range(-recoilAmount * 0.5f, recoilAmount * 0.5f);
        float recoilY = Random.Range(-recoilAmount, -recoilAmount * 0.5f);
        float recoilZ = Random.Range(-recoilAmount * 0.3f, recoilAmount * 0.3f);
        
        recoilOffset = new Vector3(recoilX, recoilY, recoilZ);
    }
    
    // Public methods for external scripts
    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }
    
    public bool IsAiming => isAiming;
}