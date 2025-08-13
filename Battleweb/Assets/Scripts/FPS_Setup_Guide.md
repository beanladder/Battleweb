# FPS Controller Setup Guide

## Player Setup

1. **Player GameObject Structure:**
   ```
   Player (Capsule with CharacterController)
   ├── PlayerBody (Empty GameObject for rotation)
   │   └── Main Camera
   │       └── WeaponHolder (Empty GameObject)
   │           └── YourGunModel
   └── GroundCheck (Empty GameObject positioned at player's feet)
   ```

2. **Weapon Positioning:**
   - WeaponHolder Local Position: `(0, 0, 0)`
   - Gun Model Local Position: `(0.5, -0.3, 0.5)` (adjust as needed)
   - Gun Model Local Rotation: `(0, 0, 0)`

2. **Add Components to Player:**
   - CharacterController
   - FPSController script

3. **FPSController Configuration:**
   - Drag PlayerBody to "Player Body" field
   - Drag Main Camera to "Player Camera" field
   - Drag GroundCheck to "Ground Check" field
   - Set Ground Mask to include ground layers
   - Adjust movement speeds as needed

## Weapon Setup

1. **Add Components to Gun:**
   - WeaponAnimator script
   - WeaponController script
   - AudioSource (for weapon sounds)

2. **WeaponAnimator Configuration:**
   - Drag Player to "Fps Controller" field
   - Adjust sway, bob, and recoil settings
   - Set aim position for ADS

3. **WeaponController Configuration:**
   - Set fire rate, damage, range
   - Add audio clips for fire/reload/empty sounds
   - Add ParticleSystem for muzzle flash
   - Create fire point (empty GameObject at barrel end)

## Input Setup

Make sure your Input Manager has these axes configured:
- Horizontal (A/D keys)
- Vertical (W/S keys)
- Mouse X
- Mouse Y
- Jump (Spacebar)

## Controls

- **WASD:** Move
- **Mouse:** Look around
- **Left Shift:** Run
- **Left Ctrl:** Crouch
- **Space:** Jump
- **Left Click:** Fire
- **Right Click:** Aim
- **R:** Reload

## BF3-Style Features Included

- **Realistic Movement:** Walk/run/crouch with smooth transitions
- **Weapon Sway:** Mouse movement affects weapon position
- **Head Bob:** Different bob patterns for walk/run/crouch
- **Procedural Recoil:** Camera kick with recovery
- **Breathing Animation:** Subtle idle weapon movement
- **ADS System:** Right-click to aim down sights

## Next Steps for BF3 Authenticity

1. Add weapon switching system
2. Implement prone stance
3. Add bullet drop and ballistics
4. Create suppression effects
5. Add weapon attachments system
6. Implement advanced recoil patterns per weapon