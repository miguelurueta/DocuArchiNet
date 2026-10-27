Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

'Solo contratos y transformaciones: sin consultas ni sesiones.
Public NotInheritable Class ReglasProveedorIndices
    Public Shared Function ClaveComoRegla(clave As ClaveProveedorIndices) As ReglaPerfilImportacion
        Return New ReglaPerfilImportacion With {.DestinoTipo = "INDICE_GABINETE", .DestinoRef = "CLAVE", .FuenteTipo = clave.FuenteTipo,
            .EtapaResolucion = If(clave.FuenteTipo = "EXPEDIENTE", "TRAS_EXPEDIENTE", "PREFLIGHT"), .Obligatoria = True,
            .ValorConstante = clave.ValorConstante, .PropiedadExpediente = clave.PropiedadExpediente,
            .Extraccion = clave.Extraccion, .Separador = clave.Separador, .Posicion = clave.Posicion,
            .QuitarExtension = clave.QuitarExtension, .RecortarEspacios = clave.RecortarEspacios,
            .TipoValor = clave.TipoValor, .FormatoFecha = clave.FormatoFecha, .LongitudMaxima = clave.LongitudMaxima}
    End Function

    Public Shared Function Valida(regla As ReglaPerfilImportacion) As Boolean
        Dim fuente = regla.Proveedor
        If regla.DestinoTipo <> "INDICE_GABINETE" OrElse fuente Is Nothing OrElse fuente.Clave Is Nothing OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(fuente.ProveedorId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(fuente.VersionId) OrElse
            fuente.Propiedad Is Nothing OrElse Not Regex.IsMatch(fuente.Propiedad, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z") Then Return False
        If Not {"CONSTANTE", "CARPETA", "ARCHIVO", "EXPEDIENTE"}.Contains(fuente.Clave.FuenteTipo) OrElse
            Not {"TEXTO", "ENTERO", "DECIMAL", "FECHA"}.Contains(fuente.Clave.TipoValor) OrElse
            regla.Extraccion <> "COMPLETO" OrElse regla.QuitarExtension OrElse Not String.IsNullOrEmpty(regla.PropiedadExpediente) OrElse
            regla.EtapaResolucion <> If(fuente.Clave.FuenteTipo = "EXPEDIENTE", "TRAS_EXPEDIENTE", "PREFLIGHT") Then Return False
        'La conversion de texto es explicita por regla; fechas nativas siguen siendo canonicas.
        If regla.TipoValor = "FECHA" AndAlso Not {"yyyy-MM-dd", "dd/MM/yyyy H:mm:ss"}.Contains(regla.FormatoFecha) Then Return False
        Return New ResolvedorReglasPerfilImportacion().ValidarEstructura(New List(Of ReglaPerfilImportacion) From {ClaveComoRegla(fuente.Clave)}).Count = 0
    End Function
End Class
