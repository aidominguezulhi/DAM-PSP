using Common;
using System.Collections.Generic;

namespace Common {
    public class DataBase {
        
        Dictionary<string, double> dic = new Dictionary<string, double>();

        public DataBase() {
            dic.Add("uno", 10000);
            dic.Add("dos", 10000);
            dic.Add("tres", 10000);
            dic.Add("cuatro", 10000);
            dic.Add("cinco", 10000);
            dic.Add("seis", 10000);
            dic.Add("siete", 10000);
            dic.Add("ocho", 10000);
            dic.Add("nueve", 10000);
            dic.Add("diez", 10000);
        }

        private void Sumar(string usuario, double cantidad) {
            dic[usuario] = dic[usuario] + cantidad;
        }

        private void Restar(string usuario, double cantidad) {
            dic[usuario] = dic[usuario] - cantidad;
        }

        public bool Transferir(string origen, string destino, double cantidad) {
            if ( (origen == null) || (destino == null) || !dic.ContainsKey(origen) || !dic.ContainsKey(destino) ) {
                return false;
            } else {
                if (dic[origen] < cantidad) {
                    return false;
                } else {
                    Restar(origen, cantidad);
                    Sumar(destino, cantidad);
                    return true;
                }
            }
        }
        public bool Transferir(Operacion op) {
            return Transferir(op.Origen, op.Destino, op.Cantidad);
        }
        public double Saldo(string usuario) {
            return dic[usuario];
        }

    }
}
