//
//  XrAuthIOS.h
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AuthenticationServices/AuthenticationServices.h>


//! Project version number for XrAuthIOS.
FOUNDATION_EXPORT double XrAuthIOSVersionNumber;

//! Project version string for XrAuthIOS.
FOUNDATION_EXPORT const unsigned char XrAuthIOSVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <XrAuthIOS/PublicHeader.h>

#import <XrAuthIOS/XrAuthIOS.h>

#ifdef __cplusplus
extern "C" {
#endif
    typedef void (*RequestXrAuthCallback)(const int resultCode, const char* result);
#ifdef __cplusplus
}
#endif
