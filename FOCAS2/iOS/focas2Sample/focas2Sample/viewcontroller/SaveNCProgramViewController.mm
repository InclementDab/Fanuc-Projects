//
//  SaveNCProgramViewController.m
//  focas2Sample
//

#import "SaveNCProgramViewController.h"
#import "FileSaveManager.h"
#import "NCProgramDetailViewController.h"

@implementation SaveNCProgramViewController
/**
 viewDidLoad event processing method
 */
- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    self.tableview.tableFooterView = [UIView new];
}

/**
 didReceiveMemoryWarning event processing method
 */

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
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
    UIViewController * vc;
    BOOL isNoAppear = NO;
    if (!segue.identifier) {
        vc = [segue destinationViewController];
        if (![vc isMemberOfClass:[NCProgramDetailViewController class]] ){
            return;
        }
    }
    else if ([segue.identifier compare:@"toReturnNCProgramDir"] == NSOrderedSame){
        vc = [self presentingViewController];
        if(![vc isMemberOfClass:[NCProgramDetailViewController class]] ){
            return;
        }
        isNoAppear = YES;
    }
    static_cast<NCProgramDetailViewController *>(vc).isNoAppear = isNoAppear;;
}

/**
 viewWillAppear event processing method
 
 @param animated [in] If YES, the view is being added to the window using an animation.
 */
-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    // Set current to Document.
    FileSaveManager *fsManager = [FileSaveManager sharedManager];
    [fsManager setDefaultPath];
    self.lblCurDir.text = [fsManager getCurDir];
    
    [self refreshTable];
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
    return [[FileSaveManager sharedManager] countOfContents];
}

/**
 Update cells
 
 @param cell [in] update target cell
 @param indexPath [in] index path for table view
 */
-(void)updateCells:(UITableViewCell *)cell atIndexPath:(NSIndexPath *)indexPath {
    FileSaveManager *fsManager = [FileSaveManager sharedManager];
    NSUInteger row = (NSUInteger)indexPath.row;
    cell.textLabel.text = [fsManager getContentsName:row];
    if ([fsManager isDir:row]) {
        cell.imageView.image = [UIImage imageNamed:@"GenericFolderIcon-32x32"];
    } else {
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

-(UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"cell"];
    if (!cell) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"cell"];
    }
    [self updateCells:cell atIndexPath:indexPath];
    
    return cell;
}

/**
 New Folder Alert
 */
-(void)newFolderAlert {
    __weak typeof(self) weakself = self;
    
    @autoreleasepool {
        UIAlertController *alertController =
        [UIAlertController alertControllerWithTitle:NSLocalizedString(@"new folder", nil)
                                            message:NSLocalizedString(@"input new folder name", nil)
                                     preferredStyle:UIAlertControllerStyleAlert];
        [alertController addTextFieldWithConfigurationHandler:^(UITextField *textField) {
            textField.placeholder = NSLocalizedString(@"folder name", nil);
            // Change notification.
            [[NSNotificationCenter defaultCenter] addObserver:weakself selector:@selector(alertTextFieldDidChange:) name:UITextFieldTextDidChangeNotification object:textField];
        }];
        
        // cancel
        [alertController addAction:
         [UIAlertAction actionWithTitle:NSLocalizedString(@"Cancel",nil) style:UIAlertActionStyleCancel handler:^(UIAlertAction *action) {
            // Change notification is unnecessary.
            [[NSNotificationCenter defaultCenter] removeObserver:weakself name:UITextFieldTextDidChangeNotification object:nil];
        }]];
        
        UIAlertAction * creatAction = [UIAlertAction actionWithTitle:NSLocalizedString(@"create folder", nil)
                                                               style:UIAlertActionStyleDefault
                                                             handler:^(UIAlertAction * action) {
            UITextField * textField = alertController.textFields[0];
            if (textField.text.length > 0) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [[FileSaveManager sharedManager] makeDir:textField.text];
                    // Update table.
                    [self refreshTable];
                });
            }
            // Change notification is unnecessary.
            [[NSNotificationCenter defaultCenter] removeObserver:weakself name:UITextFieldTextDidChangeNotification object:nil];
        }];
        creatAction.enabled = NO;
        [alertController addAction:creatAction];
        
        [self presentViewController:alertController animated:NO completion:nil];
    }
}

/**
 Alert text field Didchange
 */
-(void)alertTextFieldDidChange:(NSNotification *)notification {
    UIAlertController * alertController = (UIAlertController *)self.presentedViewController;
    if (alertController) {
        UITextField * textField = alertController.textFields.firstObject;
        UIAlertAction * createAction = alertController.actions.lastObject;
        
        createAction.enabled = textField.text.length > 0;
    }
}

/**
 Refresh table
 */
-(void)refreshTable {
    [self.tableview reloadData];
    [self.tableview deselectRowAtIndexPath:[self.tableview indexPathForSelectedRow] animated:YES];
    [self updateVisibleCells];
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    FileSaveManager *fsManager = [FileSaveManager sharedManager];
    NSUInteger row = (NSUInteger)indexPath.row;
    if ([fsManager isDir:row]) {
        [self moveDir:row];
    }
}

/**
 Move current folder
 */
-(void)moveDir:(NSUInteger)row {
    FileSaveManager *fsManager = [FileSaveManager sharedManager];
    [fsManager moveDir:row];
    self.lblCurDir.text = [fsManager getCurDir];
    [self refreshTable];
}

/**
 Make folder
 */
- (IBAction)makeDir:(id)sender {
    // Display alart and determine the folder name to be created.
    [self newFolderAlert];
}

/**
 Click to save
 */
- (IBAction)clickSave:(id)sender {
    
    FileSaveManager *fsm = [FileSaveManager sharedManager];
    
    // Save the current folder to the file with the program name.
    [fsm saveFile];
    
    UIAlertController* alert =
    [UIAlertController alertControllerWithTitle:[fsm getProgName]
                                        message:NSLocalizedString(@"save complete", nil)
                                 preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *okAction =
    [UIAlertAction actionWithTitle:NSLocalizedString(@"OK", nil)
                             style:UIAlertActionStyleDefault
                           handler:^(UIAlertAction * action) {
                               [self performSegueWithIdentifier:@"toReturnNCProgramDir" sender:self];
                           }];
    [alert addAction:okAction];
    [self presentViewController:alert animated:YES completion:nil];
}
@end
