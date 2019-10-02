//
//  ConnectionInfoViewController.h
//  focas2Sample
//

#import <UIKit/UIKit.h>

/**
 Connection NC information screen view controller class
 */
@interface ConnectionInfoViewController : UIViewController
@property (weak, nonatomic) IBOutlet UITextField *ncNameTextField;
@property (weak, nonatomic) IBOutlet UITextField *ipaddrTextField;
@property (weak, nonatomic) IBOutlet UITextField *portnumTextField;
@property (weak, nonatomic) IBOutlet UITextField *timeoutTextField;
@property (weak, nonatomic) IBOutlet UIButton *addButton;
@property (weak, nonatomic) IBOutlet UILabel *lblNCName;
@property (weak, nonatomic) IBOutlet UILabel *lblIPAddress;
@property (weak, nonatomic) IBOutlet UILabel *lblPortNum;
@property (weak, nonatomic) IBOutlet UILabel *lblTimeout;
@property (weak, nonatomic) IBOutlet UIImageView *imageView;
@property (weak, nonatomic) IBOutlet UIButton *qrcodeButton;

@property (weak, nonatomic) IBOutlet NSLayoutConstraint *addButtonSizeConstaint;
@property (weak, nonatomic) IBOutlet NSLayoutConstraint *addButtonLeadingQRCodeButton;
@property (weak, nonatomic) IBOutlet NSLayoutConstraint *addButtonLeadingSuperView;

@property NSString* name;
@property NSString* ipa;
@property ushort port;
@property LONG timeout;
@property BOOL isSelected;

- (IBAction)addButtonTap:(id)sender;
@end
