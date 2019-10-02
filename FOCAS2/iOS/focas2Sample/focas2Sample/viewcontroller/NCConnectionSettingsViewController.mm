//
//  NCConnectionSettingsViewController.m
//  focas2Sample
//

#import "NCConnectionSettingsViewController.h"
#import "NCConnectManager.h"
#import "ConnectionInfoViewController.h"
#import "MessageViewController.h"
#import "NCProgramViewController.h"
#import "EdittableNCProgramDetialViewController.h"
#import "NCConnectionTableViewCell.h"
#import "NCGetStatus.h"

@interface NCConnectionSettingsViewController ()
{
    NSTimer *tmgetStatus;
}
@property NSIndexPath* selectedIndexPath;
@property BOOL isCanceled;
@property NSMutableDictionary* NCGetStatusDic;
@property NSMutableDictionary* iconHiddenDic;
@end

@implementation NCConnectionSettingsViewController
@synthesize connectNCTableView;

static const NSTimeInterval STATE_INTERVAL_DEFUALT = 10.0f;

/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.    
    connectNCTableView.tableFooterView = [UIView new];
    // Use custom cell.
    [connectNCTableView registerNib:[UINib nibWithNibName:@"NCConnectionTableViewCell" bundle:nil] forCellReuseIdentifier:@"cell"];
    
    if (self.selectedFunc == NCConnection) {
        self.addButton.hidden = NO;
        
    } else {
        self.addButton.hidden = YES;
    }
    
    connectNCTableView.rowHeight = UITableViewAutomaticDimension;
    
    self.NCGetStatusDic = [NSMutableDictionary new];
    self.iconHiddenDic = [NSMutableDictionary new];
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
- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self refreshTable];
    self.isCanceled = NO;
}

/**
 viewDidAppear event processing method
 
 @param animated [in] If YES, the view was added to the window using an animation.
 */
-(void) viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    
    [self startTimer];
    
}

/**
 refresh table
 */
-(void)refreshTable {
    [self.connectNCTableView reloadData];
    // deselect cell
    [self.connectNCTableView deselectRowAtIndexPath:[self.connectNCTableView indexPathForSelectedRow] animated:YES];
    [self updateVisibleCells];
    [self checkConnectSettingsIsMax];
}

/**
 Check if the NCConnectSetting information is the maximum number.
 */
-(void)checkConnectSettingsIsMax {
    if (NC_CONNECTION_SETTINGS_MAX <= [[NCConnectManager sharedManager] countOfNcConnectDic]) {
        // Disable adding when the maximum number is exceeded.
        self.addButton.enabled = NO;
        return;
    }
    self.addButton.enabled = YES;
}

/**
 viewWillDisappear event processing method
 
 @param animated [in] If YES, the disappearamce of the view is being animated.
 */
-(void) viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];
    
    self.isCanceled = YES;
    switch (self.selectedFunc) {
        case NCConnection:
            // Maintain connection information.
            [[NCConnectManager sharedManager] saveToFile];
            break;
        case GetNCState:
            // Timer stop.
            if (tmgetStatus.isValid) {
                [tmgetStatus invalidate];
            }
            break;
        case GetNCProgram:
            break;
        case SetNCProgram:
            break;
        default:
            break;
    }
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
    return [[NCConnectManager sharedManager] countOfNcConnectDic];
}

/**
 Update cells
 
 @param cell [in] update target cell
 @param indexPath [in] index path for table view
 */
-(void)updateCells:(UITableViewCell *)cell atIndexPath:(NSIndexPath *)indexPath {
    NSString *text = [[NCConnectManager sharedManager] keyNameInNcConnectDicAtIndex:(NSUInteger) indexPath.row];
    
    NCConnectionTableViewCell *customCell = (NCConnectionTableViewCell *)cell;
    
    if (self.selectedFunc == GetNCState) {
        [customCell setConnectionName:text];
        if ([[self.iconHiddenDic allKeys] containsObject:indexPath]) {
            customCell.imageView.hidden = [[self.iconHiddenDic objectForKey:indexPath] boolValue];
        }
    } else {
        customCell.textLabel.text = text;
        customCell.lblConnectName.text = nil;
    }
}

/**
 Update visible cells
 */
-(void)updateVisibleCells {
    NSInteger count = [self.connectNCTableView numberOfRowsInSection:0];
    for (NSInteger i = 0; i < count; i++) {
        NSIndexPath * path = [NSIndexPath indexPathForRow:i inSection:0];
        UITableViewCell *cel = [self.connectNCTableView cellForRowAtIndexPath:path];
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
    NCConnectionTableViewCell *cell = [connectNCTableView dequeueReusableCellWithIdentifier:@"cell" forIndexPath:indexPath];
    [self updateCells:cell atIndexPath:indexPath];
    return cell;
}

/**
 Tells the delegete that the specified row is now selected.
 
 @param tableView [in] A table-view object informing the delegate about the new row selection.
 @param indexPath [in] An index path locating the new selected row in tableView.
 */
-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    self.selectedIndexPath = indexPath;
    
    switch (self.selectedFunc) {
        case NCConnection:
            [self performSegueWithIdentifier:@"toSetting" sender:self];
            break;
        case GetNCState:
            [self performSegueWithIdentifier:@"toMessageView" sender:self];
            break;
        case GetNCProgram:
        case SetNCProgram:
            [self performSegueWithIdentifier:@"getsetNCProgram" sender:self];
            break;
        default:
            break;
    }
}

/**
 Asks the data source to verify that th given row is editable.
 
 @param tableView [in] The table-view object requesting this information.
 @param indexPath [in] An index path locating a row in tableView.
 */
-(BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath {
    return (self.selectedFunc == NCConnection);
}

/**
 Asks the data source to commit the insertion or deletion of a specified row in the receiver.
 
 @param tableView [in] The table-view object requesting the insertion or deletion.
 @param editingStyle [in] The cell editing style corresponding to a insertiong or deletion requested for the row specified by indexPath.
 @param indexPath [in] An index path locating a row in tableView.
 */
-(void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath {
    if (editingStyle == UITableViewCellEditingStyleDelete) {
        // Delete.
        [[NCConnectManager sharedManager] deleteSetting:(NSUInteger)indexPath.row];
        [self refreshTable];
    }
}

/**
 Unwind Segue - view closing event processing
 
 @param segue [in] pointer to UIStoryboadSegue
 */
-(IBAction)unwindNCConnectionSettings:(UIStoryboardSegue *)segue {
}

/**
 Processing at screen transition by storyboard
 
 @param segue [in] pointer to UIStoryboadSegue
 @param sender [in] pointer to sender
 */
-(void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    NSString * identifier = [segue identifier];    
    UIViewController *uiView = [segue destinationViewController];
    switch (self.selectedFunc) {
        case NCConnection:
            [self prepareForConnectionInfoViewControllerSegue:uiView identifier:identifier];
            break;
        case GetNCState:
            [self prepareForNCStatusViewControllerSegue:uiView identifier:identifier];
            break;
        case GetNCProgram:
        case SetNCProgram:
            [self prepareForNCProgramViewControllerSegue:uiView identifier:identifier];
            break;
        default:
            break;
    }
}

/**
 Processing at screen transition by storyboard
 To ConnectionInfoViewController
 
 @param uiViewController [in] pointer to UIViewController
 @param identifier [in] pointer to identifier
 */
-(void)prepareForConnectionInfoViewControllerSegue:(UIViewController*)uiViewController identifier:(NSString *)identifier {
    ConnectionInfoViewController* vc;
    if ([uiViewController isMemberOfClass:[ConnectionInfoViewController class]]) {
        vc = static_cast<ConnectionInfoViewController*>(uiViewController);
    } else {
        return;
    }
    
    if ([identifier isEqualToString:@"toSetting"]) {
        NCConnectionSetting *setting = [[NCConnectManager sharedManager] getConnectNCInfoFromIndex:(NSUInteger)self.selectedIndexPath.row];
        if (setting != nil) {
            vc.name = setting.ncName;
            vc.ipa = setting.ipAddr;
            vc.port = setting.portNo;
            vc.timeout = setting.timeoutVal;
            vc.isSelected = YES;
        } else {
            vc.isSelected = NO;
        }
    } else if ([identifier isEqualToString:@"addSetting"]) {
        vc.isSelected = NO;
    }
}

/**
 Processing at screen transition by storyboard
 To MessageViewController
 
 @param uiViewController [in] pointer to UIViewController
 @param identifier [in] pointer to identifier
 */
-(void)prepareForNCStatusViewControllerSegue:(UIViewController*)uiViewController identifier:(NSString *)identifier {
    if (![uiViewController isMemberOfClass:[MessageViewController class]]) {
        return;
    }
    static_cast<MessageViewController *>(uiViewController).selectedPath = self.selectedIndexPath;
}

/**
 Processing at screen transition by storyboard
 To NCProgramViewController
 
 @param uiViewController [in] pointer to UIViewController
 @param identifier [in] pointer to identifier
 */
-(void)prepareForNCProgramViewControllerSegue:(UIViewController*)uiViewController identifier:(NSString *)identifier {
    
    if (![uiViewController isMemberOfClass:[NCProgramViewController class]]) {
        if([uiViewController isMemberOfClass:[EdittableNCProgramDetialViewController class]]) {
            static_cast<EdittableNCProgramDetialViewController *>(uiViewController).isNoAppear = YES;
        }
        
        return;
    }
    NCProgramViewController* vc = static_cast<NCProgramViewController *>(uiViewController);
    vc.selectedPathIndex = (NSUInteger)self.selectedIndexPath.row;
    vc.selectedFunc = self.selectedFunc;
}

/**
 back button tap event processing method
 
 @param sender [in] pointer to sender
 */
- (IBAction)btnReturnTouchUp:(id)sender {
    if(self.selectedFunc == SetNCProgram) {
        [self performSegueWithIdentifier:@"toManageNCProgram" sender:self];
    } else {
        [self performSegueWithIdentifier:@"toMenu" sender:self];
    }
}

/**
 timer start
 */
-(void)startTimer {
    if (self.selectedFunc != GetNCState) {
        return;
    }
    
    NSInteger count = [self.connectNCTableView numberOfRowsInSection:0];
    for (NSInteger i = 0; i < count; i++) {
        NCGetStatus * statusGetter = [[NCGetStatus alloc] initWithSelectedIndexPath:(NSUInteger)i];
        [self.NCGetStatusDic setObject:statusGetter forKey:[NSIndexPath indexPathForRow:i inSection:0]];
    }
    
    [self getAllCellsStatus];
    
    tmgetStatus = [NSTimer scheduledTimerWithTimeInterval:STATE_INTERVAL_DEFUALT target:self selector:@selector(getStatusTimer:) userInfo:nil repeats:YES];
}

/**
 Timer event processing method
 
 @param timer [in] a NSTimer object
 */
-(void)getStatusTimer:(NSTimer *)timer {
    [self getAllCellsStatus];
}

/**
 Get all cells of CNC's Status
 */
-(void)getAllCellsStatus {
    NSInteger count = [self.connectNCTableView numberOfRowsInSection:0];
    for (NSInteger i = 0; i < count; i++) {
        NSIndexPath * path = [NSIndexPath indexPathForRow:i inSection:0];
        UITableViewCell *cell = [self.connectNCTableView cellForRowAtIndexPath:path];
        [self getStatus:(NCConnectionTableViewCell *)cell];
    }
}

/**
 Get a cell of CNC's Status
 
 @param cell [in] a NCConnectionTableViewCell object
 */
-(void)getStatus:(NCConnectionTableViewCell *)cell {
    
    NSIndexPath *path = [self.connectNCTableView indexPathForCell:cell];
    NCGetStatus * statusGetter = [self.NCGetStatusDic objectForKey:path];
    ODBST2 statinfo = {0};
    ODBERR err = {0};

    short ret = [statusGetter getStatus:&statinfo err:&err];
    
    // Decide display of X image.
    BOOL isHidden = (EW_OK == ret || EW_BUSY == ret);
    cell.imageView.hidden = isHidden;
    [self.iconHiddenDic setObject:[NSNumber numberWithBool:isHidden] forKey:path];
    if (ret != EW_OK) {
        
        // Hide each state.
        [cell setStatusErase];
        return;
    }
    
    // Display each status.
    [cell setStatus:(EMERGENCY_STATUS)statinfo.emergency alarm:(ALARM_STATUS)statinfo.alarm aut:(AUT_STATUS)statinfo.aut];
    
}

@end
