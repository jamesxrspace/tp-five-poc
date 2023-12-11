package com.xrspace.xrauth.appauth

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.util.Log
import androidx.browser.customtabs.CustomTabsIntent
import com.xrspace.xrauth.callback.AuthError
import com.xrspace.xrauth.callback.AuthResult
import com.xrspace.xrauth.util.RequestType
import net.openid.appauth.AuthState
import net.openid.appauth.AuthorizationException
import net.openid.appauth.AuthorizationRequest
import net.openid.appauth.AuthorizationResponse
import net.openid.appauth.AuthorizationService
import net.openid.appauth.AuthorizationServiceConfiguration
import net.openid.appauth.ResponseTypeValues
import net.openid.appauth.TokenResponse
import org.greenrobot.eventbus.EventBus
import java.util.Locale
import java.util.UUID

class AuthActivity : Activity() {
    companion object {
        private const val TAG = "[XrAuth]AuthActivity"
        private const val TAG_CLIENT_ID = "client_id"
        private const val TAG_AUTH_URI = "auth_uri"
        private const val TAG_TOKEN_URI = "token_uri"
        private const val TAG_LOGOUT_URI = "logout_uri"
        private const val TAG_REDIRECT_URI = "redirect_uri"
        private const val PATH_AUTH = "/oidc/auth"
        private const val PATH_TOKEN = "/oidc/token"
        private const val PATH_LOGOUT = "/login/profile/logout"

        const val TAG_REQUEST_TYPE = "request_type"
        const val TAG_INTENT_UUID = "intent_uuid"

        fun prepareLoginIntent(
            context: Context,
            clientId: String,
            domain: String,
            redirectUri: String
        ): Intent {
            Log.d(TAG, "prepare login intent")
            val intent = Intent(context, AuthActivity::class.java)
            val authUri = String.format(Locale.getDefault(), "%s%s", domain, PATH_AUTH)
            val tokenUri = String.format(Locale.getDefault(), "%s%s", domain, PATH_TOKEN)

            intent.putExtra(TAG_REQUEST_TYPE, RequestType.LOGIN.code)
            intent.putExtra(TAG_AUTH_URI, authUri)
            intent.putExtra(TAG_TOKEN_URI, tokenUri)
            intent.putExtra(TAG_CLIENT_ID, clientId)
            intent.putExtra(TAG_REDIRECT_URI, redirectUri)
            intent.putExtra(TAG_INTENT_UUID, UUID.randomUUID().toString())
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            return intent
        }

        fun prepareLogoutIntent(
            context: Context,
            domain: String,
            redirectUri: String
        ): Intent {
            Log.d(TAG, "prepare logout intent")
            val intent = Intent(context, AuthActivity::class.java)
            val logoutUri = String.format(
                Locale.getDefault(),
                "%s%s?redirect_uri=%s",
                domain,
                PATH_LOGOUT,
                redirectUri
            )
            intent.putExtra(TAG_REQUEST_TYPE, RequestType.LOGOUT.code)
            intent.putExtra(TAG_LOGOUT_URI, logoutUri)
            intent.putExtra(TAG_INTENT_UUID, UUID.randomUUID().toString())
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            return intent
        }
    }

    private lateinit var appAuthService: AuthorizationService
    private lateinit var eventId: String

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        // if intent.getStringExtra(TAG_INTENT_UUID) is null, means it'll response to nowhere.
        // So just finish this activity.
        val uuid = intent.getStringExtra(TAG_INTENT_UUID)
        if (uuid == null) {
            Log.e(TAG, "Intent uuid is null. Terminate AuthActivity.")
            finish()
            return
        }

        Log.d(TAG, "Start AuthActivity")
        eventId = uuid
        appAuthService = AuthorizationService(this)
        dispatchTask(intent)
    }

    override fun onActivityResult(requestType: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestType, resultCode, data)
        Log.d(TAG, "onActivityResult: reqType($requestType), resultCode($resultCode)")

        when (RequestType.fromInt(requestType)) {
            RequestType.LOGIN -> loginReport(data)
            RequestType.LOGOUT -> postSuccessResult("logout success") // Always success from Authing
        }
    }

    override fun onDestroy() {
        Log.d(TAG, "End AuthActivity")
        appAuthService.dispose()
        super.onDestroy()
    }

    private fun dispatchTask(intent: Intent) {
        val clientId = intent.getStringExtra(TAG_CLIENT_ID) ?: ""
        val authUri = intent.getStringExtra(TAG_AUTH_URI) ?: ""
        val tokenUri = intent.getStringExtra(TAG_TOKEN_URI) ?: ""
        val logoutUri = intent.getStringExtra(TAG_LOGOUT_URI) ?: ""
        val redirectUri = intent.getStringExtra(TAG_REDIRECT_URI) ?: ""
        val type = intent.getIntExtra(TAG_REQUEST_TYPE, 0)

        when (RequestType.fromInt(type)) {
            RequestType.LOGIN -> {
                Log.d(TAG, "RequestType: LOGIN")
                val config =
                    AuthorizationServiceConfiguration(Uri.parse(authUri), Uri.parse(tokenUri))
                appAuthLogin(
                    config,
                    clientId,
                    Uri.parse(redirectUri)
                )
            }

            RequestType.LOGOUT -> {
                Log.d(TAG, "RequestType: LOGOUT")
                customTabsLogout(logoutUri)
            }
        }
    }

    /**
     * Open Authing sign in page via AppAuth.
     */
    private fun appAuthLogin(
        config: AuthorizationServiceConfiguration,
        clientId: String, redirectUri: Uri
    ) {
        val authRequest = AuthorizationRequest.Builder(
            config, clientId,
            ResponseTypeValues.CODE, redirectUri
        )
            .setScopes(
                AuthorizationRequest.Scope.OPENID,
                AuthorizationRequest.Scope.OFFLINE_ACCESS,
                AuthorizationRequest.Scope.PROFILE,
                AuthorizationRequest.Scope.EMAIL,
                "username"
            )
            .setPrompt(AuthorizationRequest.Prompt.CONSENT)
            .setDisplay(AuthorizationRequest.Display.POPUP)
            .build()

        val authIntent = appAuthService.getAuthorizationRequestIntent(authRequest)
        this.startActivityForResult(authIntent, RequestType.LOGIN.code)
    }

    /**
     * Open a custom tab to logout.
     * After logout will end to call onActivityResult() of this activity.
     * So call finish() in onActivityResult() to end this activity.
     */
    private fun customTabsLogout(url: String) {
        val customTabsIntent = CustomTabsIntent.Builder().build()
        customTabsIntent.launchUrl(this, Uri.parse(url))
        this.startActivityForResult(customTabsIntent.intent, RequestType.LOGOUT.code)
    }

    private fun loginReport(data: Intent?) {
        if (data == null) {
            return postFailResult(
                AuthError.AuthCodeFailed.appendMessage("Fail to get auth code from url string."),
                null
            )
        }

        val authException = AuthorizationException.fromIntent(data)
        if (authException != null) {
            Log.e(
                TAG, "AuthException: (${authException.code}) ${authException.errorDescription}"
            )
            val error =
                if (authException.code == AuthorizationException.GeneralErrors.USER_CANCELED_AUTH_FLOW.code) AuthError.UserCancel
                else AuthError.AuthCodeFailed
            return postFailResult(error, authException)
        }

        val authResponse = AuthorizationResponse.fromIntent(data)
            ?: return postFailResult(
                AuthError.AuthCodeFailed.appendMessage("AppAuthResponse is null"),
                null
            )
        Log.d(TAG, "Get authorization code success")
        codeToToken(authResponse)
    }

    private fun codeToToken(codeResponse: AuthorizationResponse) {
        appAuthService.performTokenRequest(
            codeResponse.createTokenExchangeRequest()
        ) { tokenResponse: TokenResponse?, tokenException: AuthorizationException? ->
            val tokenState = AuthState().apply {
                this.update(tokenResponse, tokenException)
            }

            Log.d(TAG, "Authorized? " + tokenState.isAuthorized)
            if (tokenResponse == null || !tokenState.isAuthorized) {
                postFailResult(
                    AuthError.AuthTokenFailed.appendMessage("code to token failed"),
                    tokenException
                )
                return@performTokenRequest
            }

            postSuccessResult(tokenResponse.jsonSerializeString())
        }
    }

    private fun postFailResult(error: AuthError, e: Exception?) {
        postFailResult(error.code, error.message, e)
    }

    private fun postFailResult(errorCode: Int, reason: String, e: Exception?) {
        Log.w(TAG, "report failed: $reason")
        val bundle = Bundle()
        bundle.putInt(AuthResult.KEY_ERROR_CODE, errorCode)
        bundle.putString(AuthResult.KEY_ERROR_MESSAGE, reason)
        if (e != null) {
            bundle.putSerializable(AuthResult.KEY_EXCEPTION, e)
        }
        postResult(AuthResult(AuthResult.Status.FAIL, eventId, bundle))
    }

    private fun postSuccessResult(data: String) {
        Log.i(TAG, "report success")
        val bundle = Bundle()
        bundle.putString(AuthResult.KEY_RESULT, data)
        postResult(AuthResult(AuthResult.Status.SUCCESS, eventId, bundle))
    }

    private fun postResult(result: AuthResult) {
        EventBus.getDefault().post(result)
        finish()
    }
}