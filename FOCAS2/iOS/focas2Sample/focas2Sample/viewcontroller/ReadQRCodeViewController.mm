//
//  ReadQRCodeViewController.m
//  focas2Sample
//

#import <AVFoundation/AVFoundation.h>
#import "ReadQRCodeViewController.h"

/**
 Video orientation from device orientation
 
 @return orientation
 */
static AVCaptureVideoOrientation videoOrientationFromDeviceOrientation(UIDeviceOrientation deviceOrientation) {
    AVCaptureVideoOrientation orientation;
    switch (deviceOrientation) {
        case UIDeviceOrientationUnknown:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationPortrait:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationPortraitUpsideDown:
            orientation = AVCaptureVideoOrientationPortraitUpsideDown;
            break;
        case UIDeviceOrientationLandscapeLeft:
            orientation = AVCaptureVideoOrientationLandscapeRight;
            break;
        case UIDeviceOrientationLandscapeRight:
            orientation = AVCaptureVideoOrientationLandscapeLeft;
            break;
        case UIDeviceOrientationFaceUp:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationFaceDown:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
    }
    return orientation;
}

@interface ReadQRCodeViewController () <AVCaptureMetadataOutputObjectsDelegate>

@property(nonatomic, copy, readwrite) NSString* ncName;
@property(nonatomic, copy, readwrite) NSString* ipAddr;
@property(nonatomic, copy, readwrite) NSString* portNoStr;
@property(nonatomic, copy, readwrite) NSString* timeoutValStr;

@property (strong, nonatomic) AVCaptureSession *session;
@property (strong, nonatomic) AVCaptureVideoPreviewLayer *previewLayer;

@property (strong, nonatomic) UIView *qrMarkView;
@end

@implementation ReadQRCodeViewController

static NSString *const QRCODE_ITEM_NAME         = @"name";
static NSString *const QRCODE_ITEM_IPADDRESS    = @"ipa";
static NSString *const QRCODE_ITEM_PORT         = @"port";
static NSString *const QRCODE_ITEM_TIMEOUT      = @"timeout";

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 viewWillAppear event processing method
 
 @param animated [in] If YES, the view is being added to the window using an animation.
 */
-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self checkCameraPrivacy];
}


/**
 viewWillDisappear event processing method
 
 @param animated [in] If YES, the disappearamce of the view is being animated.
 */
-(void)viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];
    [self teardownAVCapture];
}

/**
 Check access permission to camera
 */
-(void)checkCameraPrivacy {
    AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    
    switch (status) {
        case AVAuthorizationStatusAuthorized:
            // When the use of the camera is permitted in the privacy setting.
            [self setupAVCapture];
            break;
        case AVAuthorizationStatusDenied:
            // When the use of the camera is prohibited in the privacy setting.
            [self openSettingsURL];
            break;
        case AVAuthorizationStatusRestricted:
            // In the case of function restriction.
            break;
        case AVAuthorizationStatusNotDetermined:
            // A dialog prompting permission setting is displayed at initial startup.
            [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
                if (granted) {
                    // Processing when permitted.
                    dispatch_async(dispatch_get_main_queue(), ^{
                        [self setupAVCapture];
                    });
                } else {
                    // Processing when not permitted.
                    dispatch_async(dispatch_get_main_queue(), ^{
                        // Return unconditionally.
                        [self performSegueWithIdentifier:@"returnToConnectionInfo" sender:self];
                    });
                }
            }];
            break;
    }
}

/**
 Display setting screen
 */
-(void)openSettingsURL {
    UIAlertController* alert =
    [UIAlertController alertControllerWithTitle:NSLocalizedString(@"Access to the camera is not allowed", nil)
                                                                   message:nil
                                                            preferredStyle:UIAlertControllerStyleAlert];
    
    // cancel
    [alert addAction:
     [UIAlertAction actionWithTitle:NSLocalizedString(@"No Change", nil)
                              style:UIAlertActionStyleCancel handler:^(UIAlertAction *action) {
        [self performSegueWithIdentifier:@"returnToConnectionInfo" sender:self];
    }]];
    [alert addAction:
     [UIAlertAction actionWithTitle:NSLocalizedString(@"Setting Change", nil)
                              style:UIAlertActionStyleDefault
                            handler:^(UIAlertAction * action) {
                                NSURL *settingsURL = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
                                // Rebooting occurs when setting is changed on setting screen.
                                [[UIApplication sharedApplication] openURL:settingsURL];
                                
                                [self performSegueWithIdentifier:@"returnToConnectionInfo" sender:self];
                            }]];
    [self presentViewController:alert animated:YES completion:nil];
}

/**
 Setup for AV capture
 */
-(void) setupAVCapture {
    
    // Session generation.
    self.session = [AVCaptureSession new];
    [self.session setSessionPreset:AVCaptureSessionPreset640x480];
    
    // Input setting.
    NSError *err = nil;
    AVCaptureDevice *device = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    AVCaptureDeviceInput *deviceInput = [AVCaptureDeviceInput deviceInputWithDevice:device error:&err];
    if ([self.session canAddInput:deviceInput]) {
        [self.session addInput:deviceInput];
    }
    
    AVCaptureMetadataOutput *output = [AVCaptureMetadataOutput new];
    [output setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];
    [self.session addOutput:output];
    output.metadataObjectTypes = output.availableMetadataObjectTypes;
    
    // Screen display setting.
    self.previewLayer = [[AVCaptureVideoPreviewLayer alloc] initWithSession:self.session];
    [self.previewLayer setBackgroundColor:[[UIColor blackColor] CGColor]];
    [self.previewLayer setVideoGravity:AVLayerVideoGravityResizeAspect];
    
    // Correspond to rotation.
    if (self.previewLayer.connection.supportsVideoOrientation) {
        self.previewLayer.connection.videoOrientation = videoOrientationFromDeviceOrientation([UIDevice currentDevice].orientation);
    }
    
    // Size setting.
    CALayer* rootLayer = [self.viewer layer];
    [rootLayer setMasksToBounds:YES];
    [self.previewLayer setFrame:[rootLayer bounds]];
    [rootLayer addSublayer:self.previewLayer];
    
    // A red frame is displayed for the detected bar code.
    self.qrMarkView = [UIView new];
    self.qrMarkView.autoresizingMask = UIViewAutoresizingFlexibleTopMargin;
    self.qrMarkView.layer.borderWidth = 4;
    self.qrMarkView.layer.borderColor = [[UIColor redColor] CGColor];
    self.qrMarkView.frame = CGRectMake(0, 0, 0, 0);
    [self.viewer addSubview:self.qrMarkView];
    [self.viewer bringSubviewToFront:self.qrMarkView];
    
    [self.session startRunning];
}

/**
 Teardown AV Capture
 */
-(void)teardownAVCapture {
    [self.session stopRunning];
    
    for (AVCaptureOutput * output in self.session.outputs) {
        [self.session removeOutput:output];
    }
    
    for (AVCaptureInput * input in self.session.inputs) {
        [self.session removeInput:input];
    }
    
    self.previewLayer = nil;
    self.session = nil;
    
}

/**
 View Didlayouts subviews (Respond to changes in autolayout)
 */
-(void)viewDidLayoutSubviews {
    [super viewDidLayoutSubviews];
    
    CALayer* rootLayer = [self.viewer layer];
    [rootLayer setMasksToBounds:YES];
    [self.previewLayer setFrame:[rootLayer bounds]];
    
    if (self.previewLayer.connection.supportsVideoOrientation) {
        self.previewLayer.connection.videoOrientation = videoOrientationFromDeviceOrientation([UIDevice currentDevice].orientation);
    }
}

/**
 Reading capture
 */
-(void)captureOutput:(AVCaptureOutput *)captureOutput didOutputMetadataObjects:(NSArray *)metadataObjects fromConnection:(AVCaptureConnection *)connection {
    
    // Get character string in code.
    NSString *strValue =nil;
    
    for(AVMetadataObject *data in metadataObjects) {
        if (NSOrderedSame == [data.type compare:AVMetadataObjectTypeQRCode]) {
            
            AVMetadataMachineReadableCodeObject * barcord = (AVMetadataMachineReadableCodeObject *)[self.previewLayer transformedMetadataObjectForMetadataObject:data];
            self.qrMarkView.frame = barcord.bounds;
            
            // Get character string in code.
            strValue = [(AVMetadataMachineReadableCodeObject *)data stringValue];
            break;
        }
    }
    // Decryption processing.
    [self analyze:strValue];
    
    if (self.ncName && self.ipAddr && self.portNoStr && self.timeoutValStr) {
        [self performSegueWithIdentifier:@"returnToConnectionInfo" sender:self];
    }
}

/**
 Analysis of QR code string
 */
-(void)analyze:(NSString *)strVal {
    NSArray *arr = [strVal componentsSeparatedByString:@":"];
    
    [arr enumerateObjectsUsingBlock:^(NSString* strUnit, NSUInteger idx, BOOL * /*stop*//*not use*/){
        NSArray *settingArr = [strUnit componentsSeparatedByString:@"="];
        if (2 == [settingArr count]) {
            NSString* strItem = [settingArr objectAtIndex:0];
            NSString* strSettingsValue = [settingArr objectAtIndex:1];
            if (NSOrderedSame == [strItem compare:QRCODE_ITEM_NAME]) {
                self.ncName = strSettingsValue;
            } else if (NSOrderedSame == [strItem compare:QRCODE_ITEM_IPADDRESS]) {
                self.ipAddr = strSettingsValue;
            } else if (NSOrderedSame == [strItem compare:QRCODE_ITEM_PORT]) {
                self.portNoStr = strSettingsValue;
            } else if (NSOrderedSame == [strItem compare:QRCODE_ITEM_TIMEOUT]) {
                self.timeoutValStr = strSettingsValue;
            }
        }
    }];
}

@end
