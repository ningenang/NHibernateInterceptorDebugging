using System;
using System.ComponentModel;
using System.DirectoryServices;

namespace DAL.DTO.Classes
{
	#region Person

	/// <summary>
	/// Person object for NHibernate mapped table 'Person'.
	/// </summary>
	[Serializable]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class AbstractPerson : DTO<Person>, System.IComparable
	{
		#region Member Variables

		protected int _personID;
		
		protected string _userIdentification;
		
		protected DateTime? _createdDate;
		protected int? _createdByPersonID;
		protected DateTime? _modifiedDate;
		protected int? _modifiedByPersonID;
		protected int? _loggedInPersonID;
		
		protected static String _sortExpression = "PersonID";
		protected static SortDirection _sortDirection = SortDirection.Ascending;

		#endregion

		#region Constructors

		public AbstractPerson() { }

		public AbstractPerson(string userIdentification, DateTime? createdDate, int? createdByPersonID, DateTime? modifiedDate, int? modifiedByPersonID, int? loggedInPersonID)
		{
			this._userIdentification = userIdentification;
			this._createdDate = createdDate;
			this._createdByPersonID = createdByPersonID;
			this._modifiedDate = modifiedDate;
			this._modifiedByPersonID = modifiedByPersonID;
			this._loggedInPersonID = loggedInPersonID;
		}



		#endregion

		#region Public Properties

		public virtual int PersonID
		{
			get { return _personID; }
			set { _personID = value; }
		}
		public virtual string UserIdentification
		{
			get { return _userIdentification; }
			set
			{
				if (value != null && value.Length > 100)
					throw new ArgumentOutOfRangeException("Invalid value for UserIdentification", value, value.ToString());
				_userIdentification = value;
			}
		}
		public virtual DateTime? CreatedDate
		{
			get { return _createdDate; }
			set { _createdDate = value; }
		}
		public virtual int? CreatedByPersonID
		{
			get { return _createdByPersonID; }
			set { _createdByPersonID = value; }
		}
		public virtual DateTime? ModifiedDate
		{
			get { return _modifiedDate; }
			set { _modifiedDate = value; }
		}
		public virtual int? ModifiedByPersonID
		{
			get { return _modifiedByPersonID; }
			set { _modifiedByPersonID = value; }
		}
		public virtual int? LoggedInPersonID
		{
			get { return _loggedInPersonID; }
			set { _loggedInPersonID = value; }
		}

		public static String SortExpression
		{
			get { return _sortExpression; }
			set { _sortExpression = value; }
		}

		public static SortDirection SortDirection
		{
			get { return _sortDirection; }
			set { _sortDirection = value; }
		}
		#endregion

		#region IComparable Methods
		public virtual int CompareTo(object obj)
		{
			if (!(obj is Person))
				throw new InvalidCastException("This object is not of type Person");

			int relativeValue;
			switch (SortExpression)
			{
				case "PersonID":
					relativeValue = this.PersonID.CompareTo(((Person)obj).PersonID);
					break;
				case "UserIdentification":
					relativeValue = (this.UserIdentification != null) ? ((String)this.UserIdentification).CompareTo(((Person)obj).UserIdentification) : -1;
					break;
				case "CreatedDate":
					relativeValue = (this.CreatedDate != null) ? ((DateTime)this.CreatedDate).CompareTo(((Person)obj).CreatedDate) : -1;
					break;
				case "CreatedByPersonID":
					relativeValue = (this.CreatedByPersonID != null) ? ((Int32)this.CreatedByPersonID).CompareTo(((Person)obj).CreatedByPersonID) : -1;
					break;
				case "ModifiedDate":
					relativeValue = (this.ModifiedDate != null) ? ((DateTime)this.ModifiedDate).CompareTo(((Person)obj).ModifiedDate) : -1;
					break;
				case "ModifiedByPersonID":
					relativeValue = (this.ModifiedByPersonID != null) ? ((Int32)this.ModifiedByPersonID).CompareTo(((Person)obj).ModifiedByPersonID) : -1;
					break;
				case "LoggedInPersonID":
					relativeValue = (this.LoggedInPersonID != null) ? ((Int32)this.LoggedInPersonID).CompareTo(((Person)obj).LoggedInPersonID) : -1;
					break;
				default:
					goto case "PersonID";
			}
			if (AbstractPerson.SortDirection == SortDirection.Ascending)
				relativeValue *= -1;
			return relativeValue;
		}
		#endregion
	}

	#endregion
}
