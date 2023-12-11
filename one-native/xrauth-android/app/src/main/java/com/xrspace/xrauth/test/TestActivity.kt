package com.xrspace.xrauth.test

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import com.xrspace.xrauth.callback.ResultListener
import com.xrspace.xrauth.test.R.id
import com.xrspace.xrauth.test.R.layout
import com.xrspace.xrauth.util.Ta
import java.util.Locale

class TestActivity : AppCompatActivity() {
    private lateinit var auth: com.xrspace.xrauth.XrAuth
    private lateinit var region:String
    private lateinit var buildEnv:String
    private lateinit var clientId: String
    private lateinit var domain: String
    private lateinit var audDomain: String
    private val testKey = "com.xrspace.data"
    private var testValue = "{\"test\":\"yes\",\"origin\":true}"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(layout.activity_test)
        region = getString(R.string.REGION)
        buildEnv = getString(R.string.BUILD_ENV)
        clientId = getString(R.string.AUTHING_CLIENT_ID)
        domain = getString(R.string.AUTHING_DOMAIN)
        audDomain = getString(R.string.SERVER_DOMAIN)

        auth = com.xrspace.xrauth.XrAuth(this)
        adjustLayout()
    }

    fun testLogin(@Suppress("UNUSED_PARAMETER")view: View) {
        val redirectUri: String = String.format(
            Locale.getDefault(),
            "%s/api/v1/auth/authing/authn/android/%s", audDomain, this.packageName
        )
        auth.login(
            clientId,
            domain,
            redirectUri,
            object : ResultListener<String> {
                override fun onSuccess(result: String) {
                    Log.d(Ta.g, "Login success: $result")
                    testValue = result
                }

                override fun onFailure(code: Int, message: String) {
                    Log.d(Ta.g, "Login failed: $code, $message")
                }
            })
    }

    fun testLogout(@Suppress("UNUSED_PARAMETER")view: View) {
        val redirectUri = String.format(
            Locale.getDefault(),
            "%s/api/v1/auth/authing/authn/android/%s.logout",
            audDomain,
            this.packageName
        )
        auth.logout(
            clientId,
            domain,
            redirectUri,
            object : ResultListener<String> {
                override fun onSuccess(result: String) {
                    Log.d(Ta.g, "Logout success: $result")
                }

                override fun onFailure(code: Int, message: String) {
                    Log.d(Ta.g, "Logout failed: $code, $message")
                }
            })
    }

    fun testSaveInfo(@Suppress("UNUSED_PARAMETER")view: View) {
        auth.saveInfo(
            testKey,
            testValue,
            object : ResultListener<Boolean> {
                override fun onSuccess(result: Boolean) {
                    Log.d(Ta.g, "Write success: $result")
                }

                override fun onFailure(code: Int, message: String) {
                    Log.d(Ta.g, "Write failed: $code, $message")
                }
            })
    }

    fun testReadInfo(@Suppress("UNUSED_PARAMETER")view: View) {
        auth.readInfo(testKey, object : ResultListener<String> {
            override fun onSuccess(result: String) {
                Log.d(Ta.g, "Read success: $result")
            }

            override fun onFailure(code: Int, message: String) {
                Log.d(Ta.g, "Read failed: $code, $message")
            }
        })
    }

    fun testDeleteInfo(@Suppress("UNUSED_PARAMETER")view: View) {
        auth.deleteInfo(testKey, object : ResultListener<Boolean> {
            override fun onSuccess(result: Boolean) {
                Log.d(Ta.g, "Delete success: $result")
            }

            override fun onFailure(code: Int, message: String) {
                Log.d(Ta.g, "Delete failed: $code, $message")
            }
        })
    }

    private fun adjustLayout(){
        if (region.isNotEmpty()) {
            region = region.uppercase()
        }
        // set textview 'tvRegion' text to region value
        val tvRegion = findViewById<android.widget.TextView>(id.tvRegion)
        tvRegion.text = "Region : $region"

        if (buildEnv.isNotEmpty()) {
            buildEnv = buildEnv.uppercase()
        }
        val tvBuildEnv = findViewById<android.widget.TextView>(id.tvBuildEnv)
        tvBuildEnv.text = "BuildEnv : $buildEnv"
    }
}