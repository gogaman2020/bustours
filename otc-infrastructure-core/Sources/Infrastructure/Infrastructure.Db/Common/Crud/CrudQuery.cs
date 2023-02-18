using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Db.SqlScriptManagement;

namespace Infrastructure.Db.Common.Crud
{
    public abstract class CrudQuery<TEntity, TQuery> : ICrudQuery
        where TEntity : class
        where TQuery : class
    {
        protected static readonly string SelectName = $"{nameof(CrudOperation.Select)}{typeof(TEntity).Name}";
        protected static readonly string InsertName = $"{nameof(CrudOperation.Insert)}{typeof(TEntity).Name}";
        protected static readonly string DeleteName = $"{nameof(CrudOperation.Delete)}{typeof(TEntity).Name}";
        protected static readonly string UpdateName = $"{nameof(CrudOperation.Update)}{typeof(TEntity).Name}";
        
        protected static readonly SqlScriptGetter<TQuery> Getter = new SqlScriptGetter<TQuery>();

        protected virtual IEnumerable<string> SelectSections => null;
        protected virtual IEnumerable<string> SelectParamNames => Array.Empty<string>();

        string ICrudQuery.GetSelect(IEnumerable<string> paramNames) => Getter.Get(ResolveName(CrudOperation.Select), SelectSections, 
            (paramNames ?? Array.Empty<string>()).Union(SelectParamNames));

        string ICrudQuery.GetDelete(IEnumerable<string> paramNames) => Getter.Get(ResolveName(CrudOperation.Delete), null, paramNames);

        string ICrudQuery.GetInsert(IEnumerable<string> paramNames) => Getter.Get(ResolveName(CrudOperation.Insert), null, paramNames);

        string ICrudQuery.GetUpdate(IEnumerable<string> paramNames) => Getter.Get(ResolveName(CrudOperation.Update), null, paramNames);

        protected virtual string ResolveName(CrudOperation operation)
        {
            switch (operation)
            {
                case CrudOperation.Select:
                    return SelectName;
                case CrudOperation.Insert:
                    return InsertName;
                case CrudOperation.Update:
                    return UpdateName;
                case CrudOperation.Delete:
                    return DeleteName;
            }

            throw new ArgumentException($"Can not resolve script name {operation}");
        }
    }
}