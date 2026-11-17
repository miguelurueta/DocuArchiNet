Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Private Shared Function HuellaPlanPersistido(plan As PlanImportacionCarpetas) As Byte()
        If plan.EsquemaDoc = 1US Then Return PerfilImportacionJson.HuellaPlan(plan)
        If plan.EsquemaDoc = 2US Then Return PlanSegmentadoImportacionJson.HuellaPlan(plan)
        Throw NoDisponible()
    End Function

    Private Shared Sub ValidarReferenciasSegmentadas(plan As PlanImportacionCarpetas)
        Dim manifiesto = plan.ReferenciaManifiestoSegmentado, resultado = plan.ReferenciaResultadoSegmentado
        If manifiesto Is Nothing OrElse resultado Is Nothing OrElse manifiesto.Esquema <> 2US OrElse resultado.Esquema <> 2US OrElse
            manifiesto.PreparacionId <> resultado.PreparacionId OrElse manifiesto.Huella <> plan.Huella OrElse resultado.Huella <> plan.Huella OrElse
            manifiesto.Cantidad < 2 OrElse resultado.Cantidad < 1 Then Throw NoDisponible()
        IdOperacion(manifiesto.PreparacionId)
        plan.PreparacionId = manifiesto.PreparacionId
    End Sub

    Private Shared Sub CargarPlanSegmentado(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, plan As PlanImportacionCarpetas)
        Using cmd = ComandoOperacion(conexion, tx, "SELECT contexto_doc,config_doc,estado,creado_utc,expira_utc FROM ra_integracion_preparacion WHERE preparacion_id=@prep AND plan_id=@plan AND ambito_ref=@ambito AND usuario_ref=@usuario AND nivel_ref=@nivel AND version_id=@version",
            "@prep", IdOperacion(plan.PreparacionId), "@plan", IdOperacion(plan.PlanId), "@ambito", contexto.AmbitoRef,
            "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef, "@version", IdOperacion(plan.Snapshot.VersionId))
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() OrElse CStr(reader("estado")) <> "SELLADA" OrElse
                    Not MismoDestino(contexto, LeerPersistido(Of ContextoImportacionCarpetas)(CStr(reader("contexto_doc")))) OrElse
                    Not PerfilImportacionJson.MismaPublicacion(LeerPersistido(Of SnapshotPerfilImportacionOperativa)(CStr(reader("config_doc"))), plan.Snapshot) OrElse
                    TextoUtc(CDate(reader("creado_utc"))) <> plan.CreadoUtc OrElse TextoUtc(CDate(reader("expira_utc"))) <> plan.ExpiraUtc Then Throw NoDisponible()
            End Using
        End Using
        Dim fragmentos As New List(Of FragmentoPlanImportacion)()
        Using cmd = ComandoOperacion(conexion, tx, "SELECT tipo,numero,contenido_doc,contenido_hash FROM ra_integracion_preparacion_resultado WHERE preparacion_id=@prep AND tipo IN ('RAICES','ARCHIVOS','ELEMENTOS','INCIDENCIAS','ELEGIBLES','CONFIRMABLE') ORDER BY tipo,numero", "@prep", IdOperacion(plan.PreparacionId))
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    fragmentos.Add(New FragmentoPlanImportacion With {.Tipo = CStr(reader("tipo")), .Numero = Convert.ToUInt32(reader("numero"), CultureInfo.InvariantCulture),
                        .Contenido = CStr(reader("contenido_doc")), .Huella = Hexadecimal(DirectCast(reader("contenido_hash"), Byte()))})
                End While
            End Using
        End Using
        Try
            PlanSegmentadoImportacionJson.Reconstruir(plan, fragmentos)
            If plan.Manifiesto.Raices.Count + plan.Manifiesto.Archivos.Count <> plan.ReferenciaManifiestoSegmentado.Cantidad OrElse
                plan.Resultado.Elementos.Count + plan.Resultado.Incidencias.Count + plan.Resultado.UnidadesElegibles.Count + 1 <> plan.ReferenciaResultadoSegmentado.Cantidad Then Throw NoDisponible()
            ValidarInventarioPersistible(plan)
            If Not Iguales(HuellaPlanPersistido(plan), BytesHuella(plan.Huella)) Then Throw NoDisponible()
        Catch ex As PerfilImportacionException
            Throw NoDisponible()
        End Try
    End Sub
End Class
