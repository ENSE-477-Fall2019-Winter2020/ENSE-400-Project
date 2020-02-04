﻿using System;
using System.Collections.Generic;
using System.Text;
using CareOnDemand.Helpers;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System.Threading.Tasks;

namespace CareOnDemand.Models
{
    public class LoginModel
    {
        private string username; 
        private string password;
        public LoginModel(string userEmail, string userPassword)
        {
            username = userEmail;
            password = userPassword;
        }

        public async Task Login()
        {
            AmazonCognitoIdentityProviderClient provider =
                    new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.CACentral1);
            CognitoUserPool userPool = new CognitoUserPool(Settings.AWS_COGNITO_POOL_ID, Settings.AWS_CLIENT_ID, provider);
            CognitoUser user = new CognitoUser(username, Settings.AWS_CLIENT_ID, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var accessToken = authResponse.AuthenticationResult.AccessToken;

            Console.Out.WriteLine(accessToken);
        }
    }
}
