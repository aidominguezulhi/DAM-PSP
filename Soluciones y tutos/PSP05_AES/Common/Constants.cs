using System;
using System.Collections.Generic;
using System.Net;

namespace Common
{
    public class Constants
    {
        public const int tcpPort = 11000;
        public const string tcpHost = "127.0.0.1";

        public const string ftpHost = "192.168.33.10";
        public const string ftpUser = "user123";
        public const string ftpPass = "user123";
        public const string ftpBloquesPath = "/bloques";
        public const string BloqueFileIni = "bloque";
        public const string BloqueFileExt = ".xml";

        public const int opsXblock = 10;
        public const string resumenInicial = "00000000";
        public const string minadoMinimo = "00";

        public const string fileIV = "aes.IV";
        public const string fileKey = "aes.key";
        public const string remotePathAES = "/claveSimetrica";
        public const string remotePathIV = remotePathAES + "/" + fileIV;
        public const string remotePathKey = remotePathAES + "/" + fileKey;
        public const string msgPipe = "PSP05_PIPE";
        public const string msgAesShared = "AES SHARED";
        public const string msgServerAccepts = "SERVER ACCEPTS OPERATIONS";
        public const string msgServerNotAllowed = "OPERATIONS NOT ALLOWED";
    }
}
