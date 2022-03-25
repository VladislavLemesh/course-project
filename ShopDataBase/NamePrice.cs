using System;
using System.IO;
using System.Text;

namespace ShopDataBase
{
    public class NamePrice
    {
        public int size = 100;
        public MyList[] items;
        public int fullItem = 0;
        public int compare = 0;
        public NamePrice()
        {
            items = new MyList[size];
            string path = "Price.txt";

            if (File.Exists(path))
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    while (!sr.EndOfStream)
                    {
                        string[] str = sr.ReadLine().Split(';');
                        Add(new Item<string, int>(str[0], Convert.ToInt32(str[1])));
                    }           
        }

        public bool Add(Item<string, int> item)
        {
            if (item.Key.Length != 0)
            {
                var k = GetHash(item.Key);
                if (items[k] == null)
                {
                    fullItem++;
                    items[k] = new MyList();
                    items[k].Add(item);
                    return true;
                }
                else
                {
                    if (Search(item.Key) != null)
                    {
                        return false;
                    }
                    else
                    {
                        fullItem++;
                        items[k].Add(item);
                        return true;
                    }
                }
            }
            else return false;
        }

        public Item<string, int> Search(string name)
        {
            if (name.Length != 0)
            {
                var k = GetHash(name);
                if (items[k] == null)
                {
                    return null;
                }
                else
                {
                    Item<string, int> found = items[k].Find(name);
                    compare = items[k].countCompare;
                    if (found != null)
                    {
                        if (found.Key == name)
                        {
                            return found;
                        }
                        else
                            return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public void Remove(string name)
        {
            if (name.Length != 0)
            {
                var k = GetHash(name);
                if (items[k] == null)
                {
                    return;
                }
                else
                {
                    if (Search(name) != null)
                    {
                        fullItem--;
                        Item<string, int> found = items[k].Find(name);
                        items[k].Remove(found);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }
        public int GetHash(string name)
        {
            int sum = 0;

            for (int i = 0; i < name.Length; i++)
            {
                sum += name[i];
            }

            return sum % size;
        }

    }
}