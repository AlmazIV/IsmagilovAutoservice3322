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

namespace Ismagilov_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            //добавляем строки
            //загрузить в список из бд
            var currentServices = IsmagilovAutoserviceEntities.GetContext().Service.ToList();
            //связать с нашим листвью
            ServiceListView.ItemsSource = currentServices;
            //добавили строки

            ComboType.SelectedIndex = 0;

            //вызываем UpdateServices()
            UpdateServices();
        }

        private void UpdateServices()
        {
            //берем из бд данные таблицы Сервис
            var currentServices = IsmagilovAutoserviceEntities.GetContext().Service.ToList();

            //прописываем фильтрацию по условию задания
            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) <= 100)).ToList();
            }

            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) < 5)).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 5 && Convert.ToInt32(p.Discount) < 15)).ToList();
            }

            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 15 && Convert.ToInt32(p.Discount) < 30)).ToList();
            }

            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 30 && Convert.ToInt32(p.Discount) < 70)).ToList();
            }

            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 70 && Convert.ToInt32(p.Discount) < 100)).ToList();
            }


            //реализуем поиск данных в листвью при вводе текста в окно поиска
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по убыванию
                ServiceListView.ItemsSource = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по возрастанию
                ServiceListView.ItemsSource = currentServices.OrderBy(p => p.Cost).ToList();
            }


        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();

        }

        private void RBurronUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();

        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();

        }


    }
}
