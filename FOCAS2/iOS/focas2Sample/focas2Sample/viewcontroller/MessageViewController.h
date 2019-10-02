//
//  MessageViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface MessageViewController : UIViewController
@property (nonatomic, copy) NSIndexPath * selectedPath;

@property (weak, nonatomic) IBOutlet UILabel *lblNCName;
@property (weak, nonatomic) IBOutlet UITextView *almmsgTextView;
@property (weak, nonatomic) IBOutlet UITextView *opemsgTextView;
@property (weak, nonatomic) IBOutlet UIView *statusView;
@property (weak, nonatomic) IBOutlet UILabel *lblCollecting;
@end
