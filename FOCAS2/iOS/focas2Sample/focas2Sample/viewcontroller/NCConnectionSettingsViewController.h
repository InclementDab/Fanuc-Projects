//
//  NCConnectionSettingsViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

/**
 Connection NC list screen view controller class
 */
@interface NCConnectionSettingsViewController : UIViewController<UITableViewDelegate, UITableViewDataSource>
@property (weak, nonatomic) IBOutlet UITableView *connectNCTableView;
@property (weak, nonatomic) IBOutlet UIButton *addButton;
- (IBAction)btnReturnTouchUp:(id)sender;

@property NCFUNC_ENUM selectedFunc;
@end
