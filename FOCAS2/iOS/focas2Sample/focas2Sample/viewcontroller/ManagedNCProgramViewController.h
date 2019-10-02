//
//  ManagedNCProgramViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface ManagedNCProgramViewController : UIViewController<UITableViewDelegate, UITableViewDataSource>
@property (weak, nonatomic) IBOutlet UITableView *tableview;
@property (weak, nonatomic) IBOutlet UILabel *lblCurDir;

@end
