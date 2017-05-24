using Infrastructure;

namespace Util
{
	public class LoggedInPersonIDInterceptor : NHibernate.EmptyInterceptor
	{
		/// <summary>
		/// Returns the ID of the currently logged in person if found
		/// </summary>
		public int? LoggedInPersonID
		{

			get
			{
				return UnityWrapper.Resolve<HasLoggedInPersonID>().LoggedInPersonID;
			}
		}


		/// <summary>
		/// Fires on update
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="currentState"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
											string[] propertyNames, NHibernate.Type.IType[] types)
		{
			return SetLoggedInPersonID(currentState, propertyNames);
		}

		/// <summary>
		/// Fires on insert
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="currentState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public override bool OnSave(object entity, object id, object[] currentState,
							  string[] propertyNames, NHibernate.Type.IType[] types)
		{
			return SetLoggedInPersonID(currentState, propertyNames);
		}

		/// <summary>
		/// Apply LoggedInPersonID to the given row state if applicable.
		/// Returns true if state is modified.
		/// </summary>
		/// <param name="currentState"></param>
		/// <param name="propertyNames"></param>
		/// <returns></returns>
		protected bool SetLoggedInPersonID(object[] currentState, string[] propertyNames)
		{
			int max = propertyNames.Length;

			var lipid = LoggedInPersonID;

			for (int i = 0; i < max; i++)
			{
				if (propertyNames[i].ToLower() == "loggedinpersonid" && currentState[i] == null && lipid.HasValue)
				{
					currentState[i] = lipid;

					return true;
				}
			}

			return false;
		}
	}
}
