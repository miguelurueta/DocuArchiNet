Imports System
Partial Class Reportes
    Inherits System.Web.UI.Page
    Public UserContras As String = ""
    Dim Dato_Sql As String = ""
    Dim Dato_Sql_Consulta As String = ""
    Dim Conectio_Documnent As String = ""

   

    'Protected Sub TreeView1_SelectedNodeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TreeView1.SelectedNodeChanged

    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim Ref_Car_Conec As New conect.vb.Dbase_Conction_Mysql
        'Dim Parametro_Consulta As String = ""
        'Dim Ref As New consulta
        'Dim Result As String = ""
        'Dim Datos_Nodo As String = ""
        ''leer parametro de conexion
        'Dim Contenido_Config As String = ""
        'Dim Fichero_Config As String = Server.MapPath("./config/rerportador.txt")
        ''referencriacion del datareader que devuelbe el componente
        'Result = Ref.Leer_Parametro_Conexion(Conectio_Documnent, Fichero_Config)
        'If Result <> "YES" Then

        'End If
        'UserContras = Request.Params("name")
        ''lista reportes workflow por grupos

        'If Not Page.IsPostBack Then
        '    Me.TreeView1.Nodes.Clear()
        '    Result = Ref.Listar_Reportes_Grupos_Treview(Me.TreeView1, UserContras, Conectio_Documnent)
        'End If
        'If Page.IsPostBack Then
        '    Dim Matri_Nodo() As String
        '    Erase Matri_Nodo
        '    Me.TableConsulta.Rows.Clear()
        '    'consulta dato de nodo seleccionado
        '    Result = Ref.NodoChild_Selecionado(Me.TreeView1, Datos_Nodo)
        '    If Result <> "YES" Then
        '        Exit Sub
        '    Else
        '        Matri_Nodo = Split(Datos_Nodo, "|")
        '    End If
        '    Result = ""
        '    If Not Matri_Nodo Is Nothing Then
        '        'consulta el codigo de consulta
        '        Result = Ref.Datos_Sql_Reporte(Conectio_Documnent, Matri_Nodo(0), Dato_Sql)
        '        If Dato_Sql <> "" Then
        '            Dim MatriSql() As String = Split(Dato_Sql, "//")
        '            Dato_Sql_Consulta = MatriSql(0)
        '            Consulta_Reporte(Dato_Sql)
        '        End If
        '    End If
        'End If

    End Sub
    Sub Consulta_Reporte(ByVal Dato_Sql As String)
        'Dim MatriSql() As String = Split(Dato_Sql, "//")
        'Dim MatriParametros() As String
        'Dim Ref As New consulta
        'Dim Result As String = ""
        'Erase MatriParametros
        ''verifica que tenga parametros la consulta
        'If Not MatriSql Is Nothing And UBound(MatriSql) > 0 Then
        '    MatriParametros = Split(MatriSql(1), "#")
        'End If
        ''Verifica parametros de consulta
        'Dim i As Integer = 0
        'Dim Darodoc As String = ""
        'Dim CodigoSQL2 As String = ""
        'If Not MatriParametros Is Nothing Then
        '    If MatriParametros(0) <> "" Then
        '        Crear_Parametro_consulta(MatriParametros)
        '    Else
        '        Result = Ref.Resultado_Consulta(Me.TableConsulta, Conectio_Documnent, Trim(MatriSql(0)))
        '    End If
        'End If
        'If i = 0 Then

        'End If

    End Sub
    Sub Crear_Parametro_consulta(ByVal Matri_Parameros() As String)
        'Dim i As Integer = 0
        'Dim RegisTro = New TableRow
        'Dim CellDa = New TableCell
        'Dim Lebol = New Label
        'Lebol.Text = "Por Favor Ingrese Los Parametros de Consulta "
        'Lebol.Font.Size = 10
        'CellDa.Controls.Add(Lebol)
        'RegisTro.Controls.Add(CellDa)
        'CellDa = New TableCell
        'RegisTro.Controls.Add(CellDa)
        'Me.TableParametro.Controls.Add(RegisTro)
        'For i = 0 To UBound(Matri_Parameros) - 1
        '    RegisTro = New TableRow
        '    CellDa = New TableCell
        '    Dim TexboxC As New TextBox
        '    Dim Labelc As New Label
        '    TexboxC.ID = Matri_Parameros(i)
        '    Labelc.Text = Matri_Parameros(i)
        '    Labelc.Font.Size = 10
        '    'CellDa.ForeColor = Color.MidnightBlue
        '    'CellDa.Font.Bold = True
        '    CellDa.controls.add(Labelc)
        '    CellDa.controls.add(TexboxC)
        '    RegisTro.Controls.Add(CellDa)
        '    Me.TableParametro.Controls.Add(RegisTro)
        'Next
        'RegisTro = New TableRow
        'CellDa = New TableCell
        'Dim Butonc As New Button
        'Butonc.Text = "Consultar"
        'AddHandler Butonc.Click, AddressOf Button_Click
        'CellDa.controls.add(Butonc)
        'RegisTro.Controls.Add(CellDa)
        'Me.TableParametro.Controls.Add(RegisTro)
        ''Asignar matris de parametros al boton
        'Butonc.ID = "1|"
        'For i = 0 To UBound(Matri_Parameros)
        '    Butonc.ID = Butonc.ID & Matri_Parameros(i) & "|"
        'Next


    End Sub
    Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Dim Matri_Parametro() As String
        'Dim RefDato_Sql_Consulta As String = Trim(Dato_Sql_Consulta)
        'Dim ref As New consulta
        'Dim Result As String = ""
        'Erase Matri_Parametro
        'Matri_Parametro = Split(sender.clientid, "|")
        'Dim i As Integer = 0
        'Dim Darodoc As String
        ''Remplazar parametros en la cosnulta
        'If Not Matri_Parametro Is Nothing Then
        '    For i = 1 To UBound(Matri_Parametro)
        '        Darodoc = "#" & Matri_Parametro(i)
        '        If InStr(RefDato_Sql_Consulta, Darodoc) Then
        '            Dim ValorParametro As String = ""
        '            obtener_valor_parametro(Matri_Parametro(i), ValorParametro)
        '            RefDato_Sql_Consulta = RefDato_Sql_Consulta.Replace(Darodoc, "'" & ValorParametro & "' ")
        '        End If
        '    Next
        '    Result = ref.Resultado_Consulta(Me.TableConsulta, Conectio_Documnent, RefDato_Sql_Consulta)

        'End If

    End Sub

    Function obtener_valor_parametro(ByVal Parametro As String, ByRef Valor_Dato As String) As String
        'Dim i As Integer = 0
        'Dim i2 As Integer = 0
        'Dim RefCel As New TableCell
        'Dim Reftextbox As New Object
        'For i = 1 To Me.TableParametro.Rows.Count - 1
        '    RefCel = Me.TableParametro.Rows(i).Cells(0)
        '    For i2 = 0 To RefCel.Controls.Count - 1
        '        Reftextbox = RefCel.Controls(i2)
        '        If Reftextbox.ID = Parametro Then
        '            Valor_Dato = Reftextbox.Text
        '            Exit For
        '        End If
        '    Next

        'Next
        'obtener_valor_parametro = "YES"
    End Function

    'Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkButton1.Click
    '    Server.Transfer("default.aspx", True)
    'End Sub
End Class
