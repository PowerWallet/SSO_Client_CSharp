FinApps API - Single Sign On
===========================

####Company Level Authentication:


Tenant Level authentication, is in a http header field named:

``X-FinApps-Token``

The format of the value of the Tenant Header Field is as follows:

``CompanyIdentifier:CompanyToken``

eg. ``X-FinApps-Token : acme:FgWhxtzgLzQohL1UoViMAkfdsgfghdfgdf=``


####User Level Authentication:

User Level authentication, is done via the http authentication header field.

The http authentication field is:

 ``Authorization``

The format of the field is as follows:

``Basic Email:UserToken``

The value portion ( Email:UserToken ) is [Base64][1] Encoded.

eg. ``Authorization : Basic dGVzdDA2MDJAZXhhbXBsZS5jb206aUNReWpGSTlTUHhGMU4xN3Q0RXZOWFQ2N0F1Rm5tWTdPWnFhNWsydWNnaz0=``


###Company Api 


####Create New User
##### `POST /v1/users/new` 
```

Input:

Requires Company Level Authentication
Body:
{
  "firstname" :	User's First Name (optional)
  "lastname" : User's First Name (optional)
  "email" : User's Email, must be unique.
  "password" : User's Password, min size 6 chars, must contain UPPER and lowercase letters and a number
  "postalcode" : User's Postal Code
}

 
Output:

{
    UserToken: "iCQyjFI9SPxF1N17t4EvNXT67AuFnmY7OZqa5k2ucgk="
    SessionToken: null
    SessionRedirectUrl: null
    Email: "test@example.com"
    FirstName: null
    LastName: null
    PostalCode: "12345"
}


```

####Update User Profile
##### `PUT /v1/users/update` 
```

Input:

Requires Company Level Authentication
Requires User Level Authentication
Body:
{
  "firstname" :	User's First Name (optional)
  "lastname" : User's First Name (optional)
  "email" : User's Email, must be unique.
  "postalcode" : User's Postal Code
}

 
Output:

{
    UserToken: "iCQyjFI9SPxF1N17t4EvNXT67AuFnmY7OZqa5k2ucgk="
    SessionToken: null
    SessionRedirectUrl: null
    Email: "test@example.com"
    FirstName: null
    LastName: null
    PostalCode: "10016"
}

```

####Update User Password
##### `PUT /v1/users/updatepassword` 
```

Input:

Requires Company Level Authentication
Requires User Level Authentication
Body:
{
  "oldpassword" : User's old Password
  "newpassword" : User's new Password, min size 6 chars, must contain UPPER and lowercase letters and a number
}

 
Output:

{
    UserToken: "vCHR/lpl4Uo2by1W54K8t0ucndm0Zni7XYgdRSuSvAI="
    SessionToken: null
    SessionRedirectUrl: null
    Email: "test@example.com"
    FirstName: null
    LastName: null
    PostalCode: "10016"
}


```

####Delete User's Account
##### `DELETE /v1/users/delete` 
```

Input:

Requires Company Level Authentication
Requires User Level Authentication

 
Output:

{
    UserToken: null
    SessionToken: null
    SessionRedirectUrl: null
    Email: null
    FirstName: null
    LastName: null
    PostalCode: null
}


```

####Login User (create new session)
##### `POST /v1/users/login` 
```

Input:

Requires Company Level Authentication
Requires User Level Authentication
Body:
{
    "clientip" :	User's IP (optional, recommended)
}
 
Output:

{
    UserToken: "REDACTED"
    SessionToken: "a5a550ec-0c37-46c4-bdf6-976283759c9f"
    SessionRedirectUrl: https://www.finapps.com/app/session/new/a5a550ec-0c37-46c4-bdf6-976283759c9f
    Email: "test@example.com"
    FirstName: null
    LastName: null
    PostalCode: "10016"
}


```

[1]: https://en.wikipedia.org/wiki/Base64
