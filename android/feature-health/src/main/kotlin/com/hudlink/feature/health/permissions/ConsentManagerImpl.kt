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
