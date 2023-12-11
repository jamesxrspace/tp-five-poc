package com.xrspace.xrauth;

import android.content.Context
import android.util.Log
import com.xrspace.xrauth.callback.ResultListener
import io.flutter.embedding.engine.plugins.FlutterPlugin
import io.flutter.plugin.common.BinaryMessenger
import io.flutter.plugin.common.MethodCall
import io.flutter.plugin.common.MethodChannel
import io.flutter.plugin.common.MethodChannel.MethodCallHandler
import com.xrspace.xrauth.callback.AuthError


public class XrAuthPlugin(context: Context) : MethodCallHandler, FlutterPlugin {
    companion object {
        private const val CHANNEL_NAME = "com.xrspace.xrauth.plugin"
        private lateinit var channel: MethodChannel
    }

    private var TAG = "[XrAuth]XrAuthPlugin"
    private var xrauth: XrAuth = XrAuth(context)

    override fun onAttachedToEngine(binding: FlutterPlugin.FlutterPluginBinding) {
        Log.d(TAG, "onAttachedToEngine")
        channel = MethodChannel(binding.binaryMessenger, CHANNEL_NAME)
        channel.setMethodCallHandler(this)
    }

    override fun onDetachedFromEngine(binding: FlutterPlugin.FlutterPluginBinding) {
        Log.d(TAG, "onDetachedFromEngine")
    }

    override fun onMethodCall(call: MethodCall, callback: MethodChannel.Result) {
        Log.d(TAG, "onMethodCall")
        when (call.method) {
            "login" -> login(call, callback)
            "logout" -> logout(call, callback)
            "saveData" -> saveData(call, callback)
            "readData" -> readData(call, callback)
            "deleteData" -> deleteData(call, callback)
            else -> callback.notImplemented()
        }
    }

    private fun login(call: MethodCall, callback: MethodChannel.Result) {
        val clientId = call.argument<String>("clientId")
        val domain = call.argument<String>("domain")
        val redirectUri = call.argument<String>("redirectUri")
        if (clientId == null || domain == null || redirectUri == null) {
            callback.error(
                AuthError.RequestFailed.code.toString(),
                "Failed by invalid parameter(s)",
                null
            )
            return
        }
        xrauth.login(clientId, domain, redirectUri, object : ResultListener<String> {
            override fun onSuccess(result: String) {
                callback.success(result)
            }

            override fun onFailure(code: Int, message: String) {
                callback.error(code.toString(), message, null)
            }
        })
    }

    private fun logout(call: MethodCall, callback: MethodChannel.Result) {
        val clientId = call.argument<String>("clientId")
        val domain = call.argument<String>("domain")
        val redirectUri = call.argument<String>("redirectUri")

        xrauth.logout(clientId!!, domain!!, redirectUri!!, object : ResultListener<String> {
            override fun onSuccess(result: String) {
                callback.success(result)
            }

            override fun onFailure(code: Int, message: String) {
                callback.error(code.toString(), message, null)
            }
        })
    }

    private fun saveData(call: MethodCall, callback: MethodChannel.Result) {
        val key = call.argument<String>("key")
        val value = call.argument<String>("value")

        xrauth.saveInfo(key!!, value!!, object : ResultListener<Boolean> {
            override fun onSuccess(result: Boolean) {
                callback.success(true)
            }

            override fun onFailure(code: Int, message: String) {
                callback.error(code.toString(), message, null)
            }
        })
    }

    private fun readData(call: MethodCall, callback: MethodChannel.Result) {
        val key = call.argument<String>("key")

        xrauth.readInfo(key!!, object : ResultListener<String> {
            override fun onSuccess(result: String) {
                callback.success(result)
            }

            override fun onFailure(code: Int, message: String) {
                callback.error(code.toString(), message, null)
            }
        })
    }

    private fun deleteData(call: MethodCall, callback: MethodChannel.Result) {
        val key = call.argument<String>("key")

        xrauth.deleteInfo(key!!, object : ResultListener<Boolean> {
            override fun onSuccess(result: Boolean) {
                callback.success(true)
            }

            override fun onFailure(code: Int, message: String) {
                callback.error(code.toString(), message, null)
            }
        })
    }
}
