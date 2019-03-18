using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

namespace UsbDetection
{
    class Program
    {
        static void Main()
        {

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_USBHub");
            ManagementObjectCollection managementObjectSearcherList = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in managementObjectSearcherList)
            {
                Console.WriteLine(managementObject["Name"]);
            }

            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();

            void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                foreach (var property in instance.Properties)
                {
                    Console.WriteLine(property.Name + " = " + property.Value);
                }
            }

            void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                foreach (var property in instance.Properties)
                {
                    Console.WriteLine(property.Name + " = " + property.Value);
                }
            }
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
