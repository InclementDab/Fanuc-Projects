//
//  NCGetStatus.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

#import <fwlibios/fwlibios.h>
#import "fwlib64.h"

#import "NCConnectManager.h"
/**
 Get CNC's status class
 */
@interface NCGetStatus : NSObject
-(id)initWithSelectedIndexPath:(NSUInteger)row;
-(short)getStatus:(ODBST2 *)statinfo err:(ODBERR *)err;
-(short)getAlarmMessageAndOperatorMessage:(ODBERR *)err almmsg:(NSString* __strong *)almmsg opemsg:(NSString* __strong*)opemsg;
@end
