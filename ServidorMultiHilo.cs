using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace LogamServer
{
    /* CLASE QUE SE ENCARGAs DE TRATAR LAS PETICIONES DE CONEXION QUE RECIBE EL SERVIDOR */
    class ServidorMultiHilo
    {
        Socket cliente;
        Thread ClientThread;
        ManejoArchivo myarchivo = new ManejoArchivo();
    
        /* CONSTRUCTOR DE LA CLASE, EN DONDE SE ESTABLECE EL TIEMPO MAXIMO DE RESPUESTA QUE PUEDE ESPERAR EL 
         * SERVIDOR LA ENTRADA DE INFORMACION */
        public ServidorMultiHilo(Socket cliente)
        {
            this.cliente = cliente;
            /* ESTABLECE EL TIEMPO MAXIMO DE ESPERA EN LA RECEPCION DE INFORMACION */
            cliente.ReceiveTimeout = Convert.ToInt32(myarchivo.Datos[7].ToString());           
            ClientThread = new Thread(Comunicacion);
            ClientThread.IsBackground = true;
            ClientThread.Start();
        }

        public void Comunicacion()
        {
            byte[] dataReceive = new byte[70];
            byte[] dataSend = new byte[70];
            string IP = cliente.RemoteEndPoint.ToString();
            string[] Datos = IP.Split(':');
            IP = Datos[0].ToString();
            int largoCadena = Convert.ToInt32(myarchivo.Datos[6].ToString());
            
            while (cliente.Connected == true)
            {
                try
                {       
                    /* INSTRUCCIONES QUE SE ENCARGAN DE LA COMUNICACION ENTRE EL SERVIDOR Y EL CLIENTE */
                        int bytesReci = cliente.Receive(dataReceive);
                        string mensaje = Encoding.Default.GetString(dataReceive, 0, bytesReci);
                        
                        if (mensaje == "1012ghost")
                        {
                            Console.WriteLine(">> {0} esta en Linea", cliente.RemoteEndPoint);
                            cliente.Disconnect(true);
                        }
                        else
                        {
                            ComponeCadena cadena = new ComponeCadena(mensaje,largoCadena);
                            string Resul = ConexioSQL(cadena.DevuelveCodTarjeta(), cadena.DevuelveFecha(), cadena.DevuelveHora(), IP, cadena.DevuelveEvento(), cadena.DevuelveFechaHora());
                            Console.WriteLine("{0} {1}", cliente.RemoteEndPoint, Resul);
                            dataSend = Encoding.Default.GetBytes(Resul);
                            cliente.Send(dataSend);
                            cliente.Disconnect(true);
                        }
                }

                /* EXCEPTION LANZADA CUANDO SE CUMPLE EL TIEMPO MAXIMO DE ESPERA EN LA RECEPCION DE INFORMACION */
                catch (SocketException)
                {
                    Console.WriteLine(" >> El cliente {0} se ha desconectado ",cliente.RemoteEndPoint);
                    cliente.Disconnect(true);
                }

                catch (Exception)
                {
                    Console.WriteLine(" >> El cliente {0} se ha desconectado ", cliente.RemoteEndPoint);
                    cliente.Disconnect(true);
                }
            } 
        }

        /* FUNCION LA CUAL ENVIA LA CADENA A EL SERVIDOR */
        public string ConexioSQL(string nroTarjeta,string FechaMarca,string HoraMarca,string codLector, string evento, string FechaHora)
        {
             SqlConnection cone;
             string ServidorBBDD = myarchivo.Datos[2].ToString();
             string BBDD = myarchivo.Datos[3].ToString();
             string USUARIO = myarchivo.Datos[4].ToString();
             string PASS = myarchivo.Datos[5].ToString();

            try
            {
                string Resultado;
                string cadena = (@"data source =" +  ServidorBBDD + "; initial catalog =" + BBDD + "; user id =" + USUARIO +"; password =" + PASS +"");
                cone = new SqlConnection(cadena);
                SqlCommand ProcedureStore = new SqlCommand();

                
                cone.Open();

                ProcedureStore.Connection = cone;
                ProcedureStore.CommandType = CommandType.StoredProcedure;
                ProcedureStore.CommandText = "Proc_JAX_Controles_Generales";
                ProcedureStore.CommandTimeout = 10;

                ProcedureStore.Parameters.Add(new SqlParameter("@Nro_Tarjeta", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@FechaMarca1", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@HoraMarca", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@Cod_Lector", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@Evento", SqlDbType.Int));
                ProcedureStore.Parameters.Add(new SqlParameter("@Fecha_con_Hora1", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@Cod_Error", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@Cod_InternoTrab", SqlDbType.Char));
                ProcedureStore.Parameters.Add(new SqlParameter("@Cod_ServCasino", SqlDbType.Char)); 
                ProcedureStore.Parameters.Add(new SqlParameter("@Cod_Empresa", SqlDbType.Char));


                ProcedureStore.Parameters["@Nro_Tarjeta"].Value = nroTarjeta;
                ProcedureStore.Parameters["@FechaMarca1"].Value = FechaMarca;
                ProcedureStore.Parameters["@HoraMarca"].Value = HoraMarca;
                ProcedureStore.Parameters["@Cod_Lector"].Value = codLector;
                ProcedureStore.Parameters["@Evento"].Value = evento;
                ProcedureStore.Parameters["@Fecha_con_Hora1"].Value = FechaHora;

                ProcedureStore.Parameters["@Cod_Error"].Direction = ParameterDirection.Output;
                ProcedureStore.Parameters["@Cod_Error"].Size = 100;
                ProcedureStore.Parameters["@Cod_InternoTrab"].Direction = ParameterDirection.Output;
                ProcedureStore.Parameters["@Cod_InternoTrab"].Size = 13;
                ProcedureStore.Parameters["@Cod_ServCasino"].Direction = ParameterDirection.Output;
                ProcedureStore.Parameters["@Cod_ServCasino"].Size = 3;
                ProcedureStore.Parameters["@Cod_Empresa"].Direction = ParameterDirection.Output;
                ProcedureStore.Parameters["@Cod_Empresa"].Size = 4;

                ProcedureStore.ExecuteScalar();

                Resultado = Convert.ToString(ProcedureStore.Parameters["@Cod_Error"].Value);
                cone.Close();
                return Resultado;
            }
            catch (Exception)
            {
                Console.WriteLine("Hay un problema en la conexion hacia el servidor");
                return "Servidor Fuera de Linea";
            }
        }
    }
}
