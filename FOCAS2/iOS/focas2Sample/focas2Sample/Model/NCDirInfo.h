//
//  NCDirInfo.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

/**
 NCDir information class - convert from ODBPDFDIR structure
 */
@interface NCDirInfo : NSObject
@property (nonatomic, readonly) short data_kind;
@property (nonatomic, readonly) short year;
@property (nonatomic, readonly) short mon;
@property (nonatomic, readonly) short day;
@property (nonatomic, readonly) short hour;
@property (nonatomic, readonly) short min;
@property (nonatomic, readonly) short sec;
@property (nonatomic, readonly) LONG size;
@property (nonatomic, readonly) ULONG attr;
@property (nonatomic, readonly, copy) NSString* d_f;
@property (nonatomic, readonly, copy) NSString* comment;
@property (nonatomic, readonly, copy) NSString* o_time;

-(id)initWithUpFolder;

-(id)initWithAllProperty:(short)data_kind year:(short)year mon:(short)mon day:(short)day hour:(short)hour min:(short)min sec:(short)sec size:(LONG)size attr:(ULONG)attr d_f:(NSString *)d_f comment:(NSString*)comment o_time:(NSString*)o_time;


@end
