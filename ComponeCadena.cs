using System;
using System.Collections.Generic;
using System.Text;

namespace LogamServer
{
    class ComponeCadena
    {
        private string cadena;

        public ComponeCadena(string cadena)
        {
            this.cadena = cadena;
        }

        /* ESTA FUNCION SE LE DEBE ESPECIFICAR EL LARGO DE CODIGO DEL TRABAJADOR */
        public string DevuelveCodTarjeta()
        {
            string CodTarjeta = cadena.Remove(21);
            CodTarjeta = CodTarjeta.Remove(0,11);
            return CodTarjeta;
        }

        public string DevuelveFecha()
        {
            string Fecha = cadena.Remove(0, 21);
            Fecha = Fecha.Remove(8);
            return Fecha;
        }

        /* METODO EL CUAL PERMITE OBTENER LA HORA DEL STRING ENVIADO DESDE EL CLIENTE */
        public string DevuelveHora()
        {
            string Hora = cadena.Remove(0,29);
            Hora = Hora.Remove(6, 2);
            Hora = Hora.Insert(2, ":");
            Hora = Hora.Insert(5, ":");
            return Hora;
        }

        /* METODO EL CUAL PERMITE OBTENER EL EVENTO ENVIADO DESDE EL CLIENTE */
        public string DevuelveEvento()
        {
            string evento = cadena.Remove(0, 34);
            evento = evento.Remove(0, 1);
            evento = evento.Remove(1, 1);
            return evento;
        }

        /* METODOD EL CUAL PERMITE OBTENER LA FECHA Y LA HORA ENVIADA DESDE EL CLIENTE */
        public string DevuelveFechaHora()
        {
            string FechaHora = cadena.Remove(0,21);
            FechaHora = FechaHora.Remove(14);
            FechaHora = FechaHora.Insert(4, "/");
            FechaHora = FechaHora.Insert(7, "/");
            FechaHora = FechaHora.Insert(10, " ");
            FechaHora = FechaHora.Insert(13, ":");
            FechaHora = FechaHora.Insert(16, ":");
            return FechaHora;
        }
    }
}
