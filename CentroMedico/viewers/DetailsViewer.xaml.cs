using CentroMedico.models;
using CentroMedico.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            // Datos de Identidad (Letras Grandes)
            txtNombrePaciente.Text = Patient.name;
            txtDatosBasicos.Text = $"F. Nacim: {Patient.birthdate:dd/MM/yyyy}";

            // Sincronización de peso y altura
            UpdateWeightAndHeight();
            txtUltimosDatos.Text = $"Peso: {Patient.weight} kg  •  Altura: {Patient.height} cm";

            // Energía del Consultante
            txtTipoPaciente.Text = string.IsNullOrEmpty(Patient.type_patient) ? "ESTÁNDAR" : Patient.type_patient;

            // --- SECCIÓN ASTRAL (COMENTADA HASTA ACTUALIZAR BD) ---
            // lblSol.Text = Patient.sun_sign ?? "--";
            // lblAsc.Text = Patient.ascendant ?? "--";
            // lblLuna.Text = Patient.moon_sign ?? "--";

            // Por ahora, dejamos valores fijos para que no se vea vacío
            lblSol.Text = "--";
            lblAsc.Text = "--";
            lblLuna.Text = "--";

            try
            {
                using (var db = new ConsultorioContext())
                {
                    consulationList = db.Consulations
                        .Where(c => c.patient_id == Patient.id)
                        .OrderByDescending(c => c.date)
                        .ToList();

                    historyList = db.Histories
                        .Where(h => h.patient_id == Patient.id)
                        .ToList();
                }

                listHistorial.ItemsSource = consulationList;
                listHistories.ItemsSource = historyList;
            }
            catch (Exception ex)
            {
                // Mensaje con estilo místico pero útil para ingeniería
                MessageBox.Show($"Error al canalizar el Akasha: {ex.Message}");
            }
        }

        private void ReloadPatientData()
        {
            try
            {
                using (var db = new ConsultorioContext())
                {
                    var updated = db.Patients.Find(Patient.id);
                    if (updated != null)
                    {
                        Patient.name = updated.name;
                        Patient.type_patient = updated.type_patient;
                        Patient.birthdate = updated.birthdate;
                        Patient.weight = updated.weight;
                        Patient.height = updated.height;
                        // Aquí también comentarás los campos astrales al recargar
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al sincronizar expediente: {ex.Message}");
            }
        }

        private void UpdateWeightAndHeight()
        {
            try
            {
                using (var db = new ConsultorioContext())
                {
                    var lastConsult = db.Consulations
                        .Where(c => c.patient_id == Patient.id)
                        .OrderByDescending(c => c.date)
                        .FirstOrDefault();

                    if (lastConsult != null)
                    {
                        var patientDb = db.Patients.Find(Patient.id);
                        if (patientDb != null)
                        {
                            patientDb.weight = lastConsult.weight;
                            patientDb.height = lastConsult.height;
                            db.SaveChanges();
                            Patient.weight = lastConsult.weight;
                            Patient.height = lastConsult.height;
                        }
                    }
                }
            }
            catch { /* Ignorar errores de actualización automática */ }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            UpdatePatientViewer updateModal = new UpdatePatientViewer(Patient);
            updateModal.PatientUpdated += (s, args) => { ReloadPatientData(); LoadVisualDesign(); };
            updateModal.ShowDialog();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Desvanecer este expediente?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new ConsultorioContext())
                {
                    var p = db.Patients.Find(Patient.id);
                    if (p != null) { db.Patients.Remove(p); db.SaveChanges(); this.Close(); }
                }
            }
        }

        private void BtnSeguimiento_Click(object sender, RoutedEventArgs e)
        {
            CreateMedicalNote notaWindow = new CreateMedicalNote(Patient.id);
            notaWindow.NoteSaved += (s, args) => LoadVisualDesign();
            notaWindow.ShowDialog();
        }

        private void BtnHistopatografia_Click(object sender, RoutedEventArgs e)
        {
            CreateHistoryViewer historyWindow = new CreateHistoryViewer(Patient.id, Patient.name);
            historyWindow.HistorySaved += (s, args) => LoadVisualDesign();
            historyWindow.ShowDialog();
        }
    }
}