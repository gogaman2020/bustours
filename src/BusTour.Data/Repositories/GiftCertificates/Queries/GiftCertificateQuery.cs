using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.SqlScriptManagement;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.GiftCertificates.Queries
{
    public class GiftCertificateQuery : CrudQuery<GiftCertificate, GiftCertificateQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) => Getter.Get(SelectName, null, fields);
        public static string SelectPayment(IEnumerable<string> fields) => Getter.Get("SelectPayment", null, fields);
        public static string SelectOrder(IEnumerable<string> fields) => Getter.Get("SelectOrder", null, fields);
        public static string SelectClient(IEnumerable<string> fields) => Getter.Get("SelectClient", null, fields);
        public static string UpsertClient(IEnumerable<string> fields) => Getter.Get("UpsertClient", null, fields);
        public static string SelectGiftCertificateCount(IEnumerable<string> fields) => Getter.Get("SelectGiftCertificateCount", null, fields);
    }
}