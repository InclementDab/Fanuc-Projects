//
//  FileSaveManager.h
//  focas2Sample
//

#import <Foundation/Foundation.h>
#import "DocumentPathManager.h"

/**
 File saving manager class  
 */
@interface FileSaveManager : DocumentPathManager
+(FileSaveManager *)sharedManager;
-(void)setDefaultPath;
-(NSString *)getCurDir;
-(NSUInteger)countOfContents;
-(NSString *)getContentsName:(NSUInteger)row;
-(BOOL)isDir:(NSUInteger)row;
-(void)setPrognameAndProgData:(NSString *)progName progData:(NSString *)progData;
-(void)saveFile;
-(void)makeDir:(NSString *)folderName;
-(void)moveDir:(NSUInteger)row;
-(BOOL)canDelete:(NSUInteger)row;
-(void)deleteFile:(NSUInteger)row;
-(void)setSelectedFileName:(NSUInteger)row;
-(NSString *)getProgName;
-(NSString *)getProgData;
-(NSString *)getProgDataFromFile;
-(NSString *)getProgFileName;
@end
