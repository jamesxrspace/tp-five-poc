package com.xrspace.xrauth.util

import android.content.Context
import android.content.SharedPreferences
import java.lang.ref.WeakReference

class XrStorage(contextWeakReference: WeakReference<Context>) : IXrStorage {
    private lateinit var sharedPreferences: SharedPreferences

    init {
        val context = contextWeakReference.get()
        if (context != null) {
            sharedPreferences = context.getSharedPreferences("XrAuth", Context.MODE_PRIVATE)
        }
    }

    override fun store(key: String, value: String) {
        sharedPreferences.edit().putString(key, value).apply()
    }

    override fun retrieveString(key: String): String? {
        return sharedPreferences.getString(key, null)
    }

    override fun remove(key: String) {
        sharedPreferences.edit().remove(key).apply()
    }
}