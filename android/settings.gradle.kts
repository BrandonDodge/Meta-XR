/*
 Code Artifact: settings.gradle.kts
 Description: Configures Gradle plugin repositories, dependency repositories, root project name, and included modules.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-02-15
 Revision History:
 - 2026-02-15 - Zach Sevart - Very simple interface of app, basically just confirming it works.
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 Preconditions: Gradle is run from the Android project with the version catalog and module folders present.
 Acceptable Inputs: Valid Gradle plugin aliases, Android SDK values, module paths, and dependency aliases from libs.versions.toml.
 Unacceptable Inputs: Missing version-catalog entries, unavailable SDK packages, or module names that are not included in settings.gradle.kts.
 Postconditions: Gradle has the plugin, Android, Kotlin, and dependency information needed to configure the requested module.
 Return Values: Gradle scripts do not return application values; Gradle consumes the declarations during configuration.
 Error and Exception Conditions: Gradle can fail for missing plugins, broken wrapper files, unresolved dependencies, or invalid SDK/toolchain versions.
 Side Effects: Configures build tasks, generated sources, Android manifests, and dependency graphs for the companion app.
 Invariants: Module names and dependency aliases must stay aligned with settings.gradle.kts and libs.versions.toml.
 Known Faults: The checked-in gradle-wrapper.jar must be valid before wrapper-based builds can run.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}

dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
    }
}

rootProject.name = "HudLink"

include(":app")
include(":core-data")
include(":core-storage")
include(":feature-health")
include(":feature-location")
