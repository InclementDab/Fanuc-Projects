//
//  NCConnectionTableViewCell.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface NCConnectionTableViewCell : UITableViewCell
@property (weak, nonatomic) IBOutlet UILabel *lblConnectName;
@property (weak, nonatomic) IBOutlet UILabel *lblEmergencyStatus;
@property (weak, nonatomic) IBOutlet UILabel *lblAlmStatus;
@property (weak, nonatomic) IBOutlet UILabel *lblMode;

-(void) setConnectionName:(NSString *)name;
-(void) setStatusErase;
-(void) setStatus:(EMERGENCY_STATUS) emergency alarm:(ALARM_STATUS)alarm aut:(AUT_STATUS)aut;
@end
