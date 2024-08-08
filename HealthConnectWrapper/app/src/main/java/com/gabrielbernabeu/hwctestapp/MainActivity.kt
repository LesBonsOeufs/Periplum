package com.gabrielbernabeu.hwctestapp

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.gabrielbernabeu.hcwforunity.Plugin

class MainActivity : AppCompatActivity()
{
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Plugin.init(this)
        Plugin.checkAvailability()
        Plugin.startStepsTracker(30)
    }
}