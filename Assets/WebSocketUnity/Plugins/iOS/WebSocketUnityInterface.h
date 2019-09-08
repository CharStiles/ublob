//----------------------------------------------
// WebSocketUnity
// Copyright (c) 2015, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

#import <Foundation/Foundation.h>

@class SRWebSocket;

@interface WebSocketUnityInterface : NSObject
{
    SRWebSocket *webSocket;
}

- (id)initWithURL:(NSString *)url;

// basic methods
- (void)connect;
- (void)close;
- (void)send:(id)data;
- (BOOL)isOpened;

// callbacks overridable
- (void)onDidOpen:(NSString *)socket;
- (void)onDidFailWithError:(NSError *)error;
- (void)onDidCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean;
- (void)onDidReceiveMessage:(id)message;

@end
