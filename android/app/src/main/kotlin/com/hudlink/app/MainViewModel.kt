/*
 Code Artifact: MainViewModel.kt
 Description: Collects mock repository streams, manages start-stop state, and exposes bounded debug logs to the UI.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-02-15
 Revision History:
 - 2026-02-15 - Zach Sevart - Very simple interface of app, basically just confirming it works.
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
package com.hudlink.app

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.hudlink.core.data.repository.HealthRepository
import com.hudlink.core.data.repository.LocationRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class MainViewModel @Inject constructor(
    private val healthRepository: HealthRepository,
    private val locationRepository: LocationRepository
) : ViewModel() {

    private val _logEntries = MutableStateFlow<List<LogEntry>>(emptyList())
    val logEntries: StateFlow<List<LogEntry>> = _logEntries.asStateFlow()

    private val _isCollecting = MutableStateFlow(false)
    val isCollecting: StateFlow<Boolean> = _isCollecting.asStateFlow()

    private val maxLogEntries = 100
    private val collectionJobs = mutableListOf<Job>()

    fun startCollecting() {
        if (_isCollecting.value) return
        _isCollecting.value = true

        // Collect heart rate data
        collectionJobs += viewModelScope.launch {
            healthRepository.observeHeartRate().collect { data ->
                addLogEntry(LogEntry.fromHeartRate(data))
            }
        }

        // Collect GPS data
        collectionJobs += viewModelScope.launch {
            locationRepository.observeGps().collect { data ->
                addLogEntry(LogEntry.fromGps(data))
            }
        }

        // Collect heading data
        collectionJobs += viewModelScope.launch {
            locationRepository.observeHeading().collect { data ->
                addLogEntry(LogEntry.fromHeading(data))
            }
        }
    }

    fun stopCollecting() {
        _isCollecting.value = false
        collectionJobs.forEach { it.cancel() }
        collectionJobs.clear()
    }

    fun clearLogs() {
        _logEntries.value = emptyList()
    }

    private fun addLogEntry(entry: LogEntry) {
        _logEntries.value = (_logEntries.value + entry).takeLast(maxLogEntries)
    }
}
