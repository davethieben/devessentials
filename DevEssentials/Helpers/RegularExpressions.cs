namespace Essentials
{
    /// <summary>
    /// Contains various commonly used regular expression
    /// </summary>
    /// <remarks></remarks>
    public class RegularExpressions
    {
        /// <summary>
        /// Matches all valid US Phone number in either XXX-XXX-XXXX format or XXXXXXXXXX format.
        /// </summary>
        /// <remarks></remarks>
        public const string USPhone = "^[2-9]\\d{2}-\\d{3}-\\d{4}$|^[2-9]\\d{2}\\d{3}\\d{4}$";

        /// <summary>
        /// Matches 3 digit area code.
        /// </summary>
        /// <remarks></remarks>
        public const string AreaCode = "^[2-9]\\d{2}$";

        /// <summary>
        /// Matches any valid HTTP Address.
        /// NOTE: Taken from http://www.regexlib.com.
        /// Author: M H
        /// </summary>
        /// <remarks></remarks>
        public const string URL = "(http|https):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?";

        /// <summary>
        /// Matches any valid e-mail addresses.
        /// </summary>
        /// <remarks></remarks>
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        //used for filenames
        public const string SafeName = "^[A-Za-z0-9 \'._-]+$";

        //public const string SafeAlphaNumeric = @"^[A-za-z0-9 _\-'/:]+$";

        //used for custom field names
        public const string SafeAlphaNumericCustomFieldName = @"^[A-Za-z0-9 _\-'!@#%&\$/:\?,.()<>=]+$";

        public const string SafeAlphaNumericLoose = @"^[-\w\s\""\'=!@#%&,:;\.\$\{\[\(\|\)\]\}\*\+\?/\\’“”]*$";

        public const string Guid = "^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$";
    }
}
