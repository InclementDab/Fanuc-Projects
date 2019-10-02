//
//  NCDirectoryManager.m
//  focas2Sample
//

#import "NCDirectoryManager.h"
@interface NCDirectoryManager()
@property (nonatomic, copy, readwrite) NSString* deviceName;
@property (nonatomic, copy, readwrite) NSString* dir;
@property NSMutableDictionary<NSString*, NSArray<NCDirInfo*>*> *ncDirInfoDic;
@property NSArray *pathComponet;
@end

@implementation NCDirectoryManager
static NCDirectoryManager * _sharedInstance = nil;

/**
 Generation of singletons
 
 @return pointer of NCDirectoryManager
 */
+(NCDirectoryManager*)sharedManager {
    @synchronized (self) {
        if(!_sharedInstance) {
            _sharedInstance = [NCDirectoryManager new];
        }
    }
    return _sharedInstance;
}

/**
 initialize
 
 @return pointer of self
 */
-(id)init {
    self = [super init];
    if(self) {
        [self refresh];
    }
    return self;
}

/**
 refresh - own propery reinitilize
 */
-(void)refresh {
    self.deviceName = nil;
    self.dir = nil;
    self.ncDirInfoDic = [NSMutableDictionary new];
    self.pathComponet = [NSArray new];
}

/**
 Set dir and deviceName
 */
-(void)setCurrentDir:(NSString *)dir {
    if (![dir hasSuffix:@"/"]) {
        dir = [dir stringByAppendingString:@"/"];
    }
    self.dir = dir;
    self.pathComponet = [dir pathComponents];
    if (2 < [self.pathComponet count]) {
        self.deviceName = self.pathComponet[1];
    }
}

/**
 Check currentDir is top
 
 @return YES - currentDir is top / NO - currentDir is not top
 */
-(BOOL)isCurrentIsTop {
    if (3 == [self.pathComponet count]) {
        if ([self.pathComponet[2] compare:@"/"] == NSOrderedSame) {
            // In case of DeviceMame/ it can not go up.
            return YES;
        }
    }
    return NO;
}

/**
 Add CNC's path information under current folder
 
 @param array [in] array of CNC's path information for to be added
 */
-(void)addNCDirInfoArray:(NSArray<NCDirInfo*> *)array {
    [self.ncDirInfoDic setObject:array forKey:self.dir];
}

/**
 Get CNC's path information under current folder
 
 @return array of CNC's path information
 */
-(NSArray<NCDirInfo*>*)getNCDirInfoArray {
    return [self.ncDirInfoDic objectForKey:self.dir];
}

/**
 Get CNC's path information count
 
 @return CNC's path information count
 */
-(NSInteger)getChildrensCount {
    NSArray* childArray = [self.ncDirInfoDic objectForKey:self.dir];
    if (!childArray) {
        return 0;
    }
    return [childArray count];
}

/**
 Get upper folder name
 
 @return Strings of upper folder name
 */
-(NSString *)getUpperDirName {
    NSMutableString * dirName = [NSMutableString stringWithString:@"/"];
    NSInteger count = [self.pathComponet count];
    
    for (int i=0; i < count - 2; i++) {
        NSString * str = [self.pathComponet objectAtIndex:i];
        [dirName appendString:str];
        if ([str compare:@"/"] != NSOrderedSame) {
            [dirName appendString:@"/"];
        }
    }
    
    return dirName;
}

/**
 Get child folder name
 
 @param childDirName [in] Specify child folder name
 @return Strings of child folder path
 */
-(NSString *)getDirName:(NSString *)childDirName {
    NSString * dirName = [self.dir stringByAppendingString:childDirName];
    if (![dirName hasSuffix:@"/"]) {
        dirName = [dirName stringByAppendingString:@"/"];
    }
    
    return dirName;
}

/**
 Get folder name for display
 
 @return Strings of current folder name for display
 */
-(NSString *)getDispDir {
    NSString *replaceString = [self.dir stringByReplacingOccurrencesOfString:[NSString stringWithFormat:@"//%@" ,self.deviceName] withString:@""];
    return replaceString;
}
@end
