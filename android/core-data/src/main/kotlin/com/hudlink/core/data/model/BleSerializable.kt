package com.hudlink.core.data.model

import java.nio.ByteBuffer

/**
 * Contract for serializing health metrics into compact, versioned binary 
 * payloads suitable for Bluetooth Low Energy transmission.
 * Derived from Sprint 3 Requirement FR-3.6.
 */
interface BleSerializable {
    /**
     * Payload version for backward compatibility during deserialization on Unity target.
     */
    val payloadVersion: Byte

    /**
     * Serializes the object into a compact binary format.
     */
    fun toByteArray(): ByteArray
}
