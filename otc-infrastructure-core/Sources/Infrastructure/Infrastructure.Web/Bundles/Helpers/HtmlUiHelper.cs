using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Web.Bundles.Helpers
{
    public static class HtmlUiHelper
    {
        private static readonly string _formControlCssClass = "form-control";
        public static readonly string UnknownValue = "0";
        public static readonly string StandardSelectValue = "-1";
        public static readonly int StandardSelectValueInt = -1;
        public static readonly string StandardSelectText = "Выбрать";
        public static readonly string AnySelectText = "Любой";
        public static readonly string AllSelectText = "Все";

        public static string MultipleDropDown(string name, List<SelectListItem> listItems, string addCssClass = null)
        {
            return DropDown(name, listItems, isMultiple: true, addCssClass: addCssClass);
        }

        public static string DropDown(string name,
            IEnumerable<SelectListItem> listItems = null,
            bool isMultiple = false,
            bool canBeNull = true,
            string value = null,
            string defaultValue = null,
            bool isDisabled = false,
            string addCssClass = null,
            bool isIgnoreNotFoundListItemException = false,
            string additionalElement = null)
        {
            if (listItems == null)
            {
                listItems = new List<SelectListItem>();
            }
            else
            {
                if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(defaultValue))
                {
                    SetSelectedListItem(listItems, value, defaultValue, isIgnoreNotFoundListItemException);
                }
            }

            var sb = new StringBuilder();

            var nameId = string.Format("name=\"{0}\" id=\"{0}\"", name);
            var cssClass = _formControlCssClass;
            if (!string.IsNullOrEmpty(addCssClass))
                cssClass += " " + addCssClass;

            sb.AppendFormat("<select class=\"select2 {0}\" data-live-search=\"true\" data-size=\"10\" {1} {2} {3}>",
                        cssClass, nameId, isMultiple ? "multiple" : string.Empty, isDisabled ? "disabled" : string.Empty);

            if (!isMultiple && canBeNull)
            {
                var existSelected = listItems.Any(x => x.Selected);
                sb.AppendFormat("<option value=\"{0}\" {1}>{2}</option>",
                    StandardSelectValue, !existSelected ? "selected" : string.Empty, StandardSelectText);
            }

            foreach (var item in listItems)
            {
                sb.AppendFormat("<option value=\"{0}\" {1} {2}>{3}</option>",
                    item.Value,
                    item.Selected ? "selected" : string.Empty,
                    (item.Disabled ? "disabled=\"disabled\"" : string.Empty) +
                        (item.Group != null ? " data-title=\"" + item.Group.Name + "\"" : string.Empty),
                    item.Text);
            }

            sb.Append("</select>");

            if (additionalElement != null)
            {
                sb.Append(additionalElement);
            }

            return sb.ToString();
        }

        public static void SetSelectedListItem(IEnumerable<SelectListItem> listItems,
            string fieldValue,
            string defaultFieldValue,
            bool isIgnoreNotFoundListItemException = false,
            bool autoSelectSingleValue = true)
        {
            SelectListItem selectedItem = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                selectedItem = listItems.SingleOrDefault(x => x.Value == fieldValue);
                if (selectedItem == null && !isIgnoreNotFoundListItemException)
                {
                    throw new Exception($"Не найден выбранный элемент '{fieldValue}'.");
                }
            }
            else if (!string.IsNullOrEmpty(defaultFieldValue))
            {
                selectedItem = listItems.SingleOrDefault(x => x.Value == defaultFieldValue);
            }
            else if (listItems != null && autoSelectSingleValue && listItems.Count() == 1)
            {
                selectedItem = listItems.FirstOrDefault();
            }

            if (selectedItem != null)
            {
                selectedItem.Selected = true;
                foreach (var item in listItems)
                {
                    if (item != selectedItem)
                    {
                        item.Selected = false;
                    }
                }
            }
        }
    }
}
