//
//  NCStatusViewController.m
//  focas2Sample
//

#import "NCStatusViewController.h"
#import "NCGetStatus.h"
#import "NCConnectManager.h"
#import "NCStatusConverter.h"

@interface NCStatusViewController () {
    UIActivityIndicatorView *indicator;
    NSTimer *tmgetStatus;
}
@end

@implementation NCStatusViewController
/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    indicator = [[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    indicator.frame = CGRectMake(0, 0, 50, 50);
    indicator.center = self.view.center;
    indicator.hidesWhenStopped = YES;
    
    self.lblStatus.text = NSLocalizedString(@"now collectiong", nil);
    self.viewGetStatus.hidden = NO;
    
    [self.view addSubview:indicator];
    self.lblNCName.text = [[NCConnectManager sharedManager] getConnectNCInfoFromIndex:(NSUInteger)self.selectedPath.row].ncName;
    
    // TODO : Fナック様内部検討結果次第
    self.lblo3dchk.hidden = YES;
    self.lblo3dchk_val.hidden = YES;
    self.lblext_opt.hidden = YES;
    self.lblext_opt_val.hidden = YES;
    self.lblrestart.hidden = YES;
    self.lblrestart_val.hidden = YES;
    
    [indicator startAnimating];
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
}

/**
 viewDidAppear event processing method
 
 @param animated [in] If YES, the view was added to the window using an animation.
 */
- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    [self getNCStatus];
}

/**
 viewWillDisappear event processing method
 
 @param animated [in] If YES, the disappearamce of the view is being animated.
 */
-(void)viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];
    if ([tmgetStatus isValid]) {
        // Stop if valid.
        [tmgetStatus invalidate];
    }
}

/**
 Get NC status
 */
-(void)getNCStatus {
    @autoreleasepool {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{
            NCGetStatus *ncs = [[NCGetStatus alloc] initWithSelectedIndexPath:(NSUInteger)self.selectedPath.row];
            ODBST2 statinfo = {0};
            ODBERR err = {0};
            NC_SYSTEM sys = SYSTEM_NONE;
            
            dispatch_async(dispatch_get_main_queue(), ^{
                /*if (ret != EW_OK) {
                    // Acquisition failure, detailed error information display.
                    self.lblStatus.text = [NSString stringWithFormat:NSLocalizedString(@"collect failed", nil), ret, err.err_no, err.err_no];
                } else {
                    // Displaying the acquisition result.
                    [self setStatusInfo:statinfo system:sys];
                    self.viewGetStatus.hidden = YES;
                }*/
                [indicator stopAnimating];
            });
        });
    }
}

/**
 Set status information
 
 @param statinfo
 @param sys
 */
-(void)setStatusInfo:(ODBST2)statinfo system:(NC_SYSTEM)sys {
    NCStatusConverter *conv = [[NCStatusConverter alloc] initWithStatInfo:statinfo system:sys];
    
    self.lblhdck_val.text = conv.hdck_status;
    self.lbltmmode_val.text = conv.tmmode_status;
    self.lblaut_val.text = conv.aut_status;
    self.lblrun_val.text = conv.run_status;
    self.lblmotion_val.text = conv.motion_status;
    self.lblmstb_val.text = conv.mstb_status;
    self.lblemergency_val.text = conv.emergency_status;
    self.lblalarm_val.text = conv.alarm_status;
    self.lbledit_val.text = conv.edit_status;
    self.lblwarning_val.text = conv.warning_status;
    self.lblo3dchk_val.text = conv.o3dchk_status;
    self.lblext_opt_val.text = conv.ext_opt_status;
    self.lblrestart_val.text = conv.restart_status;
}

// タイマーイベント処理(現状実行しない）
-(void)getStatusTimer:(NSTimer *)timer {
    if (!indicator.isAnimating) {
        // 取得中の表示
        self.lblStatus.text = NSLocalizedString(@"now collectiong", nil);
        self.viewGetStatus.hidden = NO;
        [indicator startAnimating];
        [self getNCStatus];
    }
}
@end
