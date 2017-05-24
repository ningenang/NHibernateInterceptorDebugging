﻿using DAL.DAO;
using DAL.DAO.Classes;
using DAL.DTO.Classes;
using DAL.SessionManagement;
using NHibernate;
using System;

namespace DAL.Service.Classes
{
	/// <summary>
	/// Service for accessing <see cref="Voyage" /> objects.
	/// </summary>
	public sealed partial class VoyageService : ServiceBase<Voyage>, IVoyageService
	{
		// Do not edit this file! It's content will be overwritten on code-generation.
		private IVoyageDAO contextDAO;

		protected override IDAO<Voyage, int> ContextDAO
		{
			get { return contextDAO; }
		}

		public VoyageService()
			: this(SessionManager.Instance.DefaultSessionKey)
		{
		}

		public VoyageService(Guid sessionKey)
			: base(sessionKey)
		{
			ISession session = SessionManager.Instance.GetSession(sessionKey);
			contextDAO = new DAOFactory().GetVoyageDAO(session);
		}
	}
}