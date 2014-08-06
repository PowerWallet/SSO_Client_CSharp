.Net 4.0.0
===============

#### Create a user:

    using FinApps.SSO.MVC5.Models;
    using FinApps.SSO.RestClient_Base.Annotations;
    using FinApps.SSO.RestClient_Base.Model;
    using FinApps.SSO.RestClient_NET40;
    
    var user = new FinAppsUser
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "user@example.com",
                Password = "Password@2",
                PostalCode = "10001"
            };
    var client = new FinAppsRestClient(
                baseUrl: "https://finapps.com/api/v1/",
                companyIdentifier: "ACME Inc.",
                companyToken: "my-secret-token!");
                
    FinAppsUser newUser = await _client.NewUser(user);
    if (newUser.Errors != null && newUser.Errors.Any())
        // handle errors
    } else {
        string userToken = newUser.UserToken;
        // save userToken locally
    }
    


#### Initiate a FinApps session:

    using FinApps.SSO.MVC5.Models;
    using FinApps.SSO.RestClient_Base.Annotations;
    using FinApps.SSO.RestClient_Base.Model;
    using FinApps.SSO.RestClient_NET40;
    

    var credentials = new FinAppsCredentials
            {
                Email = "user@example.com",
                FinAppsUserToken = "4Btuz6TJQU/KcKe8Te+l8F2Gi0ut4x7HMSD56vh3rUk="
            };
    var client = new FinAppsRestClient(
                baseUrl: "https://finapps.com/api/v1/",
                companyIdentifier: "ACME Inc.",
                companyToken: "my-secret-token!");           
                
    FinAppsUser newSessionUser = await _client.NewSession(user.ToFinAppsCredentials(), Request.UserHostAddress);
    if (newSessionUser.Errors != null && newSessionUser.Errors.Any())
        // handle errors
    } else {
        string redirectUrl = newSessionUser.SessionRedirectUrl;
        // redirect to FinApps using above url
    }    
      
    
    
#### Update a User Profile:

    using FinApps.SSO.MVC5.Models;
    using FinApps.SSO.RestClient_Base.Annotations;
    using FinApps.SSO.RestClient_Base.Model;
    using FinApps.SSO.RestClient_NET40;
    

    var credentials = new FinAppsCredentials
            {
                Email = "user@example.com",
                FinAppsUserToken = "4Btuz6TJQU/KcKe8Te+l8F2Gi0ut4x7HMSD56vh3rUk="
            };
    var user = new FinAppsUser
            {
                FirstName = "John L",
                LastName = "Perez",
                Email = "user.new@example.com",
                PostalCode = "12345"
            };            
    var client = new FinAppsRestClient(
                baseUrl: "https://finapps.com/api/v1/",
                companyIdentifier: "ACME Inc.",
                companyToken: "my-secret-token!");           
                
    FinAppsUser updatedUser = await _client.UpdateUserProfile(credentials, user);
    if (updatedUser.Errors != null && updatedUser.Errors.Any())
        // handle errors
    } else {
        string userToken = updatedUser.UserToken;
        // save userToken locally
    }   
    
    
#### Update a User Password:

    using FinApps.SSO.MVC5.Models;
    using FinApps.SSO.RestClient_Base.Annotations;
    using FinApps.SSO.RestClient_Base.Model;
    using FinApps.SSO.RestClient_NET40;
    

    var credentials = new FinAppsCredentials
            {
                Email = "user@example.com",
                FinAppsUserToken = "4Btuz6TJQU/KcKe8Te+l8F2Gi0ut4x7HMSD56vh3rUk="
            };
            
    var client = new FinAppsRestClient(
                baseUrl: "https://finapps.com/api/v1/",
                companyIdentifier: "ACME Inc.",
                companyToken: "my-secret-token!");           
                
    FinAppsUser updatedUser = await _client.UpdateUserPassword(credentials, oldPassword, newPassword);
    if (updatedUser.Errors != null && updatedUser.Errors.Any())
        // handle errors
    } else {
        string userToken = updatedUser.UserToken;
        // save userToken locally
    }    
        
         
#### Delete a User:

    using FinApps.SSO.MVC5.Models;
    using FinApps.SSO.RestClient_Base.Annotations;
    using FinApps.SSO.RestClient_Base.Model;
    using FinApps.SSO.RestClient_NET40;
    

    var credentials = new FinAppsCredentials
            {
                Email = "user@example.com",
                FinAppsUserToken = "4Btuz6TJQU/KcKe8Te+l8F2Gi0ut4x7HMSD56vh3rUk="
            };
            
    var client = new FinAppsRestClient(
                baseUrl: "https://finapps.com/api/v1/",
                companyIdentifier: "ACME Inc.",
                companyToken: "my-secret-token!");           
                
    FinAppsUser deletedUser = await _client.DeleteUser(credentials);
    if (deletedUser.Errors != null && deletedUser.Errors.Any())
        // handle errors
    } 
    
            
#### Demo Application     
    
Demo: [http://sso-mvc4.apphb.com/](http://sso-mvc4.apphb.com/)

Demo Source: [https://github.com/PowerWallet/SSO_Client_CSharp/tree/master/src/MVC4/](https://github.com/PowerWallet/SSO_Client_CSharp/tree/master/src/MVC4/)
    
