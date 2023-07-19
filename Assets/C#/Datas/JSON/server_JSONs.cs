using JSON_card;
using System.Linq;
using System.Net.Mail;

namespace JSON_server
{
    #region registration requests

    public class ClientLogin
    {
        public string name;
        public string password;
    }

    public class ClientSignIN
    {
        public string name;

        public string password;
        public string email;
    }

    public class Client_changeEmail
    {
        public string token;

        private string old_email_priv;
        private string new_email_priv;

        public string old_email
        {
            set
            {
                if (data_validator.CheckEmail(value))
                {
                    old_email_priv = value;
                }
                else
                {
                    return;
                }
            }
        }
        public string new_email
        {
            set
            {
                if (data_validator.CheckEmail(value))
                {
                    new_email_priv = value;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public class UserData
    {
        public string token;
        public uint UserID;

        public uint RoomID;
    }
    #endregion

    #region room enter requests

    public class ServerCreateRoom
    {
        public uint RoomID;

        public string token;

        public int isPrivate;

        public string key;

        public uint bet;

        public uint cards;

        public int type;

        public int maxPlayers;

        public uint roomOwner;
    }

    public class ServerJoinRoom
    {
        public uint uid;

        public string Token;

        public uint RoomID;

        public string key;

        public uint roomOwner;

        public int type;
    }

    public class ServerExitRoom
    {
        public uint rid;
        public string token;
    }
    #endregion

    #region playing requests

    public class Throw
    {
        public uint UserID;
        public uint RoomID;
        public Card card;
    }

    public class Battle
    {
        public uint UserID;
        public uint RoomID;

        public Card attakingCard;
        public Card attakedCard;
    }

    #endregion

    #region chat server requests

    public class SendMessage
    {
        public uint RoomID;
        public string token;
        public string message;
    }
    #endregion

    public class AvatarData
    {
        public uint UserID;
        public string avatarImage;
    }

    public static class data_validator
    {
        public static bool CheckPassword(string pass)
        {
            //min 6 chars, max 12 chars
            if (pass.Length < 6 || pass.Length > 12)
                return false;

            //No white space
            if (pass.Contains(" "))
                return false;

            //At least 1 upper case letter
            if (!pass.Any(char.IsUpper))
                return false;

            //At least 1 lower case letter
            if (!pass.Any(char.IsLower))
                return false;

            //No two similar chars consecutively
            for (int i = 0; i < pass.Length - 1; i++)
            {
                if (pass[i] == pass[i + 1])
                    return false;
            }

            //At least 1 special char
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();
            foreach (char c in specialCharactersArray)
            {
                if (pass.Contains(c))
                    return true;
            }
            return false;
        }

        public static bool CheckEmail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }
}
