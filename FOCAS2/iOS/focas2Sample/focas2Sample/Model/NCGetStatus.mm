//
//  NCGetStatus.mm
//  focas2Sample
//

#import "NCGetStatus.h"

@interface NCGetStatus()
@property NCHandleOperationThread *thread;
@property NSUInteger rowNum;
@end

@implementation NCGetStatus
/**
 initilize with selected index path
 
 @param row [in] row of selected index path
 */
-(id)initWithSelectedIndexPath:(NSUInteger )row {
    self = [super init];
    
    if (self) {
        self.rowNum = row;
        self.thread = [[NCConnectManager sharedManager] getHandleOperationThreadFromRow:row];
    }
    
    return self;
}

/**
 Get CNC's status
 
 @param statinfo [out] pointer to ODBST structure including status information of CNC.
 @param err [out] pointer to ODBERR structure including error information of CNC.
 @return CNC's library execute result.
 */
-(short)getStatus:(ODBST2 *)statinfo err:(ODBERR *) err {

    if (!self.thread.isAlive) {
        *err = self.thread.err;
        [self.thread run];
        return self.thread.retVal;
    }
    
    if (0 == self.thread.handle) {
        return self.thread.retVal;
    }
    
    [self.thread getStateOnThread:statinfo];
    
    if (self.thread.retVal != EW_OK) {
        *err = self.thread.err;
    }
    if (self.thread.retVal == EW_SOCKET) {
        // Release the handle (socket is burned out).
        [self.thread stopThread];
    }
    
    return self.thread.retVal;
}

/**
 Get CNC's alarm message and operator's message
 
 @param err [out] pointer to ODBERR structure including error information of CNC.
 @param almmsg [out] alarm message
 @param opemsg [out] operator's message
 @return CNC's library execute result.
 */

-(short)getAlarmMessageAndOperatorMessage:(ODBERR *)err almmsg:(NSString* __strong *)almmsg opemsg:(NSString* __strong*)opemsg {

    if (!self.thread.isAlive) {
        *err = self.thread.err;
        [self.thread run];
        return self.thread.retVal;
    }
    
    if (0 == self.thread.handle) {
        *err = self.thread.err;
        return self.thread.retVal;
    }
    
    [self.thread getAlmMsgAndOpeMsgOnThread:almmsg opemsg:opemsg];
    
    if (self.thread.retVal != EW_OK) {
        *err = self.thread.err;
    }
    if (self.thread.retVal == EW_SOCKET) {
        // Release the handle (socket is burned out).
        [self.thread stopThread];
    }
    
    return self.thread.retVal;
}

@end
