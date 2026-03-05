using System;
using System.Windows;
using System.Windows.Controls;
using CentroMedico.Database;
using CentroMedico.models;

namespace CentroMedico.viewers
{
    public partial class CreatePatientViewer : Window
    {
        // El signo ? evita la advertencia CS8618 (valor nulo)
        public event EventHandler? PatientCreated;

        private readonly string[] signosList = { "Aries ♈", "Tauro ♉", "Géminis ♊", "Cáncer ♋", "Leo ♌", "Virgo ♍",
                                                "Libra ♎", "Escorpio ♏", "Sagitario ♐", "Capricornio ♑", "Acuario ♒", "Piscis ♓" };

        public CreatePatientViewer()
        {
            InitializeComponent();
            LoadFormLists();
            dobInput.DisplayDateEnd = DateTime.Now;
        }

        private void LoadFormLists()
        {
            // Llenar Horas
            for (int i = 1; i <= 12; i++) hourPicker.Items.Add(i.ToString("D2"));
            hourPicker.SelectedIndex = 11;

            // Llenar Minutos
            for (int i = 0; i < 60; i += 5) minutePicker.Items.Add(i.ToString("D2"));
            minutePicker.SelectedIndex = 0;

            // Llenar Signos
            foreach (var s in signosList)
            {
                ascSignPicker.Items.Add(s);
                moonSignPicker.Items.Add(s);
            }
        }

        private void CalculateData(object sender, SelectionChangedEventArgs e)
        {
            if (dobInput.SelectedDate == null) return;
            DateTime birth = dobInput.SelectedDate.Value;

            int age = DateTime.Today.Year - birth.Year;
            if (birth > DateTime.Today.AddYears(-age)) age--;
            ageResultLabel.Text = $"{age} años";

            sunSignResult.Text = GetZodiacSign(birth.Day, birth.Month);
        }

        private string GetZodiacSign(int day, int month)
        {
            if ((month == 3 && day >= 21) || (month == 4 && day <= 19)) return "Aries ♈";
            if ((month == 4 && day >= 20) || (month == 5 && day <= 20)) return "Tauro ♉";
            if ((month == 5 && day >= 21) || (month == 6 && day <= 20)) return "Géminis ♊";
            if ((month == 6 && day >= 21) || (month == 7 && day <= 22)) return "Cáncer ♋";
            if ((month == 7 && day >= 23) || (month == 8 && day <= 22)) return "Leo ♌";
            if ((month == 8 && day >= 23) || (month == 9 && day <= 22)) return "Virgo ♍";
            if ((month == 9 && day >= 23) || (month == 10 && day <= 22)) return "Libra ♎";
            if ((month == 10 && day >= 23) || (month == 11 && day <= 21)) return "Escorpio ♏";
            if ((month == 11 && day >= 22) || (month == 12 && day <= 21)) return "Sagitario ♐";
            if ((month == 12 && day >= 22) || (month == 1 && day <= 19)) return "Capricornio ♑";
            if ((month == 1 && day >= 20) || (month == 2 && day <= 18)) return "Acuario ♒";
            return "Piscis ♓";
        }

        private void saveData(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fullNameInput.Text) || dobInput.SelectedDate == null)
            {
                MessageBox.Show("Por favor, llena los campos de nombre y fecha.");
                return;
            }

            try
            {
                // Captura de datos de ComboBoxes
                string birthTime = $"{hourPicker.SelectedItem}:{minutePicker.SelectedItem} {((ComboBoxItem)ampmPicker.SelectedItem).Content}";
                string gender = (genderPicker.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "No especificado";
                string maritalStatus = (maritalStatusPicker.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "No especificado";

                using (var db = new ConsultorioContext())
                {
                    var newPatient = new patientModel
                    {
                        name = fullNameInput.Text.ToUpper(),
                        birthdate = dobInput.SelectedDate.Value,

                        // Nota: Asegúrate de que tu clase 'patientModel' tenga estos campos.
                        // Si te marca error aquí, es porque debes agregarlos a tu clase patientModel.cs
                        /*
                        gender = gender,
                        marital_status = maritalStatus,
                        education = educationInput.Text,
                        religion = religionInput.Text,
                        occupation = occupationInput.Text,
                        socioeconomic_level = socioeconomicInput.Text,
                        address = addressInput.Text,
                        sun_sign = sunSignResult.Text,
                        moon_sign = moonSignPicker.SelectedItem?.ToString(),
                        asc_sign = ascSignPicker.SelectedItem?.ToString(),
                        birth_time = birthTime
                        */
                    };

                    db.Patients.Add(newPatient);
                    db.SaveChanges();
                }

                MessageBox.Show("Registro manifestado con éxito ✨");
                PatientCreated?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void fullNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int caretIndex = fullNameInput.SelectionStart;
            fullNameInput.Text = fullNameInput.Text.ToUpper();
            fullNameInput.SelectionStart = caretIndex;
        }

        private void closeModal(object sender, RoutedEventArgs e) => this.Close();
    }
}