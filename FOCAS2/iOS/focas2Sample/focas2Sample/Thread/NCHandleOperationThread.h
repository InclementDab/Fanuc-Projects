//
//  NCHandleOperationThread.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

#import "fwlib64.h"
#import <fwlibios/fwlibios.h>

/**
 Thread for getting CNC library handle
 */
@interface NCHandleOperationThread : NSObject

@property(nonatomic, copy, readonly) NSString* ipAddr;
@property(nonatomic, readonly) ushort portNo;
@property(nonatomic, readonly) LONG timeoutVal;
@property(nonatomic, readonly) ushort handle;

@property(atomic, readonly) BOOL isAlive;
@property(atomic, readonly) ODBERR err;
@property(atomic, readonly) short retVal;

-(id)initWithNCConnectSettings:(NSString *)ipa port:(ushort)port timeout:(LONG)timeout;
-(void)run;
-(void)stopThread;

-(void) getStateOnThread : (ODBST2 *)statinfo;
-(void) getAlmMsgAndOpeMsgOnThread:(NSString* __strong *)almmsg opemsg:(NSString* __strong*)opemsg;

-(void) getcurrentdirOnThread:(NSString * __strong *)nsstrDirName;
-(void) movecurrentDirOnThread:(NSString *)dir;
-(void) getAllDirOnThread:(NSString*)curDir isTop:(BOOL)isTop isOnlyDir:(BOOL)isOnlyDir dicArray:(NSMutableArray *__strong *)dicArray;
-(void) uploadNCProgramOnThread:(NSString *)progName progData:(NSString *__strong *)progData;
-(void) downloadNCProgramOnThread:(NSString *)dir progName:(NSString *)progName progData:(NSString *)progData;
@end
