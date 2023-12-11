package com.xrspace.tpfive

// Fix java.lang.NoSuchFieldError exception.
// Reference: https://github.com/juicycleff/flutter-unity-view-widget/issues/836
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugins.GeneratedPluginRegistrant
import android.os.Bundle
import android.os.PersistableBundle
import com.xraph.plugin.flutter_unity_widget.FlutterUnityActivity;
import com.xrspace.xrauth.XrAuthPlugin

class MainActivity: FlutterUnityActivity() {
    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        GeneratedPluginRegistrant.registerWith(flutterEngine)

        // 註冊plugin
        flutterEngine.plugins.add(XrAuthPlugin(this))
    }
}
