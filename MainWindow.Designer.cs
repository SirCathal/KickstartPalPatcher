namespace KickstartPalPatcherGui
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpenRoms = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.miAboutInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lvKickstarts = new System.Windows.Forms.ListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chChecksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPatched = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPatchedChecksum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuBar.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBar
            // 
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miEdit,
            this.miAbout});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(964, 24);
            this.menuBar.TabIndex = 0;
            this.menuBar.Text = "menuStrip1";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpenFolder,
            this.miFileOpenRoms,
            this.toolStripSeparator1,
            this.miFileConvert,
            this.toolStripSeparator2,
            this.miFileExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(37, 20);
            this.miFile.Text = "&File";
            // 
            // miFileOpenFolder
            // 
            this.miFileOpenFolder.Name = "miFileOpenFolder";
            this.miFileOpenFolder.Size = new System.Drawing.Size(195, 22);
            this.miFileOpenFolder.Text = "&Open Folder";
            this.miFileOpenFolder.Click += new System.EventHandler(this.miFileOpenFolder_Click);
            // 
            // miFileOpenRoms
            // 
            this.miFileOpenRoms.Name = "miFileOpenRoms";
            this.miFileOpenRoms.Size = new System.Drawing.Size(195, 22);
            this.miFileOpenRoms.Text = "Open &Roms";
            this.miFileOpenRoms.Click += new System.EventHandler(this.miFileOpenRoms_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // miFileConvert
            // 
            this.miFileConvert.Name = "miFileConvert";
            this.miFileConvert.Size = new System.Drawing.Size(195, 22);
            this.miFileConvert.Text = "&Convert selected Roms";
            this.miFileConvert.Click += new System.EventHandler(this.miFileConvert_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
            // 
            // miFileExit
            // 
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.Size = new System.Drawing.Size(195, 22);
            this.miFileExit.Text = "Exit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // miEdit
            // 
            this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miEditSelectAll,
            this.miEditSelectNone});
            this.miEdit.Name = "miEdit";
            this.miEdit.Size = new System.Drawing.Size(39, 20);
            this.miEdit.Text = "&Edit";
            // 
            // miEditSelectAll
            // 
            this.miEditSelectAll.Name = "miEditSelectAll";
            this.miEditSelectAll.Size = new System.Drawing.Size(137, 22);
            this.miEditSelectAll.Text = "Select &All";
            this.miEditSelectAll.Click += new System.EventHandler(this.miEditSelectAll_Click);
            // 
            // miEditSelectNone
            // 
            this.miEditSelectNone.Name = "miEditSelectNone";
            this.miEditSelectNone.Size = new System.Drawing.Size(137, 22);
            this.miEditSelectNone.Text = "Select &None";
            this.miEditSelectNone.Click += new System.EventHandler(this.miEditSelectNone_Click);
            // 
            // miAbout
            // 
            this.miAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAboutInfo});
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(52, 20);
            this.miAbout.Text = "&About";
            // 
            // miAboutInfo
            // 
            this.miAboutInfo.Name = "miAboutInfo";
            this.miAboutInfo.Size = new System.Drawing.Size(95, 22);
            this.miAboutInfo.Text = "&Info";
            this.miAboutInfo.Click += new System.EventHandler(this.miAboutInfo_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLable,
            this.statusBarProgress});
            this.statusBar.Location = new System.Drawing.Point(0, 428);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(964, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusBarLable
            // 
            this.statusBarLable.Name = "statusBarLable";
            this.statusBarLable.Size = new System.Drawing.Size(42, 17);
            this.statusBarLable.Text = "Status:";
            this.statusBarLable.Visible = false;
            // 
            // statusBarProgress
            // 
            this.statusBarProgress.Name = "statusBarProgress";
            this.statusBarProgress.Size = new System.Drawing.Size(100, 16);
            this.statusBarProgress.Visible = false;
            // 
            // lvKickstarts
            // 
            this.lvKickstarts.CheckBoxes = true;
            this.lvKickstarts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chInfo,
            this.chChecksum,
            this.chPatched,
            this.chPatchedChecksum});
            this.lvKickstarts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvKickstarts.FullRowSelect = true;
            this.lvKickstarts.GridLines = true;
            this.lvKickstarts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvKickstarts.HideSelection = false;
            this.lvKickstarts.Location = new System.Drawing.Point(0, 24);
            this.lvKickstarts.Name = "lvKickstarts";
            this.lvKickstarts.Size = new System.Drawing.Size(964, 404);
            this.lvKickstarts.TabIndex = 2;
            this.lvKickstarts.UseCompatibleStateImageBehavior = false;
            this.lvKickstarts.View = System.Windows.Forms.View.Details;
            // 
            // chFilename
            // 
            this.chFilename.Text = "Filename";
            this.chFilename.Width = 320;
            // 
            // chInfo
            // 
            this.chInfo.Text = "Info";
            this.chInfo.Width = 320;
            // 
            // chChecksum
            // 
            this.chChecksum.Text = "Checksum";
            this.chChecksum.Width = 120;
            // 
            // chPatched
            // 
            this.chPatched.Text = "Patched";
            // 
            // chPatchedChecksum
            // 
            this.chPatchedChecksum.Text = "Patched Checksum";
            this.chPatchedChecksum.Width = 120;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 450);
            this.Controls.Add(this.lvKickstarts);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuBar);
            this.Name = "MainWindow";
            this.Text = "Kickstart Pal Patcher 2";
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miFileOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem miAboutInfo;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusBarLable;
        private System.Windows.Forms.ToolStripProgressBar statusBarProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miFileConvert;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStripMenuItem miEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem miEditSelectNone;
        private System.Windows.Forms.ListView lvKickstarts;
        private System.Windows.Forms.ToolStripMenuItem miFileOpenRoms;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miFileExit;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.ColumnHeader chInfo;
        private System.Windows.Forms.ColumnHeader chChecksum;
        private System.Windows.Forms.ColumnHeader chPatched;
        private System.Windows.Forms.ColumnHeader chPatchedChecksum;
    }
}

