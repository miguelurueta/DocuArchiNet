Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Public Class ResolvedorReglasPerfilImportacion
    Public Const MaximoReglas As Integer = 128
    Public Const MaximoEntrada As Integer = 4096
    Private Shared ReadOnly CamposExpediente As String() = {"CODIGO", "NOMBRE", "FECHA_INICIAL", "FECHA_FINAL", "RESPONSABLE"}
    Public Shared ReadOnly PropiedadesClasificacion As String() = {"SERIE_ID", "SERIE_CODIGO", "SERIE_NOMBRE", "SUBSERIE_ID", "SUBSERIE_CODIGO", "SUBSERIE_NOMBRE"}
    Private Shared ReadOnly PropiedadesExpediente As String() = {"ID", "CODIGO", "NOMBRE", "FECHA_INICIAL", "FECHA_FINAL", "RESPONSABLE", "SERIE_ID", "SERIE_CODIGO", "SERIE_NOMBRE", "SUBSERIE_ID", "SUBSERIE_CODIGO", "SUBSERIE_NOMBRE"}
    Private Shared ReadOnly FormatosFecha As String() = {"yyyy-MM-dd", "yyyyMMdd", "dd-MM-yyyy", "dd_MM_yyyy", "dd/MM/yyyy H:mm:ss"}

    Public Function ValidarEstructura(reglas As List(Of ReglaPerfilImportacion)) As List(Of IncidenciaPerfilImportacion)
        Dim errores As New List(Of IncidenciaPerfilImportacion)()
        If reglas Is Nothing OrElse reglas.Count > MaximoReglas Then
            errores.Add(New IncidenciaPerfilImportacion("reglas", "LIMITE_REGLAS", "La colección de reglas es obligatoria y admite hasta 128 reglas."))
            Return errores
        End If
        Dim destinos As New HashSet(Of String)(StringComparer.Ordinal)
        For Each regla In reglas
            If regla Is Nothing Then
                errores.Add(New IncidenciaPerfilImportacion("reglas", "REGLA_INVALIDA", "No se admiten reglas nulas."))
                Continue For
            End If
            If regla.ReglaId IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(regla.ReglaId) Then Agregar(errores, "ReglaId", "REFERENCIA_INVALIDA")
            Dim campo = If(regla.DestinoTipo, "") & ":" & If(regla.DestinoRef, "")
            If Not destinos.Add(campo) Then Agregar(errores, campo, "DESTINO_DUPLICADO")
            If Not {"CAMPO_EXPEDIENTE", "INDICE_GABINETE", "TIPOLOGIA"}.Contains(regla.DestinoTipo) Then Agregar(errores, campo, "DESTINO_INVALIDO")
            If String.IsNullOrWhiteSpace(regla.DestinoRef) OrElse regla.DestinoRef.Length > 128 OrElse regla.DestinoRef.Any(Function(c) AscW(c) < 33 OrElse AscW(c) > 126) Then Agregar(errores, campo, "REFERENCIA_INVALIDA")
            If regla.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso Not CamposExpediente.Contains(regla.DestinoRef) Then Agregar(errores, campo, "CAMPO_INVALIDO")
            If regla.DestinoTipo = "TIPOLOGIA" AndAlso regla.DestinoRef <> "TIPO_DOCUMENTAL" Then Agregar(errores, campo, "CAMPO_INVALIDO")
            If regla.DestinoTipo = "TIPOLOGIA" AndAlso Not {"TEXTO", "REFERENCIA"}.Contains(regla.TipoValor) Then Agregar(errores, campo, "TIPO_INVALIDO")
            If Not {"CONSTANTE", "CARPETA", "ARCHIVO", "EXPEDIENTE", "PROVEEDOR"}.Contains(regla.FuenteTipo) Then Agregar(errores, campo, "FUENTE_INVALIDA")
            If regla.FuenteTipo = "PROVEEDOR" Then
                If Not ReglasProveedorIndices.Valida(regla) Then Agregar(errores, campo, "PROVEEDOR_INVALIDO")
            ElseIf regla.Proveedor IsNot Nothing Then
                Agregar(errores, campo, "PROVEEDOR_NO_APLICA")
            End If
            If regla.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso (regla.FuenteTipo = "ARCHIVO" OrElse regla.FuenteTipo = "EXPEDIENTE") Then Agregar(errores, campo, "FUENTE_CIRCULAR_O_AMBIGUA")
            If regla.FuenteTipo = "EXPEDIENTE" Then
                If regla.EtapaResolucion <> "TRAS_EXPEDIENTE" OrElse Not PropiedadesExpediente.Contains(regla.PropiedadExpediente) Then Agregar(errores, campo, "DEPENDENCIA_INVALIDA")
                If regla.PropiedadExpediente = "ID" AndAlso Not {"TEXTO", "ENTERO", "REFERENCIA"}.Contains(regla.TipoValor) Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
                If {"SERIE_ID", "SUBSERIE_ID"}.Contains(regla.PropiedadExpediente) AndAlso Not {"TEXTO", "ENTERO", "REFERENCIA"}.Contains(regla.TipoValor) Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
                If {"SERIE_CODIGO", "SERIE_NOMBRE", "SUBSERIE_CODIGO", "SUBSERIE_NOMBRE"}.Contains(regla.PropiedadExpediente) AndAlso regla.TipoValor <> "TEXTO" Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
                If {"CODIGO", "NOMBRE"}.Contains(regla.PropiedadExpediente) AndAlso regla.TipoValor <> "TEXTO" Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
                If regla.PropiedadExpediente = "RESPONSABLE" AndAlso Not {"TEXTO", "REFERENCIA"}.Contains(regla.TipoValor) Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
                If {"FECHA_INICIAL", "FECHA_FINAL"}.Contains(regla.PropiedadExpediente) AndAlso
                    (Not {"FECHA", "TEXTO"}.Contains(regla.TipoValor) OrElse (regla.TipoValor = "FECHA" AndAlso regla.FormatoFecha <> "yyyy-MM-dd")) Then Agregar(errores, campo, "TIPO_DEPENDENCIA_INVALIDO")
            ElseIf regla.FuenteTipo <> "PROVEEDOR" AndAlso (regla.EtapaResolucion <> "PREFLIGHT" OrElse Not String.IsNullOrEmpty(regla.PropiedadExpediente)) Then
                Agregar(errores, campo, "ETAPA_INVALIDA")
            End If
            If Not {"COMPLETO", "SEGMENTO"}.Contains(regla.Extraccion) Then Agregar(errores, campo, "EXTRACCION_INVALIDA")
            If regla.Extraccion = "SEGMENTO" AndAlso (String.IsNullOrEmpty(regla.Separador) OrElse regla.Separador.Length > 16 OrElse regla.Posicion < 0 OrElse regla.Posicion >= 128 OrElse Not {"ARCHIVO", "CARPETA"}.Contains(regla.FuenteTipo)) Then Agregar(errores, campo, "SEGMENTO_INVALIDO")
            If regla.QuitarExtension AndAlso regla.FuenteTipo <> "ARCHIVO" Then Agregar(errores, campo, "EXTENSION_INVALIDA")
            If Not {"TEXTO", "ENTERO", "DECIMAL", "FECHA", "REFERENCIA"}.Contains(regla.TipoValor) Then Agregar(errores, campo, "TIPO_INVALIDO")
            If regla.LongitudMaxima < 1 OrElse regla.LongitudMaxima > MaximoEntrada Then Agregar(errores, campo, "LONGITUD_INVALIDA")
            If regla.TipoValor = "FECHA" AndAlso Not FormatosFecha.Contains(regla.FormatoFecha) Then Agregar(errores, campo, "FORMATO_FECHA_INVALIDO")
            If regla.DestinoTipo = "CAMPO_EXPEDIENTE" Then
                If {"CODIGO", "NOMBRE"}.Contains(regla.DestinoRef) AndAlso regla.TipoValor <> "TEXTO" Then Agregar(errores, campo, "TIPO_INVALIDO")
                If {"FECHA_INICIAL", "FECHA_FINAL"}.Contains(regla.DestinoRef) AndAlso regla.TipoValor <> "FECHA" Then Agregar(errores, campo, "TIPO_INVALIDO")
                If regla.DestinoRef = "RESPONSABLE" AndAlso regla.TipoValor <> "REFERENCIA" Then Agregar(errores, campo, "TIPO_INVALIDO")
            End If
            If regla.ValorConstante IsNot Nothing AndAlso regla.ValorConstante.Length > MaximoEntrada Then Agregar(errores, campo, "ENTRADA_EXCEDIDA")
        Next
        Return errores
    End Function

    Public Function Resolver(reglas As List(Of ReglaPerfilImportacion), ejemplo As EjemploPerfilImportacion) As ResultadoEnsayoPerfilImportacion
        Dim resultado As New ResultadoEnsayoPerfilImportacion()
        resultado.Incidencias.AddRange(ValidarEstructura(reglas))
        If resultado.Incidencias.Count > 0 Then Return resultado
        If ejemplo Is Nothing OrElse Longitud(ejemplo.NombreArchivo) > MaximoEntrada OrElse Longitud(ejemplo.NombreCarpeta) > MaximoEntrada OrElse ejemplo.Expediente Is Nothing Then
            Agregar(resultado.Incidencias, "ejemplo", "EJEMPLO_INVALIDO")
            Return resultado
        End If
        For Each regla In reglas.OrderBy(Function(r) r.Orden).ThenBy(Function(r) r.DestinoTipo, StringComparer.Ordinal).ThenBy(Function(r) r.DestinoRef, StringComparer.Ordinal)
            Dim campo = regla.DestinoTipo & ":" & regla.DestinoRef
            Dim valor As String = Nothing
            Dim salida As New ValorReglaPerfilImportacion With {.FuenteTipo = regla.FuenteTipo, .DestinoTipo = regla.DestinoTipo, .DestinoRef = regla.DestinoRef, .TipoValor = regla.TipoValor}
            Select Case regla.FuenteTipo
                Case "PROVEEDOR"
                    Agregar(resultado.Incidencias, campo, "FUENTE_NO_RESUELTA")
                    Continue For
                Case "CONSTANTE" : valor = regla.ValorConstante
                Case "CARPETA" : valor = ejemplo.NombreCarpeta
                Case "ARCHIVO" : valor = ejemplo.NombreArchivo
                Case "EXPEDIENTE"
                    If Not ejemplo.Expediente.TryGetValue(regla.PropiedadExpediente, valor) Then
                        salida.Diferido = True
                        salida.PropiedadPendiente = regla.PropiedadExpediente
                        resultado.Valores.Add(salida)
                        Continue For
                    End If
            End Select
            If Longitud(valor) > MaximoEntrada Then
                Agregar(resultado.Incidencias, campo, "ENTRADA_EXCEDIDA")
                Continue For
            End If
            If valor IsNot Nothing Then
                If regla.QuitarExtension Then
                    Dim punto = valor.LastIndexOf("."c)
                    If punto > 0 Then valor = valor.Substring(0, punto)
                End If
                If regla.Extraccion = "SEGMENTO" Then
                    Dim partes = valor.Split(New String() {regla.Separador}, StringSplitOptions.None)
                    If regla.Posicion >= partes.Length Then
                        Agregar(resultado.Incidencias, campo, "SEGMENTO_AUSENTE")
                        Continue For
                    End If
                    valor = partes(regla.Posicion)
                End If
                If regla.RecortarEspacios Then valor = valor.Trim()
            End If
            Dim convertido As String = Nothing
            Dim errorValor = Convertir(regla, valor, convertido)
            If errorValor IsNot Nothing Then
                Agregar(resultado.Incidencias, campo, errorValor)
            Else
                salida.Valor = convertido
                resultado.Valores.Add(salida)
            End If
        Next
        Dim inicial = resultado.Valores.FirstOrDefault(Function(v) v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = "FECHA_INICIAL")
        Dim final = resultado.Valores.FirstOrDefault(Function(v) v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = "FECHA_FINAL")
        If inicial IsNot Nothing AndAlso final IsNot Nothing AndAlso Not String.IsNullOrEmpty(inicial.Valor) AndAlso Not String.IsNullOrEmpty(final.Valor) AndAlso String.CompareOrdinal(inicial.Valor, final.Valor) > 0 Then Agregar(resultado.Incidencias, "CAMPO_EXPEDIENTE:FECHA_FINAL", "RANGO_FECHAS_INVALIDO")
        Return resultado
    End Function

    Private Shared Function Convertir(regla As ReglaPerfilImportacion, valor As String, ByRef convertido As String) As String
        Dim requerido = regla.Obligatoria OrElse (regla.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso {"CODIGO", "NOMBRE", "FECHA_INICIAL"}.Contains(regla.DestinoRef))
        If String.IsNullOrWhiteSpace(valor) Then
            If requerido Then Return "VALOR_REQUERIDO"
            If valor Is Nothing AndAlso regla.FuenteTipo = "EXPEDIENTE" AndAlso PropiedadesClasificacion.Contains(regla.PropiedadExpediente) Then Return Nothing
            'Una fuente presente que produjo texto vacío no es una omisión de la regla.
            Return "VALOR_VACIO"
        End If
        If valor.Length > regla.LongitudMaxima Then Return "LONGITUD_EXCEDIDA"
        If valor.Any(Function(c) Char.IsSurrogate(c) OrElse c = ChrW(0)) Then Return "CARACTER_NO_REPRESENTABLE"
        If regla.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso regla.DestinoRef = "CODIGO" AndAlso valor.Length > 45 Then Return "CODIGO_EXCEDE_45"
        Select Case regla.TipoValor
            Case "TEXTO", "REFERENCIA" : convertido = valor
            Case "ENTERO"
                Dim numero As Long
                If Not Long.TryParse(valor, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, numero) Then Return "ENTERO_INVALIDO"
                convertido = numero.ToString(CultureInfo.InvariantCulture)
            Case "DECIMAL"
                Dim numero As Decimal
                If Not Decimal.TryParse(valor, NumberStyles.AllowLeadingSign Or NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, numero) Then Return "DECIMAL_INVALIDO"
                convertido = numero.ToString(CultureInfo.InvariantCulture)
            Case "FECHA"
                Dim fecha As DateTime
                If regla.FuenteTipo = "CONSTANTE" AndAlso regla.FormatoFecha <> "yyyy-MM-dd" Then Return "FECHA_CONSTANTE_NO_CANONICA"
                If Not DateTime.TryParseExact(valor, regla.FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) OrElse fecha.Year < 1000 Then Return "FECHA_INVALIDA"
                convertido = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        End Select
        Return Nothing
    End Function

    Private Shared Function Longitud(valor As String) As Integer
        Return If(valor Is Nothing, 0, valor.Length)
    End Function

    Private Shared Sub Agregar(errores As List(Of IncidenciaPerfilImportacion), campo As String, codigo As String)
        errores.Add(New IncidenciaPerfilImportacion(campo, codigo, "Revise la regla y el valor del campo indicado."))
    End Sub
End Class
