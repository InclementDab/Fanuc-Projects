//
//  NCConnectionSetting.mm
//  focas2Sample
//

#import "NCConnectionSetting.h"

@interface NCConnectionSetting()
@property(nonatomic, copy, readwrite) NSString* ncName;
@property(nonatomic, copy, readwrite) NSString* ipAddr;
@property(nonatomic, readwrite) ushort portNo;
@property(nonatomic, readwrite) LONG timeoutVal;
@end

@implementation NCConnectionSetting
@synthesize ncName;
@synthesize ipAddr;
@synthesize portNo;
@synthesize timeoutVal;

/**
 initilize
 
 @return pointer of self
 */
-(id)init {
    return [self initWithNCNameIpAddrPortNoTimeoutVal:nil ipa:nil port:0 timeout:0];
}

/**
 initilize with NCConnectSetting information
 
 @param name [in] NC name
 @param ipa [in] IPAddress
 @param port [in] port number
 @param timeout [in] time out value
 @return pointer of self
 */
-(id)initWithNCNameIpAddrPortNoTimeoutVal:(NSString *)name ipa:(NSString *)ipa port:(ushort)port timeout:(LONG)timeout {
    self = [super init];
    if (self) {
        self.ncName = name;
        self.ipAddr = ipa;
        self.portNo = port;
        self.timeoutVal = timeout;
    }
    return self;
}

/**
 initilize for coding
 
 @waring not using
 @param aDecoder [in] coder for decoding
 @return pointer of coding
 */
-(id)initWithCoder:(NSCoder *)aDecoder {
    NSString *name = [aDecoder decodeObjectForKey:@"ncName"];
    NSString *ipa = [aDecoder decodeObjectForKey:@"ipAddr"];
    ushort port = static_cast<ushort>([[aDecoder decodeObjectForKey:@"portNo"] intValue]);
    LONG timeout = [aDecoder decodeInt32ForKey:@"timeoutVal"];
    
    return [self initWithNCNameIpAddrPortNoTimeoutVal:name ipa:ipa port:port timeout:timeout];
}

/**
 encode with coder
 
 @warning not using
 @param aCoder [in] coder for encode
 */
-(void)encodeWithCoder:(NSCoder *)aCoder {
    [aCoder encodeObject:self.ncName forKey:@"ncName"];
    [aCoder encodeObject:self.ipAddr forKey:@"ipAddr"];
    [aCoder encodeObject:[NSNumber numberWithUnsignedShort:self.portNo] forKey:@"portNo"];
    [aCoder encodeInt32:self.timeoutVal forKey:@"timeoutVal"];
}

@end
