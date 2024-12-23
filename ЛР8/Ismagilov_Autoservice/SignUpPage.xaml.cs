﻿using System;
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
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        //добавим новое поле, которое будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if(SelectedService!=null)
                this._currentService = SelectedService;

            //При инициализации установим DataContext страницы - этот созданный объект
            //чтобы на форму подгрузить выбранные наименование услуги и длительность
            DataContext = _currentService;

            //вытащим из БД таблицу Клиент
            var _currentClient = IsmagilovAutoserviceEntities1.GetContext().Client.ToList();
            //свяжем ее с комбобоксом
            ComboClient.ItemsSource= _currentClient;
        }

        private ClientService _currentClientService=new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors=new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");
            if (TBEnd.Text == "")
                errors.AppendLine("Укажите время начала услуги");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //добавить текущие значения новой записи
            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;//тюкю нумерация с 0
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime=Convert.ToDateTime(StartDate.Text+" "+TBStart.Text);

            if (_currentClientService.ID == 0)
                IsmagilovAutoserviceEntities1.GetContext().ClientService.Add(_currentClientService);

            //сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                IsmagilovAutoserviceEntities1.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 3 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });

                if (start.Length != 2 ||!int.TryParse(start[0], out int startHour) ||!int.TryParse(start[1], out int startMin) ||startHour < 0 || startHour > 23 ||startMin < 0 || startMin > 59)
                {
                    TBEnd.Text = "";
                    return;
                }
                startHour = Convert.ToInt32(start[0].ToString()) * 60;
                startMin = Convert.ToInt32(start[1].ToString());
                StringBuilder errors = new StringBuilder();
                if (startHour > 1380 || startHour < 0)
                {
                    errors.AppendLine("Время начала услуги не должно превышать 24 часа и не должно быть меньше 0");

                }
                if (startMin > 59 || startMin < 0)
                {
                    errors.AppendLine("Время начала услуги не должно превышать 60 минут и быть меньше 0");

                }
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                int sum = startHour + startMin + _currentService.DurationInSeconds;

                int EndHour = sum / 60;
                if(EndHour>=24)
                {
                    EndHour -= 24;
                }
                int EndMin = sum % 60;
                if (EndMin < 10)
                {
                    s = EndHour.ToString() + ":0" + EndMin.ToString();
                }
                else
                {
                    s = EndHour.ToString() + ":" + EndMin.ToString();
                }
                TBEnd.Text = s;
            }
        }
    }
}
