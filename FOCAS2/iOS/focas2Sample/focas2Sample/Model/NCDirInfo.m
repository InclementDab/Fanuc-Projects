//
//  NCDirInfo.m
//  focas2Sample
//

#import "NCDirInfo.h"

@interface NCDirInfo()
@property (nonatomic, readwrite) short data_kind;
@property (nonatomic, readwrite) short year;
@property (nonatomic, readwrite) short mon;
@property (nonatomic, readwrite) short day;
@property (nonatomic, readwrite) short hour;
@property (nonatomic, readwrite) short min;
@property (nonatomic, readwrite) short sec;
@property (nonatomic, readwrite) LONG size;
@property (nonatomic, readwrite) ULONG attr;
@property (nonatomic, readwrite, copy) NSString* d_f;
@property (nonatomic, readwrite, copy) NSString* comment;
@property (nonatomic, readwrite, copy) NSString* o_time;
@end

@implementation NCDirInfo

/**
 initilize with upper folder
 
 @return pointer to self
 */
-(id)initWithUpFolder {
    return [self initWithAllProperty:0
                                year:0
                                 mon:0
                                 day:0
                                hour:0
                                 min:0
                                 sec:0
                                size:0
                                attr:0
                                 d_f:NSLocalizedString(@"upper folder", nil)
                             comment:nil
                              o_time:nil];
}

/**
 initilize with AllProperty
 
 @param data_kind [in] File data kind
 @param year [in] Last edited date (year)
 @param mon [in] Last edited date (month)
 @param day [in] Last edited date (day)
 @param hour [in] Last edited time (hour)
 @param min [in] Last edited time (minute)
 @param sec [in] Last edited time (second)
 @param size [in] file size
 @param attr [in] attribute
 @param d_f [in] name string
 @param comment [in] comment
 @param o_time [in] process time stamp
 @return pointer to self
 */
-(id)initWithAllProperty:(short)data_kind year:(short)year mon:(short)mon day:(short)day hour:(short)hour min:(short)min sec:(short)sec size:(LONG)size attr:(ULONG)attr d_f:(NSString *)d_f comment:(NSString*)comment o_time:(NSString*)o_time {
    self = [super init];
    
    if (self) {
        self.data_kind = data_kind;
        self.year = year;
        self.mon = mon;
        self.day = day;
        self.hour = hour;
        self.min = min;
        self.sec = sec;
        self.size = size;
        self.attr = attr;
        self.d_f = d_f;
        self.comment = comment;
        self.o_time = o_time;
    }
    
    return self;
}
@end
