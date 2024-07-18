package com.gabrielbernabeu.hcwforunity

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Build
import android.util.Log
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import androidx.work.ExistingPeriodicWorkPolicy
import androidx.work.PeriodicWorkRequestBuilder
import androidx.work.WorkManager
import androidx.work.workDataOf
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import java.time.Instant
import java.time.temporal.ChronoUnit
import java.util.concurrent.TimeUnit

class Plugin
{
    companion object
    {
        private val allPermissions: Set<String> =
            setOf(
                HealthPermission.getReadPermission(StepsRecord::class)
            )

        private var activity: ComponentActivity? = null
        private var healthClient: HealthConnectClient? = null

        public fun getAppContext(): Context
        {
            return activity!!.applicationContext
        }

        public fun setActivity(activity: ComponentActivity?)
        {
            this.activity = activity
        }

        public fun checkAvailability()
        {
            // Checks HealthConnect availability. If not installed but compatible with the device,
            // prompts the user to install it.
            if (HealthConnectClient.sdkStatus(getAppContext()) == HealthConnectClient.SDK_AVAILABLE) {
                healthClient = HealthConnectClient.getOrCreate(getAppContext())
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

            activity!!.applicationContext
        }

        private fun installHealthConnect()
        {
            try {
                val lViewIntent = Intent(
                    "android.intent.action.VIEW",
                    Uri.parse("https://play.google.com/store/apps/details?id=com.google.android.apps.healthdata")
                )

                lViewIntent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                getAppContext().startActivity(lViewIntent)

                Toast.makeText(
                    getAppContext(), "Install that!",
                    Toast.LENGTH_LONG
                ).show()
            } catch (e: java.lang.Exception) {
                Toast.makeText(
                    getAppContext(), "Unable to Connect, Try Again...",
                    Toast.LENGTH_LONG
                ).show()
                e.printStackTrace()
            }
        }

        private fun requestPermissions()
        {
            val lRequestPermissionActivityContract = PermissionController.createRequestPermissionResultContract()

            // Asks for all required permissions
            val lPermissionsRequestLauncher = activity!!.registerForActivityResult(
                lRequestPermissionActivityContract
            ) { granted ->
                // Permission request result handling
                if (granted.containsAll(allPermissions)) {
                    Log.i("Permissions", "All permissions granted!")
                } else {
                    Log.e("Permissions", "Some permissions denied!")
                }
            }

            lPermissionsRequestLauncher.launch(allPermissions)
        }

        public fun getTodayStepsCount_ForUnity()
        {
            getTodayStepsCount { stepsCount ->
                UnityPlayer.UnitySendMessage("AARCaller", "ReceiveTodayStepsCount", stepsCount.toString())
            }
        }

        public fun getTodayStepsCount(callback: (Long) -> Unit)
        {
            try {
                GlobalScope.launch(Dispatchers.Main) {
                    val stepsCount = tryGetTodayStepsCount()

                    Log.i("Steps", "NSteps: $stepsCount")

                    Toast.makeText(
                        getAppContext(), "NSteps: $stepsCount",
                        Toast.LENGTH_LONG
                    ).show()

                    callback(stepsCount)
                }
            }
            catch (e: java.lang.Exception)
            {
                Toast.makeText(
                    getAppContext(), "Couldn't get steps count",
                    Toast.LENGTH_LONG
                ).show()
                e.printStackTrace()
            }
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
            }
            catch (e: Exception)
            {
                Log.e("Steps", "Error trying to read steps: $e")
            }

            return lStepsCount
        }

        public fun scheduleStepCountWorker(targetSteps: Int) {
            val lWorkRequest = PeriodicWorkRequestBuilder<TargetStepsWorker>(15, TimeUnit.MINUTES)
                .setInputData(workDataOf("target_steps" to targetSteps))
                .build()

            WorkManager.getInstance(getAppContext()).enqueueUniquePeriodicWork(
                TargetStepsWorker::class.java.simpleName,
                ExistingPeriodicWorkPolicy.REPLACE,
                lWorkRequest
            )
        }
    }
}