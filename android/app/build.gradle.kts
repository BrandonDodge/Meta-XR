/*
 Code Artifact: build.gradle.kts
 Description: Builds the Android companion app module and declares app-level dependencies.
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
plugins {
    alias(libs.plugins.android.application)
    alias(libs.plugins.kotlin.android)
    alias(libs.plugins.ksp)
    alias(libs.plugins.hilt)
}

android {
    namespace = "com.hudlink.app"
    compileSdk = 34
    buildToolsVersion = "36.0.0"

    defaultConfig {
        applicationId = "com.hudlink.app"
        minSdk = 26
        targetSdk = 34
        versionCode = 1
        versionName = "0.1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_21
        targetCompatibility = JavaVersion.VERSION_21
    }

    kotlinOptions {
        jvmTarget = "21"
    }

    buildFeatures {
        viewBinding = true
    }
}

dependencies {
    implementation(project(":core-data"))
    implementation(project(":core-storage"))
    implementation(project(":feature-health"))
    implementation(project(":feature-location"))

    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.appcompat)
    implementation(libs.androidx.activity.ktx)
    implementation(libs.androidx.constraintlayout)
    implementation(libs.androidx.recyclerview)
    implementation(libs.androidx.lifecycle.runtime.ktx)
    implementation(libs.androidx.lifecycle.viewmodel.ktx)

    implementation(libs.hilt.android)
    ksp(libs.hilt.compiler)

    implementation(libs.kotlinx.coroutines.android)

    testImplementation(libs.junit)
    androidTestImplementation(libs.androidx.junit)
    androidTestImplementation(libs.androidx.espresso.core)
}
