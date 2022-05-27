using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteMultiple
{

    public class Partida
    {
        private const int MAX_PARTICIPANTES = 10;
        public int random = 0;
        public int jugadas = 0;
        public IntPtr ganador = IntPtr.Zero;
        private int identificador = 0;
        private IntPtr[] participantes = null;
        private int numParticipantes = 0;

        public Partida()
        {
            this.random = setNumAleatorio();
            this.participantes = new IntPtr[MAX_PARTICIPANTES];
            for (int i = 0; i < this.participantes.Length; i++)
            {
                this.participantes[i] = IntPtr.Zero;
            }
        } 
        public void setParticipante(IntPtr identificador)
        {
            this.participantes[this.numParticipantes] = identificador;
            this.numParticipantes++;
        }
        public int numCliente(IntPtr identificador)
        {
            for (int i = 0; i < this.participantes.Length; i++)
            {
                if (identificador.Equals(this.participantes[i]))
                {
                    return i;
                }
                else 
                {
                    return -1;
                }
            }
            return -1;
        }
        public int getJugadasTotales()
        {
            return this.jugadas;

        }
        public IntPtr getGanador()
        {
            return this.ganador;

        }
        public int getNumAleatorio()
        {
            return this.random;

        }
        private int setNumAleatorio()
        {
            return new Random().Next(1, 101);
        }
        public void setJugadas(int jugadas)
        {
            this.jugadas = jugadas;
        }
        public void setGanador(IntPtr ganador)
        {
            this.ganador = ganador;
        }
        public int getIdentificador()
        {
            
            if ((identificador >= 0) && (identificador < 10))
            {
                this.identificador++;
                return identificador;
            }
            return -1;

        }



    }
}