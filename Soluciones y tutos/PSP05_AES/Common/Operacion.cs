using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Common {
	[Serializable]

	public class Operacion : ISerializable {
		public string Origen { get; set; }
		public string Destino { get; set; }
		public double Cantidad { get; set; }
		public string Resumen { get; set; }
		public DateTime Stamp { get; set; }
		public bool Estado { get; set; }

		public Operacion() { }

		public Operacion(string origen, string destino, double cantidad, string resumen) {
			this.Origen = origen;
			this.Destino = destino;
			this.Cantidad = cantidad;
			this.Resumen = resumen;
			this.Estado = false;
			this.Stamp = DateTime.Now;
		}

		protected Operacion(SerializationInfo info, StreamingContext context) {
			this.Origen = info.GetString("origen");
			this.Destino = info.GetString("destino ");
			this.Cantidad = (double)info.GetValue("cantidad", typeof(double));
			this.Resumen = (string)info.GetValue("resumen", typeof(string));
			this.Estado = (bool)info.GetValue("estado", typeof(bool));
			this.Stamp = (DateTime)info.GetValue("fecha", typeof(DateTime));
		}

		public Operacion clone() {
			Operacion op = new Operacion();
			op.Origen = Origen;
			op.Destino = Destino;
			op.Cantidad = Cantidad;
			op.Resumen = Resumen;
			op.Estado = Estado;
			op.Stamp = Stamp;//DateTime is a value type, so when you assign it you also clone it.
			return op;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("origen", this.Origen);
			info.AddValue("destino", this.Destino);
			info.AddValue("cantidad", this.Cantidad); 
			info.AddValue("resumen", this.Resumen);
			info.AddValue("estado", this.Estado);
			info.AddValue("fecha", this.Stamp);
		}
		public string ToJson() {
			return JsonSerializer.Serialize(this);
		}
		public string ToXml() {
			using (StringWriter textWriter = new StringWriter()) {
				new XmlSerializer(typeof(Operacion)).Serialize(textWriter, this);
				return textWriter.ToString();
			}
		}
		public override string ToString()
		{
			return $"[origen: {Origen}, destino: {Destino}, cantidad: {Cantidad.ToString("0.##")}, fecha: {Stamp.ToString("yyyy-MM-ddTHH:mm:ss.fff")}]";
		}

	}

}
