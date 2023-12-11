package com.xrspace.xrauth

import com.xrspace.xrauth.callback.ResultListener

interface IXrAuth {
    fun login(
        clientId: String,
        domain: String,
        redirectUri: String,
        callback: ResultListener<String>
    )

    fun logout(
        clientId: String,
        domain: String,
        redirectUri: String,
        callback: ResultListener<String>
    )

    fun saveInfo(
        key: String,
        value: String,
        callback: ResultListener<Boolean>
    )

    fun readInfo(
        key: String,
        callback: ResultListener<String>
    )

    fun deleteInfo(
        key: String,
        callback: ResultListener<Boolean>
    )
}