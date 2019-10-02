//
//  NCDirectoryManager.h
//  focas2Sample
//

#import <Foundation/Foundation.h>
#import "NCDirInfo.h"

/**
 CNC's folder information manager class
 */
@interface NCDirectoryManager : NSObject

@property (nonatomic, copy, readonly) NSString* deviceName;
@property (nonatomic, copy, readonly) NSString* dir; // Current path on NC.（ex://CNC_MEM/USER/PATH1/)

+(NCDirectoryManager*)sharedManager;
-(void)refresh;
-(void)setCurrentDir:(NSString *)curDir;
-(BOOL)isCurrentIsTop;
-(void)addNCDirInfoArray:(NSArray<NCDirInfo*> *)array;
-(NSArray<NCDirInfo*>*)getNCDirInfoArray;
-(NSInteger)getChildrensCount;
-(NSString *)getUpperDirName;
-(NSString *)getDirName:(NSString *)childDirName;
-(NSString *)getDispDir;
@end
