package com.xrspace.xrauth.callback

import java.util.UUID

interface ResultListener<T> {
    fun onSuccess(result: T)
    fun onFailure(code: Int, message: String)
}

class ResultDispatcher<T>(private val listener: ResultListener<T>) {
    fun success(result: T) {
        listener.onSuccess(result)
    }

    fun onFailure(code: Int, message: String) {
        listener.onFailure(code, message)
    }

    fun onError(error: AuthError) {
        listener.onFailure(error.code, error.message)
    }
}