Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Implements IPreparacionSegmentadaImportacionRepository
    Private Const ColumnasCabeceraPreparacion As String = "preparacion_id,ambito_ref,usuario_ref,nivel_ref,version_id,clave,cabecera_hash,contexto_doc,config_doc,total_raices,total_archivos,total_bytes,partes_raices,partes_archivos,estado,revision,plan_id,creado_utc,expira_utc"

    Private Function OperarPreparacion(Of T)(accion As Func(Of MySqlConnection, MySqlTransaction, T)) As T
        Return Operar(Function(conexion, tx)
                          ValidarMetadatosPreparacion(LeerMetadatos(conexion, TablasPreparacion, TablasPreparacion, tx))
                          Return accion(conexion, tx)
                      End Function, IsolationLevel.RepeatableRead)
    End Function

    Public Function IniciarPreparacion(contexto As ContextoImportacionCarpetas, original As PreparacionSegmentadaImportacion) As PreparacionSegmentadaImportacion Implements IPreparacionSegmentadaImportacionRepository.IniciarPreparacion
        AutorizarOperacion(contexto)
        If original Is Nothing OrElse original.Contexto Is Nothing OrElse Not MismoDestino(contexto, original.Contexto) OrElse original.Snapshot Is Nothing OrElse
            original.Clave Is Nothing OrElse Not System.Text.RegularExpressions.Regex.IsMatch(original.Clave, "\A[0-9a-f]{32}\z") OrElse original.TotalRaices = 0 OrElse original.TotalArchivos = 0 OrElse
            original.PartesRaices = 0 OrElse original.PartesArchivos = 0 OrElse original.PartesRaices > original.TotalRaices OrElse original.PartesArchivos > original.TotalArchivos OrElse
            Fecha(original.CreadoUtc) > Ahora() OrElse Fecha(original.ExpiraUtc) <= Ahora() Then Throw SolicitudInvalida()
        BytesHuella(original.CabeceraHash) : BytesDecimales(original.TotalBytes)
        Return OperarPreparacion(Function(conexion, tx)
            Using cmd = ComandoOperacion(conexion, tx, "SELECT preparacion_id,cabecera_hash,nivel_ref FROM ra_integracion_preparacion WHERE ambito_ref=@ambito AND usuario_ref=@usuario AND clave=@clave FOR UPDATE", "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@clave", original.Clave)
                Dim id As String = Nothing
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        If Not Iguales(DirectCast(reader("cabecera_hash"), Byte()), BytesHuella(original.CabeceraHash)) OrElse CStr(reader("nivel_ref")) <> contexto.NivelRef Then Throw ConflictoOperacion()
                        id = Convert.ToString(reader("preparacion_id"), CultureInfo.InvariantCulture)
                    End If
                End Using
                If id IsNot Nothing Then Return CargarPreparacion(conexion, tx, contexto, id, False)
            End Using
            VerificarPublicacion(conexion, tx, contexto, original.Snapshot)
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_preparacion (ambito_ref,usuario_ref,nivel_ref,version_id,clave,cabecera_hash,contexto_doc,config_doc,total_raices,total_archivos,total_bytes,partes_raices,partes_archivos,estado,revision,creado_utc,expira_utc) VALUES (@ambito,@usuario,@nivel,@version,@clave,@hash,@contexto,@config,@raices,@archivos,@bytes,@pr,@pa,'BORRADOR',1,@creado,@expira)",
                "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef, "@version", IdOperacion(original.Snapshot.VersionId), "@clave", original.Clave, "@hash", BytesHuella(original.CabeceraHash),
                "@contexto", PerfilImportacionJson.Escribir(contexto), "@config", PerfilImportacionJson.Escribir(original.Snapshot), "@raices", original.TotalRaices, "@archivos", original.TotalArchivos, "@bytes", BytesDecimales(original.TotalBytes),
                "@pr", original.PartesRaices, "@pa", original.PartesArchivos, "@creado", Fecha(original.CreadoUtc), "@expira", Fecha(original.ExpiraUtc))
                UnaFila(cmd.ExecuteNonQuery())
                Return CargarPreparacion(conexion, tx, contexto, IdentidadInsertada(cmd), False)
            End Using
        End Function)
    End Function

    Public Function ObtenerReferenciaPreparacion(identidad As ContextoImportacionCarpetas, preparacionId As String) As ContextoImportacionCarpetas Implements IPreparacionSegmentadaImportacionRepository.ObtenerReferenciaPreparacion
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw AccesoDenegado()
        IdOperacion(preparacionId)
        Return OperarPreparacion(Function(conexion, tx)
            Using cmd = ComandoOperacion(conexion, tx, "SELECT contexto_doc FROM ra_integracion_preparacion WHERE preparacion_id=@id AND ambito_ref=@ambito AND usuario_ref=@usuario", "@id", IdOperacion(preparacionId), "@ambito", identidad.AmbitoRef, "@usuario", identidad.UsuarioRef)
                Dim doc = cmd.ExecuteScalar()
                If doc Is Nothing OrElse Convert.IsDBNull(doc) Then Throw AccesoDenegado()
                Dim contexto = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(doc))
                If contexto.UsuarioRef <> identidad.UsuarioRef OrElse contexto.AmbitoRef <> identidad.AmbitoRef Then Throw AccesoDenegado()
                Return contexto
            End Using
        End Function)
    End Function

    Public Function ObtenerPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String) As PreparacionSegmentadaImportacion Implements IPreparacionSegmentadaImportacionRepository.ObtenerPreparacion
        AutorizarOperacion(contexto) : IdOperacion(preparacionId)
        Return OperarPreparacion(Function(conexion, tx) CargarPreparacion(conexion, tx, contexto, preparacionId, False))
    End Function

    Public Function BuscarPreparacionPorClave(contexto As ContextoImportacionCarpetas, clave As String) As PreparacionSegmentadaImportacion Implements IPreparacionSegmentadaImportacionRepository.BuscarPreparacionPorClave
        AutorizarOperacion(contexto)
        If Not System.Text.RegularExpressions.Regex.IsMatch(If(clave, ""), "\A[0-9a-f]{32}\z") Then Throw SolicitudInvalida()
        Return OperarPreparacion(Function(conexion, tx)
            Using cmd = ComandoOperacion(conexion, tx, "SELECT preparacion_id FROM ra_integracion_preparacion WHERE ambito_ref=@ambito AND usuario_ref=@usuario AND nivel_ref=@nivel AND clave=@clave", "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef, "@clave", clave)
                Dim valor = cmd.ExecuteScalar()
                If valor Is Nothing OrElse Convert.IsDBNull(valor) Then Return Nothing
                Return CargarPreparacion(conexion, tx, contexto, Convert.ToString(valor, CultureInfo.InvariantCulture), False)
            End Using
        End Function)
    End Function

    Private Shared Function CargarPreparacion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, id As String, bloquear As Boolean) As PreparacionSegmentadaImportacion
        Using cmd = ComandoOperacion(conexion, tx, "SELECT " & ColumnasCabeceraPreparacion & " FROM ra_integracion_preparacion WHERE preparacion_id=@id AND ambito_ref=@ambito AND usuario_ref=@usuario AND nivel_ref=@nivel" & If(bloquear, " FOR UPDATE", ""), "@id", IdOperacion(id), "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef)
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Throw AccesoDenegado()
                Dim p As New PreparacionSegmentadaImportacion With {.PreparacionId = id, .Contexto = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(r("contexto_doc"))), .Snapshot = LeerPersistido(Of SnapshotPerfilImportacionOperativa)(CStr(r("config_doc"))),
                    .Clave = CStr(r("clave")), .CabeceraHash = Hexadecimal(DirectCast(r("cabecera_hash"), Byte())), .TotalRaices = Convert.ToUInt32(r("total_raices")), .TotalArchivos = Convert.ToUInt32(r("total_archivos")), .TotalBytes = Convert.ToString(r("total_bytes"), CultureInfo.InvariantCulture),
                    .PartesRaices = Convert.ToUInt32(r("partes_raices")), .PartesArchivos = Convert.ToUInt32(r("partes_archivos")), .Estado = CStr(r("estado")), .Revision = Convert.ToUInt32(r("revision")), .PlanId = If(r.IsDBNull(r.GetOrdinal("plan_id")), Nothing, Convert.ToString(r("plan_id"), CultureInfo.InvariantCulture)), .CreadoUtc = TextoUtc(CDate(r("creado_utc"))), .ExpiraUtc = TextoUtc(CDate(r("expira_utc")))}
                If Not MismoDestino(contexto, p.Contexto) OrElse p.Revision = 0 OrElse p.Snapshot Is Nothing OrElse p.Snapshot.VersionId <> Convert.ToString(r("version_id"), CultureInfo.InvariantCulture) OrElse
                    Not {"BORRADOR", "SELLADA", "EXPIRADA", "INVALIDADA"}.Contains(p.Estado) OrElse (p.Estado = "SELLADA") <> (p.PlanId IsNot Nothing) Then Throw NoDisponible()
                Return p
            End Using
        End Using
    End Function

    Public Function LeerPartesPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String) As List(Of PartePreparacionImportacion) Implements IPreparacionSegmentadaImportacionRepository.LeerPartesPreparacion
        AutorizarOperacion(contexto) : IdOperacion(preparacionId)
        Return OperarPreparacion(Function(conexion, tx)
            CargarPreparacion(conexion, tx, contexto, preparacionId, False)
            Return CargarPartesPreparacion(conexion, tx, preparacionId)
        End Function)
    End Function

    Private Shared Function CargarPartesPreparacion(conexion As MySqlConnection, tx As MySqlTransaction, id As String) As List(Of PartePreparacionImportacion)
        Dim partes As New List(Of PartePreparacionImportacion)()
        Using cmd = ComandoOperacion(conexion, tx, "SELECT tipo,numero,contenido_doc,contenido_hash,estado,sello_catalogos FROM ra_integracion_preparacion_parte WHERE preparacion_id=@id ORDER BY tipo,numero", "@id", IdOperacion(id))
            Using r = cmd.ExecuteReader()
                While r.Read()
                    Dim p As New PartePreparacionImportacion With {.Tipo = CStr(r("tipo")), .Numero = Convert.ToUInt32(r("numero")), .Contenido = CStr(r("contenido_doc")), .Huella = Hexadecimal(DirectCast(r("contenido_hash"), Byte())), .Estado = CStr(r("estado")), .SelloCatalogos = If(r.IsDBNull(r.GetOrdinal("sello_catalogos")), Nothing, CStr(r("sello_catalogos")))}
                    If Not {"RAICES", "ARCHIVOS"}.Contains(p.Tipo) OrElse p.Numero = 0 OrElse p.Contenido.Length > PerfilImportacionJson.MaximoCaracteres OrElse p.Huella <> PlanSegmentadoImportacionJson.HuellaTexto(p.Contenido) OrElse Not {"REGISTRADA", "VALIDADA"}.Contains(p.Estado) Then Throw NoDisponible()
                    partes.Add(p)
                End While
            End Using
        End Using
        Return partes
    End Function

    Private Sub ExigirBorrador(p As PreparacionSegmentadaImportacion, revision As UInteger)
        If p.Estado <> "BORRADOR" OrElse p.Revision <> revision OrElse revision = UInteger.MaxValue OrElse Fecha(p.ExpiraUtc) <= Ahora() Then Throw ConflictoOperacion()
    End Sub

    Public Function RegistrarParte(contexto As ContextoImportacionCarpetas, id As String, revision As UInteger, parte As PartePreparacionImportacion) As PreparacionSegmentadaImportacion Implements IPreparacionSegmentadaImportacionRepository.RegistrarParte
        AutorizarOperacion(contexto) : IdOperacion(id)
        If parte Is Nothing OrElse Not {"RAICES", "ARCHIVOS"}.Contains(parte.Tipo) OrElse parte.Numero = 0 OrElse parte.Contenido Is Nothing OrElse parte.Contenido.Length > PerfilImportacionJson.MaximoCaracteres OrElse parte.Huella <> PlanSegmentadoImportacionJson.HuellaTexto(parte.Contenido) Then Throw SolicitudInvalida()
        Return OperarPreparacion(Function(conexion, tx)
            Dim p = CargarPreparacion(conexion, tx, contexto, id, True)
            Dim existente = CargarPartesPreparacion(conexion, tx, id).SingleOrDefault(Function(a) a.Tipo = parte.Tipo AndAlso a.Numero = parte.Numero)
            If existente IsNot Nothing Then
                If existente.Huella <> parte.Huella OrElse existente.Contenido <> parte.Contenido Then Throw ConflictoOperacion()
                Return p
            End If
            ExigirBorrador(p, revision)
            If parte.Numero > If(parte.Tipo = "RAICES", p.PartesRaices, p.PartesArchivos) Then Throw SolicitudInvalida()
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_preparacion_parte (preparacion_id,tipo,numero,contenido_doc,contenido_hash,estado) VALUES (@id,@tipo,@numero,@doc,@hash,'REGISTRADA')", "@id", IdOperacion(id), "@tipo", parte.Tipo, "@numero", parte.Numero, "@doc", parte.Contenido, "@hash", BytesHuella(parte.Huella))
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            ActualizarPreparacion(conexion, tx, p)
            Return CargarPreparacion(conexion, tx, contexto, id, False)
        End Function)
    End Function

    Public Shared Function PrefijoResultadoParte(tipo As String, numero As UInteger) As String
        If Not {"RAICES", "ARCHIVOS"}.Contains(tipo) OrElse numero = 0 Then Throw SolicitudInvalida()
        Return If(tipo = "RAICES", "R", "A") & numero.ToString(CultureInfo.InvariantCulture) & "_"
    End Function

    Public Function GuardarValidacionParte(contexto As ContextoImportacionCarpetas, id As String, revision As UInteger, tipo As String, numero As UInteger, sello As String, fragmentos As List(Of FragmentoPlanImportacion)) As PreparacionSegmentadaImportacion Implements IPreparacionSegmentadaImportacionRepository.GuardarValidacionParte
        AutorizarOperacion(contexto) : IdOperacion(id) : BytesHuella(sello)
        Dim prefijo = PrefijoResultadoParte(tipo, numero)
        Return OperarPreparacion(Function(conexion, tx)
            Dim p = CargarPreparacion(conexion, tx, contexto, id, True), partes = CargarPartesPreparacion(conexion, tx, id)
            Dim parte = partes.SingleOrDefault(Function(a) a.Tipo = tipo AndAlso a.Numero = numero)
            If parte Is Nothing Then Throw ConflictoOperacion()
            If partes.Any(Function(a) a.Estado = "VALIDADA" AndAlso a.SelloCatalogos <> sello) Then Throw ConflictoOperacion()
            If parte.Estado = "VALIDADA" Then Return p
            ExigirBorrador(p, revision)
            GuardarFragmentos(conexion, tx, id, prefijo, fragmentos)
            Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_preparacion_parte SET estado='VALIDADA',sello_catalogos=@sello WHERE preparacion_id=@id AND tipo=@tipo AND numero=@numero AND estado='REGISTRADA'", "@sello", sello, "@id", IdOperacion(id), "@tipo", tipo, "@numero", numero)
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            ActualizarPreparacion(conexion, tx, p)
            Return CargarPreparacion(conexion, tx, contexto, id, False)
        End Function)
    End Function

    Private Shared Sub GuardarFragmentos(conexion As MySqlConnection, tx As MySqlTransaction, id As String, prefijo As String, fragmentos As List(Of FragmentoPlanImportacion))
        If fragmentos Is Nothing Then Throw SolicitudInvalida()
        For Each f In fragmentos
            If f Is Nothing OrElse Not {"RAICES", "ARCHIVOS", "ELEMENTOS", "INCIDENCIAS", "ELEGIBLES", "CONFIRMABLE"}.Contains(f.Tipo) OrElse f.Numero = 0 OrElse f.Contenido Is Nothing OrElse f.Contenido.Length > PerfilImportacionJson.MaximoCaracteres OrElse f.Huella <> PlanSegmentadoImportacionJson.HuellaTexto(f.Contenido) Then Throw SolicitudInvalida()
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_preparacion_resultado (preparacion_id,tipo,numero,contenido_doc,contenido_hash) VALUES (@id,@tipo,@numero,@doc,@hash)", "@id", IdOperacion(id), "@tipo", prefijo & f.Tipo, "@numero", f.Numero, "@doc", f.Contenido, "@hash", BytesHuella(f.Huella))
                UnaFila(cmd.ExecuteNonQuery())
            End Using
        Next
    End Sub

    Private Shared Sub ActualizarPreparacion(conexion As MySqlConnection, tx As MySqlTransaction, p As PreparacionSegmentadaImportacion)
        Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_preparacion SET revision=revision+1 WHERE preparacion_id=@id AND revision=@revision AND estado='BORRADOR'", "@id", IdOperacion(p.PreparacionId), "@revision", p.Revision)
            UnaFila(cmd.ExecuteNonQuery())
        End Using
    End Sub

    Public Function LeerResultadosPreparacion(contexto As ContextoImportacionCarpetas, id As String) As List(Of FragmentoPlanImportacion) Implements IPreparacionSegmentadaImportacionRepository.LeerResultadosPreparacion
        AutorizarOperacion(contexto) : IdOperacion(id)
        Return OperarPreparacion(Function(conexion, tx)
            CargarPreparacion(conexion, tx, contexto, id, False)
            Dim resultado As New List(Of FragmentoPlanImportacion)()
            Using cmd = ComandoOperacion(conexion, tx, "SELECT tipo,numero,contenido_doc,contenido_hash FROM ra_integracion_preparacion_resultado WHERE preparacion_id=@id ORDER BY tipo,numero", "@id", IdOperacion(id))
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        resultado.Add(New FragmentoPlanImportacion With {.Tipo = CStr(r("tipo")), .Numero = Convert.ToUInt32(r("numero")), .Contenido = CStr(r("contenido_doc")), .Huella = Hexadecimal(DirectCast(r("contenido_hash"), Byte()))})
                    End While
                End Using
            End Using
            Return resultado
        End Function)
    End Function

    Public Function SellarPreparacion(contexto As ContextoImportacionCarpetas, id As String, revision As UInteger, clave As String, original As PlanImportacionCarpetas) As PlanImportacionCarpetas Implements IPreparacionSegmentadaImportacionRepository.SellarPreparacion
        AutorizarOperacion(contexto) : IdOperacion(id)
        Return OperarPreparacion(Function(conexion, tx)
            Dim p = CargarPreparacion(conexion, tx, contexto, id, True)
            If clave <> p.Clave Then Throw ConflictoOperacion()
            If p.Estado = "SELLADA" Then Return CargarPlan(conexion, tx, contexto, IdOperacion(p.PlanId), False)
            ExigirBorrador(p, revision)
            Dim partes = CargarPartesPreparacion(conexion, tx, id)
            ExigirPartesCompletas(p, partes, True)
            If original Is Nothing OrElse original.EsquemaDoc <> 2US OrElse original.PreparacionId <> id OrElse original.CreadoUtc <> p.CreadoUtc OrElse original.ExpiraUtc <> p.ExpiraUtc OrElse Not PerfilImportacionJson.MismaPublicacion(original.Snapshot, p.Snapshot) Then Throw ConflictoOperacion()
            Dim inventario = InventarioDePartes(partes)
            If PlanSegmentadoImportacionJson.HuellaInventario(inventario) <> PlanSegmentadoImportacionJson.HuellaInventario(original.Manifiesto) Then Throw ConflictoOperacion()
            Dim plan = PrepararPersistencia(contexto, original)
            VerificarPublicacion(conexion, tx, contexto, p.Snapshot)
            Dim mr As New ReferenciaPlanSegmentadoImportacion With {.Esquema = 2US, .PreparacionId = id, .Huella = plan.Huella, .Cantidad = plan.Manifiesto.Raices.Count + plan.Manifiesto.Archivos.Count}
            Dim rr As New ReferenciaPlanSegmentadoImportacion With {.Esquema = 2US, .PreparacionId = id, .Huella = plan.Huella, .Cantidad = plan.Resultado.Elementos.Count + plan.Resultado.Incidencias.Count + plan.Resultado.UnidadesElegibles.Count + 1}
            GuardarFragmentos(conexion, tx, id, "", PlanSegmentadoImportacionJson.Fragmentar(plan))
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_plan (ambito_ref,usuario_ref,nivel_ref,version_id,estado,revision,esquema_doc,contexto_doc,config_doc,manifiesto_doc,resultado_doc,plan_hash,creado_utc,expira_utc) VALUES (@ambito,@usuario,@nivel,@version,@estado,1,2,@contexto,@config,@manifiesto,@resultado,@hash,@creado,@expira)", "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef, "@version", IdOperacion(p.Snapshot.VersionId), "@estado", plan.Estado, "@contexto", PerfilImportacionJson.Escribir(contexto), "@config", PerfilImportacionJson.Escribir(p.Snapshot), "@manifiesto", PerfilImportacionJson.Escribir(mr), "@resultado", PerfilImportacionJson.Escribir(rr), "@hash", BytesHuella(plan.Huella), "@creado", Fecha(plan.CreadoUtc), "@expira", Fecha(plan.ExpiraUtc))
                UnaFila(cmd.ExecuteNonQuery()) : plan.PlanId = IdentidadInsertada(cmd)
            End Using
            Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_preparacion SET estado='SELLADA',revision=revision+1,plan_id=@plan WHERE preparacion_id=@id AND revision=@revision AND estado='BORRADOR'", "@plan", IdOperacion(plan.PlanId), "@id", IdOperacion(id), "@revision", revision)
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            Return plan
        End Function)
    End Function

    Friend Shared Sub ExigirPartesCompletas(p As PreparacionSegmentadaImportacion, partes As List(Of PartePreparacionImportacion), validadas As Boolean)
        If partes Is Nothing OrElse CULng(partes.Count) <> CULng(p.PartesRaices) + p.PartesArchivos Then Throw ConflictoOperacion()
        For Each tipo In {"RAICES", "ARCHIVOS"}
            Dim grupo = partes.Where(Function(a) a.Tipo = tipo).OrderBy(Function(a) a.Numero).ToList(), previsto = If(tipo = "RAICES", p.PartesRaices, p.PartesArchivos)
            If CULng(grupo.Count) <> previsto Then Throw ConflictoOperacion()
            For n = 0 To grupo.Count - 1
                If grupo(n).Numero <> CUInt(n + 1) OrElse (validadas AndAlso grupo(n).Estado <> "VALIDADA") Then Throw ConflictoOperacion()
            Next
        Next
        Dim inventario = InventarioDePartes(partes)
        If inventario.Raices.Count <> p.TotalRaices OrElse inventario.Archivos.Count <> p.TotalArchivos Then Throw ConflictoOperacion()
        Dim total As ULong = 0UL
        For Each archivo In inventario.Archivos
            Dim bytes = BytesDecimales(archivo.Bytes)
            If ULong.MaxValue - total < bytes Then Throw ConflictoOperacion()
            total += bytes
        Next
        If total <> BytesDecimales(p.TotalBytes) Then Throw ConflictoOperacion()
    End Sub

    Friend Shared Function InventarioDePartes(partes As List(Of PartePreparacionImportacion)) As ManifiestoImportacionCarpetas
        Dim salida As New ManifiestoImportacionCarpetas()
        For Each parte In partes
            If parte.Tipo = "RAICES" Then
                salida.Raices.AddRange(PerfilImportacionJson.Leer(Of List(Of RaizManifiestoImportacion))(parte.Contenido))
            ElseIf parte.Tipo = "ARCHIVOS" Then
                salida.Archivos.AddRange(PerfilImportacionJson.Leer(Of List(Of ArchivoManifiestoImportacion))(parte.Contenido))
            Else
                Throw SolicitudInvalida()
            End If
        Next
        Return salida
    End Function
End Class
