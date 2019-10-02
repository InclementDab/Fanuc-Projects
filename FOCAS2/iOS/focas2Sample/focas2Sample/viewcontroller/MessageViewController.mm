//
//  MessageViewController.m
//  focas2Sample
//

#import "MessageViewController.h"
#import "NCConnectManager.h"
#import "NCGetStatus.h"

@interface MessageViewController () {
    // indicator
    UIActivityIndicatorView *indicator;
}

@end

@implementation MessageViewController

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];

    indicator = [[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    indicator.frame = CGRectMake(0, 0, 50, 50);
    indicator.center = self.view.center;
    indicator.hidesWhenStopped = YES;
    
    self.lblCollecting.text = NSLocalizedString(@"now collectiong", nil);
    self.statusView.hidden = NO;
    
    [self.view addSubview:indicator];
    
    self.lblNCName.text = [[NCConnectManager sharedManager] getConnectNCInfoFromIndex:(NSUInteger)self.selectedPath.row].ncName;
    self.almmsgTextView.textColor = [UIColor redColor];
    
    [indicator startAnimating];
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 viewDidAppear event processing method
 
 @param animated [in] If YES, the view was added to the window using an animation.
 */
- (void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    [self getAllNCAlmAndOpraterMessage];
}

/**
 Get all alarms and operator messages
 */
-(void)getAllNCAlmAndOpraterMessage {
    @autoreleasepool {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{
            NCGetStatus *ncs = [[NCGetStatus alloc] initWithSelectedIndexPath:(NSUInteger)self.selectedPath.row];
            ODBERR err = {0};
            NSString *almmsg;
            NSString *opemsg;
            short ret = EW_OK;
            while (true) {
                ret = [ncs getAlarmMessageAndOperatorMessage:&err almmsg:&almmsg opemsg:&opemsg];
                
                if (ret != EW_BUSY) {
                    break;
                }
                
                @autoreleasepool {
                    [[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:0.02f]];
                }
            }
            
            
            // Display message on screen.
            dispatch_async(dispatch_get_main_queue(), ^{
                [indicator stopAnimating];
                if (EW_OK != ret) {
                    // Acquisition failure, detailed error information display.
                    self.lblCollecting.text = [NSString stringWithFormat:NSLocalizedString(@"collect failed", nil), ret, err.err_no, err.err_no];
                    return;
                }
                self.almmsgTextView.text = almmsg;
                self.opemsgTextView.text = opemsg;
                self.statusView.hidden = YES;
                
            });
            
        });
    }
}

@end
