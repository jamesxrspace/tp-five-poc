package com.xrspace.xrauth.callback

import android.os.Bundle

data class AuthResult(
    var status: Status = Status.FAIL,
    val eventId: String,
    val data: Bundle
) {
    enum class Status {
        SUCCESS,
        FAIL
    }

    companion object {
        const val KEY_RESULT = "result"
        const val KEY_ERROR_CODE = "errorCode"
        const val KEY_ERROR_MESSAGE = "errorMessage"
        const val KEY_EXCEPTION = "exception"
    }
}
