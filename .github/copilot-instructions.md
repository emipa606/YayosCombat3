# Copilot Instruction File for RimWorld Mod "Yayo's Combat Extensions"

## Mod Overview and Purpose
"Yayo's Combat Extensions" is a RimWorld mod designed to enhance and expand the combat mechanics within the game. The mod focuses on introducing new functionalities for combat apparel, weapon systems, drug policies, and gear management to provide players with more strategic options and challenges. The primary aim is to enrich the in-game combat experience while maintaining balance and compatibility with the core mechanics of RimWorld.

## Key Features and Systems
- **Turret and Weapon Enhancements:** The mod modifies turret functionalities and introduces new weapon systems.
- **Reloadable Apparel:** Adds a system for apparel that can be reloaded, giving more depth to gear usage.
- **Drug Policies:** Expands the drug policy management system to give players more control over how drugs are handled.
- **Gear and Equipment Management:** Includes mechanisms for managing gear, including reloading, generating gear for pawns, and tracking equipment conditions.
- **Market and Trade Adjustments:** Alters trade and market interactions to account for the new combat features and related items.

## Coding Patterns and Conventions
- **Internal Classes:** Most of the mod's classes are internal, ensuring encapsulation within the mod while adhering to C# best practices for access control.
- **Class Naming:** Classes are named following CamelCase conventions and are prefixed with the component or system they modify or extend, making the structure intuitive and navigable.
- **Consistent Method Signatures:** Methods within classes are designed to be consistent with RimWorld's modding structure, providing clear, coherent implementations that follow the codebase's existing patterns.

## XML Integration
- **Def Management:** Def files are used to integrate the new systems and changes into the game. Ensure all XML def files adhere to RimWorld’s XML schema standards.
- **Implied Definitions:** Make use of the `DefGenerator_GenerateImpliedDefs_PreResolve` class to generate any necessary implied definitions within the game's framework.

## Harmony Patching
- Utilize Harmony for patching existing methods. This allows modification of the base game methods without altering the original code, ensuring greater compatibility and reduced conflict potential.
- Patches should be annotated with proper Harmony annotations and should only modify functionality where necessary.
- Maintain clear documentation within patch classes to explain the purpose and expected outcome of each patch.

## Suggestions for Copilot
- When generating C# code, follow the established class and method naming conventions to ensure consistency.
- Recommend XML snippets for def files where new items, gear, or drug policies might be added.
- Suggest Harmony prefixes, postfixes, or transpilers as needed to modify existing game behavior.
- Provide suggestions for improving encapsulation and modular coding practices, which can increase maintainability and readability of the mod code.
- Generate extensive comments within the code to articulate complex logic or intricate integration points with the game.

By adhering to these guidelines, Copilot can assist in maintaining a high-quality codebase that integrates seamlessly with RimWorld's framework, enhancing both the mod implementation process and the user experience.
