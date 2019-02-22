using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;

namespace SFTP_Download
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            string host = @"ftp.example.com";
            string username = @"...";

            string remoteDirectory = @"/...";
            string localDirectory = @"\...";

            PrivateKeyFile keyFile = new PrivateKeyFile(Application.StartupPath + @"...\key.ppk");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo conn = new ConnectionInfo(host, 22, username, methods.ToArray());
            using (var sftp = new SftpClient(conn))
            {
                try
                {
                    sftp.Connect();

                    //List all the files in the remote directory
                    var files = sftp.ListDirectory(remoteDirectory);

                    //List only the files which start with the letter N
                    List<SftpFile> fs = new List<SftpFile>();
                    fs.AddRange(files.Where(r => r.Name.StartsWith("n")));

                    //List the last modified dates of all files which start the letter N
                    List<DateTime> filesDate = new List<DateTime>();
                    filesDate.AddRange(fs.Select(q => q.LastAccessTime.Date));

                    //Pick the date of the most recently modified file whose name starts with the letter N
                    DateTime latest = filesDate.Max(p => p);

                    foreach (var file in fs)
                    {
                        string remoteFileName = file.Name;

                        if (file.LastWriteTime.Date == latest )
                        {
                            using (Stream downloadFile = File.OpenWrite(localDirectory + remoteFileName))
                            {
                                sftp.DownloadFile(remoteDirectory + remoteFileName, downloadFile);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sftp.Disconnect();
                }
            } 
        }

        //private void btnCopy_Click(object sender, EventArgs e)
        //{
        //        string host = @"ftp.example.com";
        //        string username = @"...";

        //        string remoteDirectory = @"/...";
        //        string localDirectory = @"\...";

        //        PrivateKeyFile keyFile = new PrivateKeyFile(Application.StartupPath + @"...\key.ppk");
        //        var keyFiles = new[] { keyFile };

        //        var methods = new List<AuthenticationMethod>();
        //        methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

        //        ConnectionInfo conn = new ConnectionInfo(host, 22, username, methods.ToArray());
        //        using (var sftp = new SftpClient(conn))
        //        {
        //            try
        //            {
        //                sftp.Connect();

        //                var files = sftp.ListDirectory(remoteDirectory);
        //                List<DateTime> filesDate = new List<DateTime>();
        //                filesDate.AddRange(files.Select(q => q.LastAccessTime.Date));
        //                DateTime latest = filesDate.Max(p => p);

        //                foreach (var file in files)
        //                {
        //                    string remoteFileName = file.Name;

        //                    if (!file.Name.StartsWith(".") && file.LastWriteTime.Date == latest)
        //                    {
        //                        sftp.ChangeDirectory(destinationDirectory);
        //                        using (FileStream copyFile = new FileStream(remoteDirectory + remoteFileName, FileMode.Open))
        //                        {
        //                            sftp.BufferSize = 4 * 1024;
        //                            sftp.UploadFile(copyFile, remoteFileName);
        //                        }
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //            finally
        //            {
        //                sftp.Disconnect();
        //            }
        //        }

        //    }
    }
}
