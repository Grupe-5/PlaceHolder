using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Models
{
    public static class APIErrorParser
    {
        public static string ParseErrorToString(FirebaseAuthException ex)
        {

            string errorText = "";

            switch (ex.Reason)
            {
                case AuthErrorReason.InvalidEmailAddress:
                    errorText = "Email address and/or password is invalid";
                    break;

                case AuthErrorReason.UserNotFound:
                    errorText = "This user does not exist";
                    break;

                default:
                    errorText = ex.Message;
                    break;
            }

            return errorText;
        }
    }
}
