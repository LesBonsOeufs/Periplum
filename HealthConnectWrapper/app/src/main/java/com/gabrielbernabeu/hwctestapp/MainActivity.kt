package com.gabrielbernabeu.hwctestapp

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import com.gabrielbernabeu.hcwforunity.Plugin
import java.time.Instant

class MainActivity : AppCompatActivity()
{
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Plugin.init(this)
        Plugin.checkAvailability()
        Plugin.startTargetStepsService(30)
    }
}