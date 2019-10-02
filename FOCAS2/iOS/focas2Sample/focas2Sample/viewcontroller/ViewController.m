//
//  ViewController.m
//  focas2Sample
//

#import "ViewController.h"
#import "NCConnectionSettingsViewController.h"

@interface ViewController ()

@end

@implementation ViewController

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 Unwind Segue - view closing event processing
 
 @param segue [in] pointer to UIStoryboadSegue
 */
-(IBAction)unwindToView:(UIStoryboardSegue *)segue {
}

/**
 Processing at screen transition by storyboard
 
 @param segue [in] pointer to UIStoryboadSegue
 @param sender [in] pointer to sender
 */
-(void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Teach the next ViewController what the identifier was clicked on.
    NSString * identifier = [segue identifier];
    UIViewController *uiView = [segue destinationViewController];
    NCConnectionSettingsViewController* vc;
    
    if ([uiView isMemberOfClass:[NCConnectionSettingsViewController class]]) {
        vc = (NCConnectionSettingsViewController*)uiView;
    } else {
        return;
    }
    
    if ([identifier isEqualToString:@"ConnectionSettings"]) {
        vc.selectedFunc = NCConnection;
    } else if([identifier isEqualToString:@"GetStatus"]) {
        vc.selectedFunc = GetNCState;
    } else if([identifier isEqualToString:@"GetNCProgram"]) {
        vc.selectedFunc = GetNCProgram;
    } else if([identifier isEqualToString:@"toNCConnectSetting"]) {
        vc.selectedFunc = SetNCProgram;
    }
}

@end
