//
//  NCProgramViewController.m
//  focas2Sample
//

#import "NCProgramViewController.h"
#import "NCConnectManager.h"
#import "NCGetProgram.h"
#import "NCDirectoryManager.h"
#import "NCProgramDetailViewController.h"
#import "FileSaveManager.h"

@interface NCProgramViewController () {
    UIActivityIndicatorView *indicatorProgram;
}
@property NSString* programFileName;
@property BOOL canceled;
@end

@implementation NCProgramViewController
@synthesize tableview;
@synthesize programFileName;

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    tableview.tableFooterView = [UIView new];
    
    if (GetNCProgram == self.selectedFunc) {
        self.btnRegist.hidden = YES;
    } else {
        self.btnRegist.hidden = NO;
        self.btnRegist.enabled = NO;
    }
    
    NCConnectionSetting *set = [[NCConnectManager sharedManager] getConnectNCInfoFromIndex:self.selectedPathIndex];
    
    self.lblncName.text = set.ncName;
    
    [[NCDirectoryManager sharedManager] refresh];
    
    indicatorProgram = [[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    indicatorProgram.frame = CGRectMake(0, 0, 50, 50);
    indicatorProgram.center = self.view.center;
    indicatorProgram.hidesWhenStopped = YES;
    
    self.lblcurdir.text = NSLocalizedString(@"now collectiong", nil);
    [self.view addSubview:indicatorProgram];
}

/**
 didReceiveMemoryWarning event processing method
 */
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/**
 Asks the data source to return the number of sections in table view.
 
 @param tableView [in] An object representing the table view requesting this information.
 @return the number of sections.
 */
-(NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

/**
 Return the number of rows(table cells) in a specified section.
 
 @param section [in] An index number that identifies a section of the table.Table views in a plain style have a section index of zero.
 @return The number of rows in the section.
 */
-(NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    NCDirectoryManager *manager = [NCDirectoryManager sharedManager];
    return [manager getChildrensCount];
}

/**
 Update cells
 
 @param cell [in] update target cell
 @param indexPath [in] index path for table view
 */
-(void)updateCells:(UITableViewCell *)cell atIndexPath:(NSIndexPath *)indexPath {
    NCDirectoryManager *manager = [NCDirectoryManager sharedManager];
    NSArray * array = [manager getNCDirInfoArray];
    NCDirInfo* dirinfo = [array objectAtIndex:(NSUInteger)indexPath.row];
    cell.textLabel.text = dirinfo.d_f;
    if (0 == dirinfo.data_kind) {
        cell.imageView.image = [UIImage imageNamed:@"GenericFolderIcon-32x32"];
    } else if (1 == dirinfo.data_kind) {
        cell.imageView.image = [UIImage imageNamed:@"GenericDocumentIcon-32x32"];
    }
}

/**
 Update visible cells
 */
-(void)updateVisibleCells {
    NSInteger count = [self.tableview numberOfRowsInSection:0];
    for (NSInteger i = 0; i < count; i++) {
        NSIndexPath * path = [NSIndexPath indexPathForRow:i inSection:0];
        UITableViewCell *cel = [self.tableview cellForRowAtIndexPath:path];
        [self updateCells:cel atIndexPath:path];
    }
}

/**
 Asks the data source for a cell to insert in a particular location of the table view.
 
 @param tableView [in] A table-view object requesting the cell.
 @param indexPath [in] An index path locating a row in tableView
 @return An object inheriting from UITableViewCell that the tabel view can use for the specified row.
 */
-(UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"cell"];
    if (!cell) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"cell"];
    }
    [self updateCells:cell atIndexPath:indexPath];
    
    return cell;
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
    UIViewController *uiView = [segue destinationViewController];
    if (![uiView isMemberOfClass:[NCProgramDetailViewController class]]) {
        return;
    }
    
    NCProgramDetailViewController *vc = static_cast<NCProgramDetailViewController *>(uiView);
    vc.selectedPathIndex = self.selectedPathIndex;
    vc.selectedProgramFileName = programFileName;
    vc.isNoAppear = NO;
}

/**
 Unwind Segue - view closing event processing
 
 @param segue [in] pointer to UIStoryboadSegue
 */
-(IBAction)unwindNCProgramDir:(UIStoryboardSegue *)segue {
}

/**
 viewWillAppear event processing method
 
 @param animated [in] If YES, the view is being added to the window using an animation.
 */
-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self refreshtableview];
}

/**
 viewDidAppear event processing method
 
 @param animated [in] If YES, the view was added to the window using an animation.
 */
- (void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    [self moveCurrentDir:nil];
}

/**
 viewWillDisappear event processing method
 
 @param animated [in] If YES, the disappearamce of the view is being animated.
 */
-(void)viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];
    self.canceled = YES;
}

/**
 viewDidDisappear event processing method
 
 @param animated [in] If YES, the disappearamce of the view was animated.
 */
-(void)viewDidDisappear:(BOOL)animated {
    [super viewDidDisappear:animated];
    [[NCDirectoryManager sharedManager] refresh];
}

/**
 refresh tableview
 */
-(void)refreshtableview {
    [self.tableview reloadData];
    [self.tableview deselectRowAtIndexPath:[self.tableview indexPathForSelectedRow] animated:YES];
    [self updateVisibleCells];
}

/**
 Tells the delegete that the specified row is now selected.
 
 @param tableView [in] A table-view object informing the delegate about the new row selection.
 @param indexPath [in] An index path locating the new selected row in tableView.
 */
-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    NCDirectoryManager *manager = [NCDirectoryManager sharedManager];
    NSArray * array = [manager getNCDirInfoArray];
    NCDirInfo* dirinfo = [array objectAtIndex:((NSUInteger)indexPath.row)];;
    NSString *dirName;
    
    if (0 == dirinfo.data_kind) {
        if ([dirinfo.d_f compare:NSLocalizedString(@"upper folder", nil)] == NSOrderedSame) {
            dirName = [manager getUpperDirName];
        }else {
            // Make selected folder current.
            dirName = [manager getDirName:dirinfo.d_f];
        }
        [self moveCurrentDir:dirName];
    } else if (1 == dirinfo.data_kind) {
        // Transition to Detail.
        programFileName = dirinfo.d_f;
        [self performSegueWithIdentifier:@"toDetailNCProgram" sender:self];
    }
    
    return;
}

/**
 NC library operation finished
 - indicator stop and display message
 
 @param isFailed [in] operation result YES - OK / NO - NG
 @param strDisp [in] display message
 */
-(void)finishedNCOperation:(BOOL)isFailed strDisp:(NSString *)strDisp {
    dispatch_async(dispatch_get_main_queue(), ^{
        
        if (isFailed) {
            [[NCDirectoryManager sharedManager] refresh];
        }
        
        [indicatorProgram stopAnimating];
        self.lblcurdir.text = strDisp;
        [self refreshtableview];
    });
}

/**
 Move current CNC's folder
 
 @param dirname [in] move target folder name.
 */
-(void)moveCurrentDir:(NSString *)dirname {
    [indicatorProgram startAnimating];
    
    self.canceled = NO;
    
    @autoreleasepool {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{
            NCGetProgram *dirGetter = [[NCGetProgram alloc] initWithSelectedIndexPath:self.selectedPathIndex];
            short ret = EW_OK;
            if (dirname) {
                [dirGetter movecurrentdir:dirname];
            }
            NSString *dir_name = nil;
            [dirGetter getcurrentdir:&dir_name];
            
            if ([dir_name compare:@"'"] == NSOrderedSame) {
                // Force to Top.
                dir_name = @"//CNC_MEM/";
            }
            
            // Extract the drive from the current.
            NCDirectoryManager * dicManager = [NCDirectoryManager sharedManager];
            [dicManager setCurrentDir:dir_name];
            
            NSMutableArray<NCDirInfo *> *dicArray = nil;
            ret = [dirGetter getAllDir:dir_name isTop:[dicManager isCurrentIsTop] isOnlyDir:(self.selectedFunc == SetNCProgram) dicArray:&dicArray];
            if (self.canceled) {
                return;
            }
            if (ret != EW_OK && ret != EW_NUMBER) {
                [self finishedNCOperation:YES strDisp:[NSString stringWithFormat:NSLocalizedString(@"connect failed", nil), ret, dirGetter.err.err_no, dirGetter.err.err_dtno]];
                return;
            }
            [dicManager addNCDirInfoArray:dicArray];
            [self finishedNCOperation:NO
                              strDisp:[NSString stringWithFormat:NSLocalizedString(@"device name & current dir", nil),dicManager.deviceName, [dicManager getDispDir]]];
            
            if (SetNCProgram == self.selectedFunc) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // Make it clickable.
                    self.btnRegist.enabled = YES;
                });
            }
        });
    }
}

// Registration process.
/**
 register button tap event processing method
 
 @param sender [in] pointer to sender
 */
- (IBAction)touchBtnRegist:(id)sender {
    // Deactivate the Register button.
    self.btnRegist.enabled = NO;
    self.canceled = NO;
    @autoreleasepool {
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{
            NCGetProgram *dirGetter = [[NCGetProgram alloc] initWithSelectedIndexPath:self.selectedPathIndex];
            if (self.canceled) {
                return;
            }
            FileSaveManager * fsManager = [FileSaveManager sharedManager];
            short ret = [dirGetter downloadNCProgram:[NCDirectoryManager sharedManager].dir progName:[fsManager getProgFileName] progData:[fsManager getProgData]];
            if (self.canceled) {
                return;
            }
            if (ret != EW_OK) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    UIAlertController* alert =
                    [UIAlertController alertControllerWithTitle:NSLocalizedString(@"regist failed", nil)
                                                        message:[NSString stringWithFormat:NSLocalizedString(@"regist error", nil), ret, dirGetter.err.err_no, dirGetter.err.err_dtno]
                                                 preferredStyle:UIAlertControllerStyleAlert];
                    UIAlertAction *okAction =
                    [UIAlertAction actionWithTitle:NSLocalizedString(@"OK", nil)
                                             style:UIAlertActionStyleDefault
                                           handler:^(UIAlertAction * action) {
                                               [self performSegueWithIdentifier:@"toManageNCProgram" sender:self];
                                           }];
                    [alert addAction:okAction];
                    [self presentViewController:alert animated:YES completion:nil];
                });
                return;
            }
            
            // Display completion of registration.
            dispatch_async(dispatch_get_main_queue(), ^{
                NSString *path = [[NCDirectoryManager sharedManager].dir stringByAppendingString:[fsManager getProgName]];
                UIAlertController* alert =
                [UIAlertController alertControllerWithTitle:path
                                                    message:NSLocalizedString(@"regist complete", nil)
                                             preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *okAction =
                [UIAlertAction actionWithTitle:NSLocalizedString(@"OK", nil)
                                         style:UIAlertActionStyleDefault
                                       handler:^(UIAlertAction * action) {
                                           [self performSegueWithIdentifier:@"toManageNCProgram" sender:self];
                                       }];
                [alert addAction:okAction];
                [self presentViewController:alert animated:YES completion:nil];
                self.btnRegist.enabled = YES;
            });
            
        });
    }
}
@end
