using System.Text;

namespace NCore.NHibernate.Security
{
    public class AuthorizationInformation
    {
        readonly StringBuilder builder = new StringBuilder();

        public override string ToString()
        {
            return builder.ToString();
        }

        public void AddAllow(string format, params object[] args)
        {
            builder.AppendFormat(format, args).AppendLine();
        }

        public void AddDeny(string format, params object[] args)
        {
            builder.AppendFormat(format, args).AppendLine();
        }
    }
}
