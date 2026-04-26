/*
 Code Artifact: ConsentManagerImpl.kt
 Description: Stores consent decisions locally with SharedPreferences and exposes revoke behavior.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-03-06 - Asa Maker - Implement Sprint 3 HUD-Link Unity and Android core data deliverables
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 Preconditions: Callers provide validated model values, Android framework objects, injected dependencies, or coroutine scopes required by the artifact.
 Acceptable Inputs: Values within the documented ranges, non-null dependencies supplied by Hilt or Android, and active coroutine collectors.
 Unacceptable Inputs: Out-of-range sensor values, missing injected dependencies, invalid Room enum names, or callbacks that ignore consent and retention rules.
 Postconditions: Domain data is validated, serialized, persisted, exposed to the UI, or emitted through repository flows as described below.
 Return Values: Methods return the domain type, Flow, List, String, ByteArray, Boolean, Unit, or nullable value stated in the signature.
 Error and Exception Conditions: Kotlin require checks throw IllegalArgumentException; Room, Hilt, coroutine, or Android APIs can throw their normal framework exceptions.
 Side Effects: May write local Room records, update SharedPreferences, emit Flow values, update UI state, or format data for the bridge.
 Invariants: Data remains local by default, timestamps remain UTC plus monotonic ordering, and consent can be revoked.
 Known Faults: Live Health Connect, fused-location, and Unity bridge integrations are planned; current streams use mock sources for the demo.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
package com.hudlink.feature.health.permissions

import android.content.Context
import android.content.SharedPreferences

/**
 * Implementation of ConsentManager utilizing Android SharedPreferences for local
 * persistence of explicit health data permissions.
 * Derived from Sprint 3 Requirement FR-3.5.
 */
class ConsentManagerImpl(context: Context) : ConsentManager {
    
    private val prefs: SharedPreferences = context.getSharedPreferences("hudlink_consent_prefs", Context.MODE_PRIVATE)

    override fun hasConsent(dataType: DataType): Boolean {
        return prefs.getBoolean(dataType.name, false)
    }

    override fun requestConsent(dataType: DataType, callback: (Boolean) -> Unit) {
        // In a complete Android UI this triggers an interactive permission flow.
        // For the Sprint 3 data foundation, we simulate explicit consent grant.
        saveConsent(dataType, true)
        callback(true)
    }

    override fun revokeConsent(dataType: DataType) {
        saveConsent(dataType, false)
    }

    override fun getActiveConsents(): List<DataType> {
        return DataType.values().filter { hasConsent(it) }
    }

    private fun saveConsent(dataType: DataType, granted: Boolean) {
        prefs.edit().putBoolean(dataType.name, granted).apply()
    }
}
