using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XmlToDB.Models
{
    public interface IEmpty { }

    [DataContract(IsReference = true)]
    public class Items: IEmpty
    {
        public event PropertyChangedEventHandler OnChanged = delegate { };

        [DataMember, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }

        [DataMember, MaxLength(150)]
        public string ItemName { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public int Manufacturer { get; set; }

        public bool Equals(Items secondItem)
        {
            return ItemID == secondItem.ItemID &&
                    ItemName.Equals(secondItem.ItemName, StringComparison.InvariantCultureIgnoreCase) &&
                    Price == secondItem.Price &&
                    Manufacturer.Equals(secondItem.Manufacturer);
        }

        public override string ToString()
        {
            return string.Format(
                "Item ID: {0}\nItem Name: {1}\nPrice: {2}\nManufacturer: {3}\n",
                ItemID, Manufacturer, ItemName, Price);
        }
    }

    [DataContract(IsReference = true)]
    public class Manufacturers: IEmpty
    {
        [DataMember, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ManufacturerID { get; set; }

        [DataMember, MaxLength(50)]
        public string ManufacturerName { get; set; }

        public bool Equals(Manufacturers secondItem)
        {
            return ManufacturerID == secondItem.ManufacturerID &&
                    ManufacturerName.Equals(secondItem.ManufacturerName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return string.Format(
                "Manufacturer ID: {0}\nManufacturer Name: {1}\n",
                ManufacturerID, ManufacturerName);
        }
    }

    [DataContract(IsReference = true)]
    public class AvailableItems: IEmpty
    {
        [DataMember, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [DataMember]
        public int ItemID { get; set; }

        [DataMember]
        public int ItemsCount { get; set; }

        public bool Equals(AvailableItems secondItem)
        {
            return Id == secondItem.Id &&
                    ItemID.Equals(secondItem.ItemID) &&
                    ItemsCount == ItemsCount;
        }

        public override string ToString()
        {
            return string.Format(
                "AVItem ID: {0}\nItem Name: {1}\nItemsCount: {2}\n",
                Id, ItemID.ToString(), ItemsCount);
        }
    }
}
