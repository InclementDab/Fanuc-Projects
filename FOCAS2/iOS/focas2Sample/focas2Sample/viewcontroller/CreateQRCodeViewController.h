//
//  CreateQRCodeViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

@interface CreateQRCodeViewController : UIViewController
@property (weak, nonatomic) IBOutlet UIImageView *qrcodeImageView;

@property NSString* name;
@property NSString* ipa;
@property ushort port;
@property LONG timeout;

@end
