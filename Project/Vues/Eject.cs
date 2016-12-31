/*
 * Log code : 21
 * User: Thibault MONTAUFRAY
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Tools4Libraries;

namespace Droid_Audio
{
	public partial class Eject : Form
	{
		public Eject()
		{
			InitializeComponent();
            LoadCDPeriphList();
		}

        private void LoadCDPeriphList()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            int count = 0;
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.CDRom)
                {
                    comboBoxCDList.Items.Add(d.Name.Replace(":\\", string.Empty));
                    comboBoxCDList.SelectedIndex = 0;
                    count++;
                }
            }
            if (count == 1) ButtonOKClick(null, null);
            else ShowDialog();
        }

		private void ButtonOKClick(object sender, EventArgs e)
		{
			try 
			{
				EjectMedia.Eject(@"\\.\" + comboBoxCDList.Text + ":");
				this.Close();
			} catch (Exception exp2100) 
			{
				Log.write("[ INF : 2100 ] Error on the peripheric path.\n" + exp2100.Message);
			}
			
		}
	}

	class EjectMedia
	{
		// Constants used in DLL methods
		const uint GENERICREAD = 0x80000000;
		const uint OPENEXISTING = 3;
		const uint IOCTL_STORAGE_EJECT_MEDIA = 2967560;
		const int INVALID_HANDLE = -1;

		// File Handle
		private static IntPtr fileHandle;
		private static uint returnedBytes;
		// Use Kernel32 via interop to access required methods
		// Get a File Handle
		[DllImport("kernel32", SetLastError = true)]
		static extern IntPtr CreateFile(string fileName,
		                                uint desiredAccess,
		                                uint shareMode,
		                                IntPtr attributes,
		                                uint creationDisposition,
		                                uint flagsAndAttributes,
		                                IntPtr templateFile);
		[DllImport("kernel32", SetLastError=true)]
		static extern int CloseHandle(IntPtr driveHandle);
		[DllImport("kernel32", SetLastError = true)]
		static extern bool DeviceIoControl(IntPtr driveHandle,
		                                   uint IoControlCode,
		                                   IntPtr lpInBuffer,
		                                   uint inBufferSize,
		                                   IntPtr lpOutBuffer,
		                                   uint outBufferSize,
		                                   ref uint lpBytesReturned,
		                                   IntPtr lpOverlapped);

		public static void Eject(string driveLetter)
		{
			try
			{
				// Create an handle to the drive
				fileHandle = CreateFile(driveLetter,
				                        GENERICREAD,
				                        0,
				                        IntPtr.Zero,
				                        OPENEXISTING,
				                        0,
				                        IntPtr.Zero);
				if ((int)fileHandle != INVALID_HANDLE)
				{
					// Eject the disk
					DeviceIoControl(fileHandle,
					                IOCTL_STORAGE_EJECT_MEDIA,
					                IntPtr.Zero, 0,
					                IntPtr.Zero, 0,
					                ref returnedBytes,
					                IntPtr.Zero);
				}
			}
			catch
			{
				throw new Exception(Marshal.GetLastWin32Error().ToString());
			}
			finally
			{
				// Close Drive Handle
				CloseHandle(fileHandle);
				fileHandle = IntPtr.Zero;
			}
		}
	}
}
