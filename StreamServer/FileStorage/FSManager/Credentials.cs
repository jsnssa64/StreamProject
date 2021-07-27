using SecretServices.CredentialsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSManager
{
    //  This contains Secret Credential data -> Do not know what to do with this yet
    public class SecretCredentials
    {
        public ICredentialModel credentials { get; }

        //  Dependency Inject AWS, Azure Cert or Azure CS 
        public SecretCredentials(ICredentialModel Credentials)
        {
            credentials = Credentials;
            //  Get File Path Using Credentials

        }
    }
}
