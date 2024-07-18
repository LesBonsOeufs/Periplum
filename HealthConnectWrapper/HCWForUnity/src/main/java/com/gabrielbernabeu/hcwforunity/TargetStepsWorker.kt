package com.gabrielbernabeu.hcwforunity

import android.app.NotificationChannel
import android.app.NotificationManager
import android.content.Context
import androidx.core.app.NotificationCompat
import androidx.work.CoroutineWorker
import androidx.work.WorkManager
import androidx.work.WorkerParameters
import kotlin.coroutines.resume
import kotlin.coroutines.suspendCoroutine

class TargetStepsWorker(appContext: Context, workerParams: WorkerParameters) :
    CoroutineWorker(appContext, workerParams) {

    override suspend fun doWork(): Result
    {
        val steps = suspendCoroutine { continuation ->
            Plugin.getTodayStepsCount { stepsCount ->
                continuation.resume(stepsCount)
            }
        }

        val targetSteps = inputData.getInt("target_steps", 10000)

        if (steps >= targetSteps)
        {
            sendNotification("Congratulations!", "You have reached $steps steps!")
            WorkManager.getInstance(applicationContext).cancelUniqueWork(TargetStepsWorker::class.java.simpleName)
            return Result.success()
        }

        return Result.retry()
    }

    private fun sendNotification(title: String, message: String) {
        val notificationManager = applicationContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
        val notificationChannel = NotificationChannel("steps_channel", "Steps Channel", NotificationManager.IMPORTANCE_DEFAULT)
        notificationManager.createNotificationChannel(notificationChannel)

        val notification = NotificationCompat.Builder(applicationContext, "steps_channel")
            .setContentTitle(title)
            .setContentText(message)
            //Add  small icon when possible
            .build()

        notificationManager.notify(1, notification)
    }
}