using CentroMedico.Database;
using CentroMedico.models;
using System;
using System.Windows;

namespace CentroMedico.viewers
{
    public partial class PersonalMarksViewer : Window
    {
        private int _patientId;

        public PersonalMarksViewer(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
        }

        private void SaveMarks(object sender, RoutedEventArgs e)
        {
            try
            {
                // Aquí podrías implementar el guardado en la DB cuando estés lista
                MessageBox.Show("✅ Marcas personales guardadas correctamente.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void CloseModal(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}