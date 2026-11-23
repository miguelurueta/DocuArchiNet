Option Strict On
Imports System
Imports System.Data.Common
Imports System.Globalization

Public NotInheritable Class SqlCargaExcel
    Private Sub New()
    End Sub
    Public Shared Function Cmd(cn As DbConnection, tx As DbTransaction, sql As String, ParamArray args() As Object) As DbCommand
        Dim q = cn.CreateCommand()
        q.Transaction = tx : q.CommandText = sql : q.CommandTimeout = 15
        For i = 0 To args.Length - 1 Step 2
            ImportacionExcelContextAdapter.Parametro(q, CStr(args(i)), args(i + 1))
        Next
        Return q
    End Function
    Public Shared Function Texto(r As DbDataReader, i As Integer) As String
        Return If(r.IsDBNull(i), Nothing, Convert.ToString(r(i), CultureInfo.InvariantCulture))
    End Function
    Public Shared Function Identificador(nombre As String) As String
        If Not ServicioPlantillasExcel.IdentificadorValido(nombre) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Return "`" & nombre & "`"
    End Function
    Public Shared Function Binario(hash As String) As Byte()
        If hash Is Nothing Then Return Nothing
        If Not ReglasCargaExcel.HashValido(hash) Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        Dim result(31) As Byte
        For i = 0 To result.Length - 1
            result(i) = Byte.Parse(hash.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
        Next
        Return result
    End Function
    Public Shared Function Huella(r As DbDataReader, i As Integer) As String
        Return If(r.IsDBNull(i), Nothing, BitConverter.ToString(DirectCast(r(i), Byte())).Replace("-", "").ToLowerInvariant())
    End Function
End Class
