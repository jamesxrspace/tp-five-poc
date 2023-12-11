package com.xrspace.xrauth.callback

import android.os.Parcel
import android.os.Parcelable

enum class AuthError(var code: Int, var message: String) : Parcelable {
    None(0, "None"),
    FunctionNotSupported(2001, "Function Not Supported"),
    FunctionNotImplemented(2002, "Function Not Implemented"),
    SaveDataFailed(2011, "Save data failed"),
    ReadDataFailed(2012, "Read data failed"),
    DeleteInfoFailed(2013, "Delete data failed"),
    UserCancel(2111, "User cancel login"),
    AuthCodeFailed(2112, "Failed to get auth code during login"),
    AuthTokenFailed(2113, "Failed to exchange access_token with authorization code"),
    RequestFailed(2303, "Request failed"),
    UnknownError(2999, "Unknown error");

    fun appendMessage(extend: String): AuthError {
        if (extend.isNotBlank() && !message.contains(extend))
            message = "$message: $extend"
        return this
    }

    override fun describeContents(): Int {
        return code
    }

    override fun writeToParcel(dest: Parcel, flags: Int) {
        dest.writeString(message)
    }

    companion object CREATOR : Parcelable.Creator<AuthError> {
        private fun findByCode(code: Int): AuthError {
            for (error in values()) {
                if (error.code == code) {
                    return error
                }
            }
            return UnknownError
        }

        override fun createFromParcel(parcel: Parcel): AuthError {
            val result = findByCode(parcel.readInt())
            parcel.readString()?.apply { result.message = this }
            return result
        }

        override fun newArray(size: Int): Array<AuthError?> {
            return arrayOfNulls(size)
        }
    }
}