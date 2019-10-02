//
//  EdittableNCProgramDetialViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface EdittableNCProgramDetialViewController : UIViewController
@property (weak, nonatomic) IBOutlet UITextView *textViewNCProgram;
@property (weak, nonatomic) IBOutlet UILabel *lblProgname;
@property BOOL isNoAppear;
@end
