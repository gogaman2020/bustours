using System;
using System.Collections.Generic;

namespace Infrastructure.Common.Models.ListItem
{
    public class SelectedListItemStringComparer : IEqualityComparer<SelectedListItemString>
    {
        public bool Equals(SelectedListItemString x, SelectedListItemString y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Value == y.Value && x.Text == y.Text;
        }

        public int GetHashCode(SelectedListItemString obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Text?.GetHashCode() ?? 0;

            //Get hash code for the Code field.
            int hashProductCode = obj.Value?.GetHashCode() ?? 0;

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode; ;
        }
    }
}
