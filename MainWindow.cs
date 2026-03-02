using KickstartService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;

namespace KickstartPalPatcherGui
{
    public partial class MainWindow : Form
    {
        private List<Kickstart> kickstartList = new List<Kickstart>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void miFileOpenFolder_Click(object sender, EventArgs e)
        {
            var files = KickstartTools.PickRomFolderAndScan();
            kickstartList = KickstartTools.LoadKickstarts(files);
            UpdateListView();
        }

        private void miFileOpenRoms_Click(object sender, EventArgs e)
        {
            var files = KickstartTools.PickRomFiles();
            kickstartList = KickstartTools.LoadKickstarts(files);
            UpdateListView();
        }

        private void miFileConvert_Click(object sender, EventArgs e)
        {
            var outputFolder = "";
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder to place the patched ROM files";
                dialog.ShowNewFolderButton = true;
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return;
                }
                else
                {
                    outputFolder = dialog.SelectedPath;
                }
            }

            var counter = 0;          
            statusBarProgress.Minimum = 0;
            statusBarProgress.Maximum = lvKickstarts.Items.Count;
            statusBarProgress.Value = counter;
            statusBarLable.Text = "Patching Kickstarts...";
            statusBarLable.Visible = true;

            var selectedKickstarts = kickstartList
            .Where(k => lvKickstarts.CheckedItems
                .Cast<ListViewItem>()
                .Any(item => item.Text == k.Name))
            .ToList();

            statusBarProgress.Maximum = selectedKickstarts.Count;
            statusBarProgress.Visible = true;

            var patches = KickstartTools.GetPatches();
            Dictionary<uint, Kickstart> patchesDict =
                patches
                    .GroupBy(k => k.Checksum)
                    .ToDictionary(g => g.Key, g => g.First());

            foreach (var kickstart in selectedKickstarts)
            {
                Application.DoEvents();
                KickstartTools.PatchToPal(kickstart);

                if (kickstart.Patched)
                {
                    string fileName = kickstart.Name.Replace(".rom", ".PAL.rom");
                    string originalFileName = Path.GetFileName(fileName);
                    string newFileName = Path.Combine(outputFolder, originalFileName);
                    File.WriteAllBytes(newFileName, kickstart.PalRom);
                    kickstart.Info += " Saved ";
                }
            }

            UpdateListView();
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void miEditSelectAll_Click(object sender, EventArgs e)
        {
            lvKickstarts.BeginUpdate();
            foreach (ListViewItem item in lvKickstarts.Items)
            {
                item.Checked = true;
            }
            lvKickstarts.EndUpdate();
        }

        private void miEditSelectNone_Click(object sender, EventArgs e)
        {
            lvKickstarts.BeginUpdate();
            foreach (ListViewItem item in lvKickstarts.Items)
            {
                item.Checked = false;
            }
            lvKickstarts.EndUpdate();
        }

        private void miAboutInfo_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.ShowDialog(this);
            }
        }

        private void UpdateListView()
        {
            statusBarLable.Visible = false;
            statusBarProgress.Visible = false;

            lvKickstarts.Items.Clear();
            lvKickstarts.BeginUpdate();
            foreach (var kickstart in kickstartList)
            {
                var item = new ListViewItem(kickstart.Name);
                item.SubItems.Add(kickstart.Version + " " + kickstart.Info);
                item.SubItems.Add("0x" + kickstart.Checksum.ToString("X"));
                item.SubItems.Add(kickstart.Patched ? "Yes" : "No");
                item.SubItems.Add(kickstart.Patched ? "0x" + kickstart.PalChecksum.ToString("X") : string.Empty);
                item.Checked = true;
                
                lvKickstarts.Items.Add(item);
            }

            for (int i = 0; i < 2; i++) //lvKickstarts.Columns.Count
            {
                lvKickstarts.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            lvKickstarts.EndUpdate();
        }
    }
}

