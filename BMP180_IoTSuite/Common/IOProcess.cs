using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BMP180_IoTSuite.Common
{
    public class IOProcess
    {
        // read & write data
        public static async Task WriteStringToLocalStorage(string filename, string contents)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, contents);
        }

        public static async Task<string> ReadStringFromLocalStorage(string filename)
        {
            string result = String.Empty;

            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(filename);
                result = await FileIO.ReadTextAsync(file);
            }
            catch
            { }

            return result;
        }
    }
}
