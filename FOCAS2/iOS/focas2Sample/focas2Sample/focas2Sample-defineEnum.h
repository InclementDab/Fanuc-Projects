//
//  focas2Sample-defineEnum.h
//  focas2Sample
//

#ifndef focas2Sample_defineEnum_h
#define focas2Sample_defineEnum_h

#pragma mark - ENUM
// FUNCTION_ENUM
typedef NS_ENUM(NSUInteger, NCFUNC_ENUM) {
    NCConnection = 0,
    GetNCState,
    GetNCProgram,
    SetNCProgram
};

// system
typedef NS_ENUM(NSUInteger, NC_SYSTEM) {
    SYSTEM_NONE = 0,
    SYSTEM_M = 0x204D,
    SYSTEM_T = 0x2054,
    SYSTEM_W = 0x2057
};

#pragma mark - ODBST2

// aut
typedef NS_ENUM(short, AUT_STATUS) {
    AUT_MDI = 0,
    AUT_MEMory,
    AUT_Other,
    AUT_EDIT,
    AUT_HaNDle,
    AUT_JOG,
    AUT_Teach_in_JOG,
    AUT_Teach_in_HaNDle,
    AUT_INCfeed,
    AUT_REFerence,
    AUT_ReMoTe
};

// emergency
typedef NS_ENUM(short, EMERGENCY_STATUS) {
    EMERGENCY_RELEASE = 0,
    EMERGENCY_EMerGency,
    EMERGENCY_ReSET,
    EMERGENCY_WAIT
};

// alarm
typedef NS_ENUM(short, ALARM_STATUS){
    ALARM_Other,
    ALARM_ALarM,
    ALARM_BATtery_low,
    ALARM_FAN,
    ALARM_PS_Warning,
    ALARM_FSsB_warning,
    ALARM_LeaKaGe_warning,
    ALARM_ENCCoder_warning,
    ALARM_PMC_alarm
};

typedef NS_ENUM(short, ALM_TYPE) {
    ALM_TYPE_SW = 0,
    ALM_TYPE_PW,
    ALM_TYPE_IO,
    ALM_TYPE_PS,
    ALM_TYPE_OT,
    ALM_TYPE_OH,
    ALM_TYPE_SV,
    ALM_TYPE_SR,
    ALM_TYPE_MC,
    ALM_TYPE_SP,
    ALM_TYPE_DS,
    ALM_TYPE_IE,
    ALM_TYPE_BG,
    ALM_TYPE_SN,
    ALM_TYPE_EX = 15,
    ALM_TYPE_PC = 19
};

#pragma mark - ENUM to NSString
#define GetAUT_STATUSText(type) \
[@[@"MDI", @"MEMory", @"****", @"EDIT", @"HaNDle", @"JOG", @"Teach in JOG", @"Teach in HaNDle", @"INC･feed", @"REFerence", @"ReMoTe"] objectAtIndex:type]
#define GetEMERGENCY_STATUSText(type) \
[@[@"", @"EMerGency", @"ReSET", @"WAIT"] objectAtIndex: type]
#define GetALARM_STATUSText(type) \
[@[@"***", @"ALarM", @"BATtery low", @"FAN", @"PS Warning", @"FSsB warning", @"LeaKaGe warning", @"ENCoder warning", @"PMC alarm"] \
objectAtIndex: type]
#define GetWARNING_STATUSText(type) \
[@[@"", @"WaRNing"] objectAtIndex : type]

#define GetAlmTypeText(type) [@[ \
@"SW",@"PW",@"IO",@"PS",@"OT",@"OH",@"SV",@"SR",@"MC",@"SP", \
@"DS",@"IE",@"BG",@"SN",@"",  @"EX",@"",  @"",  @"",  @"PC", \
] objectAtIndex : type]

#endif /* focas2Sample_defineEnum_h */
