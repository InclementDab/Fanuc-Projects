//
//  NCGetProgram.h
//  focas2Sample
//

#import <Foundation/Foundation.h>
#import "NCConnectionSetting.h"

#import "fwlib64.h"
#import <fwlibios/fwlibios.h>

/**
 Get NC program class
 */
@interface NCGetProgram : NSObject
@property ODBERR err;

-(id)initWithSelectedIndexPath:(NSUInteger )row;
-(void)getcurrentdir:(NSString * __strong *)nsstrDirName;
-(short)getAllDir:(NSString*)curDir isTop:(BOOL)isTop isOnlyDir:(BOOL)isOnlyDir dicArray:(NSMutableArray* __strong*)dicArray;
-(void)movecurrentdir:(NSString *)dir;
-(short)uploadNCProgram:(NSString *)progName progData:(NSString *__strong *)progData;
-(short)downloadNCProgram:(NSString *)dir progName:(NSString *)progName progData:(NSString *)progData;

@end
