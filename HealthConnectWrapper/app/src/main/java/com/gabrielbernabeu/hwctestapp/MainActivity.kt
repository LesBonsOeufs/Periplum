package com.gabrielbernabeu.hwctestapp

import android.os.Bundle
import android.os.Handler
import androidx.appcompat.app.AppCompatActivity
import com.gabrielbernabeu.hcwforunity.Plugin
import java.time.Instant

class MainActivity : AppCompatActivity()
{
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Plugin.init(this)
        Plugin.checkAvailability()
        Plugin.startStepsTracker(30, Instant.now().plusSeconds(30).toString())

        val handler = Handler(mainLooper)
        val runnable = object : Runnable {
            override fun run() {
                Plugin.startStepsTracker(60)
            }
        }

        handler.postDelayed(runnable, 5000);
    }
}