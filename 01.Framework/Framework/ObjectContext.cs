using Framework.Caching;
using Framework.Interfaces;
using Framework.User;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Framework
{
    public class ObjectContext
    {
        public readonly Guid RequestId = Guid.NewGuid();
        public bool IsAdmin { get; private set; }
        
        public ICacheManager Cache
        {
            get
            {
                return MemoryCacheManager.Instance;
            }
        }
        private ApiController Context { get { return _controller; } }
        public bool IsAuthorized
        {
            get
            {
                return this.Context.User != null && this.Context.User.Identity.IsAuthenticated;
            }
        }

        private UserPrincipal GetPrincipal()
        {
            if (Context.User != null)
            {
                return Context.User as UserPrincipal;
            }
            return null;
        }

        public UserContext GetUser
        {
            get
            {
                var identity = this.GetPrincipal();
                if (identity != null)
                {
                    return identity.User;
                }
                return null;
            }
        }
        

        public QueryFactory db
        {
            get
            {
                var connection = new SqlConnection();
                var compiler = new SqlServerCompiler();
                var db = new QueryFactory(connection, compiler);
                return db;
            }
        }

        public static ObjectContext CreateContext(ApiController controller)
        {
            return new ObjectContext(controller);
        }

        private ApiController _controller;

        private ObjectContext(ApiController controller)
        {
            _controller = controller;
            //dependency injection
        }

    }
}
