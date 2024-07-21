package com.gabrielbernabeu.hcwforunity

import android.Manifest
import android.app.NotificationChannel
import android.app.NotificationManager
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Build
import android.util.Log
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.core.app.ActivityCompat
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.time.Instant
import java.time.temporal.ChronoUnit

class Plugin
{
    companion object
    {
        private val healthPermissions: Set<String> =
            setOf(
                HealthPermission.getReadPermission(StepsRecord::class),
            )

        private val permissions: Array<String> =
            arrayOf(
                Manifest.permission.POST_NOTIFICATIONS
            )

        private var activity: ComponentActivity? = null

        public fun getAppContext(): Context
        {
            return activity!!.applicationContext
        }

        public fun init(activity: ComponentActivity?)
        {
            this.activity = activity
            val lNotificationManager = getAppContext().getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
            val lNotificationChannel = NotificationChannel("steps_channel", "Steps Channel", NotificationManager.IMPORTANCE_HIGH)
            lNotificationManager.createNotificationChannel(lNotificationChannel)
        }

        public fun checkAvailability()
        {
            // Checks HealthConnect availability. If not installed but compatible with the device,
            // prompts the user to install it.
            if (HealthConnectClient.sdkStatus(getAppContext()) == HealthConnectClient.SDK_AVAILABLE) {
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

        public fun getTodayStepsCount_ForUnity()
        {
            getStepsCountSince (getAppContext(), Instant.now().truncatedTo(ChronoUnit.DAYS)) { stepsCount ->
                UnityPlayer.UnitySendMessage("AARCaller", "ReceiveTodayStepsCount", stepsCount.toString())
            }
        }

        //This method requires context as it may be used by external services
        public fun getStepsCountSince(context: Context, since: Instant, callback: (Long) -> Unit)
        {
            try
            {
                CoroutineScope(Dispatchers.Main).launch()
                {
                    val lNow = Instant.now()
                    val lRequest = ReadRecordsRequest(
                        StepsRecord::class,
                        TimeRangeFilter.between(since, lNow))

                    val lStepsCount = HealthConnectClient.getOrCreate(context)!!.readRecords(lRequest)
                        .records
                        .sumOf { it.count }

                    Log.i("Steps", "NSteps: $lStepsCount")
                    callback(lStepsCount)
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

        public fun startTargetStepsService(targetSteps: Int)
        {
            if (TargetStepsService.isRunning)
                return;

            Intent(getAppContext(), TargetStepsService::class.java).also {
                it.putExtra("target_steps", targetSteps)
                it.putExtra("since", Instant.now().toString())
                activity!!.startService(it)
            }
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
            //Android permissions
            ActivityCompat.requestPermissions(activity!!, permissions, 0)

            //Health Connect permissions
            val lRequestPermissionActivityContract = PermissionController.createRequestPermissionResultContract()

            val lPermissionsRequestLauncher = activity!!.registerForActivityResult(
                lRequestPermissionActivityContract
            ) { granted ->
                if (granted.containsAll(healthPermissions)) {
                    Log.i("Permissions", "All health permissions granted!")
                } else {
                    Log.e("Permissions", "Some health permissions denied!")
                }
            }

            lPermissionsRequestLauncher.launch(healthPermissions)
        }
    }
}