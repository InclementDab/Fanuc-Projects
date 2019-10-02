//
//  NCStatusViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface NCStatusViewController : UIViewController
@property (nonatomic, copy) NSIndexPath* selectedPath;


@property (weak, nonatomic) IBOutlet UIView *viewGetStatus;
@property (weak, nonatomic) IBOutlet UILabel *lblNCName;
@property (weak, nonatomic) IBOutlet UILabel *lblStatus;

@property (weak, nonatomic) IBOutlet UILabel *lblhdck_val;
@property (weak, nonatomic) IBOutlet UILabel *lbltmmode_val;
@property (weak, nonatomic) IBOutlet UILabel *lblaut_val;
@property (weak, nonatomic) IBOutlet UILabel *lblrun_val;
@property (weak, nonatomic) IBOutlet UILabel *lblmotion_val;
@property (weak, nonatomic) IBOutlet UILabel *lblmstb_val;
@property (weak, nonatomic) IBOutlet UILabel *lblemergency_val;
@property (weak, nonatomic) IBOutlet UILabel *lblalarm_val;
@property (weak, nonatomic) IBOutlet UILabel *lbledit_val;
@property (weak, nonatomic) IBOutlet UILabel *lblwarning_val;
@property (weak, nonatomic) IBOutlet UILabel *lblo3dchk_val;
@property (weak, nonatomic) IBOutlet UILabel *lblext_opt_val;
@property (weak, nonatomic) IBOutlet UILabel *lblrestart_val;
@property (weak, nonatomic) IBOutlet UILabel *lblo3dchk;
@property (weak, nonatomic) IBOutlet UILabel *lblext_opt;
@property (weak, nonatomic) IBOutlet UILabel *lblrestart;

@end
