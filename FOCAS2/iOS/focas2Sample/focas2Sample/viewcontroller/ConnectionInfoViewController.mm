//
//  ConnectionInfoViewController.m
//  focas2Sample
//

#import "ConnectionInfoViewController.h"
#import "NCConnectManager.h"
#import "CreateQRCodeViewController.h"
#import "ReadQRCodeViewController.h"

@interface ConnectionInfoViewController ()
@end

@implementation ConnectionInfoViewController

static NSString *const QRCODE_STRING_FORMAT = @"name=%@:ipa=%@:port=%d:timeout=%d";

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    if (self.isSelected) {
        // edit
        self.ncNameTextField.text = self.name;
        self.ncNameTextField.enabled = NO;
        self.ipaddrTextField.text = self.ipa;
        self.portnumTextField.text = [NSString stringWithFormat:@"%d", self.port];
        self.timeoutTextField.text = [NSString stringWithFormat:@"%d", self.timeout];
        [self.addButton setTitle:NSLocalizedString(@"edit", nil) forState:UIControlStateNormal];
        
        self.qrcodeButton.hidden = YES;
        self.qrcodeButton.enabled = NO;
        
        [self createQRCode];
        
    } else {
        // add
        [self.addButton setTitle:NSLocalizedString(@"add", nil) forState:UIControlStateNormal];
        
        self.imageView.hidden = YES;
    }
}

/**
 Create QRCode
 */
-(void)createQRCode {
    CIFilter *ciFilter = [CIFilter filterWithName:@"CIQRCodeGenerator"];
    [ciFilter setDefaults];
    
    // Data setting.
    NSString *qrStr = [NSString stringWithFormat:QRCODE_STRING_FORMAT, self.name,self.ipa,self.port,self.timeout];
    NSData *data = [qrStr dataUsingEncoding:NSUTF8StringEncoding];
    [ciFilter setValue:data forKey:@"inputMessage"];
    
    // Setting the error correction level.
    [ciFilter setValue:@"M" forKey:@"inputCorrectionLevel"];
    CIImage * img = [ciFilter outputImage];
    
    // New image size.
    CGSize newSize = self.imageView.frame.size;
    
    // Calculating the ratio of the size of the source image and the new size.
    CGRect imageRect = [img extent];
    CGPoint scale = CGPointMake(newSize.width/imageRect.size.width,
                                newSize.height/imageRect.size.height);
    
    // Crop after resizing with AffineTransform.
    CIImage *filteredImage = [img imageByApplyingTransform:CGAffineTransformMakeScale(scale.x,scale.y)];
    filteredImage = [filteredImage imageByCroppingToRect:CGRectMake(0, 0, newSize.width,newSize.height)];
    
    // Convert to UIImage.
    CIContext *ciContext = [CIContext contextWithOptions:[NSDictionary dictionaryWithObject:[NSNumber numberWithBool:NO]
                                                                                     forKey:kCIContextUseSoftwareRenderer]];
    
    CGImageRef imageRef = [ciContext createCGImage:filteredImage fromRect:[filteredImage extent]];
    UIImage *outputImage  = [UIImage imageWithCGImage:imageRef scale:1.0f orientation:UIImageOrientationUp];
    CGImageRelease(imageRef);
    
    self.imageView.contentMode = UIViewContentModeRedraw;
    self.imageView.image = outputImage;
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 viewWillLayoutSubviews event processing method
 */
-(void)viewWillLayoutSubviews {
    [self updateViewConstraints];
    
    [super viewWillLayoutSubviews];
}

/**
 updateViewConstraints event processing method
 */
-(void)updateViewConstraints {
    self.addButtonLeadingQRCodeButton.active = !(self.isSelected);
    self.addButtonLeadingSuperView.active = self.isSelected;
    
    self.addButtonSizeConstaint.active = !(self.isSelected);
    [super updateViewConstraints];
}

/**
 Unwind Segue - view closing event processing
 
 @param segue [in] pointer to UIStoryboadSegue
 */
-(IBAction)unwindConnectionInfo:(UIStoryboardSegue *)segue {
    UIViewController *uv = [segue sourceViewController];
    if ([uv isMemberOfClass:[ReadQRCodeViewController class]]) {
        ReadQRCodeViewController *rdqrvc = static_cast<ReadQRCodeViewController *>(uv);
        if (rdqrvc.ncName && rdqrvc.ipAddr && rdqrvc.portNoStr && rdqrvc.timeoutValStr) {
            self.ncNameTextField.text = rdqrvc.ncName;
            self.ipaddrTextField.text = rdqrvc.ipAddr;
            self.portnumTextField.text = rdqrvc.portNoStr;
            self.timeoutTextField.text = rdqrvc.timeoutValStr;
        }
    }
}

/**
 Processing at screen transition by storyboard
 
 @param segue [in] pointer to UIStoryboadSegue
 @param sender [in] pointer to sender
 */
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    UIViewController *uv = [segue destinationViewController];
    if ([uv isMemberOfClass:[CreateQRCodeViewController class]]) {
        CreateQRCodeViewController *cqrvc = static_cast<CreateQRCodeViewController *>(uv);
        cqrvc.name = self.name;
        cqrvc.ipa = self.ipa;
        cqrvc.port = self.port;
        cqrvc.timeout = self.timeout;
    }
}

/**
 add button tap event processing method
 
 @param sender [in] pointer to sender
 */
- (IBAction)addButtonTap:(id)sender {
    // Check each content.
    if (![self validCheckNCConnection]) {
        return;
    }
    
    // Add to connectionManager.
    NCConnectionSetting *settings = [[NCConnectionSetting alloc] initWithNCNameIpAddrPortNoTimeoutVal:self.name ipa:self.ipa port:self.port timeout:self.timeout];
    
    if (!self.isSelected && [[NCConnectManager sharedManager] isContainsNCSetting:self.name]) {
        // Raise alert for Edit instead of Add.
        UIAlertController* alert = [UIAlertController alertControllerWithTitle:nil
                                                                       message:NSLocalizedString(@"It has already registered same NC name settings.\nWould you like to reregister the setteings?", nil)
                                                                preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *okAction = [UIAlertAction actionWithTitle:NSLocalizedString(@"OK", nil)
                                                           style:UIAlertActionStyleDefault
                                                         handler:^(UIAlertAction * action){
                                                             [self addConnectNCInfo:settings];
                                                         }];
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:NSLocalizedString(@"Cancel", nil)
                                                               style:UIAlertActionStyleCancel
                                                             handler:^(UIAlertAction *action) {
                                                             }];
        [alert addAction:okAction];
        [alert addAction:cancelAction];
        [self presentViewController:alert animated:YES completion:nil];
        
        return;
    }
    
    [self addConnectNCInfo:settings];
}

/**
 Add NCConntionSetting information
 
 @param setting [in] pointer to NCConnectSetting information
 */
-(void)addConnectNCInfo:(NCConnectionSetting *)setting {
    [[NCConnectManager sharedManager] addConnectNCInfo:setting];
    // Return to the previous screen.
    [self dismissViewControllerAnimated:NO completion:nil];
}

// Set value check.
/**
 validate NCConnectionSetting information
 
 @return YES - OK / NO - NG
 */
-(BOOL)validCheckNCConnection {
    BOOL ret = YES;
    
    NCConnectManager *cm = [NCConnectManager sharedManager];
    
    // Check connection name.
    ret = [cm validateNCName:self.ncNameTextField.text];
    if (!ret) {
        [self showAlert:self.lblNCName.text textField:self.ncNameTextField];
        return ret;
    }
    self.name = self.ncNameTextField.text;
    // Check ipaddress.
    ret = [cm validateIPAddressCheck:self.ipaddrTextField.text];
    if (!ret) {
        [self showAlert:self.lblIPAddress.text textField:self.ipaddrTextField];
        return ret;
    }
    self.ipa = self.ipaddrTextField.text;
    // Check port number.
    ret = [cm validatePortNumOrTimeoutValCheck:self.portnumTextField.text];
    if (!ret) {
        [self showAlert:self.lblPortNum.text textField:self.portnumTextField];
        return ret;
    }
    self.port = static_cast<ushort>([self.portnumTextField.text intValue]);
    // Checking the timeout value.
    ret = [cm validatePortNumOrTimeoutValCheck:self.timeoutTextField.text];
    if (!ret) {
        [self showAlert:self.lblTimeout.text textField:self.timeoutTextField];
        return ret;
    }
    self.timeout = [self.timeoutTextField.text intValue];
    
    return ret;
}

/**
 Diplay alert by validating NCConnectionSetting information
 */
-(void)showAlert:(NSString *)message textField:(UITextField *)textField {
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:NSLocalizedString(@"input error", nil)
                                                                   message:[NSString stringWithFormat:NSLocalizedString(@"is Invalid value", nil), message]
                                                            preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *okAction = [UIAlertAction actionWithTitle:NSLocalizedString(@"OK", nil)
                                                       style:UIAlertActionStyleDefault
                                                     handler:^(UIAlertAction * action){
                                                         [textField becomeFirstResponder];
                                                     }];
    [alert addAction:okAction];
    [self presentViewController:alert animated:YES completion:nil];
}

/**
 EndEditing
 */
- (IBAction)gesture:(id)sender {
    [self.view endEditing:YES];
}


@end
