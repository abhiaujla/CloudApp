using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AT1;

namespace AllocationsApplication
{
	partial class AllocationsViewerForm : Form
	{
#region properties

		private Allocations   AT1Allocations;
		private Configuration AT1Configuration;
		private ErrorsViewer  ErrorListViewer = new ErrorsViewer ( );
		private AboutBox      AboutBox        = new AboutBox ( );

#endregion

#region constructors

		public AllocationsViewerForm ( )
		{
			InitializeComponent ( );
		}

#endregion

#region File menu event handlers

		private void OpenAllocationsFileToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			ClearGUI ( );

			// Process allocations and configuration files.
			if ( openFileDialog1.ShowDialog ( ) == DialogResult.OK )
			{
				// Get both filenames.
				String allocationsFileName   = openFileDialog1.FileName;
				String configurationFileName = Allocations.ConfigurationFileName ( allocationsFileName );

				// Parse configuration file.
				if ( configurationFileName == null )
					AT1Configuration = new Configuration ( );
				else
				{
					using ( WebClient configurationWebClient = new WebClient ( ) )
					using ( Stream configurationStream = configurationWebClient.OpenRead ( configurationFileName ) )
					using ( StreamReader configurationFile = new StreamReader ( configurationStream ) )
					{
						Configuration.TryParse ( configurationFile, configurationFileName, out AT1Configuration,
												 out List<String> configurationErrors );
					}
				}

				// Parse Allocations file.
				using ( StreamReader allocationsFile = new StreamReader ( allocationsFileName ) )
				{
					Allocations.TryParse ( allocationsFile, allocationsFileName, AT1Configuration, out AT1Allocations,
										   out List<String> allocationsErrors );
				}

				// Refesh GUI and Log errors.
				UpdateGUI ( );
				AT1Allocations.LogFileErrors ( AT1Allocations.FileErrorsTXT );
				AT1Allocations.LogFileErrors ( AT1Configuration.FileErrorsTXT );
			}
		}

		private void ExitToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			this.Close ( );
		}

#endregion

#region Clear and Update GUI

		private void ClearGUI ( )
		{
			// As we are opening a Configuration file,
			// indicate allocations are not valid, and clear GUI.
			allocationToolStripMenuItem.Enabled = false;

			if ( allocationsWebBrowser.Document != null )
				allocationsWebBrowser.Document.OpenNew ( true );
			allocationsWebBrowser.DocumentText = String.Empty;

			if ( ErrorListViewer.WebBrowser.Document != null )
				ErrorListViewer.WebBrowser.Document.OpenNew ( true );
			ErrorListViewer.WebBrowser.DocumentText = String.Empty;
		}

		private void UpdateGUI ( )
		{
			// Update GUI:
			// - enable menu
			// - display Allocations data (whether valid or invalid)
			// - Allocations and Configuration file errors.
			if ( AT1Allocations.FileValid && AT1Configuration.FileValid )
				allocationToolStripMenuItem.Enabled = true;

			allocationsWebBrowser.DocumentText = AT1Allocations.ToStringHTML ( );

			ErrorListViewer.WebBrowser.DocumentText =
				AT1Allocations.FileErrorsHTML +
				AT1Configuration.FileErrorsHTML +
				AT1Allocations.AllocationsErrorsHTML;
		}

#endregion

#region Validate menu event handlers

		private void AllocationToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			// Check if the allocations are valid.
			AT1Allocations.Validate ( );

			// Refesh GUI and Log errors.
			UpdateGUI ( );
			AT1Allocations.LogFileErrors ( AT1Allocations.AllocationsErrorsTXT );
		}

#endregion

#region View menu event handlers

		private void ErrorListToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			if ( ErrorListViewer != null )
			{
				ErrorListViewer.WindowState = FormWindowState.Normal;
				ErrorListViewer.Show ( );
				ErrorListViewer.Activate ( );
			}
		}

#endregion

#region Help menu event handlers

		private void AboutToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			AboutBox.ShowDialog ( );
		}

#endregion

#region Generate Allocations

		public void generateAllocationsButton_Click ( object sender, EventArgs e )
		{
			var url      = urlComboBox.Text;
            //LocalWCF.ServiceClient localwcf = new LocalWCF.ServiceClient();
            //var solver = localwcf.Solve(url);
            // AT1Allocations = solver.AT1Allocations;
            // AT1Configuration = solver.AT1Configuration;
			UpdateGUI ( );
		}

        #endregion

        private void allocationsWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}