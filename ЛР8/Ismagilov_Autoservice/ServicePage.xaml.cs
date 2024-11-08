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
        int CountRecords;//Количество записей в таблице
        int CountPage;//Общее количество страниц
        int CurrentPage=0;//Текущая страница

        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;
        public ServicePage()
        {
            InitializeComponent();
            //добавляем строки
            //загрузить в список из бд
            var currentServices = IsmagilovAutoserviceEntities1.GetContext().Service.ToList();
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
            var currentServices = IsmagilovAutoserviceEntities1.GetContext().Service.ToList();

            //прописываем фильтрацию по условию задания
            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt >= 0 && p.DiscountInt <= 100)).ToList();
            }

            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt >= 0 && p.DiscountInt < 5)).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt >= 5 && p.DiscountInt < 15)).ToList();
            }

            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt >= 15 && p.DiscountInt < 30)).ToList();
            }

            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt >= 30 && p.DiscountInt < 70)).ToList();
            }

            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (p.DiscountInt  >= 70 && p.DiscountInt < 100)).ToList();
            }


            //реализуем поиск данных в листвью при вводе текста в окно поиска
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по убыванию
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по возрастанию
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }
            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices;
            //заполнение таблицы для постраничного вывода
            TableList = currentServices;
            //вызов функции отображения кол-ва страниц с параметрами:
            //направление 0 - начальная загрузка
            //0 - выбранная страница
            ChangePage(0, 0);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно редактирования/добавления услуг
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                IsmagilovAutoserviceEntities1.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = IsmagilovAutoserviceEntities1.GetContext().Service.ToList();
                UpdateServices();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Забираем Сервис, для которого нажата кнопка "Удалить"
            var currentService = (sender as Button).DataContext as Service;

            var currentClientServices = IsmagilovAutoserviceEntities1.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        IsmagilovAutoserviceEntities1.GetContext().Service.Remove(currentService);
                        IsmagilovAutoserviceEntities1.GetContext().SaveChanges();
                        ServiceListView.ItemsSource = IsmagilovAutoserviceEntities1.GetContext().Service.ToList();
                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }

        }

        private void ChangePage(int direction, int? selectedPage)//Функция отвечающая за разделение list'а
        {
            //direction - направление, 0 - начало, 1 - предыдущая страница, 2 - следующая страница
            //selectedPage - при нажатии на стрелочки передаётся null,
            //при выборе определённой страницы в этой переменной находится номер страницы

            CurrentPageList.Clear();//начальная очистка листа
            CountRecords = TableList.Count;//определение количества записей во всём списке
            //определение кол-ва страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;
            //Проверка на правильность - если 
            //CurrentPage(номер текущей страницы) "правильный"

            int min;

            if (selectedPage.HasValue)//Проверка на значение не null (т.к. может быть null)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else//если нажата стрелка
            {
                switch (direction)
                {
                    case 1://нажата кнопка ""Предыдущая страница"
                        if (CurrentPage > 0)//то есть кнопка нажата правильно и "назад" можно идти
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                            //в случаях если CurrentPage попытается выйти из диапазона внесение данных не произойдёт
                        }
                        break;

                    case 2://нажата кнопка "следующая страница"
                        if (CurrentPage < CountPage - 1)//если вперёд идти можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)//если currentPage не вышел из диапазона, то
            {
                PageListBox.Items.Clear();
                //Удаление страых значений из listbox'а номеров страниц, нужно чтобы при изменении кол-ва записей кол-во страниц динамически изменялось
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                //вывод количества записей на странице и общего количества
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBallRecords.Text = " из " + CountRecords.ToString();
                ServiceListView.ItemsSource = CurrentPageList;
                //обновить отображение списка услуг
                ServiceListView.Items.Refresh();
            }
        }



        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
                ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
                ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно записи клиента на выбранную услугу
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }
    }
}

