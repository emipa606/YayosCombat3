# GitHub Copilot Instructions for Yayo's Combat 3 (Continued)

## Mod Overview and Purpose

Yayo's Combat 3 (Continued) is a comprehensive overhaul of the combat system in RimWorld. It aims to provide a more realistic and immersive combat experience by introducing sophisticated mechanics such as a revamped armor algorithm and an ammunition system. This project continues the work from previous versions and updates it to be compatible with newer versions of RimWorld, specifically moving away from dependencies such as HugsLib starting from version 1.6.

## Key Features and Systems

- **Comprehensive Combat Overhaul**: The mod introduces significant changes to how combat works, emphasizing the importance of weapon penetration and armor technology levels.
- **Armor Algorithm**: High-tech armors are resistant to low-tech weapons, ensuring battles are influenced by technology level.
- **Ammunition System**: Ranged weapons require ammunition, which is categorized by technological level and damage type (e.g., industrial explosives for flamethrowers).
- **Loadout Customization**: Users can configure their loadout for ammunition and medicine within the drug policy framework.
- **Realistic Accuracy Algorithm**: Shooting accuracy is tied more closely to the shooter's skill level.
- **Configurable Options**: Players can control various aspects of the mod's behavior through options.

## Coding Patterns and Conventions

- **Namespace Design**: Use meaningful namespaces that reflect the functionality or category of the classes within.
- **Class Structure**: Most classes in this mod are declared as static where they encapsulate methods or functionality that can operate independently of object instances.
- **Method Naming**: Methods are named descriptively, indicating their purpose or the action they perform.

## XML Integration

Integration with XML files allows for customization and configuration of various properties and elements, such as:

- **Defining Loadouts**: XML files are used to define available ammunition types and their attributes, ensuring clarity and ease of modification.
- **Data Persistence**: Configuration and settings related to the mod are stored and read from XML to ensure persistence across saved games.

## Harmony Patching

Harmony is used extensively in the mod to modify existing game logic without altering the original source code. 

- **Patch Organization**: Ensure that Harmony patches are organized within static classes, with appropriate prefixes or postfixes, as is convention.
- **Safe Patching**: Employ techniques to ensure patches do not introduce game-breaking bugs, prioritizing compatibility and stability.

## Suggestions for Copilot

To use GitHub Copilot effectively in this modding project:

1. **Commenting**: Include comprehensive comments within the code to help Copilot understand context and generate more accurate suggestions.
2. **Function Stubs**: Generate function stubs using Copilot to quickly create method signatures based on a summary of their intended functionality.
3. **Pattern Recognition**: Leverage Copilot's ability to recognize patterns in existing code to assist in generating repetitive or boilerplate-heavy logic, such as additional weapon definitions or loadout rules.
4. **Enhance Compatibility**: Use Copilot to suggest code that enhances mod compatibility with other popular mods, ensuring Yayo's Combat 3 works harmoniously within diverse mod lists.
5. **Error Handling**: Seek Copilot's guidance in writing robust error-handling and logging code to aid in troubleshooting and debugging efforts.

By leveraging these detailed instructions and suggestions, modders can harness GitHub Copilot to improve their development workflow and the mod's quality effectively.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
