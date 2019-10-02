//
//  FileSaveManager.m
//  focas2Sample
//

#import "FileSaveManager.h"

@interface FileSaveManager()
@property (nonatomic, copy) NSString * currentDir;
@property (nonatomic, copy) NSArray * contents;
@property (nonatomic, copy) NSString * progName;
@property (nonatomic, copy) NSString * progData;


-(NSArray *)getContentsOfDirectoryAtPath:(NSString *)dst err:(NSError **)err;
@end

@implementation FileSaveManager
static FileSaveManager* _sharedInstance = nil;

/**
 Generation of singletons
 
 @return pointer of FileSaveManager
 */
+(FileSaveManager *)sharedManager {
    @synchronized (self) {
        if (!_sharedInstance) {
            _sharedInstance = [FileSaveManager new];
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
    if (self) {
        // initilize
        self.contents = [NSArray new];
        [self setDefaultPath];
    }
    return self;
}

/**
 Set defualt path
 */
-(void)setDefaultPath {
    self.currentDir = [self getDocumentsPath];
}

/**
 Get current folder name for display
 
 @return Strings of current folder name for display
 */
-(NSString *)getCurDir {
    NSString *replacedStr = [self.currentDir stringByReplacingOccurrencesOfString:[self getDocumentsPath] withString:@" "];
    if ([replacedStr hasPrefix:@" /"]) {
        replacedStr = [replacedStr substringFromIndex:2];
    }
    return replacedStr;
}

/**
 Get list of specified path contents
 
 @param dst [in] Specify target path
 @param err [out] pointer of err
 @return array of contents
 */
-(NSArray *)getContentsOfDirectoryAtPath:(NSString *)dst err:(NSError **)err {
    NSMutableArray *array = [NSMutableArray new];
    if ([self.currentDir compare:[self getDocumentsPath]] != NSOrderedSame) {
        [array addObject:NSLocalizedString(@"upper folder", nil)];
    }
    NSArray *filesArray = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:dst error:err];
    // Remove duplicates.
    NSMutableOrderedSet *set = [NSMutableOrderedSet orderedSetWithArray:filesArray];
    // Exclude configuration file.
    if ([filesArray containsObject:NAME_FOR_NCCONNECT_FILE]) {
        [set removeObject:NAME_FOR_NCCONNECT_FILE];
    }
    [array addObjectsFromArray:[set array]];
    return array;
}

/**
 Get count of contents
 
 @return count of contents
 */
-(NSUInteger)countOfContents {
    NSError *err = nil;
    self.contents = [self getContentsOfDirectoryAtPath:self.currentDir err:&err];
    if (!err) {
        return [self.contents count];
    } else {
        return 0;
    }
}

/**
 Get contents name at index
 
 @param row [in] index
 @return String of contents name
 */
-(NSString *)getContentsName:(NSUInteger)row {
    return [self.contents objectAtIndex:row];
}

/**
 Check is folder at index
 
 @param row [in] index
 @reutn YES - folder / NO - file
 */
-(BOOL)isDir:(NSUInteger)row {
    
    if ([[self getContentsName:row] compare:NSLocalizedString(@"upper folder", nil)] == NSOrderedSame) {
        return YES;
    }
    
    BOOL isDir;
    
    NSString * path = [self.currentDir stringByAppendingPathComponent:[self getContentsName:row]];
    [[NSFileManager defaultManager] fileExistsAtPath:path isDirectory:&isDir];
    return isDir;
}

/**
 Set NC program name and NC program data
 
 @param progName [in] NC program name
 @prama progData [in] NC program data
 */
-(void)setPrognameAndProgData:(NSString *)progName progData:(NSString *)progData {
    self.progName = progName;
    // If double-byte characters are found in progData, remove them.
    self.progData = [self deleteFullWidth:[progData mutableCopy]];
}


/**
 Delete fullwidth character
 
 @param str [in] Strings before deleting fullwidth character
 @return String after deleting fullwidth character
 */
- (NSString *)deleteFullWidth:(NSMutableString *)str {
    for(int i=0; i<[str length]; i++) {
        NSString *encodedChar = [[str substringWithRange:NSMakeRange(i, 1)] stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet alphanumericCharacterSet]];
        if (4 <= [encodedChar length]) {
            // Remove the corresponding character.
            [str deleteCharactersInRange:NSMakeRange(i, 1)];
            i--;
        }
    }
    NSLog(@"%@", str);
    return str;
}

/**
 Save to file
 */
-(void)saveFile {
    NSString * path = [self.currentDir stringByAppendingPathComponent:self.progName];
    NSError * err;
    [self.progData writeToFile:path atomically:YES encoding:NSUTF8StringEncoding error:&err];
}

/**
 Make specified folder
 
 @param folderName [in] Specify folder name to be made
 */
-(void)makeDir:(NSString *)folderName {
    NSString * path = [self.currentDir stringByAppendingPathComponent:folderName];
    NSError * err = nil;
    [[NSFileManager defaultManager] createDirectoryAtPath:path withIntermediateDirectories:YES attributes:nil error:&err];
}

/**
 Move folder from table's row
 
 @param row [in] table's row
 */
-(void)moveDir:(NSUInteger)row {
    if ([[self getContentsName:row] compare:NSLocalizedString(@"upper folder", nil)] == NSOrderedSame) {
        self.currentDir = [self.currentDir stringByDeletingLastPathComponent];
    } else {
        self.currentDir = [self.currentDir stringByAppendingPathComponent:[self getContentsName:row]];
    }
}

/**
 Check it can delete from table's row
 
 @param row [in] table's row
 @return YES - can delete / NO cannot delete
 */
-(BOOL)canDelete:(NSUInteger)row {
    return !([[self getContentsName:row] compare:NSLocalizedString(@"upper folder", nil)] == NSOrderedSame);
}

/**
 Delete file from table's row
 
 @param row [in] table's row
 */
-(void)deleteFile:(NSUInteger)row {
    
    NSError * err = nil;
    NSString * path = [self.currentDir stringByAppendingPathComponent:[self getContentsName:row]];
    [[NSFileManager defaultManager] removeItemAtPath:path error:&err];
}

/**
 Set file name from table's row
 
 @param row [in] table's row
 */
-(void)setSelectedFileName:(NSUInteger)row {
    self.progName = [self getContentsName:row];
}

/**
 Get program name for display
 
 @return String of program name
 */
-(NSString *)getProgName {
    NSString *str = [[self getCurDir] stringByAppendingPathComponent:self.progName];
    str = [str stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceCharacterSet]];
    if ([str hasPrefix:@"/"]) {
        str = [str substringFromIndex:1];
    }
    return str;
}

/**
 Get program data
 
 @return String of program data
 */
-(NSString *)getProgData {
    return self.progData;
}

/**
 Get program data form file
 
 @return String of program data
 */
-(NSString *)getProgDataFromFile {
    NSError *err = nil;
    NSString * path = [self.currentDir stringByAppendingPathComponent:self.progName];
    self.progData = [NSString stringWithContentsOfFile:path encoding:NSUTF8StringEncoding error:&err];
    return self.progData;
}

/**
 Get program name
 
 @return String of program name
 */
-(NSString *)getProgFileName {
    return self.progName;
}
@end
