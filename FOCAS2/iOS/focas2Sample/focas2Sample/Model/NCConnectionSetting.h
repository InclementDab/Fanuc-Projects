//
//  NCConnectionSetting.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

/**
 NC connect settings information class.
 */
@interface NCConnectionSetting : NSObject<NSCoding>
@property(nonatomic, copy, readonly) NSString* ncName;
@property(nonatomic, copy, readonly) NSString* ipAddr;
@property(nonatomic, readonly) ushort portNo;
@property(nonatomic, readonly) LONG timeoutVal;

-(id)initWithNCNameIpAddrPortNoTimeoutVal:(NSString *)name ipa:(NSString *)ipa port:(ushort)port timeout:(LONG)timeout;
@end
