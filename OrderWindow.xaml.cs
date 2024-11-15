using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Baiguzin41size
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public event Action OrderWindowClosed;
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        //private OrderProduct orderOrderProduct = new OrderProduct();
        User currentUser;
        private double Cost = 0;
        private int SetDeliveryDay(List<Product> products)
        {

            bool DeliveryStatus = false;
            foreach (var p in products)
            {
                if (p.inStock <= 3)
                {
                    DeliveryStatus = true;
                }
            }

            if (DeliveryStatus)
                return 6;
            else
                return 3;
        
    }



        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {
            InitializeComponent();

            currentUser = user;
            Cost = 0;
            var currentPickups = Baiguzin_41Entities1.GetContext().PIckUpPoint.ToList();
            PickupsCombo.ItemsSource = currentPickups.Select(x => $"{x.PickupPointIndex}, {x.PickupPointLocality}, {x.PickupPointStreet}, {x.PickupPointHouseNumber}"); 
            PickupsCombo.SelectedIndex = 0;
            int currentID = selectedOrderProducts.First().OrderID; 
            currentOrder.OrderID = currentID;

            List<Order> allOrderCodes = Baiguzin_41Entities1.GetContext().Order.ToList();
            List<int> OrderCodes = new List<int>();
            foreach (var p in allOrderCodes.Select(x => $"{x.OrderCode}").ToList())
            {
                OrderCodes.Add(Convert.ToInt32(p));
            }
            Random random = new Random();

            while (true)
            {
                int num = random.Next(100, 1000);
                if (!OrderCodes.Contains(num))
                {
                    currentOrder.OrderCode = num;
                    break;
                }
            }


            foreach (Product p in selectedProducts)
            {
                var matchingOrderProduct = selectedOrderProducts.FirstOrDefault(q => q.ProductArticleNumber == p.ProductArticleNumber);

                if (matchingOrderProduct != null)
                {
                    p.Quantityto = (int)matchingOrderProduct.Quantity;

                    if (p.ProductQuantityInStock >= p.Quantityto)
                    {
                        p.ProductQuantityInStock -= p.Quantityto;
                    }
                    else
                    {
                        p.Quantityto = p.ProductQuantityInStock;
                        p.ProductQuantityInStock = 0;
                    }
                }
                else
                {
                    p.Quantityto = 1;
                }
            }



            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;


            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantityto;
            }

            TotalCost.Text = Cost.ToString();

            OrderDP.Text = DateTime.Now.ToString();
            OrderDD.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();

            TBOrderID.Text = currentID.ToString();
            ProductListView.ItemsSource = selectedProducts;


            if (currentUser != null)
            {
                currentOrder.OrderUserID = user.UserID;
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            }
            else
            {
                FIOTB.Text = "Гость";
                currentOrder.OrderUserID = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderPickupPointID = PickupsCombo.SelectedIndex + 1;
            currentOrder.OrderData = DateTime.Now;
            currentOrder.OrderDeliveryDate = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts));
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderCode = currentOrder.OrderCode;
            foreach (var p in selectedOrderProducts)
            {
                Baiguzin_41Entities1.GetContext().OrderProduct.Add(p);
            }

            Baiguzin_41Entities1.GetContext().Order.Add(currentOrder);

            try
            {
                Baiguzin_41Entities1.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                selectedOrderProducts.Clear();
                selectedProducts.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            this.Close();
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            Cost = 0;
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].Amout++;
            if (prod.ProductQuantityInStock > 0)
            {
                prod.Quantityto++;
                prod.ProductQuantityInStock--;
            }


            ProductListView.Items.Refresh();
            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantityto;
            }

            TotalCost.Text = Cost.ToString();
            OrderDD.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();


        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantityto--;
            Cost = 0;
            var selectedOP = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedProducts.IndexOf(selectedOP);

            if (prod.Quantityto == 0)
            {
                ProductPage.btnActivity = false;
                selectedOrderProducts[index].Amout = 0;
                var pr = ProductListView.SelectedItem as Product;
                selectedOrderProducts.RemoveAt(index);
                selectedProducts.RemoveAt(index);
                if (ProductListView.Items.Count == 0)
                {
                    this.Close();
                }
            }
            else
            {
                selectedOrderProducts[index].Amout--;
                    prod.ProductQuantityInStock++;
            }
            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantityto;
            }
            OrderDD.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();
            TotalCost.Text = Cost.ToString();
            ProductListView.Items.Refresh();

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantityto = 0;
            var selectedOP = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].Amout = 0;
            var pr = ProductListView.SelectedItem as Product;
            selectedOrderProducts.RemoveAt(index);
            selectedProducts.RemoveAt(index);

            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantityto;
            }
            TotalCost.Text = Cost.ToString();
            ProductListView.Items.Refresh();
            OrderDD.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();
            if (ProductListView.Items.Count == 0)
            {
                this.Close();
            }
        }
        private void OrderWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isSaved = CheckIfOrderSaved();

            if (!isSaved)
            {
                foreach (var product in selectedProducts)
                {
                    product.ProductQuantityInStock += product.Quantityto;
                }

                try
                {
                    Baiguzin_41Entities1.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при возврате товаров на склад: " + ex.Message);
                }
            }

            OrderWindowClosed?.Invoke();
        }

        private bool CheckIfOrderSaved()
        {
            return false; 
        }
    }
}