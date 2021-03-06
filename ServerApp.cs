﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace LogamServer
{
    class ServerApp
    {
        static void Main(string[] args)
        {
            /* VARIABLES */
            ManejoArchivo myarchivo = new ManejoArchivo();
            int puerto = Convert.ToInt32(myarchivo.Datos[1].ToString());
            Socket SocketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint direccionServidor = new IPEndPoint(IPAddress.Parse(myarchivo.Datos[0].ToString()), puerto);
            SocketServidor.Bind(direccionServidor);
            Socket SocketCliente;

            /* INSTRUCCION LA CUAL ESPECIFICA LA CANTIDAD DE PETICIONES EN COLA ANTES DE QUE EL SERVIDOR DIGA 
             * QUE ESTA COPADO */

            SocketServidor.Listen(200);
            Console.WriteLine(">> JAX Server Iniciado");

            /* CICLO QUE SE ENCARGA DE RECIBIR LA PETICIONES DE LOS CLIENTES QUE SE INTENTAN CONECTAR
             * AL SERVIDOR */
            while (true)
            {
                Console.WriteLine("Esperando Conexiones");
                //Rescate rescateMarcas = new Rescate();

                try
                {
                    /* INSTRUCCION QUE SE ENCARGA DE TOMAR LAS PETICIONES DE LOS CLIENTES ENTRANTES */
                    SocketCliente = SocketServidor.Accept();
                    Console.WriteLine(">> Cliente {0} se ha conectado ...", SocketCliente.RemoteEndPoint);
                    ServidorMultiHilo cliente = new ServidorMultiHilo(SocketCliente);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("Cerrando Servidor");
                    SocketServidor.Shutdown(SocketShutdown.Both);
                    SocketServidor.Close();
                }
            }
        }
    }
}
