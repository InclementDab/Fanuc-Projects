//
//  DocumentPathManager.m
//  focas2Sample
//

#import "DocumentPathManager.h"

@implementation DocumentPathManager
static NSString* _documentPath = nil;


/**
 Get Documents path.
 
 @return string of documents path
 */
-(NSString *)getDocumentsPath {
    @synchronized (self) {
        if (!_documentPath) {
            NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
            _documentPath = [paths objectAtIndex:0];
        }
    }
    return _documentPath;
}

/**
 Get NC connect settings file path.
 
 @return NC connect settings file path
 */
-(NSString *)getNCConectFilePath {
    return [[self getDocumentsPath] stringByAppendingPathComponent:NAME_FOR_NCCONNECT_FILE];
}

@end

