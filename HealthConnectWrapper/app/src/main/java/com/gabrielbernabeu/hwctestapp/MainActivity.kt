package com.gabrielbernabeu.hwctestapp

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import java.time.Instant
import java.time.temporal.ChronoUnit

class MainActivity : AppCompatActivity()
{
    private val allPermissions: Set<String> =
        setOf(
            HealthPermission.getReadPermission(StepsRecord::class)
        )

    private var appContext: Context? = null
    private var healthClient: HealthConnectClient? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        appContext = applicationContext
        checkAvailability()
    }

    private fun checkAvailability() {
        // Checks HealthConnect availability. If not installed but compatible with the device,
        // prompts the user to install it.
        if (HealthConnectClient.sdkStatus(appContext!!) == HealthConnectClient.SDK_AVAILABLE) {
            healthClient = HealthConnectClient.getOrCreate(appContext!!)
            Log.i("Availability", "HealthConnect is installed!")
            requestPermissions()
        }
        else if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P)
        {
            Log.e("Availability", "HealthConnect is not supported!")
        }
        else
        {
            Log.e("Availability", "HealthConnect is not installed!")
            installHealthConnect()
        }
    }

    private fun installHealthConnect()
    {
        try {
            val lViewIntent = Intent(
                "android.intent.action.VIEW",
                Uri.parse("https://play.google.com/store/apps/details?id=com.google.android.apps.healthdata")
            )
            startActivity(lViewIntent)

            Toast.makeText(
                applicationContext, "Install that!",
                Toast.LENGTH_LONG
            ).show()
        } catch (e: java.lang.Exception) {
            Toast.makeText(
                applicationContext, "Unable to Connect, Try Again...",
                Toast.LENGTH_LONG
            ).show()
            e.printStackTrace()
        }
    }

    private fun requestPermissions()
    {
        val lRequestPermissionActivityContract = PermissionController.createRequestPermissionResultContract()

        // Asks for all required permissions
        val lPermissionsRequestLauncher = registerForActivityResult(
            lRequestPermissionActivityContract
        ) { granted ->
        // Permission request result handling
            if (granted.containsAll(allPermissions)) {
                Log.i("Permissions", "All permissions granted!")

                GlobalScope.launch(Dispatchers.Main) {
                    val stepsCount = tryGetTodayStepsCount()

                    Log.i("Steps", "NSteps: $stepsCount")

                    Toast.makeText(
                        applicationContext, "NSteps: $stepsCount",
                        Toast.LENGTH_LONG
                    ).show()
                }
            } else {
                Log.e("Permissions", "Some permissions denied!")
            }
        }

        lPermissionsRequestLauncher.launch(allPermissions)
    }

    private suspend fun tryGetTodayStepsCount(): Long
    {
        var lStepsCount: Long = -1L

        val lToday = Instant.now()
        val lStartOfDay = lToday.truncatedTo(ChronoUnit.DAYS)

        Log.i("Steps", "Start reading")

        try
        {
            val lRequest = ReadRecordsRequest(
                StepsRecord::class,
                TimeRangeFilter.between(lStartOfDay, lToday))

            lStepsCount = healthClient!!.readRecords(lRequest)
                .records
                .sumOf { it.count }

            Log.i("Steps", "boom")
        }
        catch (e: Exception)
        {
            Log.e("Steps", "Error trying to read steps: $e")
        }

        return lStepsCount
    }
}