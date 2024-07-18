package com.gabrielbernabeu.hwctestapp

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.gabrielbernabeu.hcwforunity.Plugin

class MainActivity : AppCompatActivity()
{
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Plugin.Companion.setActivity(this)
        Plugin.Companion.checkAvailability()
        Plugin.Companion.scheduleStepCountWorker(10)
    }
}