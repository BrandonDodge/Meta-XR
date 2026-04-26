/*
 Code Artifact: MainActivity.kt
 Description: Displays the Android companion debug screen and wires buttons, list scrolling, and ViewModel observation.
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

import android.os.Bundle
import androidx.activity.viewModels
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import androidx.recyclerview.widget.LinearLayoutManager
import com.hudlink.app.databinding.ActivityMainBinding
import dagger.hilt.android.AndroidEntryPoint
import kotlinx.coroutines.launch

@AndroidEntryPoint
class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding
    private val viewModel: MainViewModel by viewModels()
    private val logAdapter = LogAdapter()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        setupRecyclerView()
        setupButtons()
        observeViewModel()

        // Auto-start collecting on launch
        viewModel.startCollecting()
    }

    private fun setupRecyclerView() {
        binding.recyclerLogs.apply {
            adapter = logAdapter
            layoutManager = LinearLayoutManager(this@MainActivity).apply {
                stackFromEnd = true // New items appear at bottom
            }
        }
    }

    private fun setupButtons() {
        binding.buttonToggle.setOnClickListener {
            if (viewModel.isCollecting.value) {
                viewModel.stopCollecting()
            } else {
                viewModel.startCollecting()
            }
        }

        binding.buttonClear.setOnClickListener {
            viewModel.clearLogs()
        }
    }

    private fun observeViewModel() {
        lifecycleScope.launch {
            repeatOnLifecycle(Lifecycle.State.STARTED) {
                launch {
                    viewModel.logEntries.collect { entries ->
                        logAdapter.submitList(entries) {
                            // Scroll to bottom when new entries added
                            if (entries.isNotEmpty()) {
                                binding.recyclerLogs.scrollToPosition(entries.size - 1)
                            }
                        }
                    }
                }

                launch {
                    viewModel.isCollecting.collect { isCollecting ->
                        binding.buttonToggle.text = if (isCollecting) "Stop" else "Start"
                    }
                }
            }
        }
    }
}
