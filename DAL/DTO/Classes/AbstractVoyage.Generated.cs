using System;
using System.ComponentModel;
using System.DirectoryServices;

namespace DAL.DTO.Classes
{
	#region Voyage

	/// <summary>
	/// Voyage object for NHibernate mapped table 'Voyage'.
	/// </summary>
	[Serializable]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class AbstractVoyage : DTO<Voyage>, System.IComparable
	{
		#region Member Variables

		protected int _voyageID;

		protected string _shipName;
		protected string _fromLocation;
		protected string _toLocation;
		protected DateTime _etd;
		protected DateTime _eta;

		protected DateTime? _createdDate;
		protected int? _createdByPersonID;
		protected DateTime? _modifiedDate;
		protected int? _modifiedByPersonID;
		protected int? _loggedInPersonID;

		protected static String _sortExpression = "VoyageID";
		protected static SortDirection _sortDirection = SortDirection.Ascending;

		#endregion

		#region Constructors

		public AbstractVoyage() { }

		public AbstractVoyage(string shipName, string fromLocation, string toLocation, DateTime etd, DateTime eta, DateTime? createdDate, int? createdByPersonID, DateTime? modifiedDate, int? modifiedByPersonID, int? loggedInPersonID)
		{
			this._shipName = shipName;
			this._fromLocation = fromLocation;
			this._toLocation = toLocation;
			this._etd = etd;
			this._eta = eta;
			this._createdDate = createdDate;
			this._createdByPersonID = createdByPersonID;
			this._modifiedDate = modifiedDate;
			this._modifiedByPersonID = modifiedByPersonID;
			this._loggedInPersonID = loggedInPersonID;
		}



		#endregion

		#region Public Properties

		public virtual int VoyageID
		{
			get { return _voyageID; }
			set { _voyageID = value; }
		}
		public virtual string ShipName
		{
			get { return _shipName; }
			set
			{
				if (value != null && value.Length > 100)
					throw new ArgumentOutOfRangeException("Invalid value for ShipName", value, value.ToString());
				_shipName = value;
			}
		}
		public virtual string FromLocation
		{
			get { return _fromLocation; }
			set
			{
				if (value != null && value.Length > 100)
					throw new ArgumentOutOfRangeException("Invalid value for FromLocation", value, value.ToString());
				_fromLocation = value;
			}
		}
		public virtual string ToLocation
		{
			get { return _toLocation; }
			set
			{
				if (value != null && value.Length > 100)
					throw new ArgumentOutOfRangeException("Invalid value for ToLocation", value, value.ToString());
				_toLocation = value;
			}
		}
		public virtual DateTime ETD
		{
			get { return _etd; }
			set { _etd = value; }
		}
		public virtual DateTime ETA
		{
			get { return _eta; }
			set { _eta = value; }
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
			if (!(obj is Voyage))
				throw new InvalidCastException("This object is not of type Voyage");

			int relativeValue;
			switch (SortExpression)
			{
				case "VoyageID":
					relativeValue = this.VoyageID.CompareTo(((Voyage)obj).VoyageID);
					break;
				case "ShipName":
					relativeValue = (this.ShipName != null) ? ((String)this.ShipName).CompareTo(((Voyage)obj).ShipName) : -1;
					break;
				case "FromLocation":
					relativeValue = (this.FromLocation != null) ? ((String)this.FromLocation).CompareTo(((Voyage)obj).FromLocation) : -1;
					break;
				case "ToLocation":
					relativeValue = (this.ToLocation != null) ? ((String)this.ToLocation).CompareTo(((Voyage)obj).ToLocation) : -1;
					break;
				case "ETD":
					relativeValue = this.ETD.CompareTo(((Voyage)obj).ETD);
					break;
				case "ETA":
					relativeValue = this.ETA.CompareTo(((Voyage)obj).ETA);
					break;
				case "CreatedDate":
					relativeValue = (this.CreatedDate != null) ? ((DateTime)this.CreatedDate).CompareTo(((Voyage)obj).CreatedDate) : -1;
					break;
				case "CreatedByPersonID":
					relativeValue = (this.CreatedByPersonID != null) ? ((Int32)this.CreatedByPersonID).CompareTo(((Voyage)obj).CreatedByPersonID) : -1;
					break;
				case "ModifiedDate":
					relativeValue = (this.ModifiedDate != null) ? ((DateTime)this.ModifiedDate).CompareTo(((Voyage)obj).ModifiedDate) : -1;
					break;
				case "ModifiedByPersonID":
					relativeValue = (this.ModifiedByPersonID != null) ? ((Int32)this.ModifiedByPersonID).CompareTo(((Voyage)obj).ModifiedByPersonID) : -1;
					break;
				case "LoggedInPersonID":
					relativeValue = (this.LoggedInPersonID != null) ? ((Int32)this.LoggedInPersonID).CompareTo(((Voyage)obj).LoggedInPersonID) : -1;
					break;
				default:
					goto case "VoyageID";
			}
			if (AbstractVoyage.SortDirection == SortDirection.Ascending)
				relativeValue *= -1;
			return relativeValue;
		}
		#endregion
	}

	#endregion
}
