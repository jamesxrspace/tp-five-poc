package com.xrspace.xrauth.util

import android.os.Handler
import android.os.Looper
import android.util.Base64
import com.xrspace.xrauth.callback.AuthError
import com.xrspace.xrauth.callback.ResultDispatcher
import com.xrspace.xrauth.callback.ResultListener

class SecureManager(private val storage: IXrStorage) {
    private var crypto: CryptoUtil = CryptoUtil(storage, "com.xrspace.xrauth.key")
    private var workerHandler: Handler = Handler(Looper.myLooper() ?: Looper.getMainLooper())

    fun write(key: String, value: String, callback: ResultListener<Boolean>) {
        val dispatcher = ResultDispatcher(callback)
        workerHandler.postAtFrontOfQueue {
            try {
                val encryptedValue = crypto.encrypt(value.encodeToByteArray())
                val encodedValue = Base64.encodeToString(encryptedValue, Base64.DEFAULT)
                if (encodedValue.isNullOrBlank()) {
                    dispatcher.onError(AuthError.SaveDataFailed.appendMessage("Empty $key value after encoded"))
                } else {
                    storage.store(key, encodedValue)
                    dispatcher.success(true)
                }
            } catch (e: CryptoException) {
                e.printStackTrace()
                dispatcher.onError(AuthError.SaveDataFailed.appendMessage(e.toString()))
            }
        }
    }

    fun read(key: String, callback: ResultListener<String>) {
        val dispatcher = ResultDispatcher(callback)
        workerHandler.post {
            try {
                val encodedValue = storage.retrieveString(key)
                if (encodedValue.isNullOrEmpty()) {
                    dispatcher.onError(AuthError.ReadDataFailed.appendMessage("No $key value found"))
                    return@post
                }

                val decodedValue = Base64.decode(encodedValue, Base64.DEFAULT)
                if (decodedValue == null || decodedValue.isEmpty()) {
                    dispatcher.onError(AuthError.ReadDataFailed.appendMessage("Empty $key value after decoded"))
                    return@post
                }

                val decryptedValue = crypto.decrypt(decodedValue)
                if (decryptedValue == null || decryptedValue.isEmpty()) {
                    dispatcher.onError(AuthError.ReadDataFailed.appendMessage("Empty $key value after decrypted"))
                    return@post
                }
                dispatcher.success(decryptedValue.decodeToString())
            } catch (e: CryptoException) {
                e.printStackTrace()
                dispatcher.onError(AuthError.ReadDataFailed.appendMessage(e.toString()))
            }
        }
    }

    fun delete(key: String, callback: ResultListener<Boolean>) {
        val dispatcher = ResultDispatcher(callback)
        workerHandler.post {
            try {
                storage.remove(key)
                dispatcher.success(true)
            } catch (e: Exception) {
                e.printStackTrace()
                dispatcher.onError(AuthError.DeleteInfoFailed.appendMessage(e.toString()))
            }
        }
    }
}