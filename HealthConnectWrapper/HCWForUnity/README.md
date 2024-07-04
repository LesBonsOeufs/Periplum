**In the Android Player settings, activate custom main gradle, main manifest, & gradle properties.**
**Keep the Plugins folder which contains them right beneath the Assets folder.**

**Add this to your main gradle's dependencies:**

    //FOR HCW
    implementation 'androidx.health.connect:connect-client:1.0.0-alpha11'
    constraints {
        implementation("org.jetbrains.kotlin:kotlin-stdlib-jdk7:1.8.0") {
            because("kotlin-stdlib-jdk7 is now a part of kotlin-stdlib")
        }
        implementation("org.jetbrains.kotlin:kotlin-stdlib-jdk8:1.8.0") {
            because("kotlin-stdlib-jdk8 is now a part of kotlin-stdlib")
        }
    }

**Add this to your main manifest:**

    In <activity android:name>:

        Replace the activity path with "com.gabrielbernabeu.hcwforunity.UnityPlayerActivity"

    In <activity>'s <intent-filter>:

        <action android:name="androidx.health.ACTION_SHOW_PERMISSIONS_RATIONALE"/>

    In <activity>:

        <meta-data android:name="health_permissions" android:resource="@array/health_permissions" />

    In <manifest>:

        <!-- Check whether Health connect is installed or not -->
        <queries>
            <package android:name="com.google.android.apps.healthdata" />
        </queries>

**Add this to your gradle's properties:**

    android.useAndroidX=true
    android.enableJetifier=true