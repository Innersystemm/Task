using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace XmlToDB.Models
{
    [DataContract(IsReference = true)]
    public class DataWrapper
    {
        [DataMember]
        public ObservableCollection<Manufacturers> Manufacturers { get; set; }

        [DataMember]
        public ObservableCollection<AvailableItems> AvailableItems { get; set; }

        [DataMember]
        public ObservableCollection<Items> Items { get; set; }

        public DataWrapper(ObservableCollection<Manufacturers> manuf,
                           ObservableCollection<AvailableItems> avitems,
                           ObservableCollection<Items> items)
        {
            Manufacturers = manuf;
            AvailableItems = avitems;
            Items = items;
        }
    }

    public class DataSync: INotifyPropertyChanged
    {
        TypedContext context;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        DataWrapper _xmlcollection;
        public DataWrapper XMLCollection
        {
            get { return _xmlcollection; }
            set
            {
                _xmlcollection = value;
                OnPropertyChanged("XMLCollection");
            }
        }

        DataWrapper _dbcollection;
        public DataWrapper DbCollection
        {
            get { return _dbcollection; }
            set
            {
                _dbcollection = value;
                OnPropertyChanged("DbCollection");
            }
        }
        
        string _xdocpath;
        bool _loadenabled;
        string _xmlloadstate;
        uint _totalremoved,
            _totaladded,
            _totalupdated;

        public uint TotalRemoved
        {
            get { return _totalremoved; }
            set
            {
                _totalremoved = value;
                OnPropertyChanged("TotalRemoved");
            }
        }

        public uint TotalAdded
        {
            get { return _totaladded; }
            set
            {
                _totaladded = value;
                OnPropertyChanged("TotalAdded");
            }
        }

        public uint TotalUpdated
        {
            get { return _totalupdated; }
            set
            {
                _totalupdated = value;
                OnPropertyChanged("TotalUpdated");
            }
        }

        public DataSync()
        {
            context = new TypedContext("DataConnection");
            context.AvailableItems.Load();
            context.Manufacturers.Load();
            context.Items.Load();

            XMLDocPath = "Select XML flie path OR specify file for database unloading data...";
            LoadEnabled = false;

            DbCollection = new DataWrapper(context.Manufacturers.Local,
                                            context.AvailableItems.Local,
                                            context.Items.Local);
            TotalAdded = TotalRemoved = TotalUpdated = 0;
        }

        public string XMLLoadState
        {
            get { return _xmlloadstate; }
            set
            {
                _xmlloadstate = value;
                OnPropertyChanged("XMLLoadState");
            }
        } 

        public bool LoadEnabled
        {
            get { return _loadenabled; }
            set
            {
                _loadenabled = value;
                OnPropertyChanged("LoadEnabled");
            }
        }

        public string XMLDocPath
        {
            get { return _xdocpath; }
            set
            {
                _xdocpath = value;
                OnPropertyChanged("XMLDocPath");
            }
        }

        public void Open()
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == true)
            {
                LoadEnabled = true;
                XMLDocPath = of.FileName;
            }
            else LoadEnabled = false;
        }

        public void SelectPath()
        {
            SaveFileDialog sf = new SaveFileDialog() { Filter = "Xml Files(*.xml)|*.xml;" };
            if (sf.ShowDialog() == true)
            {
                XMLDocPath = sf.FileName;
                LoadEnabled = true;
            }
            else LoadEnabled = false;
        }

        public void Sync()
        {
            TotalAdded = TotalRemoved = TotalUpdated = 0;
            try
            {
                //удаляем из бд записи отсутствующие в Xml файле
                var manufArr = DbCollection.Manufacturers.ToArray();
                foreach (var id in manufArr.Select(n=>n.ManufacturerID))
                {
                    if (!XMLCollection.Manufacturers.Select(n => n.ManufacturerID).Contains(id))
                    {
                        context.Manufacturers.Remove(context.Manufacturers.Single(n=>n.ManufacturerID == id));
                        TotalRemoved++;
                        context.SaveChanges();
                    }
                }

                //ищем совпадающие id в бд и xml файле, затем сравниваем выбранные записи
                //если запись из бд отличается от записи в xml файле то данныей в ней будут замещены соответствующими данными из xml файла
                //если запись в бд не будет обнаружена то произойдет ее добавление
                foreach (int id in XMLCollection.Manufacturers.Select(n => n.ManufacturerID))
                {
                    var dbItem = DbCollection.Manufacturers.FirstOrDefault(n => n.ManufacturerID == id);
                    var xmlItem = XMLCollection.Manufacturers.Single(n => n.ManufacturerID == id);

                    if (dbItem != null)
                    {
                        if (!dbItem.Equals(xmlItem))
                        {
                            dbItem.ManufacturerName = xmlItem.ManufacturerName;
                            TotalUpdated++;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.Manufacturers.Add(xmlItem);
                        TotalAdded++;
                        context.SaveChanges();
                    }
                }

                //удаляем из бд записи отсутствующие в Xml файле
                var avArr = DbCollection.AvailableItems.ToArray();
                foreach (var id in avArr.Select(n => n.Id))
                {
                    if (!XMLCollection.AvailableItems.Select(n => n.Id).Contains(id))
                    {
                        context.AvailableItems.Remove(context.AvailableItems.Single(n => n.Id == id));
                        TotalRemoved++;
                        context.SaveChanges();
                    }
                }

                foreach (int id in XMLCollection.AvailableItems.Select(n => n.Id))
                {
                    var dbItem = DbCollection.AvailableItems.FirstOrDefault(n => n.Id == id);
                    var xmlItem = XMLCollection.AvailableItems.Single(n => n.Id == id);

                    if (dbItem != null)
                    {
                        if (!dbItem.Equals(xmlItem))
                        {
                            dbItem.ItemID = xmlItem.ItemID;
                            dbItem.ItemsCount = xmlItem.ItemsCount;
                            TotalUpdated++;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.AvailableItems.Add(xmlItem);
                        TotalAdded++;
                        context.SaveChanges();
                    }
                }

                //удаляем из бд записи отсутствующие в Xml файле
                var itemsArr = DbCollection.Items.ToArray();
                foreach (var id in itemsArr.Select(n => n.ItemID))
                {
                    if (!XMLCollection.Items.Select(n => n.ItemID).Contains(id))
                    {
                        context.Items.Remove(context.Items.Single(n => n.ItemID == id));
                        TotalRemoved++;
                        context.SaveChanges();
                    }
                }

                foreach (int id in XMLCollection.Items.Select(n => n.ItemID))
                {
                    var dbItem = DbCollection.Items.FirstOrDefault(n => n.ItemID == id);
                    var xmlItem = XMLCollection.Items.Single(n => n.ItemID == id);

                    if (dbItem != null)
                    {
                        if (!dbItem.Equals(xmlItem))
                        {
                            dbItem.ItemName = xmlItem.ItemName;
                            dbItem.Price = xmlItem.Price;
                            dbItem.Manufacturer = xmlItem.Manufacturer;
                            TotalUpdated++;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.Items.Add(xmlItem);
                        TotalAdded++;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                if (null != e.InnerException)
                    MessageBox.Show(e.InnerException.ToString());
            }
        }

        public void Serialize()
        {
            try
            {
                SerializeObject(DbCollection);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Deserialize()
        {
            try
            {
                XMLCollection = Deserialize<DataWrapper>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        void SerializeObject<T>(T objectToSerialize)
        {
            try
            {
                using (MemoryStream memStr = new MemoryStream())
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(memStr, objectToSerialize);

                    memStr.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(memStr))
                    {
                        string result = streamReader.ReadToEnd();
                        using (StreamWriter sw = new StreamWriter(XMLDocPath))
                        {
                            sw.WriteLine(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public T Deserialize<T>()
        {
            try
            {
                using (FileStream fs = new FileStream(XMLDocPath, FileMode.Open))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                    XmlDictionaryReader reader =
                        XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

                    T serializedData = (T)dcs.ReadObject(reader);
                    return serializedData;
                }
            }
            catch (Exception e)
            {
                throw e;
            }   
        }
    }
}