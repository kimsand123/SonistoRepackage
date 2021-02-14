using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;


namespace SonistoRepackage
{
    class ComputerUsers : IDisposable
    {
        public ComputerUsers() 
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string get()
        {
            string path = string.Format("WinNT://{0},computer", Environment.MachineName);
            IEnumerable<string> userNames=null;
            try
            {
                var computerEntry = new DirectoryEntry(path);
            
                userNames = computerEntry.Children
                    .Cast<DirectoryEntry>()
                    .Where(childEntry => childEntry.SchemaClassName == "User")
                    .Select(userEntry => userEntry.Path);

                foreach (string name in userNames)
                {
                    if (name.Contains("test"))
                    {
                        return name;
                    }
                }
            }
            catch
            {

            }
            finally
            {
               
            }
            return null;
        }
    }
}
