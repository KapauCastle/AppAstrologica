using CentroMedico.models;
using CentroMedico.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CentroMedico.viewers
{
    public partial class DetailsViewer : Window
    {
        private patientModel Patient;
        List<consulationModel> consulationList = new List<consulationModel>();
        List<historyModel> historyList = new List<historyModel>();

        public DetailsViewer(patientModel patient)
        {
            InitializeComponent();
            Patient = patient;
            LoadVisualDesign();
        }

        private void LoadVisualDesign()
        {
            if (Patient == null) return;

            txtNombrePaciente.Text = Patient.name;
            txtDatosBasicos.Text = $"📅 F. Nacim: {Patient.birthdate:dd/MM/yyyy}";

            try
            {
                using (var db = new ConsultorioContext())
                {
                    consulationList = db.Consulations.Where(c => c.patient_id == Patient.id).OrderByDescending(c => c.date).ToList();
                    historyList = db.Histories.Where(h => h.patient_id == Patient.id).ToList();
                }
                listHistorial.ItemsSource = consulationList;
                listHistories.ItemsSource = historyList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnBiografia_Click(object sender, RoutedEventArgs e)
        {
            CreateBiography biographyWindow = new CreateBiography();
            biographyWindow.Owner = this;
            biographyWindow.ShowDialog();
        }

        // CORREGIDO: Ahora abre la ventana de Marcas Personales
        private void BtnMarcas_Click(object sender, RoutedEventArgs e)
        {
            PersonalMarksViewer marksWindow = new PersonalMarksViewer(Patient.id);
            marksWindow.Owner = this;
            marksWindow.ShowDialog();
        }

        private void BtnHistopatografia_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new CreateHistoryViewer(Patient.id, Patient.name);
            historyWindow.HistorySaved += (s, args) => LoadVisualDesign();
            historyWindow.ShowDialog();
        }

        private void BtnSeguimiento_Click(object sender, RoutedEventArgs e)
        {
            CreateMedicalNote notaWindow = new CreateMedicalNote(Patient.id);
            notaWindow.NoteSaved += (s, args) => LoadVisualDesign();
            notaWindow.ShowDialog();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e) { /* Lógica editar */ }
        private void BtnEliminar_Click(object sender, RoutedEventArgs e) { /* Lógica eliminar */ }
    }
}