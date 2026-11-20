Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

'Nunca serializa un plan agregado; cada registro usa el canonizador cerrado
'existente. El dominio2 no modifica las huellas/cupos de perfiles o planes1.
Public NotInheritable Class PlanSegmentadoImportacionJson
    Public Const MaximoEntradasParte As Integer = 100
    Public Const MaximoCaracteresTransporte As Integer = 65536
    Private Sub New()
    End Sub

    Public Shared Function Clonar(Of T)(valor As T) As T
        If Object.ReferenceEquals(valor, Nothing) Then Return Nothing
        Return PerfilImportacionJson.Leer(Of T)(PerfilImportacionJson.Escribir(valor))
    End Function

    Public Shared Function ClonarPlan(original As PlanImportacionCarpetas) As PlanImportacionCarpetas
        If original Is Nothing OrElse original.EsquemaDoc <> 2 OrElse original.Manifiesto Is Nothing OrElse original.Resultado Is Nothing Then Throw Invalido()
        Dim salida As New PlanImportacionCarpetas With {
            .PlanId = original.PlanId, .PreparacionId = original.PreparacionId, .EsquemaDoc = 2US,
            .Contexto = Clonar(original.Contexto), .Snapshot = Clonar(original.Snapshot),
            .Estado = original.Estado, .Revision = original.Revision, .Huella = original.Huella,
            .CreadoUtc = original.CreadoUtc, .ExpiraUtc = original.ExpiraUtc,
            .Manifiesto = New ManifiestoImportacionCarpetas(), .Resultado = New ResultadoPreparacionImportacion()}
        salida.Manifiesto.Raices = original.Manifiesto.Raices.Select(Function(r) Clonar(r)).ToList()
        salida.Manifiesto.Archivos = original.Manifiesto.Archivos.Select(Function(a) Clonar(a)).ToList()
        salida.Resultado.PuedeConfirmar = original.Resultado.PuedeConfirmar
        salida.Resultado.UnidadesElegibles = original.Resultado.UnidadesElegibles.ToList()
        salida.Resultado.Elementos = original.Resultado.Elementos.Select(Function(e) Clonar(e)).ToList()
        salida.Resultado.Incidencias = original.Resultado.Incidencias.Select(Function(i) Clonar(i)).ToList()
        Return salida
    End Function

    Public Shared Function ClonarCompatible(plan As PlanImportacionCarpetas) As PlanImportacionCarpetas
        If plan Is Nothing Then Throw Invalido()
        If plan.EsquemaDoc <> 2US Then Return Clonar(plan)
        Return ClonarPlan(plan)
    End Function

    Public Shared Function DocumentoRecuperacion(plan As PlanImportacionCarpetas) As String
        If plan Is Nothing Then Throw Invalido()
        If plan.EsquemaDoc <> 2US Then Return PerfilImportacionJson.Escribir(plan)
        Return PerfilImportacionJson.Escribir(New With {.esquema = 2US, .planId = plan.PlanId, .preparacionId = plan.PreparacionId, .estado = plan.Estado,
            .revision = plan.Revision, .huellaDeclarada = plan.Huella, .huellaCalculada = Hexadecimal(HuellaPlan(plan))})
    End Function

    Public Shared Function ClonarBarrera(vista As VistaBarreraCreacionImportacion) As VistaBarreraCreacionImportacion
        If vista Is Nothing OrElse vista.Plan Is Nothing Then Throw Invalido()
        If vista.Plan.EsquemaDoc <> 2US Then Return Clonar(vista)
        Return New VistaBarreraCreacionImportacion With {.Plan = ClonarPlan(vista.Plan), .Lote = Clonar(vista.Lote), .Raiz = Clonar(vista.Raiz),
            .Raices = If(vista.Raices Is Nothing, Nothing, vista.Raices.Select(Function(r) Clonar(r)).ToList()),
            .Archivos = If(vista.Archivos Is Nothing, Nothing, vista.Archivos.Select(Function(a) Clonar(a)).ToList())}
    End Function

    Public Shared Function ClonarReconciliacion(vista As VistaReconciliacionImportacion) As VistaReconciliacionImportacion
        If vista Is Nothing OrElse vista.Plan Is Nothing Then Throw Invalido()
        If vista.Plan.EsquemaDoc <> 2US Then Return Clonar(vista)
        Return New VistaReconciliacionImportacion With {.Plan = ClonarPlan(vista.Plan), .Lote = Clonar(vista.Lote), .Raiz = Clonar(vista.Raiz), .Archivo = Clonar(vista.Archivo), .Intento = Clonar(vista.Intento)}
    End Function

    Public Shared Function Fragmentar(plan As PlanImportacionCarpetas) As List(Of FragmentoPlanImportacion)
        If plan Is Nothing OrElse plan.EsquemaDoc <> 2 Then Throw Invalido()
        Dim salida As New List(Of FragmentoPlanImportacion)()
        AgregarFragmentos(salida, "RAICES", plan.Manifiesto.Raices)
        AgregarFragmentos(salida, "ARCHIVOS", plan.Manifiesto.Archivos)
        AgregarFragmentos(salida, "ELEMENTOS", plan.Resultado.Elementos)
        AgregarFragmentos(salida, "INCIDENCIAS", plan.Resultado.Incidencias)
        AgregarFragmentos(salida, "ELEGIBLES", plan.Resultado.UnidadesElegibles)
        AgregarFragmentos(salida, "CONFIRMABLE", New List(Of Boolean) From {plan.Resultado.PuedeConfirmar})
        Return salida
    End Function

    Private Shared Sub AgregarFragmentos(Of T)(salida As List(Of FragmentoPlanImportacion), tipo As String, valores As IEnumerable(Of T))
        If valores Is Nothing Then Throw Invalido()
        Dim parte As New List(Of String)(), numero = 1UI, caracteres = 2
        For Each valor In valores
            Dim individual = PerfilImportacionJson.Escribir(valor)
            'Dos corchetes, separadores y todos los escapes cuentan.
            If caracteres + individual.Length + If(parte.Count = 0, 0, 1) > PerfilImportacionJson.MaximoCaracteres OrElse parte.Count = MaximoEntradasParte Then
                If parte.Count = 0 Then Throw Invalido()
                AgregarDocumento(salida, tipo, numero, "[" & String.Join(",", parte) & "]")
                numero += 1UI : parte.Clear() : caracteres = 2
            End If
            caracteres += individual.Length + If(parte.Count = 0, 0, 1)
            parte.Add(individual)
            If individual.Length + 2 > PerfilImportacionJson.MaximoCaracteres Then Throw Invalido()
        Next
        If parte.Count > 0 OrElse numero = 1UI Then AgregarDocumento(salida, tipo, numero, "[" & String.Join(",", parte) & "]")
    End Sub

    Public Shared Sub Reconstruir(plan As PlanImportacionCarpetas, fragmentos As List(Of FragmentoPlanImportacion))
        If plan Is Nothing OrElse plan.EsquemaDoc <> 2 OrElse fragmentos Is Nothing Then Throw Invalido()
        Dim tipos = New String() {"RAICES", "ARCHIVOS", "ELEMENTOS", "INCIDENCIAS", "ELEGIBLES", "CONFIRMABLE"}
        If fragmentos.Any(Function(f) f Is Nothing OrElse Not tipos.Contains(f.Tipo)) Then Throw Invalido()
        plan.Manifiesto = New ManifiestoImportacionCarpetas() : plan.Resultado = New ResultadoPreparacionImportacion()
        For Each tipo In tipos
            Dim partes = fragmentos.Where(Function(f) f.Tipo = tipo).OrderBy(Function(f) f.Numero).ToList()
            If partes.Count = 0 Then Throw Invalido()
            For indice = 0 To partes.Count - 1
                Dim parte = partes(indice)
                If parte.Numero <> CUInt(indice + 1) OrElse parte.Contenido Is Nothing OrElse parte.Contenido.Length > PerfilImportacionJson.MaximoCaracteres OrElse parte.Huella <> HuellaTexto(parte.Contenido) Then Throw Invalido()
                Select Case tipo
                    Case "RAICES" : plan.Manifiesto.Raices.AddRange(LeerParte(Of RaizManifiestoImportacion)(parte))
                    Case "ARCHIVOS" : plan.Manifiesto.Archivos.AddRange(LeerParte(Of ArchivoManifiestoImportacion)(parte))
                    Case "ELEMENTOS" : plan.Resultado.Elementos.AddRange(LeerParte(Of ValoresElementoImportacion)(parte))
                    Case "INCIDENCIAS" : plan.Resultado.Incidencias.AddRange(LeerParte(Of IncidenciaImportacionCarpetas)(parte))
                    Case "ELEGIBLES" : plan.Resultado.UnidadesElegibles.AddRange(LeerParte(Of String)(parte))
                    Case "CONFIRMABLE"
                        Dim valores = LeerParte(Of Boolean)(parte)
                        If partes.Count <> 1 OrElse valores.Count <> 1 Then Throw Invalido()
                        plan.Resultado.PuedeConfirmar = valores(0)
                End Select
            Next
        Next
    End Sub

    Private Shared Function LeerParte(Of T)(parte As FragmentoPlanImportacion) As List(Of T)
        Dim valores = PerfilImportacionJson.Leer(Of List(Of T))(parte.Contenido)
        If valores.Count > MaximoEntradasParte Then Throw Invalido()
        Return valores
    End Function

    Private Shared Sub AgregarDocumento(salida As List(Of FragmentoPlanImportacion), tipo As String, numero As UInteger, documento As String)
        salida.Add(New FragmentoPlanImportacion With {.Tipo = tipo, .Numero = numero, .Contenido = documento, .Huella = HuellaTexto(documento)})
    End Sub

    Public Shared Function HuellaTexto(texto As String) As String
        Using sha = SHA256.Create()
            Return Hexadecimal(sha.ComputeHash(Encoding.UTF8.GetBytes(texto)))
        End Using
    End Function

    Public Shared Function HuellaPlan(plan As PlanImportacionCarpetas) As Byte()
        If plan Is Nothing OrElse plan.EsquemaDoc <> 2 Then Throw Invalido()
        Using sha = SHA256.Create()
            EscribirRegistro(sha, "DOC120_PLAN_2")
            EscribirRegistro(sha, PerfilImportacionJson.Escribir(New With {.contexto = plan.Contexto, .snapshot = plan.Snapshot, .creadoUtc = plan.CreadoUtc, .expiraUtc = plan.ExpiraUtc}))
            Incorporar(sha, "RAICES", plan.Manifiesto.Raices.OrderBy(Function(r) r.IdCliente, StringComparer.Ordinal))
            Incorporar(sha, "ARCHIVOS", plan.Manifiesto.Archivos.OrderBy(Function(a) a.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(a) a.IdCliente, StringComparer.Ordinal))
            Incorporar(sha, "ELEMENTOS", plan.Resultado.Elementos.OrderBy(Function(e) e.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(e) e.ArchivoIdCliente, StringComparer.Ordinal).ThenBy(Function(e) e.Ambito, StringComparer.Ordinal))
            Incorporar(sha, "INCIDENCIAS", plan.Resultado.Incidencias.OrderBy(Function(i) i.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(i) i.ArchivoIdCliente, StringComparer.Ordinal).ThenBy(Function(i) i.Campo, StringComparer.Ordinal).ThenBy(Function(i) i.Codigo, StringComparer.Ordinal).ThenBy(Function(i) i.Mensaje, StringComparer.Ordinal).ThenBy(Function(i) i.Ambito, StringComparer.Ordinal).ThenBy(Function(i) i.BloqueaUnidad))
            Incorporar(sha, "ELEGIBLES", plan.Resultado.UnidadesElegibles.OrderBy(Function(v) v, StringComparer.Ordinal))
            Incorporar(sha, "CONFIRMABLE", New Boolean() {plan.Resultado.PuedeConfirmar})
            sha.TransformFinalBlock(New Byte() {}, 0, 0)
            Return sha.Hash
        End Using
    End Function

    Public Shared Function HuellaInventario(manifiesto As ManifiestoImportacionCarpetas) As String
        If manifiesto Is Nothing Then Throw Invalido()
        Using sha = SHA256.Create()
            EscribirRegistro(sha, "DOC120_INVENTARIO_2")
            Incorporar(sha, "RAICES", manifiesto.Raices.OrderBy(Function(r) r.IdCliente, StringComparer.Ordinal))
            Incorporar(sha, "ARCHIVOS", manifiesto.Archivos.OrderBy(Function(a) a.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(a) a.IdCliente, StringComparer.Ordinal))
            sha.TransformFinalBlock(New Byte() {}, 0, 0)
            Return Hexadecimal(sha.Hash)
        End Using
    End Function

    Private Shared Sub Incorporar(Of T)(sha As HashAlgorithm, tipo As String, valores As IEnumerable(Of T))
        EscribirRegistro(sha, tipo)
        Dim cantidad = 0
        For Each valor In valores
            EscribirRegistro(sha, PerfilImportacionJson.Escribir(valor)) : cantidad += 1
        Next
        EscribirRegistro(sha, "FIN:" & cantidad.ToString(CultureInfo.InvariantCulture))
    End Sub

    Private Shared Sub EscribirRegistro(sha As HashAlgorithm, texto As String)
        Dim bytes = Encoding.UTF8.GetBytes(texto)
        Dim longitud = Encoding.ASCII.GetBytes(bytes.Length.ToString("X8", CultureInfo.InvariantCulture) & ":")
        sha.TransformBlock(longitud, 0, longitud.Length, Nothing, 0)
        sha.TransformBlock(bytes, 0, bytes.Length, Nothing, 0)
    End Sub

    Public Shared Function Hexadecimal(bytes As Byte()) As String
        Return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant()
    End Function

    Private Shared Function Invalido() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La revisión segmentada contiene un elemento inválido o demasiado grande.")
    End Function
End Class
