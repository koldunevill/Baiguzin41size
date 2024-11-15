using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Baiguzin41size
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        int countfull = 0;
        int count = 0;
        User currentUser;
        int newOrderId;
        public static bool btnActivity = false;
        int Quantity = 0;

        List<Product> selectedProducts = new List<Product>();
        List<OrderProduct> selectedOrderProduct = new List<OrderProduct>();
       
        public ProductPage(User user)
        {
            InitializeComponent();

            if (selectedProducts.Count == 0)
            {
                BTNOrder.Visibility = Visibility.Hidden;
            }
            currentUser = user;

            if (user != null)
            {

                FIOTB.Text = user.UserSurname + "" + user.UserName + "" + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
            }
            else
            {
                FIOTB.Text = "Гость";
                RoleTB.Text = "";
            }

            ComboType2.SelectedIndex = 0;
            
            var currentProduct = Baiguzin_41Entities1.GetContext().Product.ToList();
            
            ProductListView.ItemsSource = currentProduct;
            
            countfull = Baiguzin_41Entities1.GetContext().Product.Count();
           

            UpdateProduct();

        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPAge());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }


        private void RBtnUP_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void RbtnDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void UpdateProduct()
        {
            var currentProduct = Baiguzin_41Entities1.GetContext().Product.ToList();
            if (ComboType2.SelectedIndex ==0)
            {
                currentProduct = currentProduct.Where(p => (p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount <= 100)).ToList();
            }
            if (ComboType2.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => (p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 10)).ToList();
            }
            if (ComboType2.SelectedIndex ==  2)
            {
                currentProduct = currentProduct.Where(p => (p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15)).ToList();
            }
            if (ComboType2.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => (p.ProductDiscountAmount >= 15 && p.ProductDiscountAmount < 100)).ToList();
            }


            currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (RBtnUP.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderBy(p => p.ProductCost).ToList();
            }
            if (RbtnDown.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderByDescending(p => p.ProductCost).ToList();
            }

            ProductListView.ItemsSource = currentProduct;
            count = currentProduct.Count();
            CountTB.Text = $"Выведено {count} из {countfull}";
        }

        private void ComboType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;

                var existingOrderProduct = selectedOrderProduct.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

                if (existingOrderProduct != null)
                {
                    existingOrderProduct.Quantity++;

                    var selectedProduct = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                    if (selectedProduct != null)
                    {
                        selectedProduct.Quantityto = existingOrderProduct.Quantity;
                    }
                }
                else
                {
                    var newOrderProd = new OrderProduct
                    {
                        OrderID = newOrderId,
                        ProductArticleNumber = prod.ProductArticleNumber,
                        Quantity = 1
                    };
                    selectedOrderProduct.Add(newOrderProd);
                    prod.Quantityto = 1; 
                    selectedProducts.Add(prod);
                }

                BTNOrder.Visibility = Visibility.Visible;

                ProductListView.Items.Refresh();

                ProductListView.SelectedIndex = -1;
            }
            UpdateProduct();
        }

        private void ShoeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BTNOrder_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();
            
            OrderWindow orderWindows = new OrderWindow(selectedOrderProduct, selectedProducts, currentUser  );
            orderWindows.ShowDialog();
            BTNOrder.Visibility = Visibility.Hidden;
            selectedOrderProduct.Clear();
        }
    }
}