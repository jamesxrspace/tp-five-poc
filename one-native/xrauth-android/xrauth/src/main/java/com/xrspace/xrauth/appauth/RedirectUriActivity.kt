package com.xrspace.xrauth.appauth

import android.app.Activity
import android.os.Bundle
import net.openid.appauth.AuthorizationManagementActivity

/**
 * This Activity will catch the broadcast from the redirect_uri of browser : xrspace://domain/path?code=xxx
 * and then send the intent to AuthorizationManagementActivity (of AppAuth) to handle the response.
 */
class RedirectUriActivity: Activity() {
    override fun onCreate(savedInstanceBundle: Bundle?) {
        super.onCreate(savedInstanceBundle)
        startActivity(
            AuthorizationManagementActivity.createResponseHandlingIntent(
                this, intent.data
            )
        )
        finish()
    }
}