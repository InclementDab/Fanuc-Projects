//
//  DocumentPathManager.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

/**
 Getter class of Documents path
 */
@interface DocumentPathManager : NSObject
-(NSString *)getDocumentsPath;
-(NSString *)getNCConectFilePath;
@end

static NSString* const NAME_FOR_NCCONNECT_FILE = @"NCConnectData.csv";
