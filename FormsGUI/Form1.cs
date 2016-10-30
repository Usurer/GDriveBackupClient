using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDriveClientLib.Abstractions;
using Autofac;

namespace FormsGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            tableRemote.AutoScroll = true;
            tableLocal.AutoScroll = true;

            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            CompareFiles();
        }

        private void CompareFiles()
        {
            IContainer container = new Initializer().RegisterComponents();

            var localFSManager = new LocalFileSystemLib.FileManager();

            Cursor.Current = Cursors.WaitCursor;

            var localTree = localFSManager.GetTree(@"C:\GDrive", 2);

            var googleFSManager = new GoogleDriveFileSystemLib.FileManager(container.Resolve<IGoogleDriveService>());

            var root = googleFSManager.GetTree("root", 2);
            var comparisonResult = Utils.CompareTwoEnumerables(root.Children, localTree.Children);

            Cursor.Current = Cursors.Default;

            progressRemote.Minimum = 0;
            progressRemote.Maximum = 99;
            progressRemote.Value = 0;
            foreach (var driveChild in root.Children.OrderBy(x => x.NodeType).ThenBy(x => x.Name))
            {
                var currentRow = tableRemote.RowCount - 1;
                var label = new Label();
                label.Text = driveChild.Name;
                label.Width = tableLocal.GetColumnWidths()[0];
                tableRemote.Controls.Add(label);
                
                tableRemote.SetRow(label, currentRow);
                tableRemote.SetColumn(label, 0);
                tableRemote.RowCount++;

                if (comparisonResult.CommonNodesFistSequenceIds.Contains(driveChild.Id))
                {
                    label.ForeColor = Color.Green;
                }

                if (progressRemote.Value < progressRemote.Maximum)
                {
                    progressRemote.Value++;
                }
                else
                {
                    progressRemote.Value = progressRemote.Minimum;
                }
            }
            progressRemote.Visible = false;

            progressLocal.Minimum = 0;
            progressLocal.Maximum = 99;
            progressLocal.Value = 0;
            foreach (var localChild in localTree.Children.OrderBy(x => x.NodeType).ThenBy(x => x.Name))
            {
                var currentRow = tableLocal.RowCount - 1;
                var label = new Label();
                label.Text = localChild.Name;
                label.Width = tableLocal.GetColumnWidths()[0];
                tableLocal.Controls.Add(label);

                tableLocal.SetRow(label, currentRow);
                tableLocal.SetColumn(label, 0);
                tableLocal.RowCount++;

                if (comparisonResult.CommonNodesSecondSequenceIds.Contains(localChild.Id))
                {
                    label.ForeColor = Color.Green;
                }

                if (progressLocal.Value < progressLocal.Maximum)
                {
                    progressLocal.Value++;
                }
                else
                {
                    progressLocal.Value = progressLocal.Minimum;
                }
            }
            progressLocal.Visible = false;

        }
    }
}
