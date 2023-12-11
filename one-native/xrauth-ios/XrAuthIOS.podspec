Pod::Spec.new do |spec|

    spec.name         = "XrAuthIOS"
    spec.version      = "0.1.1"
    spec.summary      = "The porting layer of iOS for XrAccountSDK"
    spec.description  = "An iOS Framework for Sign-In/Sign-Out with XrAccountSDK"
    spec.homepage     = "https://xrspace.io/"
    spec.license      = { :type => "MIT", :file => "LICENSE" }
    spec.author       = { "XRSpace" => "authur@xrspace.io"}
    spec.platform     = :ios, "13.0"
    spec.swift_version = "5.7.1"
    # FIXME: Figure out how to point to the source location in a mono repo project.
    # spec.source       = { :git => "git@gitlab.xrspace.io:XR_Backend_Service/xrsketchfabauth_ios.git",tag: "#{spec.version}" }
    spec.source       = {:https => 'file://github.com/XRSPACE-Inc/tp-five/blob/feat/game-account-porting-ios/one-native/xrauth-ios/'}
    # spec.source       = { :path => './one-native/xrauth-ios'}
    spec.source_files = "XrAuth", "XrAuthIOS/**/*.{h,m,swift}"
    spec.ios.framework = "SafariServices"
    spec.dependency "SimpleKeychain", "1.0.1"

end
