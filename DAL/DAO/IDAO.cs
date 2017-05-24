using System.Collections.Generic;

namespace DAL.DAO
{
	public interface IDAO<TDTO, TObjectId>
	{
		List<TDTO> GetAll();
		List<TDTO> GetAllCacheable();
		TDTO GetById(TObjectId id);
		TDTO LoadById(TObjectId id);
		TDTO Save(TDTO obj);
		TDTO SaveOrUpdate(TDTO obj);
		TDTO Merge(TDTO obj);
		void Delete(TDTO obj);
		void Flush();
	}
}
