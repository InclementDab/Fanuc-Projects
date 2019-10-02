//
//  SaveNCProgramViewController.h
//  focas2Sample
//


#import <UIKit/UIKit.h>

/**
 Save NC program view controller class
 */
@interface SaveNCProgramViewController : UIViewController<UITableViewDelegate, UITableViewDataSource>
@property (weak, nonatomic) IBOutlet UILabel *lblCurDir;
@property (weak, nonatomic) IBOutlet UITableView *tableview;

- (IBAction)makeDir:(id)sender;
- (IBAction)clickSave:(id)sender;
@end
