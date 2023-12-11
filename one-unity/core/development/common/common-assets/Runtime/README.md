# 目的

這個 Common-Assets Package 是用來集中控管，屬於我們自己 Custom 的 Shader 與 ShaderGraph。

並且傾向將 Shader 都統一改為 URP 的 ShaderGraph，否則未來會無法在 VisionPro 上面使用，原因如下面參考資料。

## Guideline

- Shaders 目錄 : 放置我們自定義的 Custom Shader ( 也就是 pragma shader_feature 的那種 )，Shader 的程式名稱，應建議為 Custom/MY_SHADER_NAME
- ShaderGraph 目錄 : 放置我們自定義的 Custom ShaderGraph。

### 參考資料

- [Discover how to incorporate visionOS features like passthrough and scene understanding, customize your visuals with Shader Graph, and adapt your interactions to work with spatial input.](https://developer.apple.com/videos/play/wwdc2023/10088/)
- [Unity Shader Graph in immersive apps for Vision Pro](https://developer.apple.com/forums/thread/731443)
