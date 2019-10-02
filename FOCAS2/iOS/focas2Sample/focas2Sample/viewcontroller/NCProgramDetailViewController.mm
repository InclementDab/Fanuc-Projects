//
//  NCProgramDetailViewController.m
//  focas2Sample
//

#import "NCProgramDetailViewController.h"
#import "NCConnectManager.h"
#import "NCDirectoryManager.h"
#import "NCGetProgram.h"
#import "FileSaveManager.h"

@interface NCProgramDetailViewController ()

@end

@implementation NCProgramDetailViewController

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    [self.NCProgramTextView.layer setBorderWidth:1];
    [self.NCProgramTextView.layer setBorderColor:[[UIColor blackColor] CGColor]];
    [self.NCProgramTextView.layer setCornerRadius:8];
    
    NCConnectionSetting *set = [[NCConnectManager sharedManager] getConnectNCInfoFromIndex:self.selectedPathIndex];
    self.lblNCName.text = set.ncName;
    
    NCDirectoryManager *dirmanager = [NCDirectoryManager sharedManager];
    self.lblCurPath.text = [NSString stringWithFormat:@"%@%@", dirmanager.dir, self.selectedProgramFileName];
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
-(IBAction)unwindNCProgramDetail:(UIStoryboardSegue *)segue {
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

/**
 viewDidAppear event processing method
 
 @param animated [in] If YES, the view was added to the window using an animation.
 */
-(void)viewDidAppear:(BOOL)animated {
    if (!self.isNoAppear) {
        [super viewDidAppear:animated];
        [self updateNCProgram];
    }
}

/**
 upload NC program from CNC and display NC program data
 */
-(void)updateNCProgram {
    @autoreleasepool {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{
            NSString *progData = nil;
            NCGetProgram *dirGetter = [[NCGetProgram alloc] initWithSelectedIndexPath:self.selectedPathIndex];
            
            short ret = [dirGetter uploadNCProgram:self.lblCurPath.text progData:&progData];
            if (ret != EW_OK) {
                return;
            }
            
            [[FileSaveManager sharedManager] setPrognameAndProgData:self.selectedProgramFileName progData:progData];
            dispatch_async(dispatch_get_main_queue(), ^{
                self.NCProgramTextView.text = progData;
            });
        });
    }
}

@end
