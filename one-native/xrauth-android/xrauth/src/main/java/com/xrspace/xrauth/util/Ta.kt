package com.xrspace.xrauth.util

import java.util.Locale

// This class is designed for developers to print line numbers in logcat.
// It is not recommended to use it in the official version.
public class Ta {
    companion object {
        val g: String
            get() {
                var result = "[XrAuth]"
                val elements = Thread.currentThread().stackTrace
                if (elements.size >= 4) {
                    result += String.format(
                        Locale.getDefault(),
                        "(%s:%d)",
                        elements[3].fileName,
                        elements[3].lineNumber
                    )
                }
                return result
            }
    }
}