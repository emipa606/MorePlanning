# GitHub Copilot Instructions for More Planning (Continued) Mod

## Mod Overview and Purpose

**Mod Name:** More Planning (Continued)  
**Mod Description:** An update to Alan Dariva and Usagirei's mod that adds new planning options to the Architect menu in RimWorld.

**Purpose:**  
This mod enhances the planning features in RimWorld by providing additional customization options, such as 10 customizable planning designations, the ability to show/hide planning designations, control planning opacity, and more.

## Key Features and Systems

### Features
- **10 Customizable Planning Designations:** Users can create and customize up to 10 different planning designations.
- **Show/Hide Planning Designations:** Users can toggle the visibility of planning designations to better manage their layout.
- **Control Planning Opacity:** Adjust the transparency of planning designations for better visibility on the map.
- **Cut/Copy/Paste Planning Designations:** Allows users to manipulate planning designations using standard cut, copy, and paste functionalities.
- **Remove Designations on Build/Deconstruct:** Users can choose whether planning designations should be removed when something is built or deconstructed on top of them.
- **Shift Key Override:** Holding the shift key allows users to override other planning designations.

### Systems
- **Designation System:** Handles the creation, management, and visualization of planning designations.
- **Harmony Patching:** Utilizes Harmony for patching existing RimWorld code to integrate new features seamlessly.
- **XML Integration:** Uses XML definitions to define planning designations and other mod configurations.

## Coding Patterns and Conventions

### C# Code Conventions
- **Naming Conventions:** Use PascalCase for class names, methods, and properties. Use camelCase for local variables.
- **Comments:** Write clear and concise comments to explain complex logic or non-obvious code sections.
- **Error Handling:** Implement robust error handling to manage unexpected scenarios gracefully.
- **Code Organization:** Organize code into logical namespaces and folders. Separate concerns into different classes and methods.

### XML Code Conventions
- **Structure:** Follow a consistent structure for XML files, ensuring all elements are properly nested and formatted.
- **Attributes:** Use attributes to provide additional metadata or configuration options for elements.
- **Comments:** Use XML comments to describe the purpose and functionality of different elements.

## XML Integration

**DesignationCategoryDef and DesignationDef:**
- **DesignationCategoryDef:** Defines a category for planning designations. Example:
  xml
  <DesignationCategoryDef>
    <label>Planning</label>
    <defName>Planning</defName>
  </DesignationCategoryDef>
  

- **DesignationDef:** Defines individual planning designations. Example:
  xml
  <DesignationDef>
    <defName>PlanningDesignation1</defName>
    <categoryDef>Planning</categoryDef>
    <label>Planning Designation 1</label>
    <description>Customizable planning designation.</description>
  </DesignationDef>
  

## Harmony Patching

**Harmony Patch Example:**
- **UpdateLegacy.cs:**
  csharp
  using HarmonyLib;
  using RimWorld;
  using Verse;

  [HarmonyPatch(typeof(UpdateLegacy), "Update")]
  public static class Patch_UpdateLegacy_Update
  {
      public static bool Prefix()
      {
          // Custom logic before the original method
          Log.Message("Running custom logic in UpdateLegacy.Update");
          return true; // Continue with original method
      }
  }
  

## Suggestions for Copilot

### C# Suggestions
- **Code Completion:** Request Copilot to generate complete methods or classes based on partial code snippets.
- **Error Handling:** Ask Copilot to add error handling to existing code to ensure robustness.
- **Optimization:** Request Copilot to optimize existing code for better performance or readability.

### XML Suggestions
- **Definition Generation:** Ask Copilot to generate XML definitions for new planning designations.
- **Structure Validation:** Request Copilot to validate XML structures and ensure proper nesting and formatting.

### Harmony Patching Suggestions
- **Patch Generation:** Request Copilot to generate Harmony patches for existing RimWorld methods.
- **Patch Validation:** Ask Copilot to validate Harmony patches to ensure they integrate correctly with the existing codebase.

By following these guidelines and utilizing GitHub Copilot's suggestions, you can enhance your development process and ensure that your mod remains up-to-date and well-maintained.

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

