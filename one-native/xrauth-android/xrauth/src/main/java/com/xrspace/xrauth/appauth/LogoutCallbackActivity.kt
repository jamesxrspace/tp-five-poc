package com.xrspace.xrauth.appauth

import android.app.Activity
import android.os.Bundle

/**
 * After logout, the process will return to this class, so we close the CustomTabs here.
 */
class LogoutCallbackActivity : Activity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        finish()
    }
}