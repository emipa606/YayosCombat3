# GitHub Copilot Instructions for Yayo's Combat 3 (Continued) Mod

Welcome to the Yayo's Combat 3 (Continued) mod project! This mod aims to enhance the combat system in RimWorld, introducing a powerful armor algorithm, an ammunition system, and a realistic accuracy algorithm. Below are detailed instructions to help you contribute effectively using GitHub Copilot.

## Mod Overview and Purpose

**Mod Name:** Yayo's Combat 3 (Continued)  
**Description:**  
Updates YAYOs mod based on the abandoned 1.3 update by Ogliss. No longer requires HugsLib since version 1.6. Introduces a comprehensive combat system with advanced armor, ammunition, and accuracy features.

## Key Features and Systems

1. **Advanced Armor Algorithm:** Penetration is crucial; high-tech armor is not pierced by low-tech weapons.
2. **Ammunition System:** Ranged weapons consume ammunition. Drafted pawns and non-player combatants automatically reload empty weapons using compatible inventory ammo. Non-player pawns attempting melee with an empty ranged weapon also check for ammo; active reload jobs are not restarted. Enemies without usable ammo fall back to melee combat.
3. **Realistic Accuracy Algorithm:** Accuracy is influenced by shooting skill level.
4. **Increased Projectile Speed for Stunning:** Enhances combat effectiveness.
5. **Fixed Armor Protection:** Armor now protects hands and feet when protecting arms or legs.
6. **Compatibility:** Works with most mods; incompatible with SheathYourSword due to animation issues.

## Coding Patterns and Conventions

- **Naming Conventions:** Use descriptive and meaningful names for classes, methods, and variables.
- **Code Structure:** Organize code into logical modules such as `HarmonyPatches`, `Utilities`, and `Defs`.
- **Comments:** Provide clear comments explaining complex logic or changes.

## XML Integration

- **Defs Files:** XML files define various aspects of the game, such as apparel layers, jobs, and ammo categories.
  - Example: `ApparelLayerDefs.xml`, `Jobs.xml`, `yy_ammo_categories.xml`
- **Harmony Patches:** Use Harmony to patch existing game methods for custom behavior.
  - Example: Patching `CompApparelReloadable_PostExposeData` for reloading logic.

## Harmony Patching

- **Prefix/Postfix:** Use Harmony's `Prefix` and `Postfix` methods to modify or extend existing game functionality.
  - Example: `CompApparelReloadable_PostExposeData` modifies the `PostExposeData` method to handle custom reload logic.

## Suggestions for Copilot

### General Tips

1. **Understand the Context:** Before using Copilot, ensure you understand the context and requirements of the code snippet you need.
2. **Review and Test:** Always review and test generated code to ensure it meets the mod's requirements and does not introduce bugs.

### Specific Recommendations

1. **Harmony Patching:** When generating Harmony patches, ensure that the `Prefix` and `Postfix` methods are correctly implemented and do not interfere with the original game logic.
2. **XML Definitions:** When generating XML definitions, ensure that they are correctly formatted and comply with RimWorld's XML schema.
3. **Utility Methods:** When generating utility methods, ensure they are well-documented and reusable across the mod.

## Example Usage

### Generating a Harmony Patch

**Prompt:**
csharp
// Harmony patch for CompApparelVerbOwner_Charged_PostPostMake
// Modify the PostPostMake method to handle custom verb logic


**Expected Output:**
csharp
using HarmonyLib;

namespace yayoCombat.HarmonyPatches
{
    [HarmonyPatch(typeof(CompApparelVerbOwner_Charged), nameof(CompApparelVerbOwner_Charged.PostPostMake))]
    public static class CompApparelVerbOwner_Charged_PostPostMake
    {
        public static void Postfix(CompApparelVerbOwner_Charged __instance)
        {
            // Custom verb logic here
            // Example: Modify the verb properties or add new verbs
        }
    }
}


### Generating an XML Definition

**Prompt:**
xml
<!-- Define a new job for custom combat behavior -->
<JobDef>
    <defName>Combat_Ranged</defName>
    <label>Ranged Combat</label>
    <description>Perform ranged combat.</description>
    <workTags>
        <li>WorkTag_ManualLabor</li>
    </workTags>
    <makeTargetPrisoner>True</makeTargetPrisoner>
    <jobGiverClass>JobGiver_RangedCombat</jobGiverClass>
</JobDef>


**Expected Output:**
xml
<JobDef>
    <defName>Combat_Ranged</defName>
    <label>Ranged Combat</label>
    <description>Perform ranged combat.</description>
    <workTags>
        <li>WorkTag_ManualLabor</li>
    </workTags>
    <makeTargetPrisoner>True</makeTargetPrisoner>
    <jobGiverClass>JobGiver_RangedCombat</jobGiverClass>
</JobDef>


## Contributing to the Mod

- **Code Contributions:** Submit pull requests with your code changes.
- **Issue Reporting:** Report bugs or suggest improvements through the GitHub issues section.
- **Documentation:** Contribute to the mod's documentation to help other developers.

## Additional Resources

- **Mod Documentation:** Refer to the mod's documentation for detailed information on features and implementation.
- **Discord Channel:** Join the Discord channel for real-time support and discussions.

Thank you for contributing to Yayo's Combat 3 (Continued)! Your help will greatly enhance the mod and make combat in RimWorld more realistic and engaging.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

