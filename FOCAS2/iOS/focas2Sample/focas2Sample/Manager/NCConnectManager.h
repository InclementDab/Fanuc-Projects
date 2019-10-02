//
//  NCConnectManager.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

#import "DocumentPathManager.h"
#import "NCConnectionSetting.h"

#import "NCHandleOperationThread.h"

/**
 NCConnectSettings manager class
 */
@interface NCConnectManager : DocumentPathManager

+(NCConnectManager *)sharedManager;

-(void)addConnectNCInfo:(NCConnectionSetting *)info;
-(NCConnectionSetting *)getConnectNCInfoFromIndex:(NSUInteger) row;
-(NSUInteger)countOfNcConnectDic;
-(NSString *)keyNameInNcConnectDicAtIndex:(NSUInteger)index;
-(void)deleteSetting:(NSUInteger)row;

-(bool)isContainsNCSetting:(NSString *)name;

-(void)saveToFile;
-(void)loadFromFile;

-(BOOL)validateNCName:(NSString *)ncName;
-(BOOL)validateIPAddressCheck:(NSString *)ipaddr;
-(BOOL)validatePortNumOrTimeoutValCheck:(NSString *)val;

-(void)getAllNCHandle;
-(void)stop;

-(NCHandleOperationThread *) getHandleOperationThreadFromRow:(NSUInteger)rowNum;

@end
