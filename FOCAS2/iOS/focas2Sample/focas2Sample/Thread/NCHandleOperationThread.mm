//
//  NCHandleOperationThread.m
//  focas2Sample
//

#import "NCHandleOperationThread.h"

#import "NCDirInfo.h"

@interface NCHandleOperationThread()
@property(nonatomic, copy, readwrite) NSString* ipAddr;
@property(nonatomic, readwrite) ushort portNo;
@property(nonatomic, readwrite) LONG timeoutVal;
@property(nonatomic, readwrite) ushort handle;
@property(nonatomic, readwrite) NSString* cnc_dir;

@property(atomic, readwrite) NSThread * thread;
@property(atomic, readwrite) BOOL isAlive;
@property(atomic, readwrite) ODBERR err;
@property(atomic, readwrite) short retVal;
@end

@implementation NCHandleOperationThread
@synthesize handle;
@synthesize isAlive;
@synthesize err;
@synthesize retVal;
@synthesize cnc_dir;

/**
 initilize
 
 @return pointer of self
 */
-(id)init {
    return [self initWithNCConnectSettings:nil port:0 timeout:0];
}

/**
 initilize with NC Connect Settings Data
 
 @param ipa [in] Specify CNC's IPAddress.
 @param port [in] Specify port number.
 @param timeout [in] Specify seconds for timeout.
 @return pointer of self
 */
-(id)initWithNCConnectSettings:(NSString *)ipa port:(ushort)port timeout:(LONG)timeout {
    self = [super init];
    if (self) {
        self.ipAddr = ipa;
        self.portNo = port;
        self.timeoutVal = timeout;
        // At the start, initialize with EW_BUSY.
        retVal = EW_BUSY;
        cnc_dir = @"'";
        err = {0};
    }
    
    self.thread = [[NSThread alloc] initWithTarget:self selector:@selector(threadLoop:) object:nil];
    
    return self;
}

/**
 Thread Run
 */
-(void)run {
    if (!isAlive) {
        isAlive = YES;
    }
    
    if (self.thread.isFinished) {
        self.thread = [[NSThread alloc] initWithTarget:self selector:@selector(threadLoop:) object:nil];
    }
    
    [self.thread start];
}

/**
 Waiting process
 @param secs [in] waiting time(sec.)
 */
-(void)waitTime:(NSTimeInterval)secs {
    @autoreleasepool {
        [[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:secs]];
    }
}

/**
 Thread Stop
 */
-(void)stopThread {
    isAlive = NO;
    
    while(!self.thread.isFinished) {
        [self waitTime:0.02f];
    }
}

/**
 Get CNC Detail Error
 */
-(void)getDetailErr {
    cnc_getdtailerr(handle, &err);
}

/**
 Thread Operation
 
 Get the library handle and Free library handle.
 */
-(void) threadLoop:(id)userInfo {
    @autoreleasepool {
        if ([self.ipAddr length] == 0) {
            isAlive = NO;
            return;
        }
        
        retVal = cnc_allclibhndl3([self.ipAddr UTF8String], self.portNo, self.timeoutVal, &handle);
        NSLog(@"cnc_allclibhndl3 ret:%d, ipaddr:%@ portNo:%d timeout:%d handle:%d", retVal, self.ipAddr, self.portNo, self.timeoutVal, handle);
        if (retVal != EW_OK) {
            isAlive = NO;
            [self getDetailErr];
            return;
        }
        
        while (isAlive) {
            [self waitTime:0.02f];
        }
        
        cnc_freelibhndl(handle);
        handle = 0;
        cnc_exitthread();
    }
}

/**
 Equality Comarison - Is it equal to object?
 @param object [in] others(compare object)
 @return YES - equal / NO - not eqaul
 */
-(BOOL) isEqual:(id)object {
    if (self == object) {
        return YES;
    }
    
    if ([object isMemberOfClass:[self class]]) {
        NCHandleOperationThread *other = static_cast<NCHandleOperationThread *>(object);
        
        if ([self.ipAddr compare:other.ipAddr] == NSOrderedSame &&
            self.portNo == other.portNo &&
            self.timeoutVal == other.timeoutVal) {
            return YES;
        }
    }
    return NO;
}

#pragma mark - performInvocation
/**
 method invoke - Method execution on thread.
 @param anInvacation [in] NSInvocationObject
 */
-(void)performInvocation:(NSInvocation *)anInvacation {
    [anInvacation invokeWithTarget:self];
}

#pragma mark - getState
/**
 Get CNC's status on thread
 @param statinfo [out] pointer to ODBST structure including status information of CNC.
 */
-(void) getStateOnThread:(ODBST2 *)statinfo {
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:@selector(cncstatinfo2:)];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&statinfo atIndex:2];
    [anInvocation setSelector:@selector(cncstatinfo2:)];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Call cnc_statinfo2 method
 @param statinfo [out] pointer to ODBST structure including status information of CNC.
 */
-(void) cncstatinfo2:(ODBST2 *)statinfo {
    retVal = cnc_statinfo2(handle, statinfo);
    if (retVal != EW_OK) {
        [self getDetailErr];
    }
}

/**
 Get CNC's alarm message and operation's message on thread
 @param almmsg [out] alarm message
 @param opemsg [out] operator's message
 */
-(void) getAlmMsgAndOpeMsgOnThread:(NSString* __strong *)almmsg opemsg:(NSString* __strong*)opemsg {
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:@selector(cncGetAlmMsgAndOpeMsg:opemsg:)];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&almmsg atIndex:2];
    [anInvocation setArgument:&opemsg atIndex:3];
    [anInvocation setSelector:@selector(cncGetAlmMsgAndOpeMsg:opemsg:)];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Get CNC's alarm message and operation's message
 @param almmsg [out] alarm message
 @param opemsg [out] operator's message
 */
-(void) cncGetAlmMsgAndOpeMsg:(NSString* __strong *)almmsg opemsg:(NSString* __strong*)opemsg {
    // Language acquisition.
    short num = 43;
    short axis = 0;
    short length = 4 + 1;
    ODBDGN diag = {0};
    
    retVal = cnc_diagnoss(handle, num, axis, length, &diag);
    if (retVal != EW_OK) {
        [self getDetailErr];
        return;
    }
    // Acquire character codes for each country.
    NSStringEncoding encoding = [self getEncodingFromLanguage:diag.u.idata];
    // Acquisition of lineage.
    short path_no;
    short maxpath_no;
    retVal = cnc_getpath(handle, &path_no, &maxpath_no);
    if (retVal != EW_OK) {
        [self getDetailErr];
        return;
    }
    
    // Getting almMessage.
    *almmsg = [NSMutableString new];
    short type = -1;
    // Perform for all lines.
    for (int path = 1; path <= maxpath_no; path++) {
        // Changing the lineage (do not set the same place).
        if (path != path_no) {
            if (EW_OK != cnc_setpath(handle, path)) {
                break;
            }
        }
        
        // Read almMessage.
        short type = -1;
        // Here is a decision.
        short almnum = 30;
        ODBALMMSG2 *odbalmmsg2 = new ODBALMMSG2[almnum];
        memset(odbalmmsg2, 0x0, sizeof(ODBALMMSG2) * almnum);
        retVal = cnc_rdalmmsg2(handle, type, &almnum, odbalmmsg2);
        if (retVal != EW_OK) {
            delete[] odbalmmsg2;
            break;
        }
        
        // Save almMessage.
        for (int i=0; i < almnum; i++) {
            [(NSMutableString *)*almmsg appendFormat:@"PATH%02d\n", path];
            NSData *sjisData = [NSData dataWithBytes:odbalmmsg2[i].alm_msg length:odbalmmsg2[i].msg_len];
            NSString *str = [[NSString alloc] initWithData:sjisData encoding:encoding];
            [(NSMutableString *)*almmsg appendFormat:@"%@%04d %@\n", GetAlmTypeText(odbalmmsg2[i].type), odbalmmsg2[i].alm_no, str];
        }
        
        delete[] odbalmmsg2;
    }
    // Restore.
    cnc_setpath(handle, path_no);
    if (retVal != EW_OK) {
        [self getDetailErr];
        return;
    }
    
    // Getting an operaterMessage.
    *opemsg = [NSMutableString new];
    
    short openum = 5;
    OPMSG3 *opmsg = new OPMSG3[openum];
    memset(opmsg, 0x0, sizeof(OPMSG3) * openum);
    retVal = cnc_rdopmsg3(handle, type, &openum, opmsg);
    
    if (retVal != EW_OK) {
        [self getDetailErr];
        delete [] opmsg;
        return;
    }
    
    // Save operaterMessage.
    for (int i=0; i<openum; i++) {
        if (-1 < opmsg[i].datano) {
            [(NSMutableString *)*opemsg appendFormat:@"%@%04d\n", NSLocalizedString(@"No.", nil), opmsg[i].datano];
            NSData *data = [NSData dataWithBytes:opmsg[i].data length:opmsg[i].char_num];
            NSString *str = [[NSString alloc] initWithData:data encoding:encoding];
            [(NSMutableString *)*opemsg appendFormat:@"%@\n\n", str];
        }
    }
    
    delete [] opmsg;
}

/**
 Get string-encoding from language code
 @param language [in] language code
 @return string-encoding
 */
-(NSStringEncoding) getEncodingFromLanguage:(short)language {
    switch (language) {
        case 0:
            // ASCII (English).
            return NSASCIIStringEncoding;
        case 1:
        case 4:
            // ShiftJIS (Japanese, Traditional Chinese).
            return NSShiftJISStringEncoding;
        case 15:
            // GB2312 (Simplified Chinese).
            return CFStringConvertEncodingToNSStringEncoding(kCFStringEncodingGB_18030_2000);
        case 6:
            // Code page 949: Korean.
            return CFStringConvertEncodingToNSStringEncoding(kCFStringEncodingDOSKorean);
        case 16:
            // Code page 1251: Russian language.
            return NSWindowsCP1251StringEncoding;
        case 17:
            // Code page 1254: Turkish.
            return NSWindowsCP1254StringEncoding;
        default:
            // European character code.
            return NSISOLatin1StringEncoding;
    }
}

#pragma mark - getNCProgram
const int BUFF_SIZE = 1280;

/**
 Sleep - millisecond sleep
 @param msec [in] sleep time(msec.)
 */
-(int)Sleep:(uint)msec {
    return usleep(msec * 1000);
}

/**
 Reads the information of CNC's current folder on thread.
 @param nsstrDirName [out] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void) getcurrentdirOnThread:(NSString * __strong *)nsstrDirName {
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:@selector(getcurrentdir:)];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&nsstrDirName atIndex:2];
    [anInvocation setSelector:@selector(getcurrentdir:)];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Reads the information of CNC's current folder.
 @param nsstrDirName [out] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void)getcurrentdir:(NSString * __strong *)nsstrDirName {
    *nsstrDirName = [NSString stringWithString:cnc_dir];
    retVal = EW_OK;
}

/**
 Sets the current folder on thred.
 
 @param dir [in] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void) movecurrentDirOnThread:(NSString *)dir {
    [self performSelector:@selector(movecurrentDir:) onThread:self.thread withObject:dir waitUntilDone:YES];
}

/**
 Sets the current folder.
 @param dir [in] Specify the pointer for the string that stores "Current drive + folder"
 */
-(void) movecurrentDir:(NSString *)dir {
    cnc_dir = [NSString stringWithString:dir];
}

/**
 Reads the file information under the specified folder on thread.
 
 @param curDir [in] path name string
 @param isTop [in] isTop folder
 @param isOnlyDir [in] include files? YES - include / NO - do not include
 @param dicArray [out] mutableArray of files information under the specified folder
 */
-(void) getAllDirOnThread:(NSString*)curDir isTop:(BOOL)isTop isOnlyDir:(BOOL)isOnlyDir dicArray:(NSMutableArray *__strong *)dicArray {
    
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:@selector(getAllDir:isTop:isOnlyDir:dicArray:)];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&curDir atIndex:2];
    [anInvocation setArgument:&isTop atIndex:3];
    [anInvocation setArgument:&isOnlyDir atIndex:4];
    [anInvocation setArgument:&dicArray atIndex:5];
    [anInvocation setSelector:@selector(getAllDir:isTop:isOnlyDir:dicArray:)];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Reads the file information under the specified folder.
 
 @param curDir [in] path name string
 @param isTop [in] isTop folder
 @param isOnlyDir [in] include files? YES - include / NO - do not include
 @param dicArray [out] mutableArray of files information under the specified folder
 */
-(void) getAllDir:(NSString*)curDir isTop:(BOOL)isTop isOnlyDir:(BOOL)isOnlyDir dicArray:(NSMutableArray *__strong *)dicArray {
        short	num = MAX_ALL_DIR_COUNT;
        IDBPDFADIR pin;
        ODBPDFADIR *pout = new ODBPDFADIR[num];
        memset(pout, 0x0, sizeof(ODBPDFADIR) * num);
    
        strcpy(pin.path, [curDir UTF8String]);
        pin.req_num = 0;
        pin.size_kind = 2; //kbyte
        pin.type = 1; // Get size, comment, processing time stamp.
    
        retVal = cnc_rdpdf_alldir(handle, &num, &pin, pout);
    
        if (EW_OK != retVal && EW_NUMBER != retVal) {
            [self getDetailErr];
            NSLog(@"cnc_rdpdf_alldir Acquisition failure ret:%d", retVal);
            delete[] pout;
            return;
        }
    
        if (EW_NUMBER == retVal) {
            // There is no subfolder for req_num.
            num = 0;
        }
    
        *dicArray = [NSMutableArray new];
        if (!isTop) {
            NCDirInfo *info = [[NCDirInfo alloc] initWithUpFolder];
            [*dicArray addObject:info];
        }
    
        for (int i=0; i<num; i++) {
            ODBPDFADIR dirInfo = pout[i];
            // For Directory only.
            if (isOnlyDir){
                if (0 != dirInfo.data_kind) {
                    continue;
                }
            }
            NCDirInfo *info = [[NCDirInfo alloc] initWithAllProperty:dirInfo.data_kind
                                                                year:dirInfo.year
                                                                 mon:dirInfo.mon
                                                                 day:dirInfo.day
                                                                hour:dirInfo.hour
                                                                 min:dirInfo.min
                                                                 sec:dirInfo.sec
                                                                size:dirInfo.size
                                                                attr:dirInfo.attr
                                                                 d_f:[NSString stringWithUTF8String:dirInfo.d_f]
                                                             comment:[NSString stringWithUTF8String:dirInfo.comment]
                                                              o_time:[NSString stringWithUTF8String:dirInfo.o_time]];
            
            [*dicArray addObject:info];
        }
        delete[] pout;
}

/**
 Uploading NC program on thread.
 
 @param progName [in] NC program name
 @param progData [out] Specify the pointer to the buffer to be read.
 */
-(void) uploadNCProgramOnThread:(NSString *)progName progData:(NSString *__strong *)progData {
    
    SEL selc = @selector(uploadNCProgram:progData:);
    
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:selc];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&progName atIndex:2];
    [anInvocation setArgument:&progData atIndex:3];
    [anInvocation setSelector:selc];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Uploading NC program.
 
 @param progName [in] NC program name
 @param progData [out] Specify the pointer to the buffer to be read.
 */
-(void)uploadNCProgram:(NSString *)progName progData:(NSString *__strong *)progData {
    char buf[BUFF_SIZE + 1] = {0};
    LONG len ;
    short	type = 0;
    
    *progData = [NSMutableString new];
    retVal = cnc_upstart4( handle, type, const_cast<char *>([progName UTF8String])); // Read from current folder.
    if (EW_OK != retVal) {
        [self getDetailErr];
        return;
    }
    do {
        len = BUFF_SIZE ;
        retVal = cnc_upload4( handle, &len, buf ) ;
        if ( EW_BUFFER == retVal ) {
            continue ;
        }
        if ( EW_OK == retVal ) {
            buf[len] = '\0';
            [((NSMutableString *)*progData) appendString:[NSString stringWithUTF8String:buf]];
        }
        if ( buf[len-1] == '%' ) {
            break ;
        }
    } while (( retVal == EW_OK ) || ( retVal == EW_BUFFER ));
    
    retVal = cnc_upend4( handle ) ;
    if (EW_OK != retVal) {
        [self getDetailErr];
    }
}

/**
 Downloading NC program on thread.
 
 @param dir [in] Specify the CNC's folder
 @param progName [in] Specify the NC program name
 @param progData [out] Specify the NC program data.
 */
-(void)downloadNCProgramOnThread:(NSString *)dir progName:(NSString *)progName progData:(NSString *)progData {
    
    SEL selc = @selector(downloadNCProgram:progName:progData:);
    
    NSMethodSignature *aSignature = [[self class] instanceMethodSignatureForSelector:selc];
    NSInvocation *anInvocation = [NSInvocation invocationWithMethodSignature:aSignature];
    [anInvocation setTarget:self];
    [anInvocation setArgument:&dir atIndex:2];
    [anInvocation setArgument:&progName atIndex:3];
    [anInvocation setArgument:&progData atIndex:4];
    [anInvocation setSelector:selc];
    
    [self performSelector:@selector(performInvocation:) onThread:self.thread withObject:anInvocation waitUntilDone:YES];
}

/**
 Downloading NC program.
 
 @param dir [in] Specify the CNC's folder
 @param progName [in] Specify the NC program name.
 @param progData [out] Specify the NC program data.
 */
-(void)downloadNCProgram:(NSString *)dir progName:(NSString *)progName progData:(NSString *)progData {
    short type = 0;
    retVal = cnc_dwnstart4(handle, type, const_cast<char *>([dir UTF8String]));
    if (EW_OK != retVal) {
        // Download start failure.
        [self getDetailErr];
        NSLog(@"%s cnc_dwnstart4 error:%u", __func__ ,retVal);
        return;
    }
    char * prg = const_cast<char *>([progData UTF8String]);
    LONG n = 0;
    LONG len = static_cast<LONG>(strlen(prg));
    while (0 < len) {
        n = len;
        retVal = cnc_download4(handle, &n, prg);
        if ( retVal == EW_BUFFER ) {
            continue ;
        }
        if ( retVal == EW_OK ) {
            prg += n ;
            len -= n ;
        } else {
            break ;
        }
    }
    
    retVal = cnc_dwnend4(handle);
    if (EW_OK != retVal) {
        // Download failed.
        [self getDetailErr];
        NSLog(@"%s cnc_dwnend4 error:%u", __func__ ,retVal);
        return;
    }
}
@end
