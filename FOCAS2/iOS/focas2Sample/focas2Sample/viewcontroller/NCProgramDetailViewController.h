//
//  NCProgramDetailViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

/**
 NC program detial view controller class
 */
@interface NCProgramDetailViewController : UIViewController
@property (weak, nonatomic) IBOutlet UILabel *lblNCName;
@property (weak, nonatomic) IBOutlet UILabel *lblCurPath;
@property (weak, nonatomic) IBOutlet UITextView *NCProgramTextView;

@property NSUInteger selectedPathIndex;
@property NSString* selectedProgramFileName;
@property BOOL isNoAppear;
@end
