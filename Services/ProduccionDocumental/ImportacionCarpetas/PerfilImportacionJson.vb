Option Strict On
Imports System
Imports System.IO
Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

'Esquema cerrado y representación ASCII determinista para LONGTEXT en MySQL 5.1.
Public NotInheritable Class PerfilImportacionJson
    Public Const MaximoCaracteres As Integer = 262144
    Private Sub New()
    End Sub

    Private Shared Function Ajustes() As JsonSerializerSettings
        Return New JsonSerializerSettings With {
            .MissingMemberHandling = MissingMemberHandling.Error,
            .TypeNameHandling = TypeNameHandling.None,
            .StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            .DateParseHandling = DateParseHandling.None,
            .MaxDepth = 16,
            .Culture = Globalization.CultureInfo.InvariantCulture}
    End Function

    Public Shared Function Escribir(valor As Object) As String
        Dim token = JToken.FromObject(valor, JsonSerializer.Create(Ajustes()))
        Dim texto = JsonConvert.SerializeObject(Ordenar(token), Ajustes())
        If texto.Length > MaximoCaracteres Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La configuración excede el límite permitido.")
        Return texto
    End Function

    Public Shared Function Leer(Of T)(texto As String) As T
        If texto Is Nothing OrElse texto.Length > MaximoCaracteres Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Configuración inválida.")
        Try
            Using reader As New JsonTextReader(New StringReader(texto))
                reader.MaxDepth = 16
                reader.DateParseHandling = DateParseHandling.None
                Dim token = JToken.Load(reader, New JsonLoadSettings With {.DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error})
                If reader.Read() Then Throw New JsonSerializationException()
                If token.Type = JTokenType.Null Then Throw New JsonSerializationException()
                ValidarForma(token, GetType(T))
                Return token.ToObject(Of T)(JsonSerializer.Create(Ajustes()))
            End Using
        Catch ex As JsonException
            Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Configuración inválida.")
        End Try
    End Function

    Private Shared Sub ValidarForma(token As JToken, tipo As Type)
        Dim nullable = System.Nullable.GetUnderlyingType(tipo)
        If token.Type = JTokenType.Null Then
            If tipo.IsValueType AndAlso nullable Is Nothing Then Throw New JsonSerializationException()
            Return
        End If
        If nullable IsNot Nothing Then tipo = nullable
        If tipo = GetType(String) Then
            If token.Type <> JTokenType.String Then Throw New JsonSerializationException()
        ElseIf tipo = GetType(Boolean) Then
            If token.Type <> JTokenType.Boolean Then Throw New JsonSerializationException()
        ElseIf tipo.IsPrimitive OrElse tipo = GetType(Decimal) Then
            If token.Type <> JTokenType.Integer Then Throw New JsonSerializationException()
        ElseIf tipo = GetType(Dictionary(Of String, String)) Then
            If token.Type <> JTokenType.Object Then Throw New JsonSerializationException()
            For Each propiedad In DirectCast(token, JObject).Properties()
                If Not {"ID", "CODIGO", "NOMBRE", "FECHA_INICIAL", "FECHA_FINAL", "RESPONSABLE"}.Contains(propiedad.Name) Then Throw New JsonSerializationException()
                ValidarForma(propiedad.Value, GetType(String))
            Next
        ElseIf tipo.IsGenericType AndAlso tipo.GetGenericTypeDefinition() = GetType(List(Of )) Then
            If token.Type <> JTokenType.Array Then Throw New JsonSerializationException()
            For Each item In DirectCast(token, JArray)
                ValidarForma(item, tipo.GetGenericArguments()(0))
            Next
        Else
            If token.Type <> JTokenType.Object Then Throw New JsonSerializationException()
            Dim propiedades = tipo.GetProperties().ToDictionary(Function(p) p.Name, StringComparer.Ordinal)
            For Each propiedad In DirectCast(token, JObject).Properties()
                If Not propiedades.ContainsKey(propiedad.Name) Then Throw New JsonSerializationException()
                ValidarForma(propiedad.Value, propiedades(propiedad.Name).PropertyType)
            Next
        End If
    End Sub

    Public Shared Function Huella(version As VersionPerfilImportacion) As Byte()
        Dim reglas = version.Reglas.OrderBy(Function(r) r.DestinoTipo, StringComparer.Ordinal).ThenBy(Function(r) r.DestinoRef, StringComparer.Ordinal).ToArray()
        Using sha = SHA256.Create()
            Return sha.ComputeHash(Encoding.ASCII.GetBytes(Escribir(New With {.configuracion = version.Configuracion, .reglas = reglas})))
        End Using
    End Function

    Private Shared Function Ordenar(token As JToken) As JToken
        If TypeOf token Is JObject Then
            Dim salida As New JObject()
            For Each propiedad In DirectCast(token, JObject).Properties().OrderBy(Function(p) p.Name, StringComparer.Ordinal)
                salida.Add(propiedad.Name, Ordenar(propiedad.Value))
            Next
            Return salida
        End If
        If TypeOf token Is JArray Then Return New JArray(DirectCast(token, JArray).Select(Function(t) Ordenar(t)))
        Return token.DeepClone()
    End Function
End Class
