//
//  NCStatusConverter.h
//  focas2Sample
//

#import <Foundation/Foundation.h>

#import <fwlibios/fwlibios.h>
#import "fwlib64.h"

@interface NCStatusConverter : NSObject
@property (nonatomic, copy, readonly) NSString *hdck_status;
@property (nonatomic, copy, readonly) NSString *tmmode_status;
@property (nonatomic, copy, readonly) NSString *aut_status;
@property (nonatomic, copy, readonly) NSString *run_status;
@property (nonatomic, copy, readonly) NSString *motion_status;
@property (nonatomic, copy, readonly) NSString *mstb_status;
@property (nonatomic, copy, readonly) NSString *emergency_status;
@property (nonatomic, copy, readonly) NSString *alarm_status;
@property (nonatomic, copy, readonly) NSString *edit_status;
@property (nonatomic, copy, readonly) NSString *warning_status;
@property (nonatomic, copy, readonly) NSString *o3dchk_status;
@property (nonatomic, copy, readonly) NSString *ext_opt_status;
@property (nonatomic, copy, readonly) NSString *restart_status;

-(id)initWithStatInfo:(ODBST2)statinfo system:(NC_SYSTEM)sys;
@end
