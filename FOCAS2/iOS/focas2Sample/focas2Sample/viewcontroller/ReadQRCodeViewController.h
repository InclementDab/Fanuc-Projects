//
//  ReadQRCodeViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface ReadQRCodeViewController : UIViewController

@property(nonatomic, copy, readonly) NSString* ncName;
@property(nonatomic, copy, readonly) NSString* ipAddr;
@property(nonatomic, copy, readonly) NSString* portNoStr;
@property(nonatomic, copy, readonly) NSString* timeoutValStr;

@property (weak, nonatomic) IBOutlet UIView *viewer;
@end
