package com.hudlink.core.data.model

/**
 * Immutable timestamp for health metrics with ordering support.
 *
 * @property epochMillis UTC milliseconds since Unix epoch
 * @property monotonicNanos System monotonic time for ordering (avoids clock drift issues)
 */
data class MetricTimestamp(
    val epochMillis: Long,
    val monotonicNanos: Long = System.nanoTime()
) : Comparable<MetricTimestamp> {

    override fun compareTo(other: MetricTimestamp): Int =
        monotonicNanos.compareTo(other.monotonicNanos)

    companion object {
        fun now(): MetricTimestamp = MetricTimestamp(
            epochMillis = System.currentTimeMillis(),
            monotonicNanos = System.nanoTime()
        )
    }
}
