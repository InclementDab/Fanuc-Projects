//
//  NCProgramViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

/**
 NC Program screen view controller class
 */
@interface NCProgramViewController : UIViewController<UITableViewDelegate, UITableViewDataSource>
@property (weak, nonatomic) IBOutlet UITableView *tableview;
@property (weak, nonatomic) IBOutlet UILabel *lblncName;
@property (weak, nonatomic) IBOutlet UILabel *lblcurdir;
@property (weak, nonatomic) IBOutlet UIButton *btnRegist;
- (IBAction)touchBtnRegist:(id)sender;

@property NSUInteger selectedPathIndex;
@property NCFUNC_ENUM selectedFunc;

@end
