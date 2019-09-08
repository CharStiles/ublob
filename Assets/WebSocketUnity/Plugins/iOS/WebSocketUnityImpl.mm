//----------------------------------------------
// WebSocketUnity
// Copyright (c) 201(4, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

#import "WebSocketUnityImpl.h"

@implementation WebSocketUnity

- (id)initWithURL:(NSString *)url gameObjectName:(NSString *)gameObjectName{
    self = [self initWithURL:url];
    self->mGameObject = [gameObjectName copy];
    return self;
}

// callbacks override
- (void)onDidOpen:(NSString *)socket{
    NSLog(@"[WebSocketUnity implementation] onDidOpen: %@", socket);
    UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityOpen",[socket UTF8String]);
}

- (void)onDidFailWithError:(NSError *)error{
    NSLog(@"[WebSocketUnity implementation] onDidFailWithError: %@", error.description);
    UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityError",[error.description UTF8String]);
}

- (void)onDidCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean{
    NSLog(@"[WebSocketUnity implementation] onDidCloseWithCode: %@", reason);
    UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityClose",[reason UTF8String]);
}

- (void)onDidReceiveMessage:(id)message{
    NSLog(@"[WebSocketUnity implementation] onDidReceiveMessage: %@", message);
    //NSLog(@">>>> type : %@", [message class]);
    if ([message isKindOfClass:[NSString class]]) {
        // string
        UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityReceiveMessage",[message UTF8String]);
    }else{
        // data
        // need to convert into byte after (in Unity)
        // Note : I use a method for iOS 7, if you use ios6 or ios5, you can use
        // base64Encoding : [[message base64Encoding] UTF8String]
        // see documentation for more details
        // https://developer.apple.com/library/ios/documentation/cocoa/reference/foundation/classes/NSData_Class/Reference/Reference.html
        
        if([NSData instancesRespondToSelector:@selector(base64EncodedStringWithOptions:)]){
            UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityReceiveDataOnMobile", [[message base64EncodedStringWithOptions:0] UTF8String]);
        }else{
            UnitySendMessage([mGameObject UTF8String],"OnWebSocketUnityReceiveDataOnMobile", [[message base64Encoding] UTF8String]);
        }

        
    }

}

@end

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}


static  WebSocketUnity *webSocket = nil;

// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {
    
    void _Create(char* url, char* gameObjectName)
    {
    	//if(webSocket != nil)
    	//	[webSocket release];
        webSocket = [[WebSocketUnity alloc] initWithURL:CreateNSString(url) gameObjectName:CreateNSString(gameObjectName)];
    }
    
	void _Connect()
	{
        [webSocket connect];
	}
    
    void _Close()
    {
        [webSocket close];
    }
	
    void _Send(char* message)
    {
        [webSocket send:CreateNSString(message)];
    }

    void _SendData(Byte* data, uint size)
    {
        NSData* convertedData = [NSData dataWithBytes:data length:size];
        [webSocket send:convertedData];
    }
    
    bool _IsOpened()
	{
        return [webSocket isOpened];
	}
    
}

