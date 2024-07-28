package com.gabrielbernabeu.hcwforunity

import android.app.NotificationManager
import android.app.Service
import android.content.Context
import android.content.Intent
import android.os.Handler
import android.os.IBinder
import androidx.core.app.NotificationCompat
import java.time.Instant

class StepsTracker : Service()
{
    companion object
    {
        var isRunning: Boolean = false
        private set
    }

    //Milliseconds
    private val interval: Long = 60000
    private lateinit var handler: Handler
    private lateinit var runnable: Runnable

    private var targetSteps: Int = -1
    private var since: Instant = Instant.MIN
    private var until: Instant? = null

    override fun onBind(p0: Intent?): IBinder?
    {
        return null
    }

    override fun onCreate() {
        super.onCreate()
        isRunning = true
    }

    override fun onDestroy() {
        super.onDestroy()
        isRunning = false
        kill()
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int
    {
        targetSteps = intent!!.getIntExtra("target_steps", -1)
        since = Instant.parse(intent!!.getStringExtra("since"))

        if (intent!!.hasExtra("until"))
            until = Instant.parse(intent!!.getStringExtra("until"))

        refreshNotification(0)

        handler = Handler(mainLooper)
        runnable = object : Runnable {
            override fun run() {
                checkTargetSteps()
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
        if (until != null && Instant.now() >= until)
        {
            failComplete()
            return;
        }

        Plugin.getStepsCountSince(applicationContext, since)
        { stepsCount ->

            if (stepsCount >= targetSteps)
                successComplete()
            else
            {
                refreshNotification(stepsCount.toInt())
                handler.postDelayed(runnable, interval)
            }
        }
    }

    private fun successComplete()
    {
        val lNotification = NotificationCompat.Builder(applicationContext, "steps_channel")
            .setContentTitle("Congratulations!")
            .setContentText("You have reached your next point!")
            .setSmallIcon(android.R.drawable.ic_dialog_info)
            .build()

        (applicationContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager).notify(2, lNotification)
        kill()
    }

    private fun failComplete()
    {
        val lNotification = NotificationCompat.Builder(applicationContext, "steps_channel")
            .setContentTitle("Failed")
            .setContentText("You didn't reached your goal in time!")
            .setSmallIcon(android.R.drawable.ic_dialog_alert)
            .build()

        (applicationContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager).notify(2, lNotification)
        kill()
    }

    private fun kill()
    {
        handler.removeCallbacks(runnable)
        stopForeground(STOP_FOREGROUND_REMOVE)
        stopSelf()
    }
}