# Apex Drift: Visual Asset Manifest (Internal Integration Guide)

To transform the game from "Basic" to "AAA" Cinematic quality, you need to place high-end FBX and PNG files into the following directories. My code is architected to search for these exact filenames and animate them at runtime.

## 1. Character & Human Models (Assets/Resources/Models/Humans/)
| Asset Name | Type | Description |
| :--- | :--- | :--- |
| `Protagonist_HighPoly.fbx` | FBX | The main driver (Player). Rigged for Mixamo. |
| `SWAT_Tactical.fbx` | FBX | High-end SWAT models for Phase 2/5 (Arrests). |
| `Club_Dancer_A.fbx` | FBX | Realistic female model for Phase 4 (Club scenes). |
| `LipBite_Texture.png` | PNG | 4K Face texture for extreme facial close-ups. |

## 2. Vehicle Models (Assets/Resources/Models/Vehicles/)
| Asset Name | Type | Description |
| :--- | :--- | :--- |
| `Bugatti_Chiron_Elite.fbx` | FBX | The high-end supercar (Separated door geometry). |
| `Cybertruck_Apex.fbx` | FBX | The heavy armored tank vehicle. |
| `Police_Interceptor.fbx` | FBX | Aggressive high-poly cop car. |
| `Tire_Burst_VFX.fbx` | FBX | Morph-target model for physical tire flattening. |

## 3. Environment & Props (Assets/Resources/Models/Environment/)
| Asset Name | Type | Description |
| :--- | :--- | :--- |
| `Club_Interior_Neon.fbx` | FBX | Full high-poly interior for mature scenes at 0.8x speed. |
| `Cinema_Auditorium.fbx` | FBX | Theatre room with tiered seating and NPC slots. |
| `SpikeStrip_Prop.fbx` | FBX | Detailed metal spike strip with visible spikes. |

## 4. Textures & VFX (Assets/Resources/Textures/)
| Asset Name | Type | Description |
| :--- | :--- | :--- |
| `Road_4K_PBR.png` | PNG | High-end asphalt texture with reflection maps. |
| `Blood_Spray_Cinematic.png` | PNG | Realistic alpha-mapped blood splatters for Phase 5. |
| `MoneyBill_100.png` | PNG | Detailed dollar bill for the "Make It Rain" spray. |

---

> [!IMPORTANT]
> **The code is the brain, these assets are the body.** 
> My scripts (e.g., `RealisticModelManager.cs`, `CinematicDirector.cs`) use `Resources.Load` to find these items. Once they are in the folders, the game transforms instantly from a world of code into a world of hyper-realistic 4D visuals.
