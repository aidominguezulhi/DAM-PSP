using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteMultiple
{
    
    public class PartidaCliente
    {
        
        private int jugadas = 0;
        public int identificador = 0;
        private int acertante = 0;
        private int numParticipantes = 0;

        public PartidaCliente()
        {
            
        }
       
        public void setIdentificador(int ident)
        {
            this.identificador = ident;
        }
        public int getIdentificador()
        {
             return this.identificador;
        }
        public int getJugadas()
        {
            return this.jugadas;

        }
        public void setJugadas(int jugada)
        {
            this.jugadas = jugada;

        }
        public void setAcertante()
        {

            this.acertante = 1;
        }
       
    }
}