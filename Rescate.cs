using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.IO;

namespace LogamServer
{
    /* TIMER QUE SE ENCARGA DE LA RECOLECCION DE LAS MARCAS FUERA DE LINEA */
    class Rescate
    {
        Timer rescate1;

        /* CONSTRUCTORES DE LA CLASE, LOS CUALES CREAN EL EL TIMER */
        public Rescate(double TiempoRescate)
        {
            rescate1 = new Timer();
            rescate1.Enabled = true;
            rescate1.Interval = TiempoRescate;
            rescate1.Elapsed += new ElapsedEventHandler(MuestraPorConsola);
            rescate1.Start();
        }

        public Rescate()
        {
            rescate1 = new Timer();
            rescate1.Enabled = true;
            rescate1.Interval = 10000;
            //rescate1.Interval = 600000;
            rescate1.Elapsed += new ElapsedEventHandler(MuestraPorConsola);
            rescate1.Start();
        }

        public void MuestraPorConsola(object sender, EventArgs e)
        {
            Console.WriteLine("TIMER INICIADO");
            rescate1.Stop();
            rescate1.Start();
        }
    }
}
