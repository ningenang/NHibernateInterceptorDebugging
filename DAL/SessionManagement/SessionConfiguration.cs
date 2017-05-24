using System;
using System.Configuration;
using System.Web;

namespace DAL.SessionManagement
{
	public enum SessionKeyStorageTypeEnum
	{
		HTTPSESSION,
		HTTPCONTEXT,
		THREADLOCAL,
		STATIC,
		NONE
	}

	public static class SessionConfiguration
	{
		public static SessionKeyStorageTypeEnum SessionKeyStorageType
		{
			get
			{
				string strType;
				if (!string.IsNullOrWhiteSpace(strType = ConfigurationManager.AppSettings["NHibernate_SessionStorageType"]))
				{
					SessionKeyStorageTypeEnum result;

					if (Enum.TryParse<SessionKeyStorageTypeEnum>(strType, true, out result))
					{
						return result;
					}
					else
					{
						throw new Exception("Could not recognize session storage type " + strType);
					}
				}
				else
				{
					if (IsInHttpContext)
					{
						return SessionKeyStorageTypeEnum.HTTPSESSION;
					}
					else
					{
						return SessionKeyStorageTypeEnum.THREADLOCAL;
					}
				}
			}
		}

		public static bool IsInHttpContext
		{
			get
			{
				return HttpContext.Current != null;
			}
		}

		public static string DeadlockPriority
		{
			get
			{
				return ConfigurationManager.AppSettings["NHibernate_DeadlockPriority"];
			}
		}

		public static NHibernate.FlushMode FlushMode
		{
			get
			{
				NHibernate.FlushMode flushMode;

				if (Enum.TryParse<NHibernate.FlushMode>(ConfigurationManager.AppSettings["NHibernate_FlushMode"], out flushMode))
				{
					return flushMode;
				}
				else
				{
					return NHibernate.FlushMode.Auto;
				}
			}
		}
	}
}
