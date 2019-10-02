//
//  CreateQRCodeViewController.m
//  focas2Sample
//

#import "CreateQRCodeViewController.h"

@interface CreateQRCodeViewController ()

@end

@implementation CreateQRCodeViewController

static NSString *const QRCODE_STRING_FORMAT = @"name=%@:ipa=%@:port=%d:timeout=%d";

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    [self createQRCode];
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 Create QRCode image
 */
-(void)createQRCode {
    CIFilter *ciFilter = [CIFilter filterWithName:@"CIQRCodeGenerator"];
    [ciFilter setDefaults];
    
    // Data setting.
    NSString *qrStr = [NSString stringWithFormat:QRCODE_STRING_FORMAT,
                       self.name,self.ipa,self.port,self.timeout];
    NSData *data = [qrStr dataUsingEncoding:NSUTF8StringEncoding];
    [ciFilter setValue:data forKey:@"inputMessage"];
    
    // Setting the error correction level.
    [ciFilter setValue:@"M" forKey:@"inputCorrectionLevel"];
    CIImage * img = [ciFilter outputImage];
    
    // New image size.
    CGSize newSize = self.qrcodeImageView.frame.size;
    
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
    
    self.qrcodeImageView.contentMode = UIViewContentModeRedraw;
    self.qrcodeImageView.image = outputImage;
}

@end
