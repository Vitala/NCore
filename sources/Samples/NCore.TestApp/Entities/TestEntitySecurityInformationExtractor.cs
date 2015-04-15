using NCore.NHibernate.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.TestApp.Entities
{
    public class TestEntitySecurityInformationExtractor : IEntityInformationExtractor<TestEntity>
    {

        public Guid GetSecurityKeyFor(TestEntity entity)
        {
            return entity.SecurityKey;
        }

        public string GetDescription(Guid securityKey)
        {
            return "Test entity security key";
        }

        public string SecurityKeyPropertyName
        {
            get { return "SecurityKey"; }
        }
    }
}
