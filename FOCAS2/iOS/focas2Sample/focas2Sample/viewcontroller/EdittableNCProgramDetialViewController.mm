//
//  EdittableNCProgramDetialViewController.m
//  focas2Sample
//

#import "EdittableNCProgramDetialViewController.h"
#import "FileSaveManager.h"
#import "NCConnectionSettingsViewController.h"

@interface EdittableNCProgramDetialViewController ()

@end

@implementation EdittableNCProgramDetialViewController

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    [self.textViewNCProgram.layer setBorderWidth:1];
    [self.textViewNCProgram.layer setBorderColor:[[UIColor blackColor] CGColor]];
    [self.textViewNCProgram.layer setCornerRadius:8];
    
    FileSaveManager *fsManager = [FileSaveManager sharedManager];
    self.lblProgname.text = [fsManager getProgName];
    
    self.textViewNCProgram.text = [fsManager getProgDataFromFile];
    
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
    self.view.hidden = self.isNoAppear;
    if (!self.isNoAppear) {
        [super viewWillAppear:animated];
    }
}


#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
/**
 Processing at screen transition by storyboard
 
 @param segue [in] pointer to UIStoryboadSegue
 @param sender [in] pointer to sender
 */
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    UIViewController *vc = [segue destinationViewController];
    if (![vc isMemberOfClass:[NCConnectionSettingsViewController class]]) {
        return;
    }
    
    NCConnectionSettingsViewController *dstVC = static_cast<NCConnectionSettingsViewController *>(vc);
    dstVC.selectedFunc = SetNCProgram;
}

@end
