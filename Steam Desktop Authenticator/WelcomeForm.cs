using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class WelcomeForm : Form
    {
        private Manifest man;

        public WelcomeForm()
        {
            InitializeComponent();
            man = Manifest.GetManifest();
        }

        private void btnJustStart_Click(object sender, EventArgs e)
        {
            // Mark as not first run anymore
            man.FirstRun = false;
            man.Save();

            showMainForm();
        }

        private void btnImportConfig_Click(object sender, EventArgs e)
        {
            // Let the user select the config dir
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "选择原来的Steam桌面身份验证器安装的文件夹";
            DialogResult userClickedOK = folderBrowser.ShowDialog();

            if (userClickedOK == DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                string pathToCopy = null;

                if (Directory.Exists(path + "/maFiles"))
                {
                    // User selected the root install dir
                    pathToCopy = path + "/maFiles";
                }
                else if (File.Exists(path + "/manifest.json"))
                {
                    // User selected the maFiles dir
                    pathToCopy = path;
                }
                else
                {
                    // Could not find either.
                    MessageBox.Show("此文件夹不包含manifest.json文件或maFiles文件。请选择您安装Steam Desktop Authenticator的位置。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Copy the contents of the config dir to the new config dir
                string currentPath = Manifest.GetExecutableDir();

                // Create config dir if we don't have it
                if (!Directory.Exists(currentPath + "/maFiles"))
                {
                    Directory.CreateDirectory(currentPath + "/maFiles");
                }

                // Copy all files from the old dir to the new one
                foreach (string newPath in Directory.GetFiles(pathToCopy, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(pathToCopy, currentPath + "/maFiles"), true);
                }

                // Set first run in manifest
                try
                {
                    man = Manifest.GetManifest(true);
                    man.FirstRun = false;
                    man.Save();
                }
                catch (ManifestParseException)
                {
                    // Manifest file was corrupted, generate a new one.
                    try
                    {
                        MessageBox.Show("您的设置意外损坏，重置为初始值。", "Steam桌面验证器", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        man = Manifest.GenerateNewManifest(true);
                    }
                    catch (MaFileEncryptedException)
                    {
                        // An maFile was encrypted, we're fucked.
                        MessageBox.Show("对不起，由于您使用了加密，SDA无法恢复您的帐户。您需要通过删除验证器来恢复您的Steam帐户。单击确认查看指示。", "Steam桌面验证器", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Diagnostics.Process.Start(@"https://github.com/Jessecar96/SteamDesktopAuthenticator/wiki/Help!-I'm-locked-out-of-my-account");
                        this.Close();
                        return;
                    }
                }

                // All done!
                MessageBox.Show("所有帐户和设置都已导入！单击“确定”继续。", "导入账号", MessageBoxButtons.OK, MessageBoxIcon.Information);
                showMainForm();
            }

        }

        private void showMainForm()
        {
            this.Hide();
            new MainForm().Show();
        }
    }
}
