package com.xrspace.xrauth.util

/**
 * @author Dewei.chen@xrspace.io
 */
interface IXrStorage {
    fun store(key: String, value: String)
//    fun store(key: String, value: Long)
//    fun store(key: String, value: Int)
//    fun store(key: String, value: Boolean)
    fun retrieveString(key: String): String?
//    fun retrieveLong(key: String): Long?
//    fun retrieveInt(key: String): Int
//    fun retrieveBoolean(key: String): Boolean
    fun remove(key: String)
}

/**
 * @author Dewei.chen@xrspace.io
 */
open class CryptoException(message: String?, cause: Throwable?) : RuntimeException(message, cause) {
}

/**
 * @author Dewei.chen@xrspace.io
 */
class IncompatibleDeviceException(cause: Throwable) : CryptoException(
    String.format(
        "The device is not compatible with the %s class.",
        CryptoUtil::class.java.simpleName
    ), cause
) {}