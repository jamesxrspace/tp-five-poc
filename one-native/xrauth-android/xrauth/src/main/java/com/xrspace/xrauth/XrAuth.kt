package com.xrspace.xrauth

import android.content.Context
import android.util.Log
import com.xrspace.xrauth.appauth.AuthActivity
import com.xrspace.xrauth.appauth.AuthActivity.Companion.TAG_INTENT_UUID
import com.xrspace.xrauth.callback.AuthError
import com.xrspace.xrauth.callback.AuthResult
import com.xrspace.xrauth.callback.AuthResult.Companion.KEY_ERROR_CODE
import com.xrspace.xrauth.callback.AuthResult.Companion.KEY_ERROR_MESSAGE
import com.xrspace.xrauth.callback.AuthResult.Companion.KEY_RESULT
import com.xrspace.xrauth.callback.ResultDispatcher
import com.xrspace.xrauth.callback.ResultListener
import com.xrspace.xrauth.util.SecureManager
import com.xrspace.xrauth.util.XrStorage
import org.greenrobot.eventbus.EventBus
import org.greenrobot.eventbus.Subscribe
import org.greenrobot.eventbus.ThreadMode
import java.io.Closeable
import java.lang.ref.WeakReference

class XrAuth(context: Context) : IXrAuth, Closeable {
    companion object {
        private const val TAG = "[XrAuth]XrAuth"
    }

    private val appContextWeakReference: WeakReference<Context>
    private var storage: XrStorage
    private val secureManager: SecureManager
    private val authDispatcherMap = mutableMapOf<String, ResultDispatcher<String>>()

    init {
        appContextWeakReference = WeakReference(context.applicationContext)
        storage = XrStorage(appContextWeakReference)
        secureManager = SecureManager(storage)
        EventBus.getDefault().register(this)
    }

    override fun close() {
        EventBus.getDefault().unregister(this)
    }

    override fun login(
        clientId: String,
        domain: String,
        redirectUri: String,
        callback: ResultListener<String>
    ) {
        val dispatcher = ResultDispatcher(callback)
        val context = appContextWeakReference.get()
        if (context == null) {
            dispatcher.onError(AuthError.RequestFailed.appendMessage("context is null"))
            return
        }

        if (clientId.isEmpty() || domain.isEmpty() || redirectUri.isEmpty()) {
            dispatcher.onError(AuthError.RequestFailed.appendMessage("parameter is empty"))
            return
        }

        val intent = AuthActivity.prepareLoginIntent(
            context,
            clientId,
            domain,
            redirectUri
        )

        this.authDispatcherMap[intent.getStringExtra(TAG_INTENT_UUID)!!] = dispatcher

        Log.d(TAG, "Start to open login page")
        context.startActivity(intent)
    }

    override fun logout(
        clientId: String,
        domain: String,
        redirectUri: String,
        callback: ResultListener<String>
    ) {
        Log.d(TAG, "Start to logout")
        val dispatcher = ResultDispatcher(callback)
        val context = appContextWeakReference.get()
        if (context == null) {
            dispatcher.onError(AuthError.RequestFailed.appendMessage("context is null"))
            return
        }

        if (clientId.isEmpty() || domain.isEmpty() || redirectUri.isEmpty()) {
            dispatcher.onError(AuthError.RequestFailed.appendMessage("parameter is empty"))
            return
        }

        val intent = AuthActivity.prepareLogoutIntent(context, domain, redirectUri)
        this.authDispatcherMap[intent.getStringExtra(TAG_INTENT_UUID)!!] = dispatcher

        Log.d(TAG, "Start to open logout page")
        context.startActivity(intent)
    }

    override fun saveInfo(key: String, value: String, callback: ResultListener<Boolean>) {
        secureManager.write(key, value, callback)
    }

    override fun readInfo(key: String, callback: ResultListener<String>) {
        secureManager.read(key, callback)
    }

    override fun deleteInfo(key: String, callback: ResultListener<Boolean>) {
        secureManager.delete(key, callback)
    }

    @Suppress("unused")
    @Subscribe(threadMode = ThreadMode.ASYNC)
    fun onAuthEvent(result: AuthResult) {
        Log.d(TAG, "onAuthEvent: ${result.status.name}")
        val dispatcher = authDispatcherMap.remove(result.eventId)
        if (dispatcher == null) {
            Log.w(TAG, "dispatcher is null, failed to send ${result.status.name} result")
            return
        }

        val payload = result.data
        when (result.status) {
            AuthResult.Status.SUCCESS -> {
                dispatcher.success(payload.getString(KEY_RESULT) ?: "success")
            }

            AuthResult.Status.FAIL -> {
                dispatcher.onFailure(
                    payload.getInt(KEY_ERROR_CODE),
                    payload.getString(KEY_ERROR_MESSAGE) ?: "failed"
                )
            }
        }
    }
}