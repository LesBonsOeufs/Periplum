package com.gabrielbernabeu.hcwforunity

import android.app.NotificationManager
import android.app.Service
import android.content.Context
import android.content.Intent
import android.os.Handler
import android.os.IBinder
import androidx.core.app.NotificationCompat
import java.time.Instant

class TargetStepsService: Service()
{
    //Milliseconds
    private val interval: Long = 60000
    private lateinit var handler: Handler
    private lateinit var runnable: Runnable
    private var targetSteps: Int = -1
    private var since: Instant = Instant.MIN

    override fun onBind(p0: Intent?): IBinder?
    {
        return null
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int
    {
        targetSteps = intent!!.getIntExtra("target_steps", -1)
        since = Instant.parse(intent!!.getStringExtra("since"))

        refreshNotification(0)

        handler = Handler(mainLooper)
        runnable = object : Runnable {
            override fun run()
            {
                checkTargetSteps()
                handler.postDelayed(this, interval)
            }
        }

        handler.post(runnable)

        return super.onStartCommand(intent, flags, startId)
    }

    //Must be called at least once for foreground
    private fun refreshNotification(currentSteps: Int)
    {
        val lNotification =  NotificationCompat.Builder(applicationContext, "steps_channel")
            .setContentTitle("Counting steps...")
            .setContentText("${targetSteps - currentSteps} remaining!")
            .setSmallIcon(android.R.drawable.ic_dialog_info)
            .setOnlyAlertOnce(true)
            .setOngoing(true)
            .build()

        startForeground(1, lNotification)
    }

    private fun checkTargetSteps()
    {
        Plugin.getStepsCountSince(since)
        { stepsCount ->

            if (stepsCount >= targetSteps)
            {
                val lNotification = NotificationCompat.Builder(applicationContext, "steps_channel")
                    .setContentTitle("Congratulations!")
                    .setContentText("You have reached your next point!")
                    .setSmallIcon(androidx.core.R.drawable.notification_bg)
                    .build()

                (applicationContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager).notify(2, lNotification)
                stopForeground(STOP_FOREGROUND_REMOVE)
            }
            else
            {
                refreshNotification(stepsCount.toInt())
            }
        }
    }
}