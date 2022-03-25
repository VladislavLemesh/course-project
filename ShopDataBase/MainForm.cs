using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShopDataBase
{
    public partial class MainForm : Form
    {
        public DictShop dictShop;
        public NamePrice namePrice;
        public AVLTreeShop ShopTree;
        public AVLTreePrice PriceTree;
        public System.Collections.Generic.List<Tuple<Item<string, string>, Item<string, int>>> dictGeneral;
        public bool isSaved = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dictShop = new DictShop();

            for (int i = 0; i < dictShop.size; i++)
            {
                if (dictShop.Items[i] != null && dictShop.Items[i].Key != "empty")
                {
                    dataGridView1.Rows.Add(dictShop.GetHash(dictShop.Items[i].Key, dictShop.Items[i].Value), i, dictShop.Items[i].Key, dictShop.Items[i].Value);
                }
            }
            dataGridView1.ClearSelection();

            namePrice = new NamePrice();
            for (int i = 0; i < namePrice.size; i++)
            {
                if (namePrice.items[i] != null)
                {
                    var current = namePrice.items[i].Head;
                    while (current != null)
                    {
                        dataGridView2.Rows.Add(namePrice.GetHash(current.Data.Key), current.Data.Key, current.Data.Value);
                        current = current.Next;
                    }
                }
            }
            dataGridView2.ClearSelection();

            ShopTree = new AVLTreeShop();
            PriceTree = new AVLTreePrice();
            dictGeneral = new System.Collections.Generic.List<Tuple<Item<string, string>, Item<string, int>>>();
            GeneralData_Load("General.txt");

            foreach (var item in dictGeneral)
            {
                dataGridView3.Rows.Add(item.Item1.Key, item.Item1.Value, item.Item2.Key, item.Item2.Value);
            }
            dataGridView3.ClearSelection();
        }

        private void buttonAddShop_Click(object sender, EventArgs e)
        {
            AddShopForm AddForm = new AddShopForm(this);
            AddForm.Show();
        }

        public void RefreshShopTable()
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < dictShop.size; i++)
            {
                if (dictShop.Items[i] != null && dictShop.Items[i].Key != "empty")
                {
                    dataGridView1.Rows.Add(dictShop.GetHash(dictShop.Items[i].Key, dictShop.Items[i].Value), i, dictShop.Items[i].Key, dictShop.Items[i].Value);
                }
            }
            dataGridView1.ClearSelection();
        }

        public void RefreshPriceTable()
        {
            dataGridView2.Rows.Clear();
            for (int i = 0; i < namePrice.size; i++)
            {
                if (namePrice.items[i] != null)
                {
                    var current = namePrice.items[i].Head;
                    while (current != null)
                    {
                        dataGridView2.Rows.Add(namePrice.GetHash(current.Data.Key), current.Data.Key, current.Data.Value);
                        current = current.Next;
                    }
                }
            }
            dataGridView2.ClearSelection();
        }

        public void RefreshGeneralTable()
        {
            dataGridView3.Rows.Clear();

            foreach (var item in dictGeneral)
            {
                var currentShop = ShopTree.Find(item.Item1.Key + item.Item1.Value);
                var currentPrice = PriceTree.Find(item.Item2.Key);
                if (currentShop != null && currentPrice != null)
                {
                    dataGridView3.Rows.Add(item.Item1.Key, item.Item1.Value, item.Item2.Key, item.Item2.Value);
                }                
            }
            dataGridView3.ClearSelection();
        }

        private void buttonRemoveShop_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.Selected)
            {
                var delShop = ShopTree.Find(dataGridView1.CurrentRow.Cells[2].Value.ToString() + dataGridView1.CurrentRow.Cells[3].Value.ToString());
                if (delShop != null)
                {                    
                    while (ShopTree.Find(dataGridView1.CurrentRow.Cells[2].Value.ToString() + dataGridView1.CurrentRow.Cells[3].Value.ToString()) != null)
                    {
                        ShopTree.Delete(delShop);
                    }
                    for (int i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        if (dataGridView3.Rows[i].Cells[0].Value.ToString() == dataGridView1.CurrentRow.Cells[2].Value.ToString() &&
                            dataGridView3.Rows[i].Cells[1].Value.ToString() == dataGridView1.CurrentRow.Cells[3].Value.ToString())   
                        {
                            var delPrice = PriceTree.Find(dataGridView3.Rows[i].Cells[2].Value.ToString());

                            if (delShop != null && delPrice != null)
                            {
                                dictGeneral.Remove(Tuple.Create(delShop, delPrice));
                                PriceTree.Delete(delPrice);
                            }
                        }
                    }
                    Notification NotForm = new Notification("Связка удалена!");
                    NotForm.Show();
                }
                else
                {
                    Notification NotForm = new Notification("Запись удалена!");
                    NotForm.Show();
                }
                dictShop.Remove(dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString());
                RefreshShopTable();
                RefreshGeneralTable();
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Введите название")
                textBox1.Text = null;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == null || textBox1.Text == "")
                textBox1.Text = "Введите название";
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Введите адрес")
                textBox2.Text = null;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == null || textBox2.Text == "")
                textBox2.Text = "Введите адрес";
        }

        private void buttonSearchShop_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            if (textBox1.Text == null || textBox2.Text == null || textBox1.Text.Equals("Введите название") || textBox2.Text.Equals("Введите адрес"))
                return;
            if (dictShop.Search(textBox1.Text, textBox2.Text) != null)
            {
                Notification NotForm = new Notification("Запись найдена! Сравнений: " + dictShop.compare.ToString());
                NotForm.Show();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[2].Value.ToString() == textBox1.Text && dataGridView1.Rows[i].Cells[3].Value.ToString() == textBox2.Text)
                    {
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }                        
                }
            }
            else
            {
                Notification NotForm = new Notification("Запись не найдена! Сравнений: " + dictShop.compare.ToString());
                NotForm.Show();
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = null;
        }

        private void textBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox2.Text = null;
        }
        
        private void buttonAddNamePrice_Click(object sender, EventArgs e)
        {
            AddNamePriceForm AddForm = new AddNamePriceForm(this);
            AddForm.Show();
        }

        private void buttonRemoveNamePrice_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.Selected)
            {
                if (PriceTree.Find(dataGridView2.CurrentRow.Cells[1].Value.ToString()) != null)
                {
                    var itemNamePrice = PriceTree.Find(dataGridView2.CurrentRow.Cells[1].Value.ToString());
                    while (PriceTree.Find(dataGridView2.CurrentRow.Cells[1].Value.ToString()) != null)
                    {
                        PriceTree.Delete(itemNamePrice);
                    }
                    for (int i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        if (dataGridView3.Rows[i].Cells[2].Value.ToString() == dataGridView2.CurrentRow.Cells[1].Value.ToString())
                        {
                            var delShop = ShopTree.Find(dataGridView3.Rows[i].Cells[0].Value.ToString() + dataGridView3.Rows[i].Cells[1].Value.ToString());

                            if (itemNamePrice != null && delShop != null)
                            {
                                dictGeneral.Remove(Tuple.Create(delShop, itemNamePrice));
                                ShopTree.Delete(delShop);
                            }
                        }
                    }
                    Notification NotForm = new Notification("Связка удалена!");
                    NotForm.Show();
                }
                else
                {
                    Notification NotForm = new Notification("Запись удалена!");
                    NotForm.Show();
                }

                namePrice.Remove(dataGridView2.CurrentRow.Cells[1].Value.ToString());
                RefreshPriceTable();
                RefreshGeneralTable();                
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Введите название")
                textBox3.Text = null;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == null || textBox3.Text == "")
                textBox3.Text = "Введите название";
        }

        private void textBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox3.Text = null;
        }

        private void buttonSearchNamePrice_Click(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
            if (namePrice.Search(textBox3.Text) != null)
            {
                for (int i = 0; i < namePrice.fullItem; i++)
                {
                    if (dataGridView2.Rows[i].Cells[1].Value.ToString() == textBox3.Text)
                    {
                        dataGridView2.Rows[i].Selected = true;
                        Notification NotForm = new Notification("Запись найдена! Сравнений: " + namePrice.compare.ToString());
                        NotForm.Show();
                        return;
                    }
                }
            }
            else
            {
                Notification NotForm = new Notification("Запись не найдена! Сравнений: " + namePrice.compare.ToString());
                NotForm.Show();
            }
        }

        public void GeneralData_Load(string path)
        {                      
            if (File.Exists(path))                           
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))                
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))                    
                        while (!sr.EndOfStream)
                        {
                            string[] str = sr.ReadLine().Split(';');
                            var ShopItem = dictShop.Search(str[0], str[1]);
                            var PriceItem = namePrice.Search(str[2]);

                            if (ShopItem != null && PriceItem != null)
                            {
                                dictGeneral.Add(Tuple.Create(ShopItem, PriceItem));
                                ShopTree.Add(ShopItem);
                                PriceTree.Add(PriceItem);
                            }
                        }     
        }

        private void buttonGeneralAdd_Click(object sender, EventArgs e)
        {
            AddGeneral AddForm = new AddGeneral(this);
            AddForm.Show();            
        }

        private void buttonGeneralDel_Click(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentCell.Selected)
            {
                var delShop = ShopTree.Find(dataGridView3.CurrentRow.Cells[0].Value.ToString() + dataGridView3.CurrentRow.Cells[1].Value.ToString());
                var delPrice = PriceTree.Find(dataGridView3.CurrentRow.Cells[2].Value.ToString()); 

                if (delPrice != null && delShop != null)
                {
                    dictGeneral.Remove(Tuple.Create(delShop, delPrice));
                    ShopTree.Delete(delShop);
                    PriceTree.Delete(delPrice);                    
                    Notification NotForm = new Notification("Запись удалена!");
                    NotForm.Show();                    
                }
                else
                {
                    Notification NotForm = new Notification("Запись не найдена!");
                    NotForm.Show();
                }
                RefreshGeneralTable();
            }            
        }

        private void buttonGeneralShopSearch_Click(object sender, EventArgs e)
        {
            SearchGeneralShop SearchForm = new SearchGeneralShop(this);
            SearchForm.Show();
        }

        public void ShowShops(Item<string, string> shop)
        {
            dataGridView3.ClearSelection();

            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                if (dataGridView3.Rows[i].Cells[0].Value.ToString() == shop.Key && dataGridView3.Rows[i].Cells[1].Value.ToString() == shop.Value)
                {
                    dataGridView3.Rows[i].Selected = true;
                }
            }
        }

        public void ShowPrice(Item<string, int> price)
        {
            dataGridView3.ClearSelection();

            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                if (dataGridView3.Rows[i].Cells[2].Value.ToString() == price.Key)
                {
                    dataGridView3.Rows[i].Selected = true;
                }
            }
        }

        private void buttonGeneralPriceSearch_Click(object sender, EventArgs e)
        {
            SearchGeneralPrice priceSearchForm = new SearchGeneralPrice(this);
            priceSearchForm.Show();
        }

        public void SaveData()
        {
            string path = "Shops.txt";

            if (File.Exists(path))            
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))                
                    using (StreamWriter sw = new StreamWriter(fs,Encoding.UTF8))                    
                        for (int i = 0; i < dictShop.size; i++)                        
                            if (dictShop.Items[i] != null && dictShop.Items[i].Key != "empty")                        
                                sw.WriteLine(dictShop.Items[i].Key + ";" + dictShop.Items[i].Value);
                            
            
            path = "General.txt";

            if (File.Exists(path))            
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))                
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        foreach (var item in dictGeneral)                                                    
                            sw.WriteLine(item.Item1.Key + ";" + item.Item1.Value + ";" + item.Item2.Key + ";" + item.Item2.Value);

            path = "Price.txt";

            if (File.Exists(path))
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    for (int i = 0; i < namePrice.size; i++)                    
                        if (namePrice.items[i] != null)
                        {
                            var current = namePrice.items[i].Head;
                            while (current != null)
                            {
                                sw.WriteLine(current.Data.Key + ";" + current.Data.Value.ToString());
                                current = current.Next;
                            }
                        }  
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !isSaved; 
            if (!isSaved)
            {
                ExitForm exitForm = new ExitForm(this);
                exitForm.Show();
            }                      
        }
    }
}