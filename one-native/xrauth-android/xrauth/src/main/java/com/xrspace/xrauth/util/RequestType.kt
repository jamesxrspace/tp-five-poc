package com.xrspace.xrauth.util

enum class RequestType(val code: Int) {
    LOGIN(1),
    LOGOUT(2);

    companion object {
        fun fromInt(value: Int) = values().first { it.code == value }
    }
}