using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.NotificationEvents
{
    public class BookingSummaryNotificationEvent : INotificationEvent
    {
        public NotificationTemplateId TemplateId => NotificationTemplateId.BookingSummary;

        private DomainOrder _order;
        private List<Seat> _seats;
        private List<Menu> _menus;
        private List<Beverage> _beverages;
        private Route _route;
        private PromoCode _promocode;
        private string _siteUrl;


        private string _email;
        private string _language;

        public int? ObjectId => _order?.Id;

        public BookingSummaryNotificationEvent(
            DomainOrder order,
            List<Seat> seats, List<Menu> menus,
            List<Beverage> beverages,
            Task<Route> route,
            PromoCode promocode,
            string email,
            string language,
            string siteUrl
            )
        {
            _order = order;
            _email = email;
            _seats = seats;
            _menus = menus;
            _beverages = beverages;
            _route = route.Result;
            _promocode = promocode;
            _language = language;
            _siteUrl = siteUrl;
        }

        public Dictionary<string, object> GetTemplateData()
        {
            string seats="";
            string seatsPrice = "";
            decimal sum = 0;
            int i = 0;
            foreach(var orderSeat in _order.Seats)
            {
                var seat = _seats.Find(x => x.Id == orderSeat.SeatId);
                if (i != 0)
                {
                    seats += "<br>";
                    seatsPrice += "<br>";
                }
                var table = seat?.Table;
                var price = (table?.IsVip == true ? _order.Tour.VipPrice : _order.Tour.SeatPrice) ?? 0;
                seats += seat.Type == SeatType.Disabled ? "Wheel chair" : $"{seat?.TableId} {seat?.Name}";
                sum += price;
                seatsPrice += "£" + Math.Round(price, 0).ToString();
                i++;
            }

            string extrasName = "";
            string extrasCount = "";
            string extrasPrice = "";
            decimal extrasSum = 0;
            i = 0;
            foreach (var item in _order.Beverages)
            {
                if (i != 0)
                {
                    extrasName += "<br>";
                    extrasCount += "<br>";
                    extrasPrice += "<br>";
                }
                _beverages.Find(x => x.Id == item.BeverageId).Name.TryGetValue(_language, out string beverages);
                extrasName += beverages;
                extrasCount += item.Amount;
                extrasPrice += "£" + _beverages.Find(x => x.Id == item.BeverageId)?.Price.ToString();
                extrasSum += _beverages.Find(x => x.Id == item.BeverageId) != null ? _beverages.Find(x => x.Id == item.BeverageId).Price* item.Amount : 0;
                i++;
            }
            foreach (var item in _order.Menus)
            {
                if (extrasName != "")
                {
                    extrasName += "<br>";
                    extrasCount += "<br>";
                    extrasPrice += "<br>";
                }

                _menus.Find(x => x.Id == item.MenuId).Name.TryGetValue(_language, out string menu);
                extrasName += menu;
                extrasCount += item.Amount;
                extrasPrice += "£" + _menus.Find(x => x.Id == item.MenuId)?.Price.ToString();
                extrasSum += _menus.Find(x => x.Id == item.MenuId) != null ? _menus.Find(x => x.Id == item.MenuId).Price* item.Amount : 0;
            }

            decimal vat = Math.Round(extrasSum * 1 / 5,0);
            _route.CityName.TryGetValue(_language, out string cityName);
            _route.Name.TryGetValue(_language, out string routeName);
            
            return new Dictionary<string, object>
            {
                 //{ "Id", _order.Id.ToString() }
                 {"Number", $"<a href='{_siteUrl}order/{_order.Hash}'>{_order.Id}</a>" },
                 {"City",  cityName},
                { "Date", _order.Tour.Departure.ToShortDateString() },
                { "Itinerary", routeName },
                { "DepartureTime", _order.Tour.Departure.ToShortTimeString() },
                { "Guests", _order.Seats.Count.ToString() },
                { "Table", "By Seats" },
                { "Seats", seats },
                { "SeatsPrice", seatsPrice },
                { "TourPrice", "£"+sum },
                { "ExtrasName", extrasName },
                { "ExtrasCount", extrasCount },
                { "ExtrasPrice", extrasPrice },
                { "isCode", _order.PromoCodeId!=null?"Yes":"No" },
                { "CodePrice",  _promocode != null ? Math.Round((decimal)(_promocode?.AmountOfDiscount)).ToString() +"%" : "" },
                { "isCertificate", _order.CertificateId.HasValue ? "Yes" : "No" },
                { "CertificatePrice", $"£{ Math.Round(_order.GiftCertificate?.Amount ?? 0) }" },
                { "TotalPrice", "£"+Math.Round(_order.TotalPrice + vat).ToString()},
                { "VATPrice", "£"+ vat.ToString()},
                { "ClientFullName", _order.Client?.FullName },
                { "ClientPhoneNumber", _order.Client?.PhoneNumber },

            };
        }

        public string GetEmail()
        {
            return _email;
;
        }
    }
}
