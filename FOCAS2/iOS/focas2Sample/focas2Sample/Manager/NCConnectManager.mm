//
//  NCConnectManager.m
//  focas2Sample
//

#include <ifaddrs.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#import "NCConnectManager.h"

@interface NCConnectManager()
@property NSMutableDictionary<NSString*, NCConnectionSetting*> * ncConnectDic;
@property NSArray *ownIpAddrs;
@property NSMutableArray<NCHandleOperationThread *>* ncHandleArray;
@end

@implementation NCConnectManager
@synthesize ncConnectDic;
@synthesize ownIpAddrs;

static NCConnectManager * _sharedInstance = nil;

static NSString * const CSV_FILE_ITEM = @"NCName,IPAddress,Port,Timeout\n";

/**
 Generation of singletons
 
 @return pointer of NCConnectManager
 */
+(NCConnectManager *)sharedManager {
    @synchronized (self) {
        if (!_sharedInstance) {
            _sharedInstance = [NCConnectManager new];
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
        ncConnectDic = [NSMutableDictionary new];
        ownIpAddrs = [self getOwnIpAddress];
        self.ncHandleArray = [NSMutableArray new];
    }
    return self;
}

/**
 Add NCConnectSetting
 
 @param info [in] Pointer of NCConnectSetting information to be added.
 */
-(void)addConnectNCInfo:(NCConnectionSetting *)info {
    [ncConnectDic setValue:info forKey:info.ncName];
    
    // Acquire a handle at add.
    [self getHandle:info];
}

/**
 Check is contains NCSetting from NC name.
 
 @param name [in] NC name
 @return YES - is contains / NO - is not contains
 */
-(bool)isContainsNCSetting:(NSString *)name {
    if (!name) {
        return NO;
    }
    return (nil != [ncConnectDic objectForKey:name]);
}

/**
 Get NCConnectSetting information from NC name.
 
 @param name [in] NC name
 @return NCConnectSetting information
 */
-(NCConnectionSetting *)getConnectNCInfoFromNCName:(NSString *)name {
    if (!name) {
        return nil;
    }
    return [ncConnectDic objectForKey:name];
}

/**
 Get NCConnectSetting information from table's row.
 
 @param row [in] table's row
 @return NCConnectSetting information
 */
-(NCConnectionSetting *)getConnectNCInfoFromIndex:(NSUInteger) row {
    // Acquire with key. If there is no such information, nil is returned.
    return [self getConnectNCInfoFromNCName:[self keyNameInNcConnectDicAtIndex:row]];
}

/**
 Get count of NCConnectSetting.
 
 @return count
 */
-(NSUInteger)countOfNcConnectDic {
    return [ncConnectDic count];
}

/**
 Get NC name from table's row.
 
 @param index [in] table's row
 @return NC name
 */
-(NSString *)keyNameInNcConnectDicAtIndex:(NSUInteger)index {
    NSArray *keys = [ncConnectDic allKeys];
    keys = [keys sortedArrayUsingComparator:^(id a, id b) {
        return [a compare:b options:NSCaseInsensitiveSearch];
    }];
    if ([keys count] <= index) {
        return nil;
    }
    return [keys objectAtIndex:index];
}

/**
 Delete NCConnectSetting information
 
 @param row [in] table's row
 */
-(void)deleteSetting:(NSUInteger)row {
    NCConnectionSetting *set = [self getConnectNCInfoFromIndex:row];
    // Release Handle.
    [self freeLibHndlAtDeleteSetting:set];
    // remove
    [ncConnectDic removeObjectForKey:set.ncName];
}

/**
 Free library handle when deleted of NCConnectSetting information
 
 @param set [in] NCConnectSetting information
 */
-(void)freeLibHndlAtDeleteSetting:(NCConnectionSetting *) set {
    [self.ncHandleArray enumerateObjectsUsingBlock:^(NCHandleOperationThread* thread, NSUInteger idx, BOOL * stop) {
        if ([thread.ipAddr compare:set.ipAddr] == NSOrderedSame && thread.portNo == set.portNo && thread.timeoutVal == set.timeoutVal) {
            // Thread stop.
            [thread stopThread];
            *stop = YES;
        }
    }];
}

/**
 Save to NCConnectSettings file
 */
-(void)saveToFile {
    // Save as csv format.
    NSMutableString *mstr = [NSMutableString new];
    // Heading on the first line.
    [mstr insertString:CSV_FILE_ITEM atIndex:0];
    
    NSArray *values = [ncConnectDic allValues];
    values = [values sortedArrayUsingComparator:^(NCConnectionSetting* a, NCConnectionSetting* b) {
        return [a.ncName compare:b.ncName options:NSCaseInsensitiveSearch];
    }];
    
    for(NCConnectionSetting* set in values) {
        [mstr appendString:[NSString stringWithFormat:@"\"%@\",\"%@\",%d,%d\n", set.ncName, set.ipAddr, set.portNo, set.timeoutVal]];
    }
    
    NSError *err = nil;
    [mstr writeToFile:[self getNCConectFilePath] atomically:YES encoding:NSUTF8StringEncoding error:&err];
}

/**
 Load from NCConnectSettings file.
 */
-(void)loadFromFile {
    // Read in csv format.
    NSError *err = nil;
    NSString *str = [NSString stringWithContentsOfFile:[self getNCConectFilePath] encoding:NSUTF8StringEncoding error:&err];
    if (err) {
        return;
    }
    NSArray *arry = [str componentsSeparatedByString:@"\n"];
    
    __block NSUInteger addCount = 0;
    
    [arry enumerateObjectsUsingBlock:^(NSString* settingStr, NSUInteger idx, BOOL * stop) {
        
        if (NC_CONNECTION_SETTINGS_MAX - 1 <= addCount) {
            *stop = YES;
        }
        
        if (0 < idx) {
            // Comma separated.
            NSArray *settings = [settingStr componentsSeparatedByString:@","];
            NSUInteger settingsCount = [settings count];
            if (4 <= settingsCount) {
                BOOL isValid = YES;
                NSString *name = nil;
                NSString *ipaddr = nil;
                ushort port = 0;
                LONG timeout = 0;
                for(NSUInteger i = 0; i <settingsCount; i++) {
                    if (!isValid || 4 <= i) {
                        break;
                    }
                    NSString *str = [settings objectAtIndex:i];
                    
                    switch(i) {
                        case 0:
                            str = [str stringByReplacingOccurrencesOfString:@"\"" withString:@""];
                            if(![self validateNCName:str]) {
                                isValid = NO;
                            }
                            name = str;
                            break;
                        case 1:
                            str = [str stringByReplacingOccurrencesOfString:@"\"" withString:@""];
                            if(![self validateIPAddressCheck:str]) {
                                isValid = NO;
                            }
                            ipaddr = str;
                            break;
                        case 2:
                            if (![self validatePortNumOrTimeoutValCheck:str]) {
                                isValid = NO;
                            } else {
                                port = static_cast<ushort>([str intValue]);
                            }
                            break;
                        case 3:
                            if (![self validatePortNumOrTimeoutValCheck:str]) {
                                isValid = NO;
                            } else {
                                timeout = [str intValue];
                            }
                            break;
                        default:
                            break;
                    }
                }
                NCConnectionSetting *set;
                if (isValid) {
                    set = [[NCConnectionSetting alloc] initWithNCNameIpAddrPortNoTimeoutVal:name ipa:ipaddr port:port timeout:timeout];
                    [self addConnectNCInfo:set];
                    addCount++;
                }
            }
        }
    }];
    
    // Get handles.
    [self getAllNCHandle];
}

/**
 Validate NC name
 
 @param ncName [in] NC name for validating
 @return YES - OK / NO - NG
 */
-(BOOL)validateNCName:(NSString *)ncName {
    // When it is empty -> NG.
    if (0 == [ncName length]) {
        return NO;
    }
    // If it is only blank -> NG.
    NSString *trimedString = [ncName stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceCharacterSet]];
    if (0 == [trimedString length]) {
        return NO;
    }
    
    return YES;
}

/**
 Validate IPAddress(Do not check connectablility)
 
 @param ipaddr [in] IPAddress for validating
 @return YES - OK / NO - NG
 */
-(BOOL)validateIPAddressCheck:(NSString *)ipaddr {
    
    if ([ownIpAddrs containsObject:ipaddr]) {
        // The own IP is assumed to be NG.
        return NO;
    }
    
    NSArray * ipseccion = [ipaddr componentsSeparatedByString:@"."];
    if ([ipseccion count] != 4) {
        // There are no four sections.
        return NO;
    }
    
    // Numerical check of each section.
    // Make sure that it is between 0 and 255 respectively.
    // 0.0.0.0 and 127.0.0.1 are NG.
    @try {
        NSInteger num = 0;
        BOOL special = NO;
        for (NSUInteger i = 0; i < [ipseccion count]; i++) {
            NSString *addr = [ipseccion objectAtIndex:i];
            NSInteger sectionNum = [addr integerValue];
            if (sectionNum < 0 || 255 < sectionNum) {
                // Enable 0-255.
                return NO;
            }
            
            if (i==0 && sectionNum==127) {
                special = YES;
            }
            if (i==3 && special && sectionNum==1) {
                // localhost is an error.
                return NO;
            }
            num += sectionNum;
        }
        if (0 == num) {
            // 0.0.0.0 is an error.
            return NO;
        }
    } @catch (NSException *exception) {
        // It is supposed to be a numberException that comes here.
        return NO;
    }
    
    return YES;
}

/**
 Validate PortNum Or TimeoutVal
 
 @param val [in] validating target.
 @return YES - OK(1-65535) / NO - NG
 */
-(BOOL)validatePortNumOrTimeoutValCheck:(NSString *)val {
    @try {
        NSInteger num = [val integerValue];
        if (num < 1 || USHRT_MAX < num ) {
            return NO;
        }
    } @catch (NSException *ex) {
        return NO;
    }
    return YES;
}

/**
 Get Own IPAddress
 
 @return Array of IPAddress strings
 */
-(NSArray *)getOwnIpAddress {
    NSMutableArray *retArray = [NSMutableArray new];
    
    struct ifaddrs *ifa_list;
    
    int n = getifaddrs(&ifa_list);
    if (-1 == n) {
        return nil;
    }
    
    for (struct ifaddrs *ifa = ifa_list; ifa != NULL; ifa = ifa->ifa_next) {
        char addrstr[256] = {0};
        
        if (ifa->ifa_addr->sa_family == AF_INET) {
            inet_ntop(AF_INET, &((struct sockaddr_in *)ifa->ifa_addr)->sin_addr, addrstr, sizeof(addrstr));
            [retArray addObject:[NSString stringWithUTF8String:addrstr]];
        }
    }
    
    freeifaddrs(ifa_list);
    
    return retArray;
}

/**
 Get library handle for all NCConnectSettings
 */
-(void)getAllNCHandle {
    NSArray *values = [ncConnectDic allValues];
    values = [values sortedArrayUsingComparator:^(NCConnectionSetting* a, NCConnectionSetting* b) {
        return [a.ncName compare:b.ncName options:NSCaseInsensitiveSearch];
    }];
    
    for(NCConnectionSetting* set in values) {
        // Acquire a handle on a thread basis.
        [self getHandle:set];
    }
}

/**
 Get library handle for specified NCConnectSetting
 
 @param set [in] Sepecify NCConnectSetting information
 */
-(void)getHandle:(NCConnectionSetting *)set {
    // Acquire a handle on a thread basis.
    NCHandleOperationThread *thread = [[NCHandleOperationThread alloc] initWithNCConnectSettings:set.ipAddr port:set.portNo timeout:set.timeoutVal];
    
    if ([self.ncHandleArray containsObject:thread]) {
        return;
    }
    
    [self.ncHandleArray addObject:thread];
    [thread run];
}

/**
 All NCHandleOperationThread Stop
 */
-(void)stop {
    for (NCHandleOperationThread * thread in self.ncHandleArray) {
        [thread stopThread];
    }
    [self.ncHandleArray removeAllObjects];
}

/**
 Get pointer of NCHandleOperationThread form table's row number.
 
 @param rowNum [in] table's row number
 @return pointer of NCHandleOperationThread
 */
-(NCHandleOperationThread *) getHandleOperationThreadFromRow:(NSUInteger)rowNum {
    NCConnectionSetting *set = [self getConnectNCInfoFromIndex:rowNum];
    if (!set) {
        return nil;
    }
    
    for (NCHandleOperationThread *thread  in self.ncHandleArray) {
        if ([thread.ipAddr compare:set.ipAddr] == NSOrderedSame && thread.portNo == set.portNo && thread.timeoutVal == set.timeoutVal) {
            return thread;
        }
    }
    
    return nil;
}

@end
