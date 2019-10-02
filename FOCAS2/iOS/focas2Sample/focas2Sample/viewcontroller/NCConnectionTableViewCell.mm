//
//  NCConnectionTableViewCell.mm
//  focas2Sample
//

#import "NCConnectionTableViewCell.h"

@interface NCConnectionTableViewCell()
@end

@implementation NCConnectionTableViewCell
/**
 Awake From Nib
 */
- (void)awakeFromNib {
    [super awakeFromNib];
    self.lblAlmStatus.text = nil;
    self.lblEmergencyStatus.text = nil;
    self.lblMode.text = nil;
}

/**
 Set selected
 */
- (void)setSelected:(BOOL)selected animated:(BOOL)animated {
    [super setSelected:selected animated:animated];
}

/**
 Set connection name
 */
-(void) setConnectionName:(NSString *)name {
    self.lblConnectName.text = name;
    self.imageView.image = [UIImage imageNamed:@"error-icon_32x32"];
    self.imageView.hidden = YES;
}

/**
 Set status erase
 */
-(void)setStatusErase {
    self.lblEmergencyStatus.text = nil;
    self.lblAlmStatus.text = nil;
    self.lblMode.text = nil;
}

/**
 Set status
 */
-(void) setStatus:(EMERGENCY_STATUS) emergency alarm:(ALARM_STATUS)alarm aut:(AUT_STATUS)aut  {
    [self setEmergencyText:emergency];
    [self setAlermText:alarm];
    [self setModeText:aut];
}

/**
 Set emergency text
 */
-(void) setEmergencyText:(EMERGENCY_STATUS)status {
    self.lblEmergencyStatus.text = [self emergencyText:status];
    if (status == EMERGENCY_RELEASE) {
        self.lblEmergencyStatus.textColor = [UIColor blackColor];
    } else {
        self.lblEmergencyStatus.textColor = [UIColor redColor];
    }
}

/**
 Set alerm text
 */
-(void) setAlermText:(ALARM_STATUS)status {
    self.lblAlmStatus.text = [self ereaseLowercaseLetterCharacter:GetALARM_STATUSText(status)];
    if (status == ALARM_Other) {
        self.lblAlmStatus.textColor = [UIColor blackColor];
    } else {
        self.lblAlmStatus.textColor = [UIColor redColor];
    }
}

/**
 Set mode text
 */
-(void) setModeText:(AUT_STATUS)status {
    self.lblMode.text = [self ereaseLowercaseLetterCharacter:GetAUT_STATUSText(status)];
}

/**
 Emergency text
 @return nil
 */
-(NSString *)emergencyText:(EMERGENCY_STATUS)info {
    switch (info) {
        case EMERGENCY_RELEASE:
            return NSLocalizedString(@"*** ***", nil);
        case EMERGENCY_EMerGency:
        case EMERGENCY_ReSET:
        case EMERGENCY_WAIT:
            return [self ereaseLowercaseLetterCharacter:GetEMERGENCY_STATUSText(info)];
        default:
            return nil;
    }
}

/**
 Erease lower case letter character
 @return original / retStr
 */
-(NSString *)ereaseLowercaseLetterCharacter:(NSString *)original {
    if (!original) {
        return original;
    }
    NSScanner *scanner = [NSScanner scannerWithString:original];
    // Set a character set to ignore (lower case alphabet).
    [scanner setCharactersToBeSkipped:[NSCharacterSet lowercaseLetterCharacterSet]];
    NSMutableString * retStr = [NSMutableString new];
    while (!scanner.isAtEnd){
        NSString *s;
        if ([scanner scanCharactersFromSet:[NSCharacterSet uppercaseLetterCharacterSet] intoString:&s]) {
            [retStr appendString:s];
        } else {
            break;
        }
    }
    
    if (0 == [retStr length]) {
        return original;
    }
    return retStr;
}


@end
