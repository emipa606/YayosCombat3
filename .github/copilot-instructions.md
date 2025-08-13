# Copilot Instructions for Yayo's Combat 3 (Continued) Mod

## Mod Overview and Purpose

Yayo's Combat 3 (Continued) is a comprehensive overhaul of RimWorld's combat system. It introduces a sophisticated armor and ammunition system that emphasizes penetration, making high-tech armor resistant to low-tech weapons. The mod enriches the vanilla gameplay by adding realism and tactical elements to combat. It is especially beneficial for players looking to expand their RimWorld experience with enhanced tactical depth.

## Key Features and Systems

1. **Armor System**: 
   - Armor and weapon penetration are key components.
   - High-tech armors are resistant to low-tech weaponry.
   - New algorithms calculate projectile deflection based on tech levels and durability.

2. **Ammunition and Loadout**: 
   - All ranged weapons require ammunition based on weapon type and tech level.
   - Includes realistic supply logistics where pawns switch to melee when out of ammo.
   - Loadouts for ammo and medication can be set in drug policy.

3. **Accuracy and Projectiles**: 
   - Improved accuracy algorithm that factors in shooting skills.
   - Projectiles have increased speed for more dynamic combat engagement.

4. **Flexibility and Options**: 
   - Highly customizable with various options to enable or disable features.
   - Compatibility with mods that add weapons and armors.

5. **Compatibility**: 
   - Automatically applies features to most modded weapons and armor.
   - Settings ensure stability when using multiple mods.

## Coding Patterns and Conventions

- **Class Structure**: 
  - Utilize internal classes to encapsulate functionality local to the mod's features.
  - Public classes are used where interactions with RimWorld's core systems occur.

- **Method Naming**: 
  - Use descriptive method names indicating their purpose or operation.
  
- **File Organization**: 
  - Files are organized by functionality, such as `Building_TurretGun_MakeGun.cs` or `Pawn_EquipmentTracker_DropAllEquipment.cs`.

## XML Integration

- XML files should define new items, ammo types, and modifications to existing objects.
- Use XML files to adjust balance and settings without requiring C# code alterations.

## Harmony Patching

- **Purpose and Usage**:
  - Harmony is used to alter the core behavior of RimWorld without changing the original source code.
  - It patches methods to introduce new behaviors or adapt existing ones, e.g., modifying durability checks or accuracy calculations.

- **Patterns**:
  - Use Harmony to prefix or postfix methods when injecting pre- or post-execution logic.
  - Patch only the necessary parts of methods to maintain compatibility with other mods.

## Suggestions for Copilot

- **Automated Design Patterns**:
  - Implement patterns for common combat functionalities, such as checking ammo or adjusting projectile speed.

- **Refactor Suggestions**:
  - Recommend refactoring large methods into smaller, focused functions to improve readability and maintainability.

- **Template Classes**:
  - Utilize templates for repetitive structures, such as generating new ammo types or armor definitions.

- **Common Tasks**:
  - Automate frequent coding tasks such as Harmony patches or XML file generation with standardized code blocks.

- **Error Checking**:
  - Check for edge cases in ammo system and armor interactions, suggesting fixes where logic may fail, such as handling null ammo arrays.

## Special Thanks

- **Nemomone3003**: For exceptional ammo graphic art.
- **Densevoid**: For crucial coding assistance.

## Further Development

- While the current development has ceased, the source code is available for enthusiasts who wish to continue the project. Feel free to evolve the mod while adhering to the quality and compatibility standards set by its predecessors.


This `.github/copilot-instructions.md` file gives a comprehensive guide on the Yayo's Combat 3 (Continued) mod, providing key insights, coding conventions, and suggestions for using GitHub Copilot effectively, ensuring smooth development and maintenance.
