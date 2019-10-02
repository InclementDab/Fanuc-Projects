//
//  ManagedNCProgramViewController.m
//  focas2Sample
//

#import "ManagedNCProgramViewController.h"
#import "FileSaveManager.h"

#import "EdittableNCProgramDetialViewController.h"

@interface ManagedNCProgramViewController ()

@end

@implementation ManagedNCProgramViewController

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
    UIViewController *vc = [segue destinationViewController];
    if (![vc isMemberOfClass:[EdittableNCProgramDetialViewController class]]) {
        return;
    }
    
    static_cast<EdittableNCProgramDetialViewController *>(vc).isNoAppear = NO;    
}

-(IBAction)unWindManageNCProgram:(UIStoryboardSegue *)segue {
    
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

-(NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

-(NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [[FileSaveManager sharedManager] countOfContents];
}

/**
 Update cells
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
        // When selecting a folder - Move current folder.
        [self moveDir:row];
    } else {
        // When file is selected - Screen transition.
        [[FileSaveManager sharedManager] setSelectedFileName:(NSUInteger)indexPath.row];
        [self performSegueWithIdentifier:@"toShowEdittableNCProgram" sender:self];
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

-(BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath {
    return [[FileSaveManager sharedManager] canDelete:(NSUInteger)indexPath.row];
}

// For deletion.
-(void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath {
    if (editingStyle == UITableViewCellEditingStyleDelete) {
        // Delete.
        [[FileSaveManager sharedManager] deleteFile:(NSUInteger)indexPath.row];
        [self refreshTable];
    }
}

@end
