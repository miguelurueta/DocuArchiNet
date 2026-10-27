Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Globalization

'Instancia sin cache de datos. Cada Ensayar comparte respuestas solo dentro de la solicitud.
Public Class CoordinadorProveedorIndices
    Private ReadOnly _repositorio As IProveedorIndicesRepository
    Private ReadOnly _proveedor As IProveedorDatosIndices
    Public Sub New(repositorio As IProveedorIndicesRepository, proveedor As IProveedorDatosIndices)
        If repositorio Is Nothing OrElse proveedor Is Nothing Then Throw New ArgumentNullException()
        _repositorio = repositorio
        _proveedor = proveedor
    End Sub
    Public Function Listar(contexto As ContextoPerfilImportacion, pagina As Integer, tamano As Integer, proveedorId As String, versionId As String) As List(Of OpcionProveedorIndices)
        Return _repositorio.Listar(contexto, pagina, tamano, proveedorId, versionId)
    End Function

    Public Function Validar(contexto As ContextoPerfilImportacion, reglas As List(Of ReglaPerfilImportacion)) As List(Of IncidenciaPerfilImportacion)
        Dim errores As New List(Of IncidenciaPerfilImportacion)()
        For Each regla In reglas.Where(Function(r) r.FuenteTipo = "PROVEEDOR")
            Dim v = _repositorio.Obtener(contexto, regla.Proveedor.ProveedorId, regla.Proveedor.VersionId)
            If Not Compatible(contexto, regla, v) Then errores.Add(New IncidenciaPerfilImportacion(Campo(regla), "PROVEEDOR_INCOMPATIBLE", "Revise la version y propiedad del proveedor."))
        Next
        Return errores
    End Function

    Public Function Ensayar(contexto As ContextoPerfilImportacion, reglas As List(Of ReglaPerfilImportacion), ejemplos As List(Of EjemploPerfilImportacion)) As List(Of ResultadoEnsayoPerfilImportacion)
        Dim resolvedor As New ResolvedorReglasPerfilImportacion()
        Dim errores = resolvedor.ValidarEstructura(reglas)
        If errores.Count > 0 Then Throw New ConfiguracionPerfilImportacionException(errores)
        Dim versiones As New Dictionary(Of String, ProveedorIndicesVersion)(StringComparer.Ordinal)
        For Each regla In reglas.Where(Function(r) r.FuenteTipo = "PROVEEDOR")
            Dim id = regla.Proveedor.ProveedorId & ":" & regla.Proveedor.VersionId
            If Not versiones.ContainsKey(id) Then versiones.Add(id, _repositorio.Obtener(contexto, regla.Proveedor.ProveedorId, regla.Proveedor.VersionId))
            If Not Compatible(contexto, regla, versiones(id)) Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Proveedor incompatible.")
        Next
        Dim respuestas As New Dictionary(Of String, ResultadoProveedorIndices)(StringComparer.Ordinal)
        Dim resultados As New List(Of ResultadoEnsayoPerfilImportacion)()
        For Each ejemplo In ejemplos
            Dim normales = reglas.Where(Function(r) r.FuenteTipo <> "PROVEEDOR").ToList()
            Dim salida = resolvedor.Resolver(normales, ejemplo)
            For Each regla In reglas.Where(Function(r) r.FuenteTipo = "PROVEEDOR")
                Dim clave = resolvedor.Resolver(New List(Of ReglaPerfilImportacion) From {ReglasProveedorIndices.ClaveComoRegla(regla.Proveedor.Clave)}, ejemplo)
                If clave.Incidencias.Count > 0 Then
                    For Each errorClave In clave.Incidencias
                        salida.Incidencias.Add(New IncidenciaPerfilImportacion(Campo(regla), errorClave.Codigo, "Revise la clave del proveedor."))
                    Next
                    Continue For
                End If
                If clave.Valores(0).Diferido Then
                    salida.Valores.Add(New ValorReglaPerfilImportacion With {.DestinoTipo = regla.DestinoTipo, .DestinoRef = regla.DestinoRef, .FuenteTipo = "PROVEEDOR", .TipoValor = regla.TipoValor, .Diferido = True, .PropiedadPendiente = regla.Proveedor.Clave.PropiedadExpediente})
                    Continue For
                End If
                Dim id = regla.Proveedor.ProveedorId & ":" & regla.Proveedor.VersionId
                'IDs decimales y prefijo separado de la clave; no hay colisiones por delimitadores en la clave.
                Dim cacheKey = id & ":" & clave.Valores(0).Valor
                If Not respuestas.ContainsKey(cacheKey) Then respuestas.Add(cacheKey, _proveedor.Consultar(contexto, versiones(id), clave.Valores(0).Valor))
                Dim respuesta = respuestas(cacheKey)
                If respuesta Is Nothing OrElse respuesta.Codigo <> "OK" Then
                    Dim codigo = If(respuesta Is Nothing, "ORIGEN_NO_DISPONIBLE", respuesta.Codigo)
                    If Not {"SIN_COINCIDENCIA", "AMBIGUO", "CONFIGURACION_INVALIDA", "ACCESO_DENEGADO", "CLAVE_INVALIDA", "ORIGEN_NO_DISPONIBLE"}.Contains(codigo) Then codigo = "ORIGEN_NO_DISPONIBLE"
                    salida.Incidencias.Add(New IncidenciaPerfilImportacion(Campo(regla), codigo, "No se pudo resolver el valor desde el proveedor."))
                    Continue For
                End If
                Dim valor As String = Nothing
                If Not respuesta.Valores.TryGetValue(regla.Proveedor.Propiedad, valor) Then
                    salida.Incidencias.Add(New IncidenciaPerfilImportacion(Campo(regla), "PROPIEDAD_AUSENTE", "La propiedad no esta disponible."))
                    Continue For
                End If
                Dim copia = PerfilImportacionJson.Leer(Of ReglaPerfilImportacion)(PerfilImportacionJson.Escribir(regla))
                copia.Proveedor = Nothing : copia.FuenteTipo = "CONSTANTE" : copia.EtapaResolucion = "PREFLIGHT" : copia.ValorConstante = valor
                If regla.TipoValor = "FECHA" AndAlso regla.FormatoFecha = "dd/MM/yyyy H:mm:ss" Then
                    Dim fecha As DateTime
                    Dim entrada = If(regla.RecortarEspacios AndAlso valor IsNot Nothing, valor.Trim(), valor)
                    If Not DateTime.TryParseExact(entrada, regla.FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) OrElse fecha.Year < 1000 Then
                        salida.Incidencias.Add(New IncidenciaPerfilImportacion(Campo(regla), "FECHA_INVALIDA", "La fecha no coincide con el formato configurado."))
                        Continue For
                    End If
                    copia.ValorConstante = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    copia.FormatoFecha = "yyyy-MM-dd"
                End If
                Dim resuelto = resolvedor.Resolver(New List(Of ReglaPerfilImportacion) From {copia}, New EjemploPerfilImportacion())
                For Each item In resuelto.Valores
                    item.FuenteTipo = "PROVEEDOR"
                    salida.Valores.Add(item)
                Next
                salida.Incidencias.AddRange(resuelto.Incidencias)
            Next
            resultados.Add(salida)
        Next
        Return resultados
    End Function

    Private Shared Function Campo(regla As ReglaPerfilImportacion) As String
        Return regla.DestinoTipo & ":" & regla.DestinoRef
    End Function
    Private Shared Function Compatible(contexto As ContextoPerfilImportacion, regla As ReglaPerfilImportacion, version As ProveedorIndicesVersion) As Boolean
        If version Is Nothing OrElse Not version.Activo OrElse version.Estado <> "PUBLICADA" OrElse version.AmbitoRef <> contexto.AmbitoRef OrElse
            version.ProveedorId <> regla.Proveedor.ProveedorId OrElse version.VersionId <> regla.Proveedor.VersionId OrElse
            version.TipoClave <> regla.Proveedor.Clave.TipoValor OrElse version.Propiedades Is Nothing Then Return False
        Dim propiedades = version.Propiedades.Where(Function(p) p.Nombre = regla.Proveedor.Propiedad).ToList()
        If propiedades.Count <> 1 Then Return False
        If regla.TipoValor = "FECHA" Then Return (propiedades(0).Tipo = "FECHA" AndAlso regla.FormatoFecha = "yyyy-MM-dd") OrElse (propiedades(0).Tipo = "TEXTO" AndAlso {"yyyy-MM-dd", "dd/MM/yyyy H:mm:ss"}.Contains(regla.FormatoFecha))
        Return propiedades(0).Tipo = regla.TipoValor OrElse (propiedades(0).Tipo = "TEXTO" AndAlso regla.TipoValor = "REFERENCIA")
    End Function
End Class
