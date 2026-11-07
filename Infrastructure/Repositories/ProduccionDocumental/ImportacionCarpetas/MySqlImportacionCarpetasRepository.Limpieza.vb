Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Implements ILimpiezaOriginalImportacionRepository

    Public Function ObtenerVistaLimpieza(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As VistaLimpiezaOriginalImportacion Implements ILimpiezaOriginalImportacionRepository.ObtenerVistaLimpieza
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId)
        Return Operar(Function(conexion, tx) CargarVistaLimpieza(conexion, tx, contexto, loteId, archivoId))
    End Function
    Private Shared Function CargarVistaLimpieza(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As VistaLimpiezaOriginalImportacion
        Dim vista As New VistaLimpiezaOriginalImportacion With {.Inventario = CargarVistaConsolidacion(conexion, tx, contexto, loteId), .ArchivoId = archivoId}
        Using cmd = ComandoOperacion(conexion, tx, "SELECT accion,codigo,actor_ref,detalle_doc FROM ra_integracion_evento WHERE lote_id=@lote AND recurso_tipo='ARCHIVO' AND recurso_id=@archivo AND accion IN ('PREPARAR_LIMPIEZA','CONCLUIR_LIMPIEZA') ORDER BY evento_id LIMIT 3 FOR UPDATE", "@lote", IdOperacion(loteId), "@archivo", IdOperacion(archivoId))
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    If Convert.ToString(reader("actor_ref"), CultureInfo.InvariantCulture) <> contexto.UsuarioRef Then Throw AccesoDenegado()
                    Dim permiso = PerfilImportacionJson.Leer(Of AutorizacionLimpiezaOriginalImportacion)(Convert.ToString(reader("detalle_doc"), CultureInfo.InvariantCulture))
                    Dim accion = Convert.ToString(reader("accion"), CultureInfo.InvariantCulture), codigo = Convert.ToString(reader("codigo"), CultureInfo.InvariantCulture)
                    If accion = "PREPARAR_LIMPIEZA" Then
                        If vista.Autorizacion IsNot Nothing OrElse codigo <> "LIMPIEZA_PREPARADA" Then Throw ConflictoOperacion()
                        vista.Autorizacion = permiso
                    Else
                        If vista.Autorizacion Is Nothing OrElse vista.CodigoFinal IsNot Nothing OrElse PerfilImportacionJson.Escribir(permiso) <> PerfilImportacionJson.Escribir(vista.Autorizacion) Then Throw ConflictoOperacion()
                        vista.CodigoFinal = codigo
                    End If
                End While
            End Using
        End Using
        Return vista
    End Function
    Public Function PrepararLimpieza(esperada As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, comprobar As Action(Of VistaLimpiezaOriginalImportacion)) As VistaLimpiezaOriginalImportacion Implements ILimpiezaOriginalImportacionRepository.PrepararLimpieza
        If esperada Is Nothing OrElse esperada.Inventario Is Nothing OrElse esperada.Inventario.Lote Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Inventario.Lote.Contexto : AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim ahoraUtc = Ahora(), actual = CargarVistaLimpieza(conexion, tx, contexto, esperada.Inventario.Lote.LoteId, esperada.ArchivoId)
                          Return PrepararLimpiezaNucleo(esperada, actual, politica, ahoraUtc, comprobar,
                              Sub(accion, codigo, permiso) RegistrarDiarioLimpieza(conexion, tx, contexto, accion, codigo, permiso, ahoraUtc))
                      End Function)
    End Function
    Public Function AplicarLimpieza(esperada As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, comprobar As Action(Of VistaLimpiezaOriginalImportacion), eliminar As Func(Of Boolean)) As LimpiezaOriginalImportacionResultado Implements ILimpiezaOriginalImportacionRepository.AplicarLimpieza
        If esperada Is Nothing OrElse esperada.Inventario Is Nothing OrElse esperada.Inventario.Lote Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Inventario.Lote.Contexto : AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim ahoraUtc = Ahora(), actual = CargarVistaLimpieza(conexion, tx, contexto, esperada.Inventario.Lote.LoteId, esperada.ArchivoId)
                          Return AplicarLimpiezaNucleo(esperada, actual, politica, ahoraUtc, comprobar, eliminar,
                              Sub(accion, codigo, permiso) RegistrarDiarioLimpieza(conexion, tx, contexto, accion, codigo, permiso, ahoraUtc))
                      End Function)
    End Function
    Private Shared Sub RegistrarDiarioLimpieza(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, accion As String, codigo As String, permiso As AutorizacionLimpiezaOriginalImportacion, instante As DateTime)
        Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento(lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES(@lote,NULL,'ARCHIVO',@archivo,@accion,@codigo,@actor,@ahora,@detalle)", "@lote", IdOperacion(permiso.LoteId), "@archivo", IdOperacion(permiso.ArchivoId), "@accion", accion, "@codigo", codigo, "@actor", contexto.UsuarioRef, "@ahora", instante, "@detalle", PerfilImportacionJson.Escribir(permiso))
            UnaFila(cmd.ExecuteNonQuery())
        End Using
    End Sub

    Friend Shared Function PrepararLimpiezaNucleo(esperada As VistaLimpiezaOriginalImportacion, actual As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, instante As DateTime, comprobar As Action(Of VistaLimpiezaOriginalImportacion), registrar As Action(Of String, String, AutorizacionLimpiezaOriginalImportacion)) As VistaLimpiezaOriginalImportacion
        ValidarParLimpieza(esperada, actual, politica, instante)
        If comprobar Is Nothing OrElse registrar Is Nothing Then Throw SolicitudInvalida()
        comprobar(CopiarVistaLimpieza(actual))
        Dim salida = CopiarVistaLimpieza(actual)
        If salida.Autorizacion IsNot Nothing Then Return salida
        salida.Autorizacion = New AutorizacionLimpiezaOriginalImportacion With {.LoteId = actual.Inventario.Lote.LoteId, .ArchivoId = actual.ArchivoId,
            .HuellaInventario = ServicioConsultaImportacion.HuellaInventarioLimpieza(actual.Inventario), .RetencionTicks = politica.Retencion.Ticks, .PreparadaUtc = instante.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)}
        registrar("PREPARAR_LIMPIEZA", "LIMPIEZA_PREPARADA", salida.Autorizacion)
        Return salida
    End Function
    Friend Shared Function AplicarLimpiezaNucleo(esperada As VistaLimpiezaOriginalImportacion, actual As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, instante As DateTime, comprobar As Action(Of VistaLimpiezaOriginalImportacion), eliminar As Func(Of Boolean), registrar As Action(Of String, String, AutorizacionLimpiezaOriginalImportacion)) As LimpiezaOriginalImportacionResultado
        ValidarParLimpieza(esperada, actual, politica, instante)
        If comprobar Is Nothing OrElse eliminar Is Nothing OrElse registrar Is Nothing OrElse esperada.Autorizacion Is Nothing OrElse actual.Autorizacion Is Nothing Then Throw SolicitudInvalida()
        If actual.CodigoFinal IsNot Nothing Then Return ServicioConsultaImportacion.ResultadoLimpieza(actual, "CONFIRMADO", actual.CodigoFinal, True)
        comprobar(CopiarVistaLimpieza(actual))
        Dim codigo = If(eliminar(), "STAGING_ELIMINADO", "STAGING_AUSENTE_AUTORIZADO")
        registrar("CONCLUIR_LIMPIEZA", codigo, actual.Autorizacion)
        Return ServicioConsultaImportacion.ResultadoLimpieza(actual, "CONFIRMADO", codigo, False)
    End Function
    Private Shared Sub ValidarParLimpieza(esperada As VistaLimpiezaOriginalImportacion, actual As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, instante As DateTime)
        ServicioConsultaImportacion.ValidarVistaLimpieza(esperada, politica, instante)
        ServicioConsultaImportacion.ValidarVistaLimpieza(actual, politica, instante)
        If esperada.ArchivoId <> actual.ArchivoId OrElse PerfilImportacionJson.Escribir(esperada.Inventario) <> PerfilImportacionJson.Escribir(actual.Inventario) OrElse (esperada.Autorizacion IsNot Nothing AndAlso (actual.Autorizacion Is Nothing OrElse PerfilImportacionJson.Escribir(esperada.Autorizacion) <> PerfilImportacionJson.Escribir(actual.Autorizacion))) Then Throw ConflictoOperacion()
    End Sub
    Private Shared Function CopiarVistaLimpieza(vista As VistaLimpiezaOriginalImportacion) As VistaLimpiezaOriginalImportacion
        Return PerfilImportacionJson.Leer(Of VistaLimpiezaOriginalImportacion)(PerfilImportacionJson.Escribir(vista))
    End Function
End Class
