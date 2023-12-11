# 問題原因

Addressable 在各別打包場景之後，如果在 Main 專案中，加載過兩個場景，就會遇到 build-in shader 重複的問題與 Error log。

## 解決方法

複製 Unity 官方預設腳本 Packages/com.unity.addressables/Editor/Build/DataBuilders/BuildScriptPackedMode.cs 的檔案。
另存並且重新命名為 BuildScriptPackagedMode_ImplicitShaderReference.cs，讓 Unity 編譯可以正確，但是內容程式碼相同。
並且加入 AddressableEditorReference.asmref 來定義這個 extension 為 Unity Addressable package 裡面的程式碼，
這樣才能用引用到 Unity 原生 internal 類別中的方法、變數等等。

再來對這份 BuildScriptPackedMode_ImplicitShaderReference.cs 的程式碼，調整並剔除打包流程中的 Build-In shader 的部分。
這樣打包場景以後，就不會有重複的 Build-in Shader 在不同包體裡面。

再來將這份檔案 BuildScriptPackedMode_ImplicitShaderReference.cs 的內容，剔除打包流程中的 Build-In shader 的部分。

## 參考來源

Unity 官方 Addressable 的打包腳本
Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset
Packages/com.unity.addressables/Editor/Build/DataBuilders/BuildScriptPackedMode.cs
