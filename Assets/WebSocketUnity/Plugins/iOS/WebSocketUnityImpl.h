//----------------------------------------------
// WebSocketUnity
// Copyright (c) 2015, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

#import <Foundation/Foundation.h>
#import "WebSocketUnityInterface.h"

@interface WebSocketUnity : WebSocketUnityInterface
{
    NSString *mGameObject;
}

- (id)initWithURL:(NSString *)url gameObjectName:(NSString *)gameObjectName;

// basic methods inherited
//- (void)connect;
//- (void)close;
//- (void)send:(id)data;
//- (BOOL)isOpened;

// callbacks overrided
- (void)onDidOpen:(NSString *)socket;
- (void)onDidFailWithError:(NSError *)error;
- (void)onDidCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean;
- (void)onDidReceiveMessage:(id)message;

@end