//
//  NCStatusConverter.mm
//  focas2Sample
//

#import "NCStatusConverter.h"

@interface NCStatusConverter()
@property (nonatomic, copy, readwrite) NSString *hdck_status;
@property (nonatomic, copy, readwrite) NSString *tmmode_status;
@property (nonatomic, copy, readwrite) NSString *aut_status;
@property (nonatomic, copy, readwrite) NSString *run_status;
@property (nonatomic, copy, readwrite) NSString *motion_status;
@property (nonatomic, copy, readwrite) NSString *mstb_status;
@property (nonatomic, copy, readwrite) NSString *emergency_status;
@property (nonatomic, copy, readwrite) NSString *alarm_status;
@property (nonatomic, copy, readwrite) NSString *edit_status;
@property (nonatomic, copy, readwrite) NSString *warning_status;
@property (nonatomic, copy, readwrite) NSString *o3dchk_status;
@property (nonatomic, copy, readwrite) NSString *ext_opt_status;
@property (nonatomic, copy, readwrite) NSString *restart_status;
@end

@implementation NCStatusConverter

/**
 initilize with stat information
 
 @param statinfo
 @param sys
 @return pointer of self
 */
-(id)initWithStatInfo:(ODBST2)statinfo system:(NC_SYSTEM)sys {
    self = [super init];
    
    if (self) {
        self.hdck_status = NSLocalizedString(GetHDCK_STATUSText(statinfo.hdck), nil);
        self.tmmode_status = NSLocalizedString(GetTMMODEText(statinfo.tmmode), nil);
        self.aut_status = GetAUT_STATUSText(statinfo.aut);
        self.run_status = GetRUN_STATUSText(statinfo.run);
        self.motion_status = GetMOTHIN_STATUSText(statinfo.motion);
        self.mstb_status = GetMSTB_STATUSText(statinfo.mstb);
        self.emergency_status = GetEMERGENCY_STATUSText(statinfo.emergency);
        self.alarm_status = GetALARM_STATUSText(statinfo.alarm);
        if (SYSTEM_M == sys) {
            self.edit_status = GetEDIT_STATUS_MText(statinfo.edit);
        } else if (SYSTEM_T == sys) {
            self.edit_status = GetEDIT_STATUS_TText(statinfo.edit);
        }
        self.warning_status = GetWARNING_STATUSText(statinfo.warning);
        self.o3dchk_status = NSLocalizedString(GetO3DCHK_STATUSText(statinfo.o3dchk), nil);
        self.ext_opt_status = NSLocalizedString(GetEXT_OPT_STATUSText(statinfo.ext_opt), nil);
        self.restart_status = NSLocalizedString(GetRESTART_STATUSText(statinfo.restart), nil);
    }
    return self;
}

@end
