//
//  NCGetProgram.mm
//  focas2Sample
//

#import "NCGetProgram.h"
#import "NCConnectManager.h"
#import "NCDirInfo.h"

@interface NCGetProgram()
@property NCHandleOperationThread *thread;
@property NSUInteger rowNum;

@end

@implementation NCGetProgram
@synthesize err;
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
 Get current folder
 
 @param nsstrDirName [out] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void)getcurrentdir:(NSString * __strong *)nsstrDirName {
    [self.thread getcurrentdirOnThread:nsstrDirName];
}

/**
 Get specify folder structure information
 
 @param curDir [in] path name string
 @param isTop [in] isTop folder
 @param isOnlyDir [in] include files? YES - include / NO - do not include
 @param dicArray [out] mutableArray of files information under the specified folder
 @return CNC's library execute result.
 */
-(short)getAllDir:(NSString*)curDir isTop:(BOOL)isTop isOnlyDir:(BOOL)isOnlyDir dicArray:(NSMutableArray *__strong *)dicArray {
    if (!self.thread.isAlive) {
        err = self.thread.err;
        [self.thread run];
        return self.thread.retVal;
    }
    
    if (0 == self.thread.handle) {
        return self.thread.retVal;
    }
    
    [self.thread getAllDirOnThread:curDir isTop:isTop isOnlyDir:isOnlyDir dicArray:dicArray];
    
    if (self.thread.retVal != EW_OK) {
        err = self.thread.err;
    }
    if (self.thread.retVal == EW_SOCKET) {
        // Release the handle (socket is burned out).
        [self.thread stopThread];
    }
    
    return self.thread.retVal;
}

/**
 Move CNC's current folder
 
 @param dir [in] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void)movecurrentdir:(NSString *)dir {
    [self.thread movecurrentDirOnThread:dir];
}

/**
 Uploading NC program.
 
 @param progName [in] NC program name
 @param progData [out] Specify the pointer to the buffer to be read.
 @return CNC's library execute result.
 */
-(short)uploadNCProgram:(NSString *)progName progData:(NSString *__strong *)progData {

    if (!self.thread.isAlive) {
        err = self.thread.err;
        [self.thread run];
        return self.thread.retVal;
    }
    
    if (0 == self.thread.handle) {
        return self.thread.retVal;
    }
    
    [self.thread uploadNCProgramOnThread:progName progData:progData];
    
    if (self.thread.retVal != EW_OK) {
        err = self.thread.err;
    }
    if (self.thread.retVal == EW_SOCKET) {
        // Release the handle (socket is burned out).
        [self.thread stopThread];
    }
    
    return self.thread.retVal;
}

/**
 Downloading NC program.
 
 @param dir [in] Specify the CNC's folder
 @param progName [in] Specify the NC program name
 @param progData [out] Specify the NC program data.
 @return CNC's library execute result.
 */
-(short)downloadNCProgram:(NSString *)dir progName:(NSString *)progName progData:(NSString *)progData {
    if (!self.thread.isAlive) {
        err = self.thread.err;
        [self.thread run];
        return self.thread.retVal;
    }
    
    if (0 == self.thread.handle) {
        return self.thread.retVal;
    }
    
    [self.thread downloadNCProgramOnThread:dir progName:progName progData:progData];
    
    if (self.thread.retVal != EW_OK) {
        err = self.thread.err;
    }
    if (self.thread.retVal == EW_SOCKET) {
        // Release the handle (socket is burned out).
        [self.thread stopThread];
    }
    
    return self.thread.retVal;
}

@end
