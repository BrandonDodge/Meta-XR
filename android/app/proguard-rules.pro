# Code Artifact: proguard-rules.pro
# Description: Keeps HUD-Link model classes intact when release builds enable shrinking or obfuscation.
# Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
# Date Created: 2026-02-15
# Revision History:
# - 2026-02-15 - Zach Sevart - Added the starter Android app release configuration.
# - 2026-04-26 - HudLink development team - Added release prologue comments for sprint submission.
# Preconditions: Android release builds read this file from app/build.gradle.kts.
# Acceptable Inputs: Valid ProGuard/R8 keep rules and fully qualified package names.
# Unacceptable Inputs: Rules that hide needed model fields, malformed keep syntax, or package names that no longer exist.
# Postconditions: Data model fields remain available for serialization and bridge formatting in shrunk builds.
# Return Values: ProGuard files do not return values; R8 consumes the rule declarations.
# Error and Exception Conditions: R8 fails on invalid rule syntax or missing referenced classes when strict checking applies.
# Side Effects: Changes what classes and members are retained in release APKs.
# Invariants: Core data model package names stay aligned with Kotlin source packages.
# Known Faults: Shrinking is currently disabled in release builds, so these rules are preparedness notes until that changes.
# Major Blocks: The keep block protects domain models used by serialization.
# Line Comments: Short rule comments are used here because ProGuard syntax should stay compact.

# HUD-Link ProGuard Rules

# Keep Room entities
-keep class com.hudlink.core.storage.entity.** { *; }

# Keep data models for serialization
-keep class com.hudlink.core.data.model.** { *; }

# Hilt
-keep class dagger.hilt.** { *; }
-keep class javax.inject.** { *; }
