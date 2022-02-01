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
using System.Windows.Shapes;

namespace HospitalApp
{
    /// <summary>
    /// Логика взаимодействия для PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();
            comboBoxReportType.ItemsSource = new string[] { "Записанные пациенты", "Свободное время" };
            comboBoxReportType.SelectedIndex = 0;
            comboBoxDoctor.ItemsSource = App.DataBase.Doctor.ToList();
            comboBoxDoctor.SelectedIndex = 0;
        }

        private void comboBoxReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxReportType.SelectedIndex == 0)
            {
                scrollAllData.Visibility = Visibility.Visible;
                scrollFreeTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                scrollAllData.Visibility = Visibility.Collapsed;
                scrollFreeTime.Visibility = Visibility.Visible;
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var doctor = (Entities.Doctor)comboBoxDoctor.SelectedItem;
            lblNothingFound.Visibility = Visibility.Collapsed;
            try
            {
                if (comboBoxReportType.SelectedIndex == 0)
                {
                    if (datePickerFrom.SelectedDate != null && datePickerUntill.SelectedDate != null)
                    {
                        if (datePickerFrom.SelectedDate < datePickerUntill.SelectedDate)
                        {
                            lblInitializePage.Visibility = Visibility.Collapsed;
                            lblNothingFound.Visibility = Visibility.Collapsed;
                            txtBlockDoctorAllData.Text += comboBoxDoctor.SelectedItem.ToString();
                            txtBlockPeriodAllData.Text = "за период: " + datePickerFrom.SelectedDate.Value.Date.ToShortDateString() + " - " + datePickerUntill.SelectedDate.Value.Date.ToShortDateString();
                            txtBlockPeriodAllData.Visibility = Visibility.Visible;
                            DGridAllData.ItemsSource = App.DataBase.AppointmentsViewAllData.Where(p => p.Date > datePickerFrom.SelectedDate && p.Date < datePickerUntill.SelectedDate && p.DoctorInfo.Contains(doctor.LastName)).ToList();
                        }
                        else
                        {
                            MessageBox.Show("Начальная дата не можеть быть меньше конечной", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    else
                    {
                        lblInitializePage.Visibility = Visibility.Collapsed;
                        DGridAllData.ItemsSource = App.DataBase.AppointmentsViewAllData.Where(p => p.DoctorInfo.Contains(doctor.LastName)).ToList();
                    }
                    if (DGridAllData.Items.Count == 0)
                    {
                        lblNothingFound.Visibility = Visibility.Visible;
                        return;
                    }
                }
                else
                {
                    if (datePickerFrom.SelectedDate != null && datePickerUntill.SelectedDate != null)
                    {
                        if (datePickerFrom.SelectedDate < datePickerUntill.SelectedDate)
                        {
                            lblInitializePage.Visibility = Visibility.Collapsed;
                            lblNothingFound.Visibility = Visibility.Collapsed;
                            txtBlockPeriodFreeTime.Text = "за период: " + datePickerFrom.SelectedDate.Value.Date.ToShortDateString() + " - " + datePickerUntill.SelectedDate.Value.Date.ToShortDateString();
                            txtBlockPeriodFreeTime.Visibility = Visibility.Visible;
                            DGridReportFreeTime.ItemsSource = App.DataBase.DoctorSchedule.Where(p => p.Date > datePickerFrom.SelectedDate && p.Date < datePickerUntill.SelectedDate && p.DoctorId.Equals(doctor.Id)).ToList();
                        }
                        else
                        {
                            MessageBox.Show("Начальная дата не можеть быть меньше конечной", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    else
                    {
                        lblInitializePage.Visibility = Visibility.Collapsed;
                        DGridReportFreeTime.ItemsSource = App.DataBase.DoctorSchedule.Where(p => p.DoctorId.Equals(doctor.Id)).ToList();
                    }
                    if (DGridReportFreeTime.Items.Count == 0)
                    {
                        lblNothingFound.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка обновления данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnResetDate_Click(object sender, RoutedEventArgs e)
        {
            datePickerFrom.SelectedDate = null;
            datePickerUntill.SelectedDate = null;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxReportType.SelectedIndex == 0)
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource idpSource = flowDocumentAllData;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, $"Report_AllData_From_{DateTime.Now.ToShortDateString()}");
                }
            }
            if (comboBoxReportType.SelectedIndex == 1)
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource idpSource = flowDocumentFreeTime;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, $"Report_Records_From_{DateTime.Now.ToShortDateString()}");
                }
            }
        }
    }
}
