Imports System.IO
Imports MySql.Data
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO.IsolatedStorage
Public Class ClassGestionArchivo
   
    Public Structure Edificio_Archivo
        Dim id_edificio As Integer
        Dim nombre_edificio As String
        Dim direccion As String
        Dim telefono As String
        Dim responsable As String
        Dim pais_edificio As String
        Dim ciudad_edificio As String
        Dim departamento_edificio As String
    End Structure
    Public Structure Piso_Archivo
        Dim id_piso As Integer
        Dim nombre_piso As String
        Dim codigo_piso As String
        Dim telefono As String
        Dim responsable As String
        Dim edificio_contenedor As String
        Dim EDIFICIO_CONTENEDOR_ID_EDIFICIO As Integer
    End Structure
    Public Structure Area_Archivo
        Dim id_area As Integer
        Dim nombre_area As String
        Dim codigo_area As String
        Dim telefono As String
        Dim responsable As String
        Dim piso_contenedor As String
        Dim tipo_archivo As String
    End Structure
    Public Structure Modulo_Archivo
        Dim id_modulo As Integer
        Dim nombre_modulo As String
        Dim codigo_modulo As String
        Dim descripcion_modulo As String
        Dim seccion_modulo As String
        Dim area_contenedora As String
        Dim piso_contenedor As String
        Dim edificio_contenedor As String
    End Structure
    Public Structure Estante_Archivo
        Dim id_estante As Integer
        Dim codigo_estante As String
        Dim codigo_unico As String
        Dim descripcion_estante As String
        Dim modulo_contenedor As String
        Dim area_contenedor As String
        Dim piso_contenedor As String
        Dim edificio_contenedor As String
    End Structure
    Public Structure Entrapño_archivo
        Dim id_entreapaño As Integer
        Dim codigo_corto As String
        Dim codigo_unico As String
        Dim estante_contenedor As String
        Dim modulo_contendor As String
        Dim area_contenedor As String
        Dim piso_contenedor As String
        Dim edificio_contenedor As String
        Dim estante As String
        Dim entre_paño As String
    End Structure
    Public Structure Tipo_Unida_Conservacion
        Dim id_tipo_unidad As Integer
        Dim tipo_unidad As Integer
        Dim nombre_unidad As String
        Dim Descripcion_unidad As String
    End Structure
    Function Listar_Entrepaño_Archivo(ByVal id_empresa As Integer, _
                                      ByRef stru_entrepaño() As Entrapño_archivo, _
                                      ByVal id_estante As Integer)
        '*************************************************************************
        'Funcion : Lista area archivo en estrucutura con el parametro id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-07-01 Modíficado para web 2016-08-30
        '*************************************************************************
        Try
            Dim sqlconsulta As String = "SELECT EP.ID_ENTREPAÑO,EP.CODIGO_CORTO,EA.CODIGO_LITERAL,EP.CODIGO_UNICO,MA.NOMBRE_MODULO" & _
            ",AA.NOMBRE_AREA,PA.NOMBRE_PISO,EC.NOMBRE_EDIFICIO,ea.CODIGO_LITERAL,ep.CODIGO_CORTO " & _
            "FROM empresa_gestion_documental as egd " & _
            "INNER JOIN  edificio_contenedor as ec on (egd.ID_EMPRESA=ec.ID_EMPRESA) " & _
            "INNER JOIN  piso_archivo as pa on (ec.ID_EDIFICIO=pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO) " & _
            "INNER JOIN area_archivo as aa on (pa.ID_PISO=aa.PISO_ARCHIVO_ID_PISO) " & _
            "INNER JOIN modulo_archivo as ma on (ma.AREA_ARCHIVO_ID_AREA=aa.ID_AREA) " & _
            "INNER JOIN estante_archivo as ea on (ma.ID_MODULO=ea.MODULO_ARCHIVO_ID_MODULO and ea.id_estante=" & id_estante & ") " & _
            "INNER JOIN entre_paño as ep on (ep.ESTANTE_ARCHIVO_ID_ESTANTE=ea.ID_ESTANTE) " & _
            " where egd.ID_EMPRESA=" & id_empresa & " ORDER BY EA.CODIGO_LITERAL,EP.ID_ENTREPAÑO,MA.NOMBRE_MODULO"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("empresa_gestion_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_Entrepaño_Archivo = "Error solicitando id entrepaño " & Result
                Exit Function
            End If
            Dim incre As Integer = 0
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_entrepaño(incre)
                    stru_entrepaño(incre).id_entreapaño = Datset.Tables(0).Rows(i).Item(0)
                    stru_entrepaño(incre).codigo_corto = Datset.Tables(0).Rows(i).Item(1)
                    stru_entrepaño(incre).estante_contenedor = Datset.Tables(0).Rows(i).Item(2)
                    stru_entrepaño(incre).codigo_unico = Datset.Tables(0).Rows(i).Item(3)
                    stru_entrepaño(incre).modulo_contendor = Datset.Tables(0).Rows(i).Item(4)
                    stru_entrepaño(incre).area_contenedor = Datset.Tables(0).Rows(i).Item(5)
                    stru_entrepaño(incre).piso_contenedor = Datset.Tables(0).Rows(i).Item(6)
                    stru_entrepaño(incre).edificio_contenedor = Datset.Tables(0).Rows(i).Item(7)
                    stru_entrepaño(incre).estante = Datset.Tables(0).Rows(i).Item(8)
                    stru_entrepaño(incre).entre_paño = Datset.Tables(0).Rows(i).Item(9)
                    incre = incre + 1
                Next
                Listar_Entrepaño_Archivo = "YES"
            Else
                Listar_Entrepaño_Archivo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Entrepaño_Archivo = "Inconsistencia general funcion Listar_Entrepaño_Archivo " & ex.Message
        End Try
    End Function
    Function Retorna_id_Entrepaño_id_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
    ByRef id_entre_paño As Integer) As String
        '***********************************************************
        'Funcion : Retorna id del entrepaño relacionado a la unidad
        'de conservacion padre
        'Fecha : 2015-01-26 Modificado para web 2016-8-30
        'Ing :Miguel Angel Urueta Miranda
        '***********************************************************
        Try
            Dim SqlConsulta As String = "select ENTRE_PAÑO_ID_ENTREPAÑO from unidad_conservacion " & _
                                              " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_Entrepaño_id_unidad_conservacion = "Error solicitando id entrepaño " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_entre_paño = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_Entrepaño_id_unidad_conservacion = "YES"
                Exit Function
            Else
                Retorna_id_Entrepaño_id_unidad_conservacion = "Imposible encontrar id del entrepaño"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_Entrepaño_id_unidad_conservacion = "Inconsistencia función Retorna_id_Entrepaño_id_unidad_conservacion" & ex.Message
        End Try

    End Function
    Function Retorna_Id_Estante_por_entrepaño(ByVal Id_entrepaño As Integer, ByRef id_estante As Integer) As String
        '*******************************************************************
        'Funcion : Retorna id el estante del entrepaño
        '
        'Fecha : 2014-07-07
        'Ing : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Dim SqlConsulta As String = "select ESTANTE_ARCHIVO_ID_ESTANTE from entre_paño " & _
                                           " where ID_ENTREPAÑO=" & Id_entrepaño
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Estante_por_entrepaño = "Error solicitando id estante " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_estante = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Id_Estante_por_entrepaño = "YES"
                Exit Function
            Else
                Retorna_Id_Estante_por_entrepaño = "Imposible encontrar id del estante"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_Estante_por_entrepaño = "Inconsistencia general función Retorna_Id_Estante_por_entrepaño " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Archivo_Edificio(ByRef treevi As TreeView, _
                                             ByVal nombre_empresa As String) As String
        '******************************************************************
        'Funcion lista los edificios relacionados a la empresa seleccionada
        'Fecha 2014-09-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                                  id_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Archivo_Edificio = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim Struedifcio() As Edificio_Archivo
            Erase Struedifcio
            Result = Listar_Edificio_archivo(id_empresa, _
                                             Struedifcio)
            If Result <> "YES" Then
                Listar_Entidad_Archivo_Edificio = "Imposible listar edificios " & Result
                Exit Function
            End If
            Result = Listar_Edificio_Archivo_treview(Struedifcio, _
                                                     treevi)
            If Result <> "YES" Then
                Listar_Entidad_Archivo_Edificio = "Imposible listar edificios en listview " & Result
                Exit Function
            End If
            Result = Me.Lista_nodes_piso_archivo_trenode(treevi, _
                                                         nombre_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Archivo_Edificio = Result
                Exit Function
            End If
            Listar_Entidad_Archivo_Edificio = "YES"
        Catch ex As Exception
            Listar_Entidad_Archivo_Edificio = "Inconsistencia general funcion Listar_Entidad_Archivo_Edificio " & ex.Message
        End Try
    End Function
    Function Listar_Edificio_archivo(ByVal id_empresa As Integer, _
                                     ByRef estru_edificio() As Edificio_Archivo) As String
        '**********************************************************
        'Funcion : Lista los edificios registrados en la empresa
        'con le parametro id empresa
        'Ing  : Miguel Angel Urueta Miranda
        'Fecha : 2014-016-19 modificado 2016-09-07 para web
        '***********************************************************
        Try
            Dim SqlConsulta As String = "SELECT ec.ID_EDIFICIO,ec.NOMBRE_EDIFICIO,ec.DIRECCION_EDIFICIO,ec.TELEFONO_EDIFICIO,ec.RESPONSABLE_EDIFICIO " & _
            " FROM  edificio_contenedor  as ec WHERE ID_EMPRESA=" & id_empresa & " order by ID_EDIFICIO"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_Edificio_archivo = "Error Listar_Edificio_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For incre As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_edificio(incre)
                    estru_edificio(incre).id_edificio = Datset.Tables(0).Rows(incre).Item(0)
                    estru_edificio(incre).nombre_edificio = Datset.Tables(0).Rows(incre).Item(1)
                    estru_edificio(incre).direccion = Datset.Tables(0).Rows(incre).Item(2)
                    estru_edificio(incre).telefono = Datset.Tables(0).Rows(incre).Item(3)
                    estru_edificio(incre).responsable = Datset.Tables(0).Rows(incre).Item(4)
                Next
                Listar_Edificio_archivo = "YES"
            Else
                Listar_Edificio_archivo = "Imposible encontrar id del estante"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Edificio_archivo = "Inconsistencia general  funcion Listar_Edificio_archivo " & ex.Message
        End Try
    End Function
    Function Listar_Edificio_Archivo_treview(ByRef estru_edificio() As Edificio_Archivo, _
                                             ByRef Treview As TreeView) As String
        '*******************************************************************
        'Funcion lista los edificios relacionados a la empresa en el treview
        'Fecha 2014-09-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Treview.Nodes.Clear()
            If estru_edificio Is Nothing Then
                Listar_Edificio_Archivo_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_edificio.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Edificio : " & estru_edificio(i).nombre_edificio
                attrNode.Value = estru_edificio(i).id_edificio & "|EDIFICIO|" & estru_edificio(i).nombre_edificio
                attrNode.ToolTip = estru_edificio(i).responsable
                attrNode.ImageUrl = "../workflow/imageneswf/building-light.png"
                Treview.Nodes.Add(attrNode)
            Next

            Listar_Edificio_Archivo_treview = "YES"
        Catch ex As Exception
            Listar_Edificio_Archivo_treview = "Inconsistencia general funcion Listar_Edificio_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Lista_nodes_piso_archivo_trenode(ByRef Tre_vie As TreeView, _
                                              ByVal nombre_empresa As String) As String
        Try
            Dim Result As String = ""
            Dim trenod As TreeNode = Nothing
            If Tre_vie.Nodes.Count > 0 Then
                Dim node_count As Integer = Tre_vie.Nodes.Count
                Dim i As Integer = 0
                For i = 0 To node_count - 1
                    Dim splinodo() As String = Tre_vie.Nodes(i).Value.Split("|")
                    If splinodo(1) = "EDIFICIO" Then
                        Result = Listar_Entidad_Piso_Archivo_edificio(Tre_vie.Nodes(i), _
                                                                      nombre_empresa)
                    End If
                Next
            End If
            Lista_nodes_piso_archivo_trenode = "YES"
        Catch ex As Exception
            Lista_nodes_piso_archivo_trenode = "Inconsistencia general función Lista_nodes_piso_archivo_trenode " & ex.Message
        End Try
    End Function
    Function Recursive_archivo_tre_view(ByRef Tre_vie As TreeView, _
                                        ByVal edificio As String) As String
        Try
            Dim Result As String = ""
            Dim trenod As TreeNode = Nothing
            If Tre_vie.Nodes.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To Tre_vie.Nodes.Count - 1
                    Dim splinodo() As String = Tre_vie.Nodes(i).Value.Split("|")
                    If splinodo(1) = "EDIFICIO" Then
                        Result = Me.Nod_CHILD_busqueda_archivo(Tre_vie.Nodes(i), edificio)
                    End If
                Next
            End If
            Recursive_archivo_tre_view = "YES"
        Catch ex As Exception
            Recursive_archivo_tre_view = "Inconsistencia general función Recursive_archivo_tre_view " & ex.Message
        End Try
    End Function
    Function Recursive_archivo_tre_view_clik(ByRef Tre_vie As TreeNode, _
                                             ByVal edificio As String, _
                                             ByVal opcion_lista_entrepano As Integer, _
                                             ByRef ref_treview_unidad As TreeView, _
                                             ByVal node_tag As String, _
                                             ByVal node_tex As String) As String
        Try

            Dim Result As String = ""
            Dim i As Integer = 0
            For i = 0 To Tre_vie.ChildNodes.Count - 1
                Dim splinodo() As String = Tre_vie.Value.Split("|")
                Result = seleccion_treview_recursive_clik(Tre_vie.ChildNodes(i).Value, Tre_vie.ChildNodes(i), edificio, _
                        opcion_lista_entrepano, ref_treview_unidad, node_tag, node_tex)

            Next
            Recursive_archivo_tre_view_clik = "YES"
        Catch ex As Exception
            Recursive_archivo_tre_view_clik = "Inconsistencia general función Recursive_archivo_tre_view " & ex.Message
        End Try
    End Function
    Function Nod_CHILD_busqueda_archivo(ByRef tre_node As TreeNode _
                                        , ByVal edificio As String) As String
        Try
            Dim Result As String = ""
            Dim i As Integer = 0
            If tre_node.ChildNodes.Count = 0 Then
                Dim splinodo() As String = tre_node.Value.Split("|")
                Result = seleccion_treview_recursive(tre_node.Value, tre_node, splinodo(1))
                Nod_CHILD_busqueda_archivo(tre_node, edificio)
            Else
                For i = 0 To tre_node.ChildNodes.Count - 1
                    Dim splinodo() As String = tre_node.ChildNodes(i).Value.Split("|")
                    Result = seleccion_treview_recursive(tre_node.ChildNodes(i).Value, tre_node.ChildNodes(i), edificio)
                Next
            End If
            Nod_CHILD_busqueda_archivo = "YES"
        Catch ex As Exception
            Nod_CHILD_busqueda_archivo = ex.Message
        End Try
    End Function
    Function Nod_CHILD_busqueda_archivo_clik(ByRef tre_node As TreeNode _
                                       ) As String
        Try
            Dim Result As String = ""
            Dim i As Integer = 0
            If tre_node.ChildNodes.Count = 0 Then
                Dim splinodo() As String = tre_node.Value.Split("|")
                Result = seleccion_treview_recursive(tre_node.Value, tre_node, splinodo(1))
                Nod_CHILD_busqueda_archivo_clik(tre_node)
            Else
                For i = 0 To tre_node.ChildNodes.Count - 1
                    Dim splinodo() As String = tre_node.ChildNodes(i).Value.Split("|")
                    Result = seleccion_treview_recursive(tre_node.ChildNodes(i).Value, tre_node.ChildNodes(i), splinodo(1))
                    Exit For
                Next
            End If
            Nod_CHILD_busqueda_archivo_clik = "YES"
        Catch ex As Exception
            Nod_CHILD_busqueda_archivo_clik = ex.Message
        End Try
    End Function
    Function seleccion_treview_recursive(ByVal tag_seleccion As String, _
                                         ByRef node As TreeNode, _
                                         ByVal nombre_empresa As String) As String
        Try
            Dim splinodo() As String = tag_seleccion.Split("|")
            Dim Result As String = ""
            Dim refclasexpediente As New ClassGaExpediente
            Dim estru_expediente() As expediente_conservacion = Nothing
            '******************************
            'Lista PISO edificio archivo
            '******************************
            If splinodo(1) = "EDIFICIO" Then
                Result = Listar_Entidad_Piso_Archivo_edificio(node, _
                                                              nombre_empresa)
                If Result <> "YES" Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
            End If
            '**********************************
            'Lista AREA piso edificio archivo
            '**********************************
            If splinodo(1) = "PISO" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
                Result = Lista_Entidad_Area_Archivo(node, nombre_empresa)
                If Result <> "YES" Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
            End If

            '*****************************************
            'Lista MODULO area piso edificio archivo
            '*****************************************
            If splinodo(1) = "AREA" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Modulo_Archivo(node, nombre_empresa)
                If Result <> "YES" Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
            End If
            '**************************************************
            'Lista ESTANTE (modulo) area piso edificio archivo
            '**************************************************
            If splinodo(1) = "MODULO" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Estante_Archivo(node, nombre_empresa)
                If Result <> "YES" Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
            End If

            '**************************************************
            'Lista ENTREPAÑO (estante) modulo area piso edificio archivo
            '**************************************************
            If splinodo(1) = "ESTANTE" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Entrepaño_Archivo(node, nombre_empresa)
                If Result <> "YES" Then
                    seleccion_treview_recursive = Result
                    Exit Function
                End If
            End If
            seleccion_treview_recursive = "YES"
        Catch ex As Exception
            seleccion_treview_recursive = "Inconsistencia general función seleccion_treview_recursive " & ex.Message
        End Try
    End Function
    Function seleccion_treview_recursive_clik(ByVal tag_seleccion As String, ByRef node As TreeNode, _
    ByVal nombre_edificio As String, ByVal opcion_lista_entrepano As Integer, _
    ByRef ref_treview_unidad As TreeView, ByVal node_tag As String, _
    ByVal node_tex As String) As String
        Try
            Dim splinodo() As String = tag_seleccion.Split("|")
            Dim Result As String = ""
            Dim refclasexpediente As New ClassGaExpediente
            Dim estru_expediente() As expediente_conservacion = Nothing
            '******************************
            'Lista PISO edificio archivo
            '******************************
            If splinodo(1) = "EDIFICIO" Then
                Result = Listar_Entidad_Piso_Archivo_edificio(node, nombre_edificio)
                If Result <> "YES" Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
            End If
            '**********************************
            'Lista AREA piso edificio archivo
            '**********************************
            If splinodo(1) = "PISO" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
                Result = Lista_Entidad_Area_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
            End If

            '*****************************************
            'Lista MODULO area piso edificio archivo
            '*****************************************
            If splinodo(1) = "AREA" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Modulo_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
            End If
            '**************************************************
            'Lista ESTANTE (modulo) area piso edificio archivo
            '**************************************************
            If splinodo(1) = "MODULO" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Estante_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
            End If

            '************************************************************
            'Lista ENTREPAÑO (estante) modulo area piso edificio archivo
            '************************************************************
            If splinodo(1) = "ESTANTE" Then
                If node.ChildNodes.Count > 0 Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                End If
                Result = Listar_Entidad_Entrepaño_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    seleccion_treview_recursive_clik = Result
                    Exit Function
                Else
                    seleccion_treview_recursive_clik = "YES"
                    Exit Function
                End If
            End If
            '***********************************************************
            'Lista expedientes en entrepaños y unidades de conservación
            '***********************************************************
            If opcion_lista_entrepano = 1 Then
                If splinodo(1) = "ENTREPAÑO" Then
                    Dim refclasunidad As New ClassUnidadConservacion
                    Dim estru_unidad() As unidad_conservacion
                    Erase estru_unidad
                    Dim id_entrepaño As Integer = splinodo(0)
                    '------------------------------------------------------
                    'Lista expedientes anidados en unidades contenedora
                    '------------------------------------------------------
                    Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, 1, estru_unidad)
                    If Result <> "YES" Then
                        seleccion_treview_recursive_clik = Result
                        Exit Function
                    End If

                    Result = refclasunidad.Listar_Unidad_Conservacion_nono_nodo_entre_pano(estru_unidad, _
                    node_tag, node_tex, node)
                    If Result <> "YES" Then
                        seleccion_treview_recursive_clik = Result
                        Exit Function
                    End If
                    '------------------------------------------------------
                    'Agrega expedientes a unidades contenedora
                    '------------------------------------------------------
                    'If node.ChildNodes.Count > 0 Then
                    '    For i As Integer = 0 To node.ChildNodes.Count - 1
                    '        estru_expediente = Nothing
                    '        '------------------------------------------------------------------------
                    '        'Retorna datos expedientes anidados en unidad contendora de expedientes
                    '        '------------------------------------------------------------------------
                    '        Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(node.ChildNodes(i).Value, estru_expediente)
                    '        If Result <> "YES" Then
                    '            seleccion_treview_recursive_clik = Result
                    '            Exit Function
                    '        End If
                    '        If Not estru_expediente Is Nothing Then
                    '            Result = refclasexpediente.Listar_expediente_treview( _
                    '                            ref_treview_unidad, estru_expediente, node.ChildNodes(i), 1)
                    '            If Result <> "YES" Then
                    '                seleccion_treview_recursive_clik = Result
                    '                Exit Function
                    '            End If
                    '        End If
                    '    Next
                    'End If
                    '------------------------------------------------------
                    'Lista expedientes anidados en entre paño
                    '------------------------------------------------------
                    'estru_expediente = Nothing
                    'Result = refclasexpediente.Listar_datos_Expediente_Conservacion_estructura_entrepano( _
                    'splinodo(0), estru_expediente)
                    'If Result <> "YES" Then
                    '    seleccion_treview_recursive_clik = Result
                    '    Exit Function
                    'End If
                    'If Not estru_expediente Is Nothing Then
                    '    Result = refclasexpediente.Listar_expediente_treview_node( _
                    '                     estru_expediente, node, 1)
                    '    If Result <> "YES" Then
                    '        seleccion_treview_recursive_clik = Result
                    '        Exit Function
                    '    End If
                    'End If

                End If
            End If

            seleccion_treview_recursive_clik = "YES"
        Catch ex As Exception
            seleccion_treview_recursive_clik = "Inconsistencia general función seleccion_treview_recursive_clik " & ex.Message
        End Try
    End Function
    Function Seleccion_Treview(ByVal tag_seleccion As String, _
                               ByRef node As TreeNode, _
                               ByVal nombre_edificio As String, _
                               ByVal node_tag As String, ByVal node_tex As String, _
                               ByRef ref_treview_unidad As TreeView) As String
        Try
            Dim splinodo() As String = tag_seleccion.Split("|")
            Dim Result As String = ""
            Dim refclasexpediente As New ClassGaExpediente
            Dim estru_expediente() As expediente_conservacion = Nothing
            '******************************
            'Lista PISO edificio archivo
            '******************************
            If splinodo(1) = "EDIFICIO" Then
                ref_treview_unidad.Nodes.Clear()
                Result = Listar_Entidad_Piso_Archivo_edificio(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
            End If
            '**********************************
            'Lista AREA piso edificio archivo
            '**********************************
            If splinodo(1) = "PISO" Then
                ref_treview_unidad.Nodes.Clear()
                Result = Lista_Entidad_Area_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
            End If

            '*****************************************
            'Lista MODULO area piso edificio archivo
            '*****************************************
            If splinodo(1) = "AREA" Then
                ref_treview_unidad.Nodes.Clear()
                Result = Listar_Entidad_Modulo_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
            End If
            '**************************************************
            'Lista ESTANTE (modulo) area piso edificio archivo
            '**************************************************
            If splinodo(1) = "MODULO" Then
                ref_treview_unidad.Nodes.Clear()
                Result = Listar_Entidad_Estante_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
            End If

            '**************************************************
            'Lista ENTREPAÑO (estante) modulo area piso edificio archivo
            '**************************************************
            If splinodo(1) = "ESTANTE" Then
                ref_treview_unidad.Nodes.Clear()
                Result = Listar_Entidad_Entrepaño_Archivo(node, _
                                                          nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
            End If

            '***********************************************************
            'Lista expedientes en entrepaños y unidades de conservación
            '***********************************************************
            If splinodo(1) = "ENTREPAÑO" Then
                Dim refclasunidad As New ClassUnidadConservacion
                Dim estru_unidad() As unidad_conservacion
                Erase estru_unidad
                Dim id_entrepaño As Integer = splinodo(0)
                '------------------------------------------------------
                'Lista expedientes anidados en unidades contenedora
                '------------------------------------------------------
                Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, _
                                                                                                          1, _
                                                                                                          estru_unidad)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
                Dim trenode As New TreeNode
                Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano(ref_treview_unidad, _
                                                                                          estru_unidad, _
                                                                                          node_tag, _
                                                                                          node_tex, _
                                                                                          trenode)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
                '------------------------------------------------------
                'Agrega expedientes a unidades contenedora
                '------------------------------------------------------
                If trenode.ChildNodes.Count > 0 Then
                    For i As Integer = 0 To trenode.ChildNodes.Count - 1
                        estru_expediente = Nothing
                        '------------------------------------------------------------------------
                        'Retorna datos expedientes anidados en unidad contendora de expedientes
                        '------------------------------------------------------------------------
                        Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(trenode.ChildNodes(i).Value, _
                                                                                                          estru_expediente, _
                                                                                                          "CODIGO_UNICO")
                        If Result <> "YES" Then
                            Seleccion_Treview = Result
                            Exit Function
                        End If
                        If Not estru_expediente Is Nothing Then
                            Result = refclasexpediente.Listar_expediente_treview( _
                                                                                 ref_treview_unidad, _
                                                                                 estru_expediente, _
                                                                                 trenode.ChildNodes(i), 1)
                            If Result <> "YES" Then
                                Seleccion_Treview = Result
                                Exit Function
                            End If
                        End If
                    Next
                End If
                '------------------------------------------------------
                'Lista expedientes anidados en entre paño
                '------------------------------------------------------
                estru_expediente = Nothing
                Result = refclasexpediente.Listar_datos_Expediente_Conservacion_estructura_entrepano( _
                                                                                                     splinodo(0), _
                                                                                                     estru_expediente)
                If Result <> "YES" Then
                    Seleccion_Treview = Result
                    Exit Function
                End If
                If Not estru_expediente Is Nothing Then
                    Result = refclasexpediente.Listar_expediente_treview( _
                                                                         ref_treview_unidad, _
                                                                         estru_expediente, _
                                                                         trenode, 1)
                    If Result <> "YES" Then
                        Seleccion_Treview = Result
                        Exit Function
                    End If
                End If

            End If
            Seleccion_Treview = "YES"
        Catch ex As Exception
            Seleccion_Treview = "Inconsistencia función Seleccion_Treview " & ex.Message
        End Try
    End Function
    Function Seleccion_treview_principal_entrepaño(ByRef TreeViewunidad As TreeView, _
                                                   ByRef UpdatePanel_unidad_treview_unidad As UpdatePanel) As String
        Try
            Dim Tagform As String = TreeViewunidad.SelectedNode.Value
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            Dim restr_unidad_conservacion() As unidad_conservacion
            Erase restr_unidad_conservacion
            If TreeViewunidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                TreeViewunidad.SelectedNode.ChildNodes.Clear()
                Dim refclasexpediente As New ClassGaExpediente
                Dim estru_expediente() As expediente_conservacion = Nothing
                '------------------------------------------------------------------------
                'Retorna datos expedientes anidados en unidad contendora de expedientes
                '------------------------------------------------------------------------
                Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(TreeViewunidad.SelectedNode.Value, _
                                                                                                  estru_expediente, _
                                                                                                  "CODIGO_UNICO")
                If Result <> "YES" Then
                    Seleccion_treview_principal_entrepaño = Result
                    Exit Function
                End If
                If Not estru_expediente Is Nothing Then
                    Result = refclasexpediente.Listar_expediente_treview( _
                                                                         TreeViewunidad, _
                                                                         estru_expediente, _
                                                                         TreeViewunidad.SelectedNode, _
                                                                         1)
                    If Result <> "YES" Then
                        Seleccion_treview_principal_entrepaño = Result
                        Exit Function
                    End If
                End If

            End If
            Seleccion_treview_principal_entrepaño = "YES"
        Catch ex As Exception
            Seleccion_treview_principal_entrepaño = "Inconsistencia general función  Seleccion_treview_principal_entrepaño " & ex.Message
        End Try
    End Function
    Function Seleccion_Treview_reubicacion(ByVal tag_seleccion As String, ByRef node As TreeNode, _
    ByVal nombre_edificio As String, _
    ByVal node_tag As String, ByVal node_tex As String, ByRef ref_treview_unidad As TreeView, ByRef update As UpdatePanel, Optional ByVal tipo_ubicacion As String = "") As String
        Try
            Dim splinodo() As String = tag_seleccion.Split("|")
            Dim Result As String = ""
            '******************************
            'Lista PISO edificio archivo
            '******************************
            If splinodo(1) = "EDIFICIO" Then
                Result = Listar_Entidad_Piso_Archivo_edificio(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
            End If
            '**********************************
            'Lista AREA piso edificio archivo
            '**********************************
            If splinodo(1) = "PISO" Then
                Result = Lista_Entidad_Area_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
            End If

            '*****************************************
            'Lista MODULO area piso edificio archivo
            '*****************************************
            If splinodo(1) = "AREA" Then
                Result = Listar_Entidad_Modulo_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
            End If
            '**************************************************
            'Lista ESTANTE (modulo) area piso edificio archivo
            '**************************************************
            If splinodo(1) = "MODULO" Then
                Result = Listar_Entidad_Estante_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
            End If

            '**************************************************************
            'Lista ENTREPAÑO (estante) modulo area piso edificio archivo
            '**************************************************************
            If splinodo(1) = "ESTANTE" Then
                Result = Listar_Entidad_Entrepaño_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
            End If

            '*****************************************************************************
            'Lista UNIDA CONSERVACON entrepaño estante modulo area piso edifício archivo
            '*****************************************************************************
            If splinodo(1) = "ENTREPAÑO" Then
                Dim refclasunidad As New ClassUnidadConservacion
                Dim estru_unidad() As unidad_conservacion
                Erase estru_unidad
                Dim id_entrepaño As Integer = splinodo(0)
                Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, 1, estru_unidad)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
                Dim no_tag_ref As String = tag_seleccion
                Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(ref_treview_unidad, estru_unidad, _
                no_tag_ref, node_tex, node)
                If Result <> "YES" Then
                    Seleccion_Treview_reubicacion = Result
                    Exit Function
                End If
                'Dim refclasexpediente As New ClassGaExpediente
                'Dim estru_expediente() As expediente_conservacion = Nothing
                'Result = refclasexpediente.Listar_datos_Expediente_Conservacion_estructura_entrepano( _
                'splinodo(0), estru_expediente)
                'If Result <> "YES" Then
                '    Seleccion_Treview_reubicacion = Result
                '    Exit Function
                'End If
                'If Not estru_expediente Is Nothing Then
                '    Result = refclasexpediente.Listar_expediente_treview_ubicacion( _
                '                    ref_treview_unidad, estru_expediente, node, 1)
                '    If Result <> "YES" Then
                '        Seleccion_Treview_reubicacion = Result
                '        Exit Function
                '    End If
                'End If

            End If
            '------------------------------------------------------
            'Lista expedientes anidados en unidades contenedoras 
            'de expedientes
            '------------------------------------------------------
            If ref_treview_unidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                'ref_treview_unidad.SelectedNode.ChildNodes.Clear()
                'Dim refclasexpediente As New ClassGaExpediente
                'Dim estru_expediente() As expediente_conservacion = Nothing
                ''------------------------------------------------------------------------
                ''Retorna datos expedientes anidados en unidad contendora de expedientes
                ''------------------------------------------------------------------------
                'Dim split() As String = ref_treview_unidad.SelectedNode.Value.ToString.Split("|")
                'Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(split(0), estru_expediente)
                'If Result <> "YES" Then
                '    Seleccion_Treview_reubicacion = Result
                '    Exit Function
                'End If
                'If Not estru_expediente Is Nothing Then
                '    Result = refclasexpediente.Listar_expediente_treview_ubicacion( _
                '                    ref_treview_unidad, estru_expediente, ref_treview_unidad.SelectedNode, 1)
                '    If Result <> "YES" Then
                '        Seleccion_Treview_reubicacion = Result
                '        Exit Function
                '    End If
                'End If

            End If
            Seleccion_Treview_reubicacion = "YES"
        Catch ex As Exception
            Seleccion_Treview_reubicacion = "Inconsistencia función Seleccion_Treview " & ex.Message
        End Try
    End Function
    Function Seleccion_Treview_archivar(ByVal tag_seleccion As String, _
                                        ByRef node As TreeNode, _
                                        ByVal nombre_edificio As String, _
                                        ByVal node_tag As String, _
                                        ByVal node_tex As String, _
                                        ByRef ref_treview_unidad As TreeView, _
                                        Optional ByVal tipo_ubicacion As String = "") As String
        Try
            Dim splinodo() As String = tag_seleccion.Split("|")
            Dim Result As String = ""
            '******************************
            'Lista PISO edificio archivo
            '******************************
            If splinodo(1) = "EDIFICIO" Then
                Result = Listar_Entidad_Piso_Archivo_edificio(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
            End If
            '**********************************
            'Lista AREA piso edificio archivo
            '**********************************
            If splinodo(1) = "PISO" Then
                Result = Lista_Entidad_Area_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
            End If

            '*****************************************
            'Lista MODULO area piso edificio archivo
            '*****************************************
            If splinodo(1) = "AREA" Then
                Result = Listar_Entidad_Modulo_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
            End If
            '**************************************************
            'Lista ESTANTE (modulo) area piso edificio archivo
            '**************************************************
            If splinodo(1) = "MODULO" Then
                Result = Listar_Entidad_Estante_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
            End If

            '**************************************************************
            'Lista ENTREPAÑO (estante) modulo area piso edificio archivo
            '**************************************************************
            If splinodo(1) = "ESTANTE" Then
                Result = Listar_Entidad_Entrepaño_Archivo(node, nombre_edificio)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
            End If

            '*****************************************************************************
            'Lista UNIDA CONSERVACON entrepaño estante modulo area piso edifício archivo
            '*****************************************************************************
            If splinodo(1) = "ENTREPAÑO" Then
                Dim refclasunidad As New ClassUnidadConservacion
                Dim estru_unidad() As unidad_conservacion
                Erase estru_unidad
                Dim id_entrepaño As Integer = splinodo(0)
                Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, 1, estru_unidad)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
                Dim no_tag_ref As String = tag_seleccion

                Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(ref_treview_unidad, estru_unidad, _
                no_tag_ref, node_tex, node)
                If Result <> "YES" Then
                    Seleccion_Treview_archivar = Result
                    Exit Function
                End If
                'Dim refclasexpediente As New ClassGaExpediente
                'Dim estru_expediente() As expediente_conservacion = Nothing
                'Result = refclasexpediente.Listar_datos_Expediente_Conservacion_estructura_entrepano( _
                'splinodo(0), estru_expediente)
                'If Result <> "YES" Then
                '    Seleccion_Treview_reubicacion = Result
                '    Exit Function
                'End If
                'If Not estru_expediente Is Nothing Then
                '    Result = refclasexpediente.Listar_expediente_treview_ubicacion( _
                '                    ref_treview_unidad, estru_expediente, node, 1)
                '    If Result <> "YES" Then
                '        Seleccion_Treview_reubicacion = Result
                '        Exit Function
                '    End If
                'End If

            End If
            '------------------------------------------------------
            'Lista expedientes anidados en unidades contenedoras 
            'de expedientes
            '------------------------------------------------------
            If ref_treview_unidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                'ref_treview_unidad.SelectedNode.ChildNodes.Clear()
                'Dim refclasexpediente As New ClassGaExpediente
                'Dim estru_expediente() As expediente_conservacion = Nothing
                ''------------------------------------------------------------------------
                ''Retorna datos expedientes anidados en unidad contendora de expedientes
                ''------------------------------------------------------------------------
                'Dim split() As String = ref_treview_unidad.SelectedNode.Value.ToString.Split("|")
                'Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(split(0), estru_expediente)
                'If Result <> "YES" Then
                '    Seleccion_Treview_archivar = Result
                '    Exit Function
                'End If
                'If Not estru_expediente Is Nothing Then
                '    Result = refclasexpediente.Listar_expediente_treview_ubicacion( _
                '                    ref_treview_unidad, estru_expediente, ref_treview_unidad.SelectedNode, 1)
                '    If Result <> "YES" Then
                '        Seleccion_Treview_archivar = Result
                '        Exit Function
                '    End If
                'End If

            End If
            Seleccion_Treview_archivar = "YES"
        Catch ex As Exception
            Seleccion_Treview_archivar = "Inconsistencia función Seleccion_Treview_archivar " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Piso_Archivo_edificio(ByRef nod As TreeNode, _
                                                  ByVal nombre_empresa As String) As String
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Dim ob As Object = Nothing
            Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                                  id_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Piso_Archivo_edificio = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim Strupiso() As Piso_Archivo
            Erase Strupiso
            Dim spli_node() As String = nod.Value.ToString.Split("|")
            Result = Listar_Piso_Archivo(id_empresa, _
                                         ob, _
                                         Strupiso, _
                                         spli_node(0))
            If Result <> "YES" Then
                Listar_Entidad_Piso_Archivo_edificio = Result
                Exit Function
            End If
            If Not Strupiso Is Nothing Then
                Result = Listar_Entidad_Piso_Archivo_edificio_treeview(nod, _
                                                                   Strupiso)
                If Result <> "YES" Then
                    Listar_Entidad_Piso_Archivo_edificio = Result
                    Exit Function
                End If
            End If
            
            Listar_Entidad_Piso_Archivo_edificio = "YES"
        Catch ex As Exception
            Listar_Entidad_Piso_Archivo_edificio = "Inconsistencia funcion Listar_Entidad_Piso_Archivo_edificio " & ex.Message

        End Try
    End Function
   
    Function Listar_Piso_Archivo(ByVal id_empresa As Integer, _
                                 ByVal RefListview As ListView, _
                                 ByRef estru_piso() As Piso_Archivo, _
                                 ByVal id_edificio As Integer) As String
        '**********************************************************
        'Funcion : Lista los pisos registrados en la empresa
        'con le parametro id empresa
        'Ing  : Miguel Angel Urueta Miranda
        'Fecha : 2014-016-19
        '***********************************************************
        Try
            Dim SqlConsulta As String = "SELECT  pa.ID_PISO,pa.NOMBRE_PISO,ec.NOMBRE_EDIFICIO,pa.TELEFONO_PISO,pa.RESPONSABLE_PISO,pa.CODIGO_UNICO_PISO " & _
            " FROM piso_archivo as pa " & _
            "inner join edificio_contenedor as ec on  (ec.ID_EDIFICIO=pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO )" & _
            " where pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO=" & id_edificio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_Piso_Archivo = "Error solicitando id estante " & Result
                Exit Function
            End If
            Erase estru_piso
            estru_piso = Nothing
            If Datset.Tables(0).Rows.Count > 0 Then
                For incre As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_piso(incre)
                    estru_piso(incre).id_piso = Datset.Tables(0).Rows(incre).Item(0)
                    estru_piso(incre).nombre_piso = Datset.Tables(0).Rows(incre).Item(1)
                    estru_piso(incre).edificio_contenedor = Datset.Tables(0).Rows(incre).Item(2)
                    estru_piso(incre).telefono = Datset.Tables(0).Rows(incre).Item(3)
                    estru_piso(incre).responsable = Datset.Tables(0).Rows(incre).Item(4)
                    estru_piso(incre).codigo_piso = Datset.Tables(0).Rows(incre).Item(5)
                Next
                Listar_Piso_Archivo = "YES"
            Else
                Listar_Piso_Archivo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Piso_Archivo = "Inconsistencia general  funcion Listar_Piso_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Piso_Archivo_edificio_treeview(ByRef nod As TreeNode, ByRef estru_piso() As Piso_Archivo) As String
        Try
            nod.ChildNodes.Clear()
            If estru_piso Is Nothing Then
                Listar_Entidad_Piso_Archivo_edificio_treeview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_piso.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Piso: " & estru_piso(i).nombre_piso
                attrNode.Value = estru_piso(i).id_piso & "|PISO|" & estru_piso(i).nombre_piso
                attrNode.ToolTip = estru_piso(i).responsable
                attrNode.ImageUrl = "../workflow/imageneswf/layer-group-light.png"
                nod.ChildNodes.Add(attrNode)
            Next
            Listar_Entidad_Piso_Archivo_edificio_treeview = "YES"
        Catch ex As Exception
            Listar_Entidad_Piso_Archivo_edificio_treeview = "Inconsistencia funcion Listar_Entidad_Piso_Archivo_edificio_treeview " & ex.Message
        End Try
    End Function
    Function Lista_Entidad_Area_Archivo(ByRef node As TreeNode, ByVal Nombre_Empresa As String) As String
        '*********************************************************
        'Funcion lista las reas en el treview
        'Fecha 2014-09-23
        'Ing Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = Reflasgest.Retorna_Id_Emprea(Nombre_Empresa, id_empresa)
            If Result <> "YES" Then
                Lista_Entidad_Area_Archivo = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim Struarea() As Area_Archivo
            Erase Struarea
            Dim nodespli() As String = node.Value.ToString.Split("|")
            Result = Listar_Area_Archivo(id_empresa, Struarea, nodespli(0))
            If Result <> "YES" Then
                Lista_Entidad_Area_Archivo = "Imposible listar areas " & Result
                Exit Function
            End If
            Result = Listar_Area_Archivo_Treview(node, Struarea)
            If Result <> "YES" Then
                Lista_Entidad_Area_Archivo = "Imposible listar areas en treview" & Result
                Exit Function
            End If
            Lista_Entidad_Area_Archivo = "YES"
        Catch ex As Exception
            Lista_Entidad_Area_Archivo = "Inconsistencia funcion Lista_Entidad_Area_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Area_Archivo(ByVal id_empresa As Integer, ByRef stru_area() As Area_Archivo, _
   ByVal id_piso As Integer)
        '*************************************************************************
        'Funcion : Lista area archivo en estrucutura con el parametro id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-07-01
        '*************************************************************************
        Try

            Dim sqlconsulta As String = "SELECT AA.ID_AREA,AA.NOMBRE_AREA,PA.NOMBRE_PISO,AA.CODIGO_UNICO_AREA,AA.TELEFONO_RESPONSABLE," & _
            "AA.NOMBRE_RESPONSABLE,AA.TIPO_ARCHIVO " & _
            "FROM empresa_gestion_documental as egd " & _
            "INNER JOIN  edificio_contenedor as ec on (egd.ID_EMPRESA=ec.ID_EMPRESA) " & _
            "INNER JOIN  piso_archivo as pa on (ec.ID_EDIFICIO=pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO and ID_PISO=" & id_piso & ") " & _
            "INNER JOIN area_archivo as aa on (pa.ID_PISO=aa.PISO_ARCHIVO_ID_PISO) " & _
            " where egd.ID_EMPRESA=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_Area_Archivo = "Error listando area archivo esposible que falte el campo TIPO_ARCHIVO función Listar_Area_Archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For incre As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_area(incre)
                    stru_area(incre).id_area = Datset.Tables(0).Rows(incre).Item(0)
                    stru_area(incre).nombre_area = Datset.Tables(0).Rows(incre).Item(1)
                    stru_area(incre).piso_contenedor = Datset.Tables(0).Rows(incre).Item(2)
                    stru_area(incre).codigo_area = Datset.Tables(0).Rows(incre).Item(3)
                    stru_area(incre).telefono = Datset.Tables(0).Rows(incre).Item(4)
                    stru_area(incre).responsable = Datset.Tables(0).Rows(incre).Item(5)
                    If Datset.Tables(0).Rows(incre).IsNull(6) = True Then
                        stru_area(incre).tipo_archivo = ""
                    Else
                        stru_area(incre).tipo_archivo = Datset.Tables(0).Rows(incre).Item(6)
                    End If
                Next
                Listar_Area_Archivo = "YES"
            Else
                Listar_Area_Archivo = "YES"
            End If

        Catch ex As Exception
            Listar_Area_Archivo = "Inconsistencia general funcion Listar_Area_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Area_Archivo_Treview(ByRef nod As TreeNode, ByRef stru_area() As Area_Archivo) As String
        '***************************************************
        'Funcion listar en el treview los datos de la 
        'estructura
        'Fecha 2014-0923
        'Ing :Miguel Angel Urueta Miranda
        '***************************************************
        Try
            nod.ChildNodes.Clear()
            If stru_area Is Nothing Then
                Listar_Area_Archivo_Treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_area.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Area: " & stru_area(i).nombre_area & " ( ARCHIVO " & stru_area(i).tipo_archivo & ")"
                attrNode.Value = stru_area(i).id_area & "|AREA|" & stru_area(i).nombre_area
                attrNode.ImageUrl = "../workflow/imageneswf/area-light.png"
                attrNode.ToolTip = stru_area(i).responsable
                nod.ChildNodes.Add(attrNode)
            Next
            Listar_Area_Archivo_Treview = "YES"
        Catch ex As Exception
            Listar_Area_Archivo_Treview = "Inconsistencia funcion Listar_Area_Archivo_Treview " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Modulo_Archivo(ByRef nod As TreeNode, ByVal nombre_empresa As String) As String
        '******************************************
        'Funcion Lista los modulos del archivo
        'Fecha 2014-09-24
        'Ing Miguel Angel Urueta Miranda
        '******************************************
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, id_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Modulo_Archivo = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim strumodulo() As Modulo_Archivo
            Erase strumodulo
            Dim splinodo() As String = nod.Value.ToString.Split("|")
            Result = Listar_Modulo_Archivo(id_empresa, strumodulo, splinodo(0))
            If Result <> "YES" Then
                Listar_Entidad_Modulo_Archivo = "Imposible listar modulos " & Result
                Exit Function
            End If
            Result = Listar_Modulo_Archivo_treview(nod, strumodulo)
            If Result <> "YES" Then
                Listar_Entidad_Modulo_Archivo = "Imposible listar areas en listview" & Result
                Exit Function
            End If
            Listar_Entidad_Modulo_Archivo = "YES"
        Catch ex As Exception
            Listar_Entidad_Modulo_Archivo = "Inconsistencia funcion Listar_Entidad_Modulo_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Modulo_Archivo(ByVal id_empresa As Integer, ByRef stru_modulo() As Modulo_Archivo, _
    ByVal id_area_archivo As Integer)
        '*************************************************************************
        'Funcion : Lista area archivo en estrucutura con el parametro id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-07-01 Session.Item("GA_LOGINUSUARIOGESTION")
        '*************************************************************************
        Try
            Dim Result As String = ""
            Dim confirma_relacion As String = "NO"
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Listar_Modulo_Archivo = "El usuario workflow no tiene usuario de gestión relacionado "
                Exit Function
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
            Else
                Result = Retorna_Estado_Relacion_usuario_gestion_area_archivo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                id_area_archivo, confirma_relacion)
                If Result <> "YES" Then
                    Listar_Modulo_Archivo = Result
                    Exit Function
                End If
                If confirma_relacion = "NO" Then
                    Listar_Modulo_Archivo = "El usuario " & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para gestionar el area y sus módulos"
                    Exit Function
                End If
            End If
            Dim sqlconsulta As String = "SELECT MA.ID_MODULO,MA.NOMBRE_MODULO,AA.NOMBRE_AREA,MA.CODIGO_MODULO,MA.SECCION_MODULO," & _
            "MA.DESCRIPCION_MODULO,PA.NOMBRE_PISO,EC.NOMBRE_EDIFICIO " & _
            "FROM empresa_gestion_documental as egd " & _
            "INNER JOIN  edificio_contenedor as ec on (egd.ID_EMPRESA=ec.ID_EMPRESA) " & _
            "INNER JOIN  piso_archivo as pa on (ec.ID_EDIFICIO=pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO) " & _
            "INNER JOIN area_archivo as aa on (pa.ID_PISO=aa.PISO_ARCHIVO_ID_PISO and ID_AREA=" & id_area_archivo & ") " & _
            "INNER JOIN modulo_archivo as ma on (ma.AREA_ARCHIVO_ID_AREA=aa.ID_AREA) " & _
            " where egd.ID_EMPRESA=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("area_archivo_has_remit_dest_interno")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_Modulo_Archivo = " Error listando MODULO archivo " & sqlconsulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For incre As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_modulo(incre)
                    stru_modulo(incre).id_modulo = Datset.Tables(0).Rows(incre).Item(0)
                    stru_modulo(incre).nombre_modulo = Datset.Tables(0).Rows(incre).Item(1)
                    stru_modulo(incre).area_contenedora = Datset.Tables(0).Rows(incre).Item(2)
                    stru_modulo(incre).codigo_modulo = Datset.Tables(0).Rows(incre).Item(3)
                    stru_modulo(incre).seccion_modulo = Datset.Tables(0).Rows(incre).Item(4)
                    stru_modulo(incre).descripcion_modulo = Datset.Tables(0).Rows(incre).Item(5)
                    stru_modulo(incre).piso_contenedor = Datset.Tables(0).Rows(incre).Item(6)
                    stru_modulo(incre).edificio_contenedor = Datset.Tables(0).Rows(incre).Item(7)
                Next
                Listar_Modulo_Archivo = "YES"
                Exit Function
            Else
                Listar_Modulo_Archivo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Modulo_Archivo = "Inconsistencia general funcion Listar_Modulo_Archivo " & ex.Message
        End Try
    End Function
    Function Retorna_Estado_Relacion_usuario_gestion_area_archivo(ByVal id_usuario_gestion As Integer, _
    ByVal id_area_archivo As Integer, ByRef estado_relacion As String) As String
        '**********************************************
        'Funcion : Retorna permiso area de archivo
        'físico usuario de gestión
        'Fecha 03 de marzo de 2015
        'Ingeniero : Miguel Angel Urueta Miranda
        '**********************************************
        Try
            estado_relacion = "NO"
            Dim sqlconsulta As String = "Select ID_RELACION from area_archivo_has_remit_dest_interno " & _
            " where AREA_ARCHIVO_ID_AREA=" & id_area_archivo & " and remit_dest_interno_id_Remit_Dest_Int=" & _
            id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("area_archivo_has_remit_dest_interno")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_Estado_Relacion_usuario_gestion_area_archivo = "Error de conexión listado relación usuario de gestion archivo función  Retorna_Estado_Relacion_usuario_gestion_area_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_relacion = "YES"
                Retorna_Estado_Relacion_usuario_gestion_area_archivo = "YES"
            Else
                estado_relacion = "NO"
                Retorna_Estado_Relacion_usuario_gestion_area_archivo = "YES"
            End If
        Catch ex As Exception
            Retorna_Estado_Relacion_usuario_gestion_area_archivo = "Inconsistencia general función Retorna_Estado_Relacion_usuario_gestion_area_archivo " & ex.Message
        End Try
    End Function
    Function Listar_Modulo_Archivo_treview(ByRef nod As TreeNode, ByRef stru_modulo() As Modulo_Archivo) As String
        '*********************************************************
        'Funcion : lista modulos en el treview
        'Fecha 2014-09-23
        'Ing Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            nod.ChildNodes.Clear()
            If stru_modulo Is Nothing Then
                Listar_Modulo_Archivo_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_modulo.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Modulo: " & stru_modulo(i).nombre_modulo
                attrNode.Value = stru_modulo(i).id_modulo & "|MODULO|" & stru_modulo(i).nombre_modulo
                attrNode.ImageUrl = "../workflow/imageneswf/columns-light.png"
                attrNode.ToolTip = stru_modulo(i).seccion_modulo
                nod.ChildNodes.Add(attrNode)
            Next
            Listar_Modulo_Archivo_treview = "YES"
        Catch ex As Exception
            Listar_Modulo_Archivo_treview = "Inconsistencia funcion Listar_Modulo_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Estante_Archivo(ByRef nod As TreeNode, ByVal nombre_empresa As String) As String
        '************************************
        'Funcion lista estante archivo 
        'Fecha 2014-09-24
        'Ing : Miguel Angel Urueta Miranda
        '************************************
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, id_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Estante_Archivo = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim struestante() As Estante_Archivo
            Erase struestante
            Dim splinodo() As String = nod.Value.ToString.Split("|")
            Result = Listar_Estante_Archivo(id_empresa, struestante, splinodo(0))
            If Result <> "YES" Then
                Listar_Entidad_Estante_Archivo = "Imposible listar estantes " & Result
                Exit Function
            End If
            Result = Listar_Estante_Archivo_treview(nod, struestante)
            If Result <> "YES" Then
                Listar_Entidad_Estante_Archivo = "Imposible listar estantes en interface " & Result
                Exit Function
            End If
            Listar_Entidad_Estante_Archivo = "YES"
        Catch ex As Exception
            Listar_Entidad_Estante_Archivo = "Inconsistencia funcion Listar_Entidad_Estante_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Estante_Archivo(ByVal id_empresa As Integer, ByRef stru_estante() As Estante_Archivo, _
    ByVal id_modulo_archivo As Integer)
        '*************************************************************************
        'Funcion : Lista area archivo en estrucutura con el parametro id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-07-01
        '*************************************************************************
        Try
            Dim sqlconsulta As String = "SELECT EA.ID_ESTANTE,EA.CODIGO_LITERAL,MA.NOMBRE_MODULO,EA.CODIGO_UNICO,EA.DESCRIPCION_ASTANTE" & _
            ",AA.NOMBRE_AREA,PA.NOMBRE_PISO,EC.NOMBRE_EDIFICIO " & _
            "FROM empresa_gestion_documental as egd " & _
            "INNER JOIN  edificio_contenedor as ec on (egd.ID_EMPRESA=ec.ID_EMPRESA) " & _
            "INNER JOIN  piso_archivo as pa on (ec.ID_EDIFICIO=pa.EDIFICIO_CONTENEDOR_ID_EDIFICIO) " & _
            "INNER JOIN area_archivo as aa on (pa.ID_PISO=aa.PISO_ARCHIVO_ID_PISO) " & _
            "INNER JOIN modulo_archivo as ma on (ma.AREA_ARCHIVO_ID_AREA=aa.ID_AREA and ID_MODULO=" & id_modulo_archivo & ") " & _
            "INNER JOIN estante_archivo as ea on (ma.ID_MODULO=ea.MODULO_ARCHIVO_ID_MODULO) " & _
            " where egd.ID_EMPRESA=" & id_empresa & " ORDER BY  MA.NOMBRE_MODULO,EA.ID_ESTANTE"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("empresa_gestion_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_Estante_Archivo = " Error listando estante archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For incre As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_estante(incre)
                    stru_estante(incre).id_estante = Datset.Tables(0).Rows(incre).Item(0)
                    stru_estante(incre).codigo_estante = Datset.Tables(0).Rows(incre).Item(1)
                    stru_estante(incre).modulo_contenedor = Datset.Tables(0).Rows(incre).Item(2)
                    stru_estante(incre).area_contenedor = Datset.Tables(0).Rows(incre).Item(5)
                    stru_estante(incre).piso_contenedor = Datset.Tables(0).Rows(incre).Item(6)
                    stru_estante(incre).edificio_contenedor = Datset.Tables(0).Rows(incre).Item(7)
                    stru_estante(incre).codigo_unico = Datset.Tables(0).Rows(incre).Item(3)
                    stru_estante(incre).descripcion_estante = Datset.Tables(0).Rows(incre).Item(4)
                Next
                Listar_Estante_Archivo = "YES"
                Exit Function
            Else
                Listar_Estante_Archivo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Estante_Archivo = "Inconsistencia general funcion Listar_Estante_Archivo " & ex.Message
        End Try
    End Function
    Function Listar_Estante_Archivo_treview(ByRef nod As TreeNode, ByRef stru_estante() As Estante_Archivo) As String
        '*****************************************
        'Funcion lista estante archivo en treview 
        'Fecha 2014-09-24
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************
        Try
            nod.ChildNodes.Clear()
            If stru_estante Is Nothing Then
                Listar_Estante_Archivo_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_estante.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Estante : " & stru_estante(i).codigo_estante
                attrNode.Value = stru_estante(i).id_estante & "|ESTANTE|" & stru_estante(i).codigo_estante
                attrNode.ImageUrl = "../workflow/imageneswf/window-maximize-light.png"
                attrNode.ToolTip = stru_estante(i).codigo_unico
                nod.ChildNodes.Add(attrNode)

            Next
            Listar_Estante_Archivo_treview = "YES"
        Catch ex As Exception
            Listar_Estante_Archivo_treview = "Inconsistencia funcion Listar_Estante_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Entrepaño_Archivo(ByRef nod As TreeNode, _
                                              ByVal nombre_empresa As String) As String
        '*******************************************************
        'Funcion : Lista los entrepaños del modulo seleccionado
        'Fecha : 2014-09-25
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************************
        Try
            Dim Result As String = ""
            Dim Reflasgest As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, id_empresa)
            If Result <> "YES" Then
                Listar_Entidad_Entrepaño_Archivo = "Imposible listar la identidad de la empresa " & Result
                Exit Function
            End If
            Dim struentrepaño() As Entrapño_archivo
            Erase struentrepaño
            Dim splinodo() As String = nod.Value.ToString.Split("|")
            '---ojo FormGAunidadConservacion.TextBoxdatosunidad.Tag = splinodo(0)
            Result = Listar_Entrepaño_Archivo(id_empresa, _
                                              struentrepaño, _
                                              splinodo(0))
            If Result <> "YES" Then
                Listar_Entidad_Entrepaño_Archivo = "Imposible listar entrepaños " & Result
                Exit Function
            End If
            Result = Listar_Entrepaño_Archivo_treview(nod, _
                                                      struentrepaño)
            If Result <> "YES" Then
                Listar_Entidad_Entrepaño_Archivo = "Imposible listar entrepaños en interface" & Result
                Exit Function
            End If
            Listar_Entidad_Entrepaño_Archivo = "YES"
        Catch ex As Exception
            Listar_Entidad_Entrepaño_Archivo = "Iconsistencia funcion Listar_Entidad_Entrpaño_Archivo " & ex.Message
        End Try
    End Function
    Function Retorna_id_Entrepaño_id_expediente(ByVal id_expediente As Integer, _
  ByRef id_entre_paño As Integer) As String
        '*************************************************************
        'Funcion : Retorna id del entrepaño relacionado al expediente
        'Fecha : 2015-03-18 Modificado para web 2016-09-15
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select ENTRE_PAÑO_ID_ENTREPAÑO from expediente_archivo " & _
                                              " where ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_Entrepaño_id_expediente = " Error solicitando id entrepaño funcion Retorna_id_Entrepaño_id_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_entre_paño = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_Entrepaño_id_expediente = "YES"
                Exit Function
            Else
                Retorna_id_Entrepaño_id_expediente = "Imposible encontrar id del entrepaño por expediente"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_Entrepaño_id_expediente = "Inconsistencia función Retorna_id_Entrepaño_id_expediente " & ex.Message
        End Try

    End Function
    Function Retorna_id_modulo_estante_archivo(ByVal id_estante As Integer, ByRef id_modulo_archivo As Integer) As String
        '*************************************************************
        'Funcion : Retorna id del modulo con el parametro del estante
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select MODULO_ARCHIVO_ID_MODULO from estante_archivo " & _
                                              " where ID_ESTANTE=" & id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_modulo_estante_archivo = " Error solicitando id entrepaño funcion Retorna_id_modulo_estante_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_modulo_archivo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_modulo_estante_archivo = "YES"
                Exit Function
            Else
                Retorna_id_modulo_estante_archivo = "Imposible encontrar id modulo del estante " & id_estante
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_modulo_estante_archivo = "Inconsistencia general función Retorna_id_modulo_estante_archivo  " & ex.Message
        End Try
    End Function
    Function Retorna_id_area_archivo_por_id_modulo(ByVal id_modulo As Integer, ByRef id_area As Integer) As String
        '*************************************************************
        'Funcion : Retorna id del area con el parametro del id modulo
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select AREA_ARCHIVO_ID_AREA from modulo_archivo " & _
                                              " where ID_MODULO=" & id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("modulo_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_area_archivo_por_id_modulo = " Error solicitando id entrepaño funcion Retorna_id_area_archivo_por_id_modulo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_area_archivo_por_id_modulo = "YES"
                Exit Function
            Else
                Retorna_id_area_archivo_por_id_modulo = "Imposible encontrar id área del modulo " & id_modulo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_area_archivo_por_id_modulo = "Inconsistencia general función Retorna_id_area_archivo_por_id_modulo  " & ex.Message
        End Try
    End Function
    Function Retorna_id_piso_archivo_por_id_area(ByVal id_area As Integer, _
                                                 ByRef id_piso As Integer) As String
        '*************************************************************
        'Funcion : Retorna id piso por el d del area
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select PISO_ARCHIVO_ID_PISO from area_archivo " & _
                                              " where ID_AREA=" & id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("area_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_piso_archivo_por_id_area = " Error solicitando id piso funcion Retorna_id_piso_archivo_por_id_area " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_piso = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_piso_archivo_por_id_area = "YES"
                Exit Function
            Else
                Retorna_id_piso_archivo_por_id_area = "Imposible encontrar id piso del área " & id_area
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_piso_archivo_por_id_area = "Inconsistencia general función Retorna_id_piso_archivo_por_id_area  " & ex.Message
        End Try
    End Function

    Function Retorna_id_edifio_archivo_por_id_piso(ByVal id_piso As Integer, ByRef id_edificio As Integer) As String
        '*************************************************************
        'Funcion : Retorna id edificio por id piso
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select EDIFICIO_CONTENEDOR_ID_EDIFICIO from piso_archivo " & _
                                              " where ID_PISO=" & id_piso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("piso_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_edifio_archivo_por_id_piso = " Error solicitando id edificio funcion Retorna_id_edifio_archivo_por_id_piso " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_edificio = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_edifio_archivo_por_id_piso = "YES"
                Exit Function
            Else
                Retorna_id_edifio_archivo_por_id_piso = "Imposible encontrar id edificio del piso " & id_piso
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_edifio_archivo_por_id_piso = "Inconsistencia general función Retorna_id_edifio_archivo_por_id_piso  " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_edificio_por_id(ByVal id_edificio As Integer, ByRef nombre_edificio As String) As String
        '*************************************************************
        'Funcion : Retorna nombre edificio
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select NOMBRE_EDIFICIO from edificio_contenedor " & _
                                              " where ID_EDIFICIO=" & id_edificio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("area_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_edificio_por_id = " Error solicitando nombre de edificio función Retorna_nombre_edificio_por_id " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_edificio = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_edificio_por_id = "YES"
                Exit Function
            Else
                Retorna_nombre_edificio_por_id = "Imposible encontrar el nombre del edificio por el id " & id_edificio
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_edificio_por_id = "Inconsistencia general función Retorna_nombre_edificio_por_id  " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_area_por_id_area(ByVal id_area As Integer, ByRef nombre_area As String, ByRef tipo_archivo As String) As String
        '*************************************************************
        'Función : Retorna nombre area por id del area
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select NOMBRE_AREA,TIPO_ARCHIVO from area_archivo " & _
                                              " where ID_AREA=" & id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("area_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_area_por_id_area = " Error solicitando nombre del area función Retorna_nombre_area_por_id_area " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_area = Datset.Tables(0).Rows(0).Item(0)
                tipo_archivo = Datset.Tables(0).Rows(0).Item(1)
                Retorna_nombre_area_por_id_area = "YES"
                Exit Function
            Else
                Retorna_nombre_area_por_id_area = "Imposible encontrar el nombre del área por el id " & id_area
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_area_por_id_area = "Inconsistencia general función Retorna_nombre_area_por_id_area " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_modulo_por_id_modulo(ByVal id_modulo As Integer, ByRef nombre_modulo As String) As String
        '*************************************************************
        'Función : Retorna nombre modulo por id modulo
        'Fecha : 2017-01-21
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select NOMBRE_MODULO from modulo_archivo " & _
                                              " where ID_MODULO=" & id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("modulo_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_modulo_por_id_modulo = " Error solicitando nombre modulo función Retorna_nombre_modulo_por_id_modulo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_modulo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_modulo_por_id_modulo = "YES"
                Exit Function
            Else
                Retorna_nombre_modulo_por_id_modulo = "Imposible encontrar el nombre del modulo por el id " & id_modulo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_modulo_por_id_modulo = "Inconsistencia general función Retorna_nombre_modulo_por_id_modulo " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_estante_por_id_estante(ByVal id_estante As Integer, ByRef nombre_estante As String) As String
        '*************************************************************
        'Función : Retorna nombre estante por id estante
        'Fecha : 2017-01-21
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select CODIGO_LITERAL from estante_archivo " & _
                                              " where ID_ESTANTE=" & id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("estante_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_estante_por_id_estante = " Error solicitando nombre estante función Retorna_nombre_estante_por_id_estante " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_estante = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_estante_por_id_estante = "YES"
                Exit Function
            Else
                Retorna_nombre_estante_por_id_estante = "Imposible encontrar el nombre del estante por el id " & id_estante
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_estante_por_id_estante = "Inconsistencia general función Retorna_nombre_estante_por_id_estante " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_entrepaño_por_id_entrepaño(ByVal id_entrepaño As Integer, ByRef nombre_entrepaño As String) As String
        '*************************************************************
        'Función : Retorna nombre entrepaño por id entrepaño
        'Fecha : 2017-01-21
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select CODIGO_CORTO from entre_paño " & _
                                              " where ID_ENTREPAÑO=" & id_entrepaño
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_entrepaño_por_id_entrepaño = " Error solicitando nombre entrepaño función Retorna_nombre_entrepaño_por_id_entrepaño " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_entrepaño = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_entrepaño_por_id_entrepaño = "YES"
                Exit Function
            Else
                Retorna_nombre_entrepaño_por_id_entrepaño = "Imposible encontrar el nombre del entrepaño por el id " & id_entrepaño
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_entrepaño_por_id_entrepaño = "Inconsistencia general función Retorna_nombre_entrepaño_por_id_entrepaño " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_piso_por_id_piso(ByVal id_piso As Integer, ByRef nombre_piso As String) As String
        '*************************************************************
        'Función : Retorna nombre piso por id del piso
        'Fecha : 2017-01-21
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************************
        Try
            Dim SqlConsulta As String = "select NOMBRE_PISO from piso_archivo " & _
                                              " where ID_PISO=" & id_piso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("piso_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_piso_por_id_piso = " Error solicitando nombre del piso función Retorna_nombre_piso_por_id_piso " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_piso = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_piso_por_id_piso = "YES"
                Exit Function
            Else
                Retorna_nombre_piso_por_id_piso = "Imposible encontrar el nombre del piso por el id " & id_piso
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_piso_por_id_piso = "Inconsistencia general función Retorna_nombre_piso_por_id_piso " & ex.Message
        End Try
    End Function
    Function Listar_Entrepaño_Archivo_treview(ByRef nod As TreeNode, _
                                              ByRef stru_entrepaño() As Entrapño_archivo) As String
        '********************************************************
        'Funcion : Lista los entrepaños del modulo seleccionado
        'en el treview interface
        'Fecha : 2014-09-25
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************************
        Try
            nod.ChildNodes.Clear()
            If stru_entrepaño Is Nothing Then
                Listar_Entrepaño_Archivo_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_entrepaño.Length - 1
                Dim attrNode As New TreeNode
                attrNode.Text = "Entrepaño : " & stru_entrepaño(i).codigo_corto
                attrNode.Value = stru_entrepaño(i).id_entreapaño & "|ENTREPAÑO|" & stru_entrepaño(i).codigo_corto
                attrNode.ImageUrl = "../workflow/imageneswf/rectangle-wide-light-pano.png"
                attrNode.ToolTip = stru_entrepaño(i).codigo_unico
                nod.ChildNodes.Add(attrNode)
            Next
            Listar_Entrepaño_Archivo_treview = "YES"
        Catch ex As Exception
            Listar_Entrepaño_Archivo_treview = "Inconsistencia funcion Listar_Entrepaño_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Seleccion_gestion_archivo_menu(ByVal value_selecion As String,
                                            ByVal tipo_event As String,
                                            ByRef page As Page) As String
        Try
            Dim Result As String = ""
            Dim ModalPopupExtender_edition_reg_edit_edificio_archivo As AjaxControlToolkit.ModalPopupExtender _
                = page.FindControl("ModalPopupExtender_edition_reg_edit_edificio_archivo")
            Dim Label_reg_edit_title As Label = page.FindControl("Label_reg_edit_title")
            Dim UpdatePanel_reg_edit_edificio_archivo As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_edificio_archivo")
            Dim DropDownList_reg_edit_pais As DropDownList = page.FindControl("DropDownList_reg_edit_pais")
            Dim DropDownList_reg_edit_departamento As DropDownList = page.FindControl("DropDownList_reg_edit_departamento")
            Dim DropDownList_reg_edit_munici_depart As DropDownList = page.FindControl("DropDownList_reg_edit_munici_depart")
            Dim TreeViewArchivo As TreeView = page.FindControl("TreeViewArchivo")
            Dim UpdatePanelViewArchivo As UpdatePanel = page.FindControl("UpdatePanelViewArchivo")
            Dim id_pais As Integer = 0
            Dim id_depar As Integer = 0
            Dim ciudad_municipio As Integer = 0
            Dim Class_pais_radicacion As New Class_pais_radicacion
            If value_selecion = "AGRE-ED" Then
                Label_reg_edit_title.Text = "Agregar nuevo edificio"
                Result = Class_pais_radicacion.Lista_Paises(id_pais,
                                                            DropDownList_reg_edit_pais,
                                                            UpdatePanel_reg_edit_edificio_archivo)
                If Result <> "YES" Then
                    Seleccion_gestion_archivo_menu = Result
                    Exit Function
                End If
                DropDownList_reg_edit_departamento.Items.Clear()
                DropDownList_reg_edit_munici_depart.Items.Clear()
                UpdatePanel_reg_edit_edificio_archivo.Update()
                ModalPopupExtender_edition_reg_edit_edificio_archivo.Show()
                Seleccion_gestion_archivo_menu = "YES"
                Exit Function
            End If
            If value_selecion = "EDIT-EL" Then
                If TreeViewArchivo.SelectedNode Is Nothing Then
                    Seleccion_gestion_archivo_menu = "Por favor seleccione el elemento de la estructura"
                    Exit Function
                End If
                Dim splinodo() As String = TreeViewArchivo.SelectedNode.Value.Split("|")
                If splinodo(1) = "EDIFICIO" Then
                    Result = Me.asigna_datos_interface_edificio(Val(splinodo(0)),
                                                                page)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    UpdatePanel_reg_edit_edificio_archivo.Update()
                    ModalPopupExtender_edition_reg_edit_edificio_archivo.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "AREA" Then
                    Result = Me.Asigna_datos_interface_area_archivo(Val(splinodo(0)),
                                                                    page)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "MODULO" Then
                    Result = Me.Asigna_datos_interface_modulo_archivo(Val(splinodo(0)),
                                                                      page)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
            End If
            Dim ModalPopupExtender_edition_reg_edit_area_piso As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_reg_edit_area_piso")
            Dim ModalPopupExtender_edition_reg_edit_piso_archivo As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_reg_edit_piso_archivo")
            Dim UpdatePanel_reg_edit_piso_archivo As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_piso_archivo")
            Dim Label_reg_edit_piso_title As Label = page.FindControl("Label_reg_edit_piso_title")
            Dim ModalPopupExtender_edition_reg_edit_modulo_area As AjaxControlToolkit.ModalPopupExtender =
            page.FindControl("ModalPopupExtender_edition_reg_edit_modulo_area")
            Dim ModalPopupExtender_edition_reg_edit_estante As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_reg_edit_estante")
            Dim ModalPopupExtender_edition_reg_edit_entrepano As AjaxControlToolkit.ModalPopupExtender =
            page.FindControl("ModalPopupExtender_edition_reg_edit_entrepano")
            Dim Label_reg_edit_entrepano_numero As Label = page.FindControl("Label_reg_edit_entrepano_numero")
            Dim DropDownList_reg_edit_entrepano_numero As DropDownList = page.FindControl("DropDownList_reg_edit_entrepano_numero")
            Dim UpdatePanel_reg_edit_entrepano As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_entrepano")
            Dim Label_titulo_area_piso As Label = page.FindControl("Label_titulo_area_piso")
            Dim UpdatePanel_reg_edit_area_piso As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_area_piso")
            Dim Label_title_reg_edit_modulo As Label = page.FindControl("Label_title_reg_edit_modulo")
            Dim UpdatePanel_reg_edit_modulo_area As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_modulo_area")
            Dim Label_title_reg_edit_estante As Label = page.FindControl("Label_title_reg_edit_estante")
            Dim UpdatePanel_reg_edit_estante As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_estante")
            Dim DropDownList_reg_edit_entrepano_numero_unidades As DropDownList = page.FindControl("DropDownList_reg_edit_entrepano_numero_unidades")
            Dim Label_reg_edit_entrepano_numero_unidades As Label = page.FindControl("Label_reg_edit_entrepano_numero_unidades")
            Dim Label_title_reg_edit_entrepano As Label = page.FindControl("Label_title_reg_edit_entrepano")
            If value_selecion = "AGRE-EL" Then
                If TreeViewArchivo.SelectedNode Is Nothing Then
                    Seleccion_gestion_archivo_menu = "Por favor seleccione el elemento de la estructura"
                    Exit Function
                End If
                Dim splinodo() As String = TreeViewArchivo.SelectedNode.Value.Split("|")
                If splinodo(1) = "EDIFICIO" Then
                    Label_reg_edit_piso_title.Text = "Agregar nuevo piso"
                    UpdatePanel_reg_edit_piso_archivo.Update()
                    ModalPopupExtender_edition_reg_edit_piso_archivo.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "PISO" Then
                    Label_titulo_area_piso.Text = "Agregar nueva área"
                    UpdatePanel_reg_edit_area_piso.Update()
                    ModalPopupExtender_edition_reg_edit_area_piso.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "AREA" Then
                    Label_title_reg_edit_modulo.Text = "Agrega nuevo modulo"
                    UpdatePanel_reg_edit_modulo_area.Update()
                    ModalPopupExtender_edition_reg_edit_modulo_area.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "MODULO" Then
                    Label_title_reg_edit_estante.Text = "Agrega nuevo estante"
                    UpdatePanel_reg_edit_estante.Update()
                    ModalPopupExtender_edition_reg_edit_estante.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "ESTANTE" Then
                    DropDownList_reg_edit_entrepano_numero.Visible = False
                    Label_reg_edit_entrepano_numero.Visible = False
                    Label_reg_edit_entrepano_numero_unidades.Visible = True
                    DropDownList_reg_edit_entrepano_numero_unidades.Visible = True
                    Label_title_reg_edit_entrepano.Text = "Registra entrepaño"
                    UpdatePanel_reg_edit_entrepano.Update()
                    ModalPopupExtender_edition_reg_edit_entrepano.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
            End If
            If value_selecion = "EDIT-EL" Then
                If TreeViewArchivo.SelectedNode Is Nothing Then
                    Seleccion_gestion_archivo_menu = "Por favor seleccione el elemento de la estructura"
                    Exit Function
                End If
                Dim splinodo() As String = TreeViewArchivo.SelectedNode.Value.Split("|")
                If splinodo(1) = "PISO" Then
                    Result = Me.asgigna_datos_interface_piso_archivo(Val(splinodo(0)),
                                                                     page)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    ModalPopupExtender_edition_reg_edit_piso_archivo.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "ENTREPAÑO" Then
                    Result = Me.Agigna_datos_interface_entepano(Val(splinodo(0)),
                                                                     page)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    ModalPopupExtender_edition_reg_edit_entrepano.Show()
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
            End If
            If value_selecion = "DELT-EL" Then
                If TreeViewArchivo.SelectedNode Is Nothing Then
                    Seleccion_gestion_archivo_menu = "Por favor seleccione el elemento de la estructura a eliminar"
                    Exit Function
                End If
                Dim splinodo() As String = TreeViewArchivo.SelectedNode.Value.Split("|")
                If splinodo(1) = "PISO" Then
                    Result = Me.Eliminar_piso_archivo(Val(splinodo(0)),
                                                      TreeViewArchivo.SelectedNode,
                                                      TreeViewArchivo,
                                                      UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "AREA" Then
                    Result = Me.Elimina_area_archivo(Val(splinodo(0)),
                                                     TreeViewArchivo.SelectedNode,
                                                     TreeViewArchivo,
                                                     UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "MODULO" Then
                    Result = Me.Elimina_modulo_archivo(Val(splinodo(0)),
                                                     TreeViewArchivo.SelectedNode,
                                                     TreeViewArchivo,
                                                     UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "ESTANTE" Then
                    Result = Me.Eliminar_estante_archivo(Val(splinodo(0)),
                                                         TreeViewArchivo.SelectedNode,
                                                         TreeViewArchivo,
                                                         UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
                If splinodo(1) = "ENTREPAÑO" Then
                    Result = Me.Elimina_entrepano_archivo(Val(splinodo(0)),
                                                         TreeViewArchivo.SelectedNode,
                                                         TreeViewArchivo,
                                                         UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Seleccion_gestion_archivo_menu = Result
                        Exit Function
                    End If
                    Seleccion_gestion_archivo_menu = "YES"
                    Exit Function
                End If
            End If
            Seleccion_gestion_archivo_menu = "YES"
        Catch ex As Exception
            Seleccion_gestion_archivo_menu = "Inconsistencia general función "
        End Try
    End Function

    Function Solicita_datos_estructura_edificio(ByVal id_edificio As Integer, _
                                                ByRef estru_edificio As Edificio_Archivo) As String
        '**********************************************************
        'Funcion : Solicita los datos de la estructura del edificio
        'por la identificación del edificio
        'Ing  : Miguel Angel Urueta Miranda
        'Fecha : 2019-01-29 modificado
        '***********************************************************
        Try
            Dim SqlConsulta As String = "SELECT ID_EDIFICIO,NOMBRE_EDIFICIO,DIRECCION_EDIFICIO,TELEFONO_EDIFICIO,RESPONSABLE_EDIFICIO " & _
            ",PAIS_UBICACION,DEPARTAMENTO_UBICACION,MUNICIPIO_UBICACION FROM  edificio_contenedor   WHERE ID_EDIFICIO=" & id_edificio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_edificio = "Error Listar_Edificio_archivo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then       
                estru_edificio.id_edificio = Datset.Tables(0).Rows(0).Item(0)
                estru_edificio.nombre_edificio = Datset.Tables(0).Rows(0).Item(1)
                estru_edificio.direccion = Datset.Tables(0).Rows(0).Item(2)
                estru_edificio.telefono = Datset.Tables(0).Rows(0).Item(3)
                estru_edificio.responsable = Datset.Tables(0).Rows(0).Item(4)
                estru_edificio.pais_edificio = Datset.Tables(0).Rows(0).Item(5)
                estru_edificio.departamento_edificio = Datset.Tables(0).Rows(0).Item(6)
                estru_edificio.ciudad_edificio = Datset.Tables(0).Rows(0).Item(7)
                Solicita_datos_estructura_edificio = "YES"
                Exit Function
            Else
                Solicita_datos_estructura_edificio = "Imposible encontrar la estructura del edificio por el siguiente id (" & id_edificio & ")"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_datos_estructura_edificio = "Inconsistencia general  función Solicita_datos_estructura_edificio " & ex.Message
        End Try
    End Function





    Function Verifica_existencia_edificio(ByVal Nombre_Edificio As String, _
                                          ByVal id_empresa As Integer, _
                                          ByRef Confirma As String) As String
        '**********************************************************************
        'Funcion : verifica la existencia del edificio dentro de la empresa
        'con el parametro nombre edificio y id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-06-18 - Modificado para web 2019-01-29
        '**********************************************************************
        Try
            Dim Parametro_Consulta As String = "select * from  edificio_contenedor" & _
                                       " where ID_EMPRESA=" & id_empresa & " and NOMBRE_EDIFICIO='" & Nombre_Edificio & "'"
             Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("edificio_contenedor")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_edificio = "Función Verifica_existencia_edificio dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Confirma = "YES"
            Else
                Confirma = "NO"
            End If
            Verifica_existencia_edificio = "YES"
        Catch ex As Exception
            Verifica_existencia_edificio = "Inconsistencia General Funcion Verifica_existencia_edificio " & ex.Message
        End Try

    End Function
    Function asigna_datos_interface_edificio(ByVal id_edificio As Integer, _
                                             ByRef page As Page) As String
        Try
            Dim ModalPopupExtender_edition_reg_edit_edificio_archivo As AjaxControlToolkit.ModalPopupExtender _
               = Page.FindControl("ModalPopupExtender_edition_reg_edit_edificio_archivo")
            Dim Label_reg_edit_title As Label = Page.FindControl("Label_reg_edit_title")
            Dim UpdatePanel_reg_edit_edificio_archivo As UpdatePanel = Page.FindControl("UpdatePanel_reg_edit_edificio_archivo")
            Dim DropDownList_reg_edit_pais As DropDownList = page.FindControl("DropDownList_reg_edit_pais")
            Dim DropDownList_reg_edit_departamento As DropDownList = page.FindControl("DropDownList_reg_edit_departamento")
            Dim DropDownList_reg_edit_munici_depart As DropDownList = page.FindControl("DropDownList_reg_edit_munici_depart")
            Dim TextBox_reg_edit_direcion As TextBox = page.FindControl("TextBox_reg_edit_direcion")
            Dim TextBox_reg_edit_telefono As TextBox = page.FindControl("TextBox_reg_edit_telefono")
            Dim TextBox_reg_edit_responsable As TextBox = page.FindControl("TextBox_reg_edit_responsable")
            Dim TextBox_reg_edit_edificio_nombre As TextBox = page.FindControl("TextBox_reg_edit_edificio_nombre")
            Dim estru_edificio As Edificio_Archivo
            Dim Result As String = ""
            Dim id_pais As Integer = 0
            Dim id_departamento As Integer = 0
            Dim id_muni_ciudad As Integer = 0
            Result = Me.Solicita_datos_estructura_edificio(id_edificio, _
                                                          estru_edificio)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If
            Dim Class_pais_radicacion As New Class_pais_radicacion

            Result = Class_pais_radicacion.solicita_id_pais_nombre(estru_edificio.pais_edificio,
                                                                   id_pais)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If
            Dim Class_depart_radicacion As New Class_depart_radicacion
            Result = Class_depart_radicacion.solicita_id_departamento_nombre(estru_edificio.departamento_edificio,
                                                                             id_departamento)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If
            Dim Class_municipio_radicacion As New Class_municipio_radicacion
            Result = Class_municipio_radicacion.solicita_id_municipio_ciudad_nombre(estru_edificio.ciudad_edificio,
                                                                                    id_muni_ciudad)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If

            Result = Class_pais_radicacion.Lista_Paises(id_pais,
                                                        DropDownList_reg_edit_pais,
                                                        UpdatePanel_reg_edit_edificio_archivo)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If

            Result = Class_depart_radicacion.Lista_departamento_Paises(id_pais,
                                                                       id_departamento,
                                                                       DropDownList_reg_edit_departamento,
                                                                       UpdatePanel_reg_edit_edificio_archivo)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If

            Result = Class_municipio_radicacion.Lista_municipios_departamento(id_departamento,
                                                                              id_muni_ciudad,
                                                                              DropDownList_reg_edit_munici_depart,
                                                                              UpdatePanel_reg_edit_edificio_archivo)
            If Result <> "YES" Then
                asigna_datos_interface_edificio = Result
                Exit Function
            End If
            TextBox_reg_edit_direcion.Text = estru_edificio.direccion
            TextBox_reg_edit_telefono.Text = estru_edificio.telefono
            TextBox_reg_edit_responsable.Text = estru_edificio.responsable
            TextBox_reg_edit_edificio_nombre.Text = estru_edificio.nombre_edificio
            Label_reg_edit_title.Text = "Edita edificio (" & estru_edificio.nombre_edificio & ")"
            UpdatePanel_reg_edit_edificio_archivo.Update()
            asigna_datos_interface_edificio = "YES"
            Exit Function
        Catch ex As Exception
            asigna_datos_interface_edificio = "Inconsistencia general función asigna_datos_interface_edificio " & ex.Message
        End Try
    End Function
    Function Registra_Edificio_Archivo(ByVal Nombre_Empresa As String, _
                                       ByVal nombre_pais As String, _
                                       ByVal nombre_departamento As String, _
                                       ByVal nombre_municipio As String, _
                                       ByVal Numero_telefono As String, _
                                       ByVal nombre_responsable As String, _
                                       ByVal direccion As String, _
                                       ByVal nombre_edificio As String, _
                                       ByRef treview As TreeView, _
                                       ByRef update As UpdatePanel) As String
        '***********************************************************************
        'Funcion : Registra edificio en la base de datos y lista el listview
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-06-18  modificado para web 2019-01-29
        '***********************************************************************
        '------Verifica campos obligatorios
        If nombre_pais = "" Then
            Registra_Edificio_Archivo = "Debe seleccionar el pais del edificio"
            Exit Function
        End If
        If nombre_departamento = "" Then
            Registra_Edificio_Archivo = "Debe seleccionar el departamento del edificio"
            Exit Function
        End If
        If nombre_municipio = "" Then
            Registra_Edificio_Archivo = "Debe seleccionar el municipio del edificio"
            Exit Function
        End If
        If direccion = "" Then
            Registra_Edificio_Archivo = "Debe digitar la dirección del edificio"
            Exit Function
        End If
        If Numero_telefono = "" Then
            Registra_Edificio_Archivo = "Debe digitar el telefono del edificio"
            Exit Function
        End If
        If nombre_responsable = "" Then
            Registra_Edificio_Archivo = "Debe digitar el nombre del responsable"
            Exit Function
        End If

        If nombre_edificio = "" Then
            Registra_Edificio_Archivo = "Debe digitar el nombre del edificio"
            Exit Function
        End If
        '------------Solicita id de la empresa
        Dim Result As String = ""
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim id_empresa As Integer = 0
        Result = Reflasgest.Retorna_Id_Emprea(Nombre_Empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registra_Edificio_Archivo = "Imposible encontrar id empresa " & Result
            Exit Function
        End If
        '-------Verifica existencia edificio
        Dim Confirma As String = ""
        Result = Verifica_existencia_edificio(nombre_edificio, _
                                              id_empresa, _
                                              Confirma)
        If Result <> "YES" Then
            Registra_Edificio_Archivo = Result
            Exit Function
        End If
        If Confirma = "YES" Then
            Registra_Edificio_Archivo = "El edificio informado ya existe en la entidad/empresa seleccionada"
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registra_Edificio_Archivo = Result
            Exit Function
        End If
        Dim SqlInsert As String = "Insert into edificio_contenedor (ID_EMPRESA,PAIS_UBICACION,DEPARTAMENTO_UBICACION," & _
        "MUNICIPIO_UBICACION,DIRECCION_EDIFICIO,TELEFONO_EDIFICIO,RESPONSABLE_EDIFICIO,FECHA_CREACION,NOMBRE_EDIFICIO) values (" & _
        id_empresa & ",'" & nombre_pais & "','" & nombre_departamento & "','" & nombre_municipio & "','" & direccion & "','" & _
        Numero_telefono & "','" & nombre_responsable & "','" & date1al & "','" & nombre_edificio & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = SqlInsert
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_Edificio_Archivo = "Imposible registrar edificio  : " & SqlInsert
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id usuario gestion
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '------------------------------------------------------------
            'Inserta los permisos de produccion del usuario
            Result = Me.Agrega_interface_Edificio_Archivo_treview(objet, _
                                                                  nombre_edificio, _
                                                                  nombre_responsable, _
                                                                  treview)
            If Result <> "YES" Then
                Registra_Edificio_Archivo = "Imposible registrar en treeview  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            update.Update()
            Registra_Edificio_Archivo = "YES"
         Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registra_Edificio_Archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_Edificio_Archivo = errorM

        End Try
    End Function
    Function Actualiza_Edificio_Archivo(ByVal Nombre_Empresa As String, _
                                        ByVal id_edificio As Integer, _
                                        ByVal nombre_pais As String, _
                                        ByVal nombre_departamento As String, _
                                        ByVal nombre_municipio As String, _
                                        ByVal Numero_telefono As String, _
                                        ByVal nombre_responsable As String, _
                                        ByVal direccion As String, _
                                        ByVal nombre_edificio As String, _
                                        ByRef trenode As TreeNode, _
                                        ByRef update As UpdatePanel) As String
        '***********************************************************************
        'Funcion : Registra edificio en la base de datos y lista el listview
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-06-18  modificado para web 2019-01-29
        '***********************************************************************
        '------Verifica campos obligatorios
        If nombre_pais = "" Then
            Actualiza_Edificio_Archivo = "Debe seleccionar el pais del edificio"
            Exit Function
        End If
        If nombre_departamento = "" Then
            Actualiza_Edificio_Archivo = "Debe seleccionar el departamento del edificio"
            Exit Function
        End If
        If nombre_municipio = "" Then
            Actualiza_Edificio_Archivo = "Debe seleccionar el municipio del edificio"
            Exit Function
        End If
        If direccion = "" Then
            Actualiza_Edificio_Archivo = "Debe digitar la dirección del edificio"
            Exit Function
        End If
        If Numero_telefono = "" Then
            Actualiza_Edificio_Archivo = "Debe digitar el telefono del edificio"
            Exit Function
        End If
        If nombre_responsable = "" Then
            Actualiza_Edificio_Archivo = "Debe digitar el nombre del responsable"
            Exit Function
        End If

        If nombre_edificio = "" Then
            Actualiza_Edificio_Archivo = "Debe digitar el nombre del edificio"
            Exit Function
        End If
        '------------Solicita id de la empresa
        Dim Result As String = ""
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim id_empresa As Integer = 0
        Result = Reflasgest.Retorna_Id_Emprea(Nombre_Empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Actualiza_Edificio_Archivo = "Imposible encontrar id empresa " & Result
            Exit Function
        End If
        Dim estru_edificio As Edificio_Archivo
        Result = Me.Solicita_datos_estructura_edificio(id_edificio, _
                                                      estru_edificio)
        If Result <> "YES" Then
            Actualiza_Edificio_Archivo = Result
            Exit Function
        End If
        '-------Verifica existencia edificio
        Dim Confirma As String = ""
        If estru_edificio.nombre_edificio <> nombre_edificio Then
            Result = Verifica_existencia_edificio(nombre_edificio, _
                                             id_empresa, _
                                             Confirma)
            If Result <> "YES" Then
                Actualiza_Edificio_Archivo = Result
                Exit Function
            End If
            If Confirma = "YES" Then
                Actualiza_Edificio_Archivo = "El edificio informado ya existe en la entidad/empresa seleccionada"
                Exit Function
            End If
        End If
       
        Dim SqlInsert As String = "Update edificio_contenedor set  PAIS_UBICACION='" & nombre_pais & "', DEPARTAMENTO_UBICACION='" & _
        nombre_departamento & "', MUNICIPIO_UBICACION='" & nombre_municipio & "', DIRECCION_EDIFICIO='" & direccion & "'," & _
        " TELEFONO_EDIFICIO='" & Numero_telefono & "'," & "RESPONSABLE_EDIFICIO='" & nombre_responsable & "', NOMBRE_EDIFICIO='" & nombre_edificio & "'" & _
        " where ID_EDIFICIO=" & id_edificio
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = SqlInsert
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_Edificio_Archivo = "Imposible registrar edificio  : " & SqlInsert
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
           
            '------------------------------------------------
            'Inserta los permisos de produccion del usuario
            '------------------------------------------------
            Result = Me.Actualiza_interface_Edificio_Archivo_treview(id_edificio, _
                                                                     nombre_edificio, _
                                                                     nombre_responsable, _
                                                                     trenode)
            If Result <> "YES" Then
                Actualiza_Edificio_Archivo = "Imposible registrar en treeview  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            update.Update()
            Actualiza_Edificio_Archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_Edificio_Archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_Edificio_Archivo = errorM

        End Try
    End Function

    Function Agrega_interface_Edificio_Archivo_treview(ByVal id_edificio As Integer, _
                                                       ByVal nombre_edificio As String, _
                                                       ByVal responsable_edificio As String, _
                                                       ByRef Treview As TreeView) As String
        '*******************************************************************
        'Funcion : Agrega edificio al treeview
        'Fecha 2019-01-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try



            Dim attrNode As New TreeNode
            attrNode.Text = "Edificio : " & nombre_edificio
            attrNode.Value = id_edificio & "|EDIFICIO|" & nombre_edificio
            attrNode.ToolTip = responsable_edificio
            attrNode.ImageUrl = "../workflow/imageneswf/building-light.png"
            Treview.Nodes.Add(attrNode)
            Agrega_interface_Edificio_Archivo_treview = "YES"
        Catch ex As Exception
            Agrega_interface_Edificio_Archivo_treview = "Inconsistencia general funcion Agrega_interface_Edificio_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Actualiza_interface_Edificio_Archivo_treview(ByVal id_edificio As Integer, _
                                                          ByVal nombre_edificio As String, _
                                                          ByVal responsable_edificio As String, _
                                                          ByRef trenode As TreeNode) As String
        '*******************************************************************
        'Funcion : Actualiza edificio al treeview
        'Fecha 2019-01-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            trenode.Text = "Edificio : " & nombre_edificio
            trenode.Value = id_edificio & "|EDIFICIO|" & nombre_edificio
            trenode.ToolTip = responsable_edificio
            trenode.ImageUrl = "../workflow/imageneswf/building-light.png"
            Actualiza_interface_Edificio_Archivo_treview = "YES"
        Catch ex As Exception
            Actualiza_interface_Edificio_Archivo_treview = "Inconsistencia general funcion Agrega_interface_Edificio_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Solicita_datos_piso_archivo_estructura(ByVal Id_Piso As Integer, _
                                                    ByRef estru_piso As Piso_Archivo) As String
        '******************************************************************
        'Funcion : Retorna los datos del piso con el parametro id piso
        'Fecha : 2014-06-30
        'Ing : Miguel Angel Urueta Miranda -Modifica para web 2019-01-31
        'Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT  ID_PISO,NOMBRE_PISO,TELEFONO_PISO,RESPONSABLE_PISO,CODIGO_UNICO_PISO,EDIFICIO_CONTENEDOR_ID_EDIFICIO " & _
                " FROM piso_archivo where ID_PISO=" & Id_Piso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_piso_archivo_estructura = "Función Solicita_datos_piso_archivo_estructura dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estru_piso.id_piso = Datset.Tables(0).Rows(0).Item(0)
                estru_piso.nombre_piso = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    estru_piso.telefono = ""
                Else
                    estru_piso.telefono = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    estru_piso.responsable = ""
                Else
                    estru_piso.responsable = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    estru_piso.codigo_piso = ""
                Else
                    estru_piso.codigo_piso = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    estru_piso.EDIFICIO_CONTENEDOR_ID_EDIFICIO = 0
                Else
                    estru_piso.EDIFICIO_CONTENEDOR_ID_EDIFICIO = Datset.Tables(0).Rows(0).Item(5)
                End If
            Else
                Solicita_datos_piso_archivo_estructura = "Imposible encontrar datos del piso (" & Id_Piso & ")"
                Exit Function
            End If
            Solicita_datos_piso_archivo_estructura = "YES"
        Catch ex As Exception
            Solicita_datos_piso_archivo_estructura = "Inconsistencia general Funcion Solicita_datos_piso_archivo_estructura " & ex.Message
        End Try
    End Function
    Function asgigna_datos_interface_piso_archivo(ByVal id_piso As Integer, _
                                                  ByRef page As Page) As String
        '---------------------------------------------
        'Función : Asigna datos a las interface editar
        'piso archivo
        'Fecha : 2019-01-31
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Dim estru_piso As Piso_Archivo
            Dim Result As String = ""
            Dim UpdatePanel_reg_edit_piso_archivo As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_piso_archivo")
            Dim Label_reg_edit_piso_title As Label = page.FindControl("Label_reg_edit_piso_title")
            Dim TextBox_reg_edit_piso_telefono As TextBox = page.FindControl("TextBox_reg_edit_piso_telefono")
            Dim TextBox_reg_edit_piso_responsable As TextBox = page.FindControl("TextBox_reg_edit_piso_responsable")
            Dim TextBox_reg_edit_piso_nombre As TextBox = page.FindControl("TextBox_reg_edit_piso_nombre")
            Result = Me.Solicita_datos_piso_archivo_estructura(id_piso, _
                                                               estru_piso)
            If Result <> "YES" Then
                asgigna_datos_interface_piso_archivo = Result
                Exit Function
            End If
            TextBox_reg_edit_piso_telefono.Text = estru_piso.telefono
            TextBox_reg_edit_piso_responsable.Text = estru_piso.responsable
            TextBox_reg_edit_piso_nombre.Text = estru_piso.nombre_piso
            Label_reg_edit_piso_title.Text = "Edita el piso (" & estru_piso.nombre_piso & ")"
            UpdatePanel_reg_edit_piso_archivo.Update()
            asgigna_datos_interface_piso_archivo = "YES"
        Catch ex As Exception
            asgigna_datos_interface_piso_archivo = "Inconsistencia general función asgigna_datos_interface_piso_archivo " & ex.Message
        End Try
    End Function
    Function Registrar_piso_archivo(ByVal id_edificio As Integer, _
                                    ByVal nombre_empresa As String, _
                                    ByVal nombre_piso As String, _
                                    ByVal telefono_piso As String, _
                                    ByVal responsable_piso As String, _
                                    ByRef node As TreeNode, _
                                    ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim id_empresa As Integer = 0
        Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registrar_piso_archivo = "Imposible encontrar id de la empresa (" & nombre_empresa & ") " & Result
            Exit Function
        End If
        '-------Verifica datos obligatorios
        If nombre_piso = "" Then
            Registrar_piso_archivo = "Debe digitar el nombre del piso "
            Exit Function
        End If
        '-------Verifica existencia del edificio en el piso
        Result = Verifica_Existencia_Piso_edificio(id_edificio, _
                                                   nombre_piso)
        If Result <> "YES" Then
            Registrar_piso_archivo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_piso_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_PISO  from EDIFICIO_CONTENEDOR where ID_EDIFICIO=" & id_edificio & _
             " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registrar_piso_archivo = "Imposible encontrar el registro del edificio (conexión) (" & id_edificio & ")"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registrar_piso_archivo = "Imposible encontrar el registro del edificio  (" & id_edificio & ")"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_piso As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_piso = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            consecutivo_piso = consecutivo_piso + 1
            '----Crea numero consecutivo unico piso
            Dim codigo_unico As String = "PISO-" & consecutivo_piso & "ED-" & id_edificio & "EMP-" & id_empresa
            '----Registra piso
            Dim sqlregistra As String = "Insert Into PISO_ARCHIVO (EDIFICIO_CONTENEDOR_ID_EDIFICIO,CONSECUTIVO_PISO,TELEFONO_PISO" & _
            ",RESPONSABLE_PISO,CODIGO_UNICO_PISO,NOMBRE_PISO,FECHA_CREACION) values (" & id_edificio & "," & consecutivo_piso & ",'" & _
            telefono_piso & "','" & responsable_piso & "','" & codigo_unico & "','" & nombre_piso & "','" & date1al & "')"
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_piso_archivo = "Imposible registrar edificio  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id piso
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "UPDATE EDIFICIO_CONTENEDOR SET CONSECUTIVO_PISO=" & consecutivo_piso & "  where ID_EDIFICIO=" & id_edificio
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_piso_archivo = "Imposible actualiza consecutivo piso  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Me.Agregar_Piso_archivo_treview(objet, _
                                                     nombre_piso, _
                                                     responsable_piso, _
                                                     node)
            If Result <> "YES" Then
                Registrar_piso_archivo = "Imposible registrar el piso  en la interface " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            update.Update()
            Registrar_piso_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registrar_piso_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registrar_piso_archivo = errorM

        End Try
    End Function
    Function Verifica_Existencia_Piso_edificio(ByVal Id_edificio As Integer, _
                                               ByVal nombre_piso As String) As String
        '***************************************************************************
        'Funcion : Verifica la existencia de nombre de piso con el parametro
        'id del edificio y nombre del piso para evitar la duplicidad de nombre
        'de piso en cada edificio
        'Fecha : 2014-06-30   
        'Ing :Miguel Angel Urueta Miranda Modificado : 2019-01-31 
        'Ing Miguel Angel Urueta Miranda
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from PISO_ARCHIVO where NOMBRE_PISO='" & nombre_piso & _
           "' and EDIFICIO_CONTENEDOR_ID_EDIFICIO=" & Id_edificio
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("PISO_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Piso_edificio = "Función Verifica_Existencia_Piso_edificio dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Piso_edificio = "El piso informado se encuentra registrado en el edificio"
                Exit Function
            Else
                Verifica_Existencia_Piso_edificio = "YES"
                Exit Function
            End If
            Verifica_Existencia_Piso_edificio = "YES"
        Catch ex As Exception
            Verifica_Existencia_Piso_edificio = "Inconsistencia general funcion Verifica_Existencia_Piso_edificio " & ex.Message
        End Try
    End Function
    Function Agregar_Piso_archivo_treview(ByVal id_piso As Integer, _
                                          ByVal nombre_piso As String, _
                                          ByVal responsable As String, _
                                          ByRef nod As TreeNode) As String
        Try

            Dim attrNode As New TreeNode
            attrNode.Text = "Piso: " & nombre_piso
            attrNode.Value = id_piso & "|PISO|" & nombre_piso
            attrNode.ToolTip = responsable
            attrNode.ImageUrl = "../workflow/imageneswf/layer-group-light.png"
            nod.ChildNodes.Add(attrNode)
            Agregar_Piso_archivo_treview = "YES"
        Catch ex As Exception
            Agregar_Piso_archivo_treview = "Inconsistencia funcion Agregar_Piso_archivo_treview " & ex.Message
        End Try
    End Function
    Function Actualiza_piso_archivo(ByVal id_piso As Integer, _
                                    ByVal nombre_empresa As String, _
                                    ByVal nombre_piso As String, _
                                    ByVal telefono_piso As String, _
                                    ByVal responsable_piso As String, _
                                    ByRef node As TreeNode, _
                                    ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Dim Reflasgest As New ClassAdmonEmpresa
        '-------Verifica datos obligatorios
        If nombre_piso = "" Then
            Actualiza_piso_archivo = "Debe digitar el nombre del piso "
            Exit Function
        End If
        Dim estru_piso As Piso_Archivo
        Result = Me.Solicita_datos_piso_archivo_estructura(id_piso, _
                                                           estru_piso)
        If Result <> "YES" Then
            Actualiza_piso_archivo = Result
            Exit Function
        End If
        '-------Verifica existencia del edificio en el piso
        If nombre_piso <> estru_piso.nombre_piso Then
            Result = Verifica_Existencia_Piso_edificio(estru_piso.EDIFICIO_CONTENEDOR_ID_EDIFICIO, _
                                                       nombre_piso)
            If Result <> "YES" Then
                Actualiza_piso_archivo = Result
                Exit Function
            End If
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "Update piso_archivo set TELEFONO_PISO='" & telefono_piso & "', RESPONSABLE_PISO='" & _
            responsable_piso & "', NOMBRE_PISO='" & nombre_piso & "'" & _
            " where ID_PISO=" & id_piso
            myCommand.CommandText = updatconsecutivo
            Dim Switc As Integer = 0
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_piso_archivo = "Imposible actualizar el piso  " & updatconsecutivo
                myConnection.Close()
                Exit Function
            End If
            Result = Me.Actualiza_Piso_archivo_treview(id_piso, _
                                                     nombre_piso, _
                                                     responsable_piso, _
                                                     node)
            If Result <> "YES" Then
                Actualiza_piso_archivo = "Imposible actualizar el piso  en la interface " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            update.Update()
            updatconsecutivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_piso_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_piso_archivo = errorM
        End Try

    End Function
    Function Actualiza_Piso_archivo_treview(ByVal id_piso As Integer, _
                                            ByVal nombre_piso As String, _
                                            ByVal responsable As String, _
                                            ByRef nod As TreeNode) As String
        Try
            nod.Text = "Piso: " & nombre_piso
            nod.Value = id_piso & "|PISO|" & nombre_piso
            nod.ToolTip = responsable
            Actualiza_Piso_archivo_treview = "YES"
        Catch ex As Exception
            Actualiza_Piso_archivo_treview = "Inconsistencia funcion Actualiza_Piso_archivo_treview " & ex.Message
        End Try
    End Function
    Function Eliminar_piso_archivo(ByVal id_piso As Integer, _
                                   ByVal trenode As TreeNode, _
                                   ByRef treview As TreeView, _
                                   ByRef update As UpdatePanel) As String
        Dim Result As String = Determina_Existencia_Piso_Area(id_piso)
        If Result <> "YES" Then
            Eliminar_piso_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "delete from piso_archivo where ID_PISO=" & Id_Piso
            myCommand.CommandText = updatconsecutivo
            Dim Switc As Integer = 0
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_piso_archivo = "Imposible eliminar el piso  " & updatconsecutivo
                myConnection.Close()
                Exit Function
            End If
            treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            Eliminar_piso_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_piso_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_piso_archivo = errorM
        End Try
    End Function
    Function Determina_Existencia_Piso_Area(ByVal Id_Piso As Integer) As String
        '************************************************************
        'Funcion : Determina la existencia de areas dentro del piso
        'Fecha : 2014-06-30
        'Ing : Miguel Angel Urueta Modificado web 2019-01-31
        'Miguel Angel Urueta
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from area_archivo where PISO_ARCHIVO_ID_PISO=" & Id_Piso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Determina_Existencia_Piso_Area = "Función Determina_Existencia_Piso_Area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Determina_Existencia_Piso_Area = "El piso que desea eliminar tiene areas registradas"
                Exit Function
            Else

                Determina_Existencia_Piso_Area = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Determina_Existencia_Piso_Area = "Inconsistencia general Funcion Determina_Existencia_Piso_Area " & ex.Message
        End Try
    End Function
    Function Registrar_area_piso_archivo(ByVal id_piso As Integer, _
                                         ByVal nombre_empresa As String, _
                                         ByVal Telefono As String, _
                                         ByVal responsable_area As String, _
                                         ByVal nombre_area As String, _
                                         ByRef node As TreeNode, _
                                         ByRef update As UpdatePanel, _
                                         ByVal tipo_archivo As String) As String

        If tipo_archivo = "" Then
            Registrar_area_piso_archivo = "Debe seleccionar el tipo de archivo"
            Exit Function
        End If
        If Telefono = "" Then
            Registrar_area_piso_archivo = "Debe digitar el número telefono"
            Exit Function
        End If
        If responsable_area = "" Then
            Registrar_area_piso_archivo = "Debe digitar el nombre responsable"
            Exit Function
        End If
        If nombre_area = "" Then
            Registrar_area_piso_archivo = "Debe digitar el nombre del área"
            Exit Function
        End If

        '------Solicita id empresa
        Dim Result As String = ""
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim id_empresa As Integer = 0
        Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registrar_area_piso_archivo = "Imposible encontrar id de la empresa (" & nombre_empresa & ") " & Result
            Exit Function
        End If

        ''------Solicita id edificio
        Dim id_edificio As Integer = -1
        Result = Me.Retorna_id_edifio_archivo_por_id_piso(id_piso, _
                                                          id_edificio)
        If Result <> "YES" Then
            Registrar_area_piso_archivo = "Imposible retornar id del edificio " & Result
            Exit Function
        End If
        '------Verifica existencia area piso
        Result = Verifica_Existencia_Area_Piso(id_piso, _
                                               nombre_area)
        If Result <> "YES" Then
            Registrar_area_piso_archivo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_area_piso_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_MODULO  from PISO_ARCHIVO where ID_PISO=" & id_piso & _
                                         " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registrar_area_piso_archivo = "Imposible Encontrar consecutivo área error de conexion (conexión) (" & id_piso & ")"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registrar_area_piso_archivo = "Imposible Encontrar consecutivo área "
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_area As Integer = 0
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_area = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            consecutivo_area = consecutivo_area + 1
            '----Crea numero consecutivo unico piso
            Dim codigo_unico As String = "AREA-" & consecutivo_area & "ED-" & id_edificio & "EMP-" & id_empresa
            '----Registra piso
            Dim sqlregistra As String = "Insert Into AREA_ARCHIVO (PISO_ARCHIVO_ID_PISO,CONSECUTIVO_AREA,TELEFONO_RESPONSABLE" & _
            ",NOMBRE_RESPONSABLE,CODIGO_UNICO_AREA,NOMBRE_AREA,FECHA_CREACION,TIPO_ARCHIVO) values (" & id_piso & "," & consecutivo_area & ",'" & _
            Telefono & "','" & responsable_area & "','" & codigo_unico & "','" & nombre_area & "','" & date1al & "','" & tipo_archivo & "')"
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_area_piso_archivo = "Imposible registrar el área  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id piso
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "UPDATE PISO_ARCHIVO SET CONSECUTIVO_MODULO=" & consecutivo_area & "  where ID_PISO=" & id_piso
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_area_piso_archivo = "Imposible actualiza consecutivo area  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------Registra piso en la interface
            Result = Agregar_Area_Archivo_Treview(nombre_area, _
                                                  objet, _
                                                  node)
            If Result <> "YES" Then
                Registrar_area_piso_archivo = "Imposible Registrar area en la interface  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            update.Update()
            Registrar_area_piso_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registrar_area_piso_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registrar_area_piso_archivo = errorM

        End Try
    End Function
    Function Retorna_Id_Edificio(ByVal Nombre_Edificio As String, _
                                 ByVal id_empresa As Integer, _
                                 ByRef id_edificio As Integer) As String
        '**********************************************************************
        'Funcion : Retorna id de edificio dentro de la empresa
        'con el parametro nombre edificio y id empresa
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-06-18 - Modificado para web  2019-02-15
        '**********************************************************************
        Try
            Dim Parametro_Consulta As String = "select ID_EDIFICIO from  edificio_contenedor" & _
                                       " where ID_EMPRESA=" & id_empresa & " and NOMBRE_EDIFICIO='" & Nombre_Edificio & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("edificio_contenedor")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Edificio = "Función Retorna_Id_Edificio dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Retorna_Id_Edificio = "YES"
                id_edificio = Datset.Tables(0).Rows(0).Item(0)
                Exit Function
            Else
                Retorna_Id_Edificio = "Imposible encontrar id de edificio (" & Nombre_Edificio & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_Edificio = "Inconsistencia General Funcion Retorna_Id_Edificio" & ex.Message
        End Try
    End Function
    Function Verifica_Existencia_Area_Piso(ByVal Id_piso As Integer, ByVal nombre_area As String) As String
        '***************************************************************************
        'Funcion : Verifica la existencia del area  con el parametro
        'id del piso y nombre del area para evitar la duplicidad de nombre
        'de area en cada piso
        'Fecha : 2014-06-30
        'Ing :Miguel Angel Urueta Miranda / Modifica para web 2019-02-15
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from AREA_ARCHIVO where NOMBRE_AREA='" & nombre_area & _
            "' and PISO_ARCHIVO_ID_PISO=" & Id_piso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREA_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Area_Piso = "Función Verifica_Existencia_Area_Piso dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Area_Piso = "El área informada se encuentra registrada en el piso"
                Exit Function
            Else
                Verifica_Existencia_Area_Piso = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Area_Piso = "Inconsistencia general funcion Verifica_Existencia_Area_Piso " & ex.Message
        End Try
    End Function
    Function Retorna_Id_Piso(ByVal Id_edificio As Integer, _
                             ByVal nombre_piso As String, _
                             ByRef id_piso As Integer) As String
        '*******************************************************************
        'Funcion : Retorna id el piso con los parametros nombre piso y id
        'edificio
        'Fecha : 2014-06-30
        'Ing : Miguel Angel Urueta Miranda / Modificado para web 2019-02-15
        '*******************************************************************
        Try
            Dim Parametro_Consulta As String = "select ID_PISO from  piso_archivo" & _
                                           " where EDIFICIO_CONTENEDOR_ID_EDIFICIO=" & Id_edificio & " and NOMBRE_PISO='" & nombre_piso & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("piso_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Piso = "Función Verifica_Existencia_Area_Piso dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_piso = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Id_Piso = "YES"
                Exit Function
            Else
                Retorna_Id_Piso = "Imposible encontrar la identificación del piso (" & nombre_piso & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_Piso = "Inconsistencia general funcion Retorna_Id_Piso " & ex.Message
        End Try
    End Function
    Function Agregar_Area_Archivo_Treview(ByVal nombre_area As String, _
                                          ByVal id_area As Integer, _
                                          ByRef nod As TreeNode) As String
        '***************************************************
        'Funcion 
        'estructura
        'Fecha 2016-02-03
        'Ing :Miguel Angel Urueta Miranda
        '***************************************************
        Try
            Dim attrNode As New TreeNode
            attrNode.Text = "Area: " & nombre_area
            attrNode.Value = id_area & "|AREA|" & nombre_area
            attrNode.ToolTip = id_area & "|AREA|" & nombre_area
            attrNode.ImageUrl = "../workflow/imageneswf/area-light.png"
            nod.ChildNodes.Add(attrNode)
            Agregar_Area_Archivo_Treview = "YES"
        Catch ex As Exception
            Agregar_Area_Archivo_Treview = "Inconsistencia funcion Agregar_Area_Archivo_Treview " & ex.Message
        End Try
    End Function
    Function Solicita_Datos_estructura_area_archivo(ByVal Id_area As Integer, _
                                            ByRef estru_area As Area_Archivo) As String
        '******************************************************************
        'Funcion : Retorna los datos del area con el parametro id area
        'Fecha : 2014-06-30
        'Ing : Miguel Angel Urueta Miranda- Modificado web 2019-02-15
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT  ID_AREA,NOMBRE_AREA,TELEFONO_RESPONSABLE,NOMBRE_RESPONSABLE,CODIGO_UNICO_AREA,TIPO_ARCHIVO " & _
                " FROM area_archivo where ID_AREA=" & Id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("piso_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_Datos_estructura_area_archivo = "Función Solicita_Datos_estructura_area_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estru_area.id_area = Datset.Tables(0).Rows(0).Item(0)
                estru_area.nombre_area = Datset.Tables(0).Rows(0).Item(1)
                estru_area.telefono = Datset.Tables(0).Rows(0).Item(2)
                estru_area.responsable = Datset.Tables(0).Rows(0).Item(3)
                estru_area.codigo_area = Datset.Tables(0).Rows(0).Item(4)
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    estru_area.tipo_archivo = ""
                Else
                    estru_area.tipo_archivo = Datset.Tables(0).Rows(0).Item(5)
                End If
                Solicita_Datos_estructura_area_archivo = "YES"
                Exit Function
            Else
                Solicita_Datos_estructura_area_archivo = "Imposible encontrar datos del area (" & Id_area & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_Datos_estructura_area_archivo = "Inconsistencia general Funcion Solicita_Datos_estructura_area_archivo " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_area_archivo(ByVal id_area As Integer, _
                                                 ByRef pag As Page) As String
        '------------------------------------------------------
        'Función : Asigna datos interface editar area archivo
        'Fecha : 2019-02-15
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru_area As Area_Archivo = Nothing
            Dim TextBox_telefono_area_piso As TextBox = pag.FindControl("TextBox_telefono_area_piso")
            Dim TextBox_responsable_area_piso As TextBox = pag.FindControl("TextBox_responsable_area_piso")
            Dim TextBox_nombre_area_piso As TextBox = pag.FindControl("TextBox_nombre_area_piso")
            Dim DropDownList_tipo_archivo_area_piso As DropDownList = pag.FindControl("DropDownList_tipo_archivo_area_piso")
            Dim UpdatePanel_reg_edit_area_piso As UpdatePanel = pag.FindControl("UpdatePanel_reg_edit_area_piso")
            Dim ModalPopupExtender_edition_reg_edit_area_piso As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_edition_reg_edit_area_piso")
            Dim Label_titulo_area_piso As Label = pag.FindControl("Label_titulo_area_piso")
            Result = Me.Solicita_Datos_estructura_area_archivo(id_area, _
                                                               stru_area)
            If Result <> "YES" Then
                Asigna_datos_interface_area_archivo = Result
                Exit Function
            End If
            TextBox_telefono_area_piso.Text = stru_area.telefono
            TextBox_responsable_area_piso.Text = stru_area.responsable
            TextBox_nombre_area_piso.Text = stru_area.nombre_area
            DropDownList_tipo_archivo_area_piso.Text = stru_area.tipo_archivo
            Label_titulo_area_piso.Text = "Edita área (" & stru_area.nombre_area & ")"
            UpdatePanel_reg_edit_area_piso.Update()
            ModalPopupExtender_edition_reg_edit_area_piso.Show()
            Asigna_datos_interface_area_archivo = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_interface_area_archivo = "Inconsistencia general función Asigna_datos_interface_area_archivo " & ex.Message
        End Try
    End Function
    Function Actualiza_datos_area_archivo(ByVal id_area As Integer, _
                                          ByVal Telefono As String, _
                                          ByVal responsable_area As String, _
                                          ByVal nombre_area As String, _
                                          ByRef node As TreeNode, _
                                          ByRef update As UpdatePanel, _
                                          ByVal tipo_archivo As String) As String
        If tipo_archivo = "" Then
            Actualiza_datos_area_archivo = "Debe seleccionar el tipo de archivo"
            Exit Function
        End If
        If Telefono = "" Then
            Actualiza_datos_area_archivo = "Debe digitar el número telefono"
            Exit Function
        End If
        If responsable_area = "" Then
            Actualiza_datos_area_archivo = "Debe digitar el nombre responsable"
            Exit Function
        End If
        If nombre_area = "" Then
            Actualiza_datos_area_archivo = "Debe digitar el nombre del área"
            Exit Function
        End If
        Dim Result As String = ""
        Dim stru_area As Area_Archivo = Nothing
        Result = Me.Solicita_Datos_estructura_area_archivo(id_area, _
                                                           stru_area)
        If Result <> "YES" Then
            Actualiza_datos_area_archivo = Result
            Exit Function
        End If
        '---Verifica existencia del área en el piso
        Dim id_piso As Integer = 0
        If UCase(nombre_area) <> UCase(stru_area.nombre_area) Then
            '------Verifica existencia area piso
            Result = Me.Retorna_id_piso_archivo_por_id_area(id_area, _
                                                           id_piso)
            If Result <> "YES" Then
                Actualiza_datos_area_archivo = Result
                Exit Function
            End If
            Result = Verifica_Existencia_Area_Piso(id_piso, _
                                                   nombre_area)
            If Result <> "YES" Then
                Actualiza_datos_area_archivo = Result
                Exit Function
            End If
        End If
        Dim sqlactualiza As String = "Update area_archivo set NOMBRE_AREA='" & nombre_area & _
      "',TELEFONO_RESPONSABLE='" & Telefono & "', NOMBRE_RESPONSABLE='" & responsable_area & "'" & _
      " ,TIPO_ARCHIVO='" & tipo_archivo & "' " & _
      " where ID_AREA=" & id_area
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlactualiza
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_datos_area_archivo = "Imposible actualizar piso  : " & sqlactualiza
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Me.Actualiza_Area_Archivo_Treview(nombre_area, _
                                                       id_area, _
                                                       node)
            If Result <> "YES" Then
                Actualiza_datos_area_archivo = "Imposible actualizar en listview  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            update.Update()
            myTrans.Commit()
            Actualiza_datos_area_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_datos_area_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_datos_area_archivo = errorM

        End Try
    End Function
    Function Actualiza_Area_Archivo_Treview(ByVal nombre_area As String, _
                                            ByVal id_area As Integer, _
                                            ByRef nod As TreeNode) As String
        '***************************************************
        'Funcion actualiza nodo area
        'estructura
        'Fecha 2016-02-03
        'Ing :Miguel Angel Urueta Miranda
        '***************************************************
        Try
            nod.Text = "Area: " & nombre_area
            nod.Value = id_area & "|AREA|" & nombre_area
            nod.ToolTip = id_area & "|AREA|" & nombre_area
            nod.ImageUrl = "../workflow/imageneswf/area-light.png"
            Actualiza_Area_Archivo_Treview = "YES"
        Catch ex As Exception
            Actualiza_Area_Archivo_Treview = "Inconsistencia funcion Actualiza_Area_Archivo_Treview " & ex.Message
        End Try
    End Function
    Function Elimina_area_archivo(ByVal id_area As Integer, _
                                  ByVal trenode As TreeNode, _
                                  ByRef treview As TreeView, _
                                  ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Result = Determina_Existencia_Modulo_Area(id_area)
        If Result <> "YES" Then
            Elimina_area_archivo = Result
            Exit Function
        End If
        Dim sql_delete As String = "delete from area_archivo where ID_AREA=" & id_area
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_delete
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_area_archivo = "Imposible eliminar el área  : " & sql_delete
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            Elimina_area_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Elimina_area_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_area_archivo = errorM

        End Try
    End Function
    Function Determina_Existencia_Modulo_Area(ByVal Id_area As Integer) As String
        '*************************************************************
        'Función : Determina la existencia de modulos dentro del area
        'Fecha : 2014-07-01
        'Ing : Miguel Angel Urueta / Modificado para web 2019-02-15
        '*************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from modulo_archivo where AREA_ARCHIVO_ID_AREA=" & Id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("modulo_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Determina_Existencia_Modulo_Area = "Función Determina_Existencia_Modulo_Area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Determina_Existencia_Modulo_Area = "El área que desea eliminar tiene modulos relacionados"
                Exit Function
            Else
                Determina_Existencia_Modulo_Area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_Existencia_Modulo_Area = "Inconsistencia general Funcion Determina_Existencia_Modulo_Area " & ex.Message
        End Try
    End Function
    Function Registrar_modulo_archivo(ByVal nombre_modulo As String, _
                                      ByVal descripcion_modulo As String, _
                                      ByVal seccion_modulo As String, _
                                      ByVal nombre_empresa As String, _
                                      ByVal id_area As Integer, _
                                      ByRef tred_node As TreeNode, _
                                      ByRef update As UpdatePanel) As String
        If nombre_modulo = "" Then
            Registrar_modulo_archivo = "Debe informar el nombre del modulo"
            Exit Function
        End If
        If descripcion_modulo = "" Then
            Registrar_modulo_archivo = "Debe informar la descripción del modulo"
            Exit Function
        End If
        If seccion_modulo = "" Then
            Registrar_modulo_archivo = "Debe informar la sección del modulo"
            Exit Function
        End If
        '------Solicita id empresa
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim Result As String = ""
        Dim id_empresa As Integer = -1
        Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registrar_modulo_archivo = "Imposible listar la identidad de la empresa " & Result
            Exit Function
        End If
        Result = Me.Verifica_Existencia_Modulo_Area(id_area, _
                                                    nombre_modulo)
        If Result <> "YES" Then
            Registrar_modulo_archivo = Result
            Exit Function
        End If
        Dim id_piso As Integer = 0
        Result = Retorna_id_piso_archivo_por_id_area(id_area, _
                                                     id_piso)
        If Result <> "YES" Then
            Registrar_modulo_archivo = Result
            Exit Function
        End If
        Dim id_edificio As Integer = 0
        Result = Me.Retorna_id_edifio_archivo_por_id_piso(id_piso, _
                                                          id_edificio)
        If Result <> "YES" Then
            Registrar_modulo_archivo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_modulo_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_MODULO  from AREA_ARCHIVO where ID_AREA=" & id_area & _
                " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registrar_modulo_archivo = "Imposible Encontrar consecutivo MODULO error  (conexión) "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registrar_modulo_archivo = "Imposible Encontrar consecutivo del modulo en el área"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_modulo As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_modulo = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            consecutivo_modulo = consecutivo_modulo + 1
            '----Crea numero consecutivo unico piso
            Dim codigo_unico As String = "MODULO-" & consecutivo_modulo & "AREA-" & id_area & "ED-" & id_edificio & "EMP-" & id_empresa
            '----Registra piso
            Dim sqlregistra As String = "Insert Into MODULO_ARCHIVO (AREA_ARCHIVO_ID_AREA,CONSECUTIVO_MODULO,SECCION_MODULO" & _
            ",DESCRIPCION_MODULO,CODIGO_MODULO,NOMBRE_MODULO,FECHA_MODULO) values (" & id_area & "," & consecutivo_modulo & ",'" & _
            seccion_modulo & "','" & descripcion_modulo & "','" & codigo_unico & "','" & nombre_modulo & "','" & date1al & "')"
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_modulo_archivo = "Imposible registrar modulo  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id piso
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "UPDATE AREA_ARCHIVO SET CONSECUTIVO_MODULO=" & consecutivo_modulo & "  where ID_AREA=" & id_area
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_modulo_archivo = "Imposible actualiza consecutivo MODULO  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Me.Agregar_Modulo_Archivo_treview(tred_node, _
                                                       objet, _
                                                       nombre_modulo)
            If Result <> "YES" Then
                Registrar_modulo_archivo = "Imposible registrar el modulo en la interface "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            update.Update()
            myTrans.Commit()
            Registrar_modulo_archivo = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registrar_modulo_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registrar_modulo_archivo = errorM
        End Try
    End Function
    Function Verifica_Existencia_Modulo_Area(ByVal Id_area As Integer, _
                                             ByVal nombre_modulo As String) As String
        '***************************************************************************
        'Funcion : Verifica la existencia del modulo  con el parametro
        'id del area y nombre del modulo para evitar la duplicidad de nombre
        'de modulo en cada area
        'Fecha : 2014-07-02
        'Ing :Miguel Angel Urueta Miranda/ Modificado para web 2019-02-15
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from MODULO_ARCHIVO where NOMBRE_MODULO='" & nombre_modulo & _
            "' and AREA_ARCHIVO_ID_AREA=" & Id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("modulo_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Modulo_Area = "Función Verifica_Existencia_Modulo_Area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Modulo_Area = "El nombre del modulo informado se encuentra registrado en el área"
                Exit Function
            Else
                Verifica_Existencia_Modulo_Area = "YES"
                Exit Function
            End If
            
        Catch ex As Exception
            Verifica_Existencia_Modulo_Area = "Inconsistencia general funcion Verifica_Existencia_Area_Piso " & ex.Message
        End Try
    End Function
    Function Agregar_Modulo_Archivo_treview(ByRef nod As TreeNode, _
                                            ByVal id_modulo As Integer, _
                                            ByVal nombre_modulo As String) As String
        '*********************************************************
        'Funcion : agrega modulo archivo treview a la interface
        'Fecha 2016-03-07
        'Ing Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim attrNode As New TreeNode
            attrNode.Text = "Modulo: " & nombre_modulo
            attrNode.Value = id_modulo & "|MODULO|" & nombre_modulo
            attrNode.ToolTip = nombre_modulo
            attrNode.ImageUrl = "../workflow/imageneswf/columns-light.png"
            nod.ChildNodes.Add(attrNode)
            Agregar_Modulo_Archivo_treview = "YES"
        Catch ex As Exception
            Agregar_Modulo_Archivo_treview = "Inconsistencia funcion Agregar_Modulo_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_modulo_archivo(ByVal id_modulo As Integer, _
                                                      ByRef stru_modulo As Modulo_Archivo) As String
        '------------------------------------------------
        'Función : Solicita datos estructura modulos
        'de archivo con el parametro de identificador del
        'modulo
        'Fecha : 2019-02-16
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT ID_MODULO,NOMBRE_MODULO,CODIGO_MODULO,SECCION_MODULO," & _
           "DESCRIPCION_MODULO  from MODULO_ARCHIVO" & _
           " where ID_MODULO=" & id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("MODULO_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_modulo_archivo = "Función Solicita_datos_estructura_modulo_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                stru_modulo.id_modulo = Datset.Tables(0).Rows(0).Item(0)
                stru_modulo.nombre_modulo = Datset.Tables(0).Rows(0).Item(1)
                stru_modulo.codigo_modulo = Datset.Tables(0).Rows(0).Item(2)
                stru_modulo.seccion_modulo = Datset.Tables(0).Rows(0).Item(3)
                stru_modulo.descripcion_modulo = Datset.Tables(0).Rows(0).Item(4)
                Solicita_datos_estructura_modulo_archivo = "YES"
                Exit Function
            Else
                Solicita_datos_estructura_modulo_archivo = "Imposible encontrar datos del area (" & id_modulo & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_modulo_archivo = "Inconsistencia general función Solicita_datos_estructura_modulo_archivo " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_modulo_archivo(ByVal id_modulo As Integer, _
                                                   ByRef pag As Page) As String
        '---------------------------------------------------------------
        'Función : Asigna los datos de la estructura a la interface del 
        'modulo
        'Fecha : 2019-02-16
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Label_title_reg_edit_modulo As Label = pag.FindControl("Label_title_reg_edit_modulo")
            Dim TextBox_reg_edit_modulo_area_nombre As TextBox = pag.FindControl("TextBox_reg_edit_modulo_area_nombre")
            Dim TextBox_reg_edit_modulo_area_descripcion As TextBox = pag.FindControl("TextBox_reg_edit_modulo_area_descripcion")
            Dim TextBox_reg_edit_modulo_area_seccion As TextBox = pag.FindControl("TextBox_reg_edit_modulo_area_seccion")
            Dim UpdatePanel_reg_edit_modulo_area As UpdatePanel = pag.FindControl("UpdatePanel_reg_edit_modulo_area")
            Dim ModalPopupExtender_edition_reg_edit_modulo_area As AjaxControlToolkit.ModalPopupExtender = _
                pag.FindControl("ModalPopupExtender_edition_reg_edit_modulo_area")
            Dim Result As String = ""
            Dim stru_modulo As Modulo_Archivo = Nothing
            Result = Me.Solicita_datos_estructura_modulo_archivo(id_modulo, _
                                                                 stru_modulo)
            If Result <> "YES" Then
                Asigna_datos_interface_modulo_archivo = Result
                Exit Function
            End If
            Label_title_reg_edit_modulo.Text = "Editar modulo (" & stru_modulo.nombre_modulo & ")"
            TextBox_reg_edit_modulo_area_nombre.Text = stru_modulo.nombre_modulo
            TextBox_reg_edit_modulo_area_descripcion.Text = stru_modulo.descripcion_modulo
            TextBox_reg_edit_modulo_area_seccion.Text = stru_modulo.seccion_modulo
            UpdatePanel_reg_edit_modulo_area.Update()
            ModalPopupExtender_edition_reg_edit_modulo_area.Show()
            Asigna_datos_interface_modulo_archivo = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_interface_modulo_archivo = "Inconsistencia general función Asigna_datos_interface_modulo_archivo " & ex.Message
        End Try
    End Function
    Function Actualiza_datos_modulo_archivo(ByVal id_modulo As Integer, _
                                            ByVal nombre_modulo As String, _
                                            ByVal descripcion_modulo As String, _
                                            ByVal seccion_modulo As String, _
                                            ByVal nombre_empresa As String, _
                                            ByRef tred_node As TreeNode, _
                                            ByRef update As UpdatePanel) As String
        If nombre_modulo = "" Then
            Actualiza_datos_modulo_archivo = "Debe informar el nombre del modulo"
            Exit Function
        End If
        If descripcion_modulo = "" Then
            Actualiza_datos_modulo_archivo = "Debe informar la descripción del modulo"
            Exit Function
        End If
        If seccion_modulo = "" Then
            Actualiza_datos_modulo_archivo = "Debe informar la sección del modulo"
            Exit Function
        End If
        Dim Result As String = ""
        Dim stru_modulo As Modulo_Archivo = Nothing
        Result = Me.Solicita_datos_estructura_modulo_archivo(id_modulo, _
                                                             stru_modulo)
        If Result <> "YES" Then
            Actualiza_datos_modulo_archivo = Result
            Exit Function
        End If
        '------------------------------------------------
        'Verifica existencia del modulo en el area
        'si hay cambios
        '------------------------------------------------
        Dim id_area As Integer = 0
        If nombre_modulo <> stru_modulo.nombre_modulo Then
            Result = Me.Retorna_id_area_archivo_por_id_modulo(id_modulo, _
                                                              id_area)
            If Result <> "YES" Then
                Actualiza_datos_modulo_archivo = Result
                Exit Function
            End If
            Result = Me.Verifica_Existencia_Modulo_Area(id_area, _
                                                   nombre_modulo)
            If Result <> "YES" Then
                Actualiza_datos_modulo_archivo = Result
                Exit Function
            End If
        End If
        Dim sqlactualiza As String = "Update modulo_archivo set NOMBRE_MODULO='" & nombre_modulo & _
        "',DESCRIPCION_MODULO='" & descripcion_modulo & "', SECCION_MODULO='" & seccion_modulo & "' where ID_MODULO=" & id_modulo
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlactualiza
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_datos_modulo_archivo = "Imposible actualizar piso  : " & sqlactualiza
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Me.Actualiza_Modulo_Archivo_treview(tred_node, _
                                                        id_modulo, _
                                                        nombre_modulo)
            If Result <> "YES" Then
                Actualiza_datos_modulo_archivo = "Imposible actualizar en listview  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            update.Update()
            myTrans.Commit()
            Actualiza_datos_modulo_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_datos_modulo_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_datos_modulo_archivo = errorM

        End Try
    End Function
    Function Actualiza_Modulo_Archivo_treview(ByRef attrNode As TreeNode, _
                                             ByVal id_modulo As Integer, _
                                             ByVal nombre_modulo As String) As String
        '*********************************************************
        'Funcion : actualiza modulo archivo treview a la interface
        'Fecha 2019-02-16
        'Ing Miguel Angel Urueta Miranda
        '*********************************************************
        Try

            attrNode.Text = "Modulo: " & nombre_modulo
            attrNode.Value = id_modulo & "|MODULO|" & nombre_modulo
            attrNode.ToolTip = nombre_modulo
            attrNode.ImageUrl = "../workflow/imageneswf/columns-light.png"
            Actualiza_Modulo_Archivo_treview = "YES"
        Catch ex As Exception
            Actualiza_Modulo_Archivo_treview = "Inconsistencia funcion Actualiza_Modulo_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Elimina_modulo_archivo(ByVal id_modulo As Integer, _
                                    ByVal trenode As TreeNode, _
                                    ByRef treview As TreeView, _
                                    ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Result = Verifica_Existencia_Estante_Modulo(id_modulo)
        If Result <> "YES" Then
            Elimina_modulo_archivo = Result
            Exit Function
        End If
        Dim sql_delete As String = "delete from modulo_archivo where ID_MODULO=" & id_modulo
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_delete
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_modulo_archivo = "Imposible eliminar el modulo  : " & sql_delete
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            Elimina_modulo_archivo = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Elimina_modulo_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_modulo_archivo = errorM

        End Try
    End Function
    Function Verifica_Existencia_Estante_Modulo(ByVal Id_modulo As Integer) As String
        '************************************************************
        'Funcion : Verifica la existencia de estantes dentro del
        'modulo
        'Fecha : 2019-02-16
        'Ing : Miguel Angel Urueta
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from estante_archivo where MODULO_ARCHIVO_ID_MODULO=" & Id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("estante_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Estante_Modulo = "Función Verifica_Existencia_Estante_Modulo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Estante_Modulo = "El modulo que desea eliminar tiene estantes relacionados"
                Exit Function
            Else
                Verifica_Existencia_Estante_Modulo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Estante_Modulo = "Inconsistencia general Funcion Verifica_Existencia_Estante_Modulo " & ex.Message
        End Try
    End Function
    Function Registra_estante_archivo(ByVal id_modulo As Integer, _
                                      ByVal nombre_empresa As String, _
                                      ByRef refnode As TreeNode, _
                                      ByRef update As UpdatePanel) As String
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim Result As String = ""
        Dim id_empresa As Integer = -1
        Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registra_estante_archivo = "Imposible listar la identidad de la empresa " & Result
            Exit Function
        End If
        Dim id_area As Integer = 0
        Result = Me.Retorna_id_area_archivo_por_id_modulo(id_modulo, _
                                                          id_area)
        If Result <> "YES" Then
            Registra_estante_archivo = Result
            Exit Function
        End If
        Dim id_piso As Integer = 0
        Result = Retorna_id_piso_archivo_por_id_area(id_area, _
                                                     id_piso)
        If Result <> "YES" Then
            Registra_estante_archivo = Result
            Exit Function
        End If
        Dim id_edificio As Integer = 0
        Result = Me.Retorna_id_edifio_archivo_por_id_piso(id_piso, _
                                                          id_edificio)
        If Result <> "YES" Then
            Registra_estante_archivo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registra_estante_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_ESTANTE  from MODULO_ARCHIVO where ID_MODULO=" & id_modulo & _
                " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registra_estante_archivo = "Imposible Encontrar consecutivo ESTANTE error de conexion "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registra_estante_archivo = "Imposible Encontrar consecutivo del estante en el modulo"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_estante As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_estante = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            '----Incrementa consecutivo
            consecutivo_estante = consecutivo_estante + 1
            Dim Codigo_Letra As String = ""
            Result = Retorna_Codigo_Alfanumerico(consecutivo_estante, _
                                                 Codigo_Letra)
            If Result <> "YES" Then
                Registra_estante_archivo = "Imposible obtener codigo letra " & Result
                Exit Function
            End If
            '----Crea numero consecutivo unico piso
            Dim codigo_unico As String = "ESTANTE-" & consecutivo_estante & "MODULO-" & id_modulo & "AREA-" & id_area & "ED-" & id_edificio & "EMP-" & id_empresa
            '------Verifica existencia
            Result = Verifica_Existencia_Estante_Modulo(id_modulo, _
                                                        codigo_unico)
            If Result <> "YES" Then     
                Registra_estante_archivo = Result
                Exit Function
            End If
            '----Registra piso
            Dim sqlregistra As String = "Insert Into ESTANTE_ARCHIVO (MODULO_ARCHIVO_ID_MODULO,CONSECUTIVO_ESTANTE,CODIGO_LITERAL" & _
            ",CODIGO_UNICO,DESCRIPCION_ASTANTE,FECHA_CREACION) values (" & id_modulo & "," & consecutivo_estante & ",'" & _
            Codigo_Letra & "','" & codigo_unico & "','" & "NA" & "','" & date1al & "')"
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_estante_archivo = "Imposible registrar estante  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id piso
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '-------Actualiza consecutivo estante en area
            Dim updatconsecutivo As String = "UPDATE MODULO_ARCHIVO SET CONSECUTIVO_ESTANTE=" & consecutivo_estante & "  where ID_MODULO=" & id_modulo
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_estante_archivo = "Imposible actualiza consecutivo estante  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Agregar_Estante_Archivo_treview(refnode, _
                                                     Codigo_Letra, _
                                                     objet, _
                                                     codigo_unico)
            If Result <> "YES" Then
                Registra_estante_archivo = "Imposible registrar el estante en la interface "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            update.Update()
            myTrans.Commit()
            Registra_estante_archivo = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registra_estante_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_estante_archivo = errorM
        End Try
    End Function
    Function Retorna_Codigo_Alfanumerico(ByVal Id_codigo As Integer, _
                                         ByRef codigo_alfanumerico As String) As String
        Try
            Dim refid_codigo As Integer = Id_codigo - 1
            codigo_alfanumerico = Id_codigo & "-" & Mid$("ABCDEFGHYJKLMNOPQRSTUVWXYZ", (refid_codigo Mod 26) + 1, 1)
            Retorna_Codigo_Alfanumerico = "YES"
        Catch ex As Exception
            Retorna_Codigo_Alfanumerico = "Inconsistencia general funcio Retorna_Codigo_Alfanumerico " & ex.Message
        End Try
    End Function
    Function Verifica_Existencia_Estante_Modulo(ByVal Id_modulo As Integer, _
                                                ByVal codigo_unico As String) As String
        '***************************************************************************
        'Funcion : Verifica la existencia del estante  con el parametro
        'id del estante y codigo unico entrepaño para evitar la duplicidad de codigo
        'de modulo en cada area
        'Fecha : 2014-07-07 Modificado web 2019-02-16
        'Ing :Miguel Angel Urueta Miranda
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from ESTANTE_ARCHIVO where CODIGO_UNICO='" & codigo_unico & _
            "' and MODULO_ARCHIVO_ID_MODULO=" & Id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ESTANTE_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Estante_Modulo = "Función Verifica_Existencia_Estante_Modulo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Estante_Modulo = "El estante informado se encuentra registrado "
                Exit Function
            Else
                Verifica_Existencia_Estante_Modulo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Estante_Modulo = "Inconsistencia general funcion Verifica_Existencia_Estante_Modulo " & ex.Message
        End Try
    End Function
    Function Agregar_Estante_Archivo_treview(ByRef nod As TreeNode, _
                                             ByVal codigo_estante As String, _
                                             ByVal id_estante As Integer, _
                                             ByVal codigo_unico As String) As String
        '*****************************************
        'Funcion lista estante archivo en treview 
        'Fecha 2016-03-07
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************
        Try
            Dim attrNode As New TreeNode
            attrNode.Text = "Estante : " & codigo_estante
            attrNode.Value = id_estante & "|ESTANTE|" & codigo_estante
            attrNode.ToolTip = codigo_unico
            attrNode.ImageUrl = "../workflow/imageneswf/window-maximize-light.png"
            nod.ChildNodes.Add(attrNode)
            Agregar_Estante_Archivo_treview = "YES"
        Catch ex As Exception
            Agregar_Estante_Archivo_treview = "Inconsistencia función Agregar_Estante_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Eliminar_estante_archivo(ByVal id_estante As Integer, _
                                      ByVal trenode As TreeNode, _
                                      ByRef treview As TreeView, _
                                      ByRef update As UpdatePanel) As String

        Dim Result As String = ""
        Result = Me.Determina_Existencia_Entrepaño_Estante(id_estante)
        If Result <> "YES" Then
            Eliminar_estante_archivo = Result
            Exit Function
        End If
        Dim id_modulo As Integer = 0
        Result = Me.Retorna_id_modulo_estante_archivo(id_estante, _
                                                    id_modulo)
        If Result <> "YES" Then
            Eliminar_estante_archivo = Result
            Exit Function
        End If
        Dim consecutivo_modulo_estante As Integer = 0
        Result = Me.Solicita_consecutivo_estante_modulo(id_modulo, _
                                                        consecutivo_modulo_estante)
        If Result <> "YES" Then
            Eliminar_estante_archivo = Result
            Exit Function
        End If
        Dim consecutivo_estante As Integer = 0
        Result = Me.Solicita_consecutivo_estante(id_estante, _
                                                 consecutivo_estante)
        If Result <> "YES" Then
            Eliminar_estante_archivo = Result
            Exit Function
        End If
        If consecutivo_estante < consecutivo_modulo_estante Then
            Eliminar_estante_archivo = "No se puede eliminar un estante intermedio e inicial, empiece por eliminar desde el último"
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_ESTANTE  from MODULO_ARCHIVO where ID_MODULO=" & id_modulo 
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Eliminar_estante_archivo = "Imposible Encontrar consecutivo ESTANTE error de conexion "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Eliminar_estante_archivo = "Imposible Encontrar consecutivo del estante en el modulo"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_estante_ As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_estante_ = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            '----Decrementa consecutivo
            consecutivo_estante_ = consecutivo_estante_ - 1

            '----Elimina estante
            Dim sqlregistra As String = "delete from estante_archivo where ID_ESTANTE=" & id_estante
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_estante_archivo = "Imposible eliminar estante  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------Actualiza consecutivo estante en area
            Dim updatconsecutivo As String = "UPDATE MODULO_ARCHIVO SET CONSECUTIVO_ESTANTE=" & consecutivo_estante_ & "  where ID_MODULO=" & id_modulo
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_estante_archivo = "Imposible actualiza consecutivo estante  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
             treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            Eliminar_estante_archivo = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_estante_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_estante_archivo = errorM
        End Try
    End Function
    Function Determina_Existencia_Entrepaño_Estante(ByVal Id_estante As Integer) As String
        '************************************************************
        'Funcion : Determina la existencia de entrepaños dentro del
        'estante
        'Fecha : 2014-07-07/ Modificado web 2019-02-16
        'Ing : Miguel Angel Urueta
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from ENTRE_PAÑO where ESTANTE_ARCHIVO_ID_ESTANTE=" & Id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ESTANTE_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Determina_Existencia_Entrepaño_Estante = "Función Determina_Existencia_Entrepaño_Estante dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Determina_Existencia_Entrepaño_Estante = "El estante que desea eliminar tiene entrepaños relacionados"
                Exit Function
            Else
                Determina_Existencia_Entrepaño_Estante = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_Existencia_Entrepaño_Estante = "Inconsistencia general Funcion Determina_Existencia_Estante_Modulo " & ex.Message
        End Try
    End Function
    Function Solicita_consecutivo_estante_modulo(ByVal id_modulo As Integer, _
                                                 ByRef consecutivo As Integer) As String
        '----------------------------------------------------
        'Función : Solicita consecutivo estante en el modulo
        'enviado como paramentro
        'Fecha : 2019-02-16
        'Ing : Miguel Angel Urueta
        '-----------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select CONSECUTIVO_ESTANTE  from MODULO_ARCHIVO where ID_MODULO=" & id_modulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("MODULO_ARCHIVO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_consecutivo_estante_modulo = "Función  Solicita_consecutivo_estante_modulo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                consecutivo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_consecutivo_estante_modulo = "YES"
                Exit Function
            Else
                Solicita_consecutivo_estante_modulo = "Imposible encontrar del estante en el modulo (" & id_modulo & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_consecutivo_estante_modulo = "Inconsistencia general función Solicita_consecutivo_estante_modulo " & ex.Message
        End Try
    End Function
    Function Solicita_consecutivo_estante(ByVal id_estante As Integer, _
                                          ByRef consecutivo_estante As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select CONSECUTIVO_ESTANTE  from estante_archivo where ID_ESTANTE=" & id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("estante_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_consecutivo_estante = "Función  Solicita_consecutivo_estante dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                consecutivo_estante = Datset.Tables(0).Rows(0).Item(0)
                Solicita_consecutivo_estante = "YES"
                Exit Function
            Else
                Solicita_consecutivo_estante = "Imposible encontrar el consecutivo del estante (" & id_estante & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_consecutivo_estante = "Inconsistencia general función Solicita_consecutivo_estante " & ex.Message
        End Try
    End Function
    Function Solicita_consecutivo_estrepano_estante(ByVal id_estante As Integer, _
                                                    ByRef consecutivo_entrepano_estante As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select CONSECUTIVO_ENTREPAÑO  from estante_archivo where ID_ESTANTE=" & id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_pano")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_consecutivo_estrepano_estante = "Función Solicita_consecutivo_estrepano_estante dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                consecutivo_entrepano_estante = Datset.Tables(0).Rows(0).Item(0)
                Solicita_consecutivo_estrepano_estante = "YES"
                Exit Function
            Else
                Solicita_consecutivo_estrepano_estante = "Imposible encontrar el consecutivo del entrepano (" & id_estante & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_consecutivo_estrepano_estante = "Inconsistencia general función Solicita_consecutivo_estrepano_estante " & ex.Message
        End Try
    End Function
    Function Solicita_consecutivo_estrepano(ByVal id_entrepano As Integer, _
                                            ByRef consecutivo_estrepano As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select CONSECUTIVO_ENTREPAÑO  from entre_paño where ID_ENTREPAÑO=" & id_entrepano
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_pano")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_consecutivo_estrepano = "Función Solicita_consecutivo_estrepano dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                consecutivo_estrepano = Datset.Tables(0).Rows(0).Item(0)
                Solicita_consecutivo_estrepano = "YES"
                Exit Function
            Else
                Solicita_consecutivo_estrepano = "Imposible encontrar el consecutivo del entrepano (" & id_entrepano & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_consecutivo_estrepano = "Inconsistencia general función Solicita_consecutivo_estrepano " & ex.Message
        End Try
    End Function
    Function Registra_entrepano_archivo(ByVal id_estante As Integer, _
                                        ByVal nombre_empresa As String, _
                                        ByVal nuemero_unidades As Integer, _
                                        ByRef refnode As TreeNode, _
                                        ByRef update As UpdatePanel) As String
        Dim Reflasgest As New ClassAdmonEmpresa
        Dim Result As String = ""
        Dim id_empresa As Integer = -1
        Result = Reflasgest.Retorna_Id_Emprea(nombre_empresa, _
                                              id_empresa)
        If Result <> "YES" Then
            Registra_entrepano_archivo = "Imposible listar la identidad de la empresa " & Result
            Exit Function
        End If
        Dim id_modulo As Integer = 0
        Result = Me.Retorna_id_modulo_estante_archivo(id_estante, _
                                                      id_modulo)
        If Result <> "YES" Then
            Registra_entrepano_archivo = "Imposible listar el modulo del estante " & Result
            Exit Function
        End If
        Dim id_area As Integer = 0
        Result = Me.Retorna_id_area_archivo_por_id_modulo(id_modulo, _
                                                          id_area)
        If Result <> "YES" Then
            Registra_entrepano_archivo = Result
            Exit Function
        End If
        Dim id_piso As Integer = 0
        Result = Retorna_id_piso_archivo_por_id_area(id_area, _
                                                     id_piso)
        If Result <> "YES" Then
            Registra_entrepano_archivo = Result
            Exit Function
        End If
        Dim id_edificio As Integer = 0
        Result = Me.Retorna_id_edifio_archivo_por_id_piso(id_piso, _
                                                          id_edificio)
        If Result <> "YES" Then
            Registra_entrepano_archivo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registra_entrepano_archivo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_ENTREPAÑO  from ESTANTE_ARCHIVO where ID_ESTANTE=" & id_estante & _
                " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registra_entrepano_archivo = "Imposible Encontrar consecutivo de entrepano error de conexion "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registra_entrepano_archivo = "Imposible Encontrar consecutivo del entrepano en el estante"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_entrepaño As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_entrepaño = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            '----Incrementa consecutivo
            consecutivo_entrepaño = consecutivo_entrepaño + 1
            '----Retorna codigo letra
            Dim Codigo_Letra As String = ""
            Result = Retorna_Codigo_Alfanumerico(consecutivo_entrepaño, Codigo_Letra)
            If Result <> "YES" Then
                mySqldatReader.Close()
                myConnection.Close()
                Registra_entrepano_archivo = "Imposible obtener codigo letra " & Result
                Exit Function
            End If
            '----Crea numero consecutivo unico piso
            Dim codigo_unico As String = "FILA-" & consecutivo_entrepaño & "ESTAN-" & id_estante & "MODULO-" & id_modulo & "AREA-" & id_area & "ED-" & id_edificio & "EMP-" & id_empresa
            '------Verifica existencia
            Result = Verifica_Existencia_Entrepaño_Estante(id_estante, codigo_unico)
            If Result <> "YES" Then
                mySqldatReader.Close()
                myConnection.Close()
                Registra_entrepano_archivo = Result
                Exit Function
            End If
            '----Registra piso
            Dim sqlregistra As String = "Insert Into ENTRE_PAÑO (ESTANTE_ARCHIVO_ID_ESTANTE,CONSECUTIVO_ENTREPAÑO,CODIGO_CORTO" & _
            ",CODIGO_UNICO,NUMERO_UNIDADES_PERMITIDAS,FECHA_CREACION) values (" & id_estante & "," & consecutivo_entrepaño & ",'" & _
            Codigo_Letra & "','" & codigo_unico & "','" & nuemero_unidades & "','" & date1al & "')"
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_entrepano_archivo = "Imposible registrar entrepaño  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------------------
            'Consulta id piso
            '------------------------------------------------------------
            Dim objet As Object = myCommand.LastInsertedId
            '-------Actualiza consecutivo piso
            Dim updatconsecutivo As String = "UPDATE ESTANTE_ARCHIVO SET CONSECUTIVO_ENTREPAÑO=" & consecutivo_entrepaño & "  where ID_ESTANTE=" & id_estante
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registra_entrepano_archivo = "Imposible actualiza consecutivo ENTREPAÑO  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------Registra estante en la interface
            Result = Agregar_Entrepaño_Archivo_treview(refnode, _
                                                       objet, _
                                                       Codigo_Letra, _
                                                       codigo_unico)
            If Result <> "YES" Then
                Registra_entrepano_archivo = "Imposible Registrar entrepaño en la interface  : " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            update.Update()
            Registra_entrepano_archivo = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registra_entrepano_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_entrepano_archivo = errorM
        End Try
    End Function
    Function Verifica_Existencia_Entrepaño_Estante(ByVal Id_estante As Integer, _
                                                   ByVal codigo_unico As String) As String
        '***************************************************************************
        'Funcion : Verifica la existencia del entrepaño  con el parametro
        'id del estante y codigo unico entrepaño para evitar la duplicidad de codigo
        'de modulo en cada area
        'Fecha : 2014-07-07
        'Ing :Miguel Angel Urueta Miranda
        'Modificado para web 2019-02-25 Ing Miguel Angel Urueta Miranda
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from ENTRE_PAÑO where CODIGO_UNICO='" & codigo_unico & _
            "' and ESTANTE_ARCHIVO_ID_ESTANTE=" & Id_estante
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ENTRE_PAÑO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Entrepaño_Estante = "Función Verifica_Existencia_Entrepaño_Estante dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_Existencia_Entrepaño_Estante = "El entrepaño informado se encuentra registrado"
                Exit Function
            Else
                Verifica_Existencia_Entrepaño_Estante = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Entrepaño_Estante = "Inconsistencia general funcion Verifica_Existencia_Entrepaño_Estante " & ex.Message
        End Try
    End Function
    Function Agregar_Entrepaño_Archivo_treview(ByRef nod As TreeNode, _
                                               ByVal id_entrepaño As Integer, _
                                               ByVal codigo_corto As String, _
                                               ByVal codigo_unico As String) As String
        '*****************************************
        'Funcion lista estante archivo en treview 
        'Fecha 2016-03-07
        'Ing : Miguel Angel Urueta Miranda
        'Modificado para web 2019-02-25
        'Ing _Miguel Angel Urueta Miranda
        '*****************************************
        Try
            Dim attrNode As New TreeNode
            attrNode.Text = "Entrepaño : " & codigo_corto
            attrNode.Value = id_entrepaño & "|ENTREPAÑO|" & codigo_corto
            attrNode.ToolTip = codigo_unico
            attrNode.ImageUrl = "../workflow/imageneswf/rectangle-wide-light-pano.png"
            nod.ChildNodes.Add(attrNode)
            Agregar_Entrepaño_Archivo_treview = "YES"
        Catch ex As Exception
            Agregar_Entrepaño_Archivo_treview = "Inconsistencia función Agregar_Entrepaño_Archivo_treview " & ex.Message
        End Try
    End Function
    Function Agigna_datos_interface_entepano(ByVal id_entre_pano As Integer, _
                                              ByRef page As Page) As String
        '---------------------------------------------
        'Función : Asigna datos a las interface editar
        'nuemero unidaddes contenidas entrpaño
        'Fecha : 2019-01-31
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Dim estru_piso As Piso_Archivo
            Dim Result As String = ""
            Dim UpdatePanel_reg_edit_entrepano As UpdatePanel = page.FindControl("UpdatePanel_reg_edit_entrepano")
            Dim Label_title_reg_edit_entrepano As Label = page.FindControl("Label_title_reg_edit_entrepano")
            Dim Label_reg_edit_entrepano_numero As Label = page.FindControl("Label_reg_edit_entrepano_numero")
            Dim DropDownList_reg_edit_entrepano_numero As DropDownList = page.FindControl("DropDownList_reg_edit_entrepano_numero")
            Dim DropDownList_reg_edit_entrepano_numero_unidades As DropDownList = page.FindControl("DropDownList_reg_edit_entrepano_numero_unidades")
            Dim Label_reg_edit_entrepano_numero_unidades As Label = page.FindControl("Label_reg_edit_entrepano_numero_unidades")
            If Label_reg_edit_entrepano_numero_unidades Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (Label_reg_edit_entrepano_numero_unidades)"
                Exit Function
            End If
            If UpdatePanel_reg_edit_entrepano Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (UpdatePanel_reg_edit_entrepano)"
                Exit Function
            End If
            If Label_title_reg_edit_entrepano Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (Label_title_reg_edit_entrepano)"
                Exit Function
            End If
            If Label_reg_edit_entrepano_numero Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (Label_reg_edit_entrepano_numero)"
                Exit Function
            End If
            If DropDownList_reg_edit_entrepano_numero Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (DropDownList_reg_edit_entrepano_numero)"
                Exit Function
            End If
            If DropDownList_reg_edit_entrepano_numero_unidades Is Nothing Then
                Agigna_datos_interface_entepano = "Imposible encontrar el control (DropDownList_reg_edit_entrepano_numero_unidades)"
                Exit Function
            End If
            Dim numero_unidades As Integer = 0
            Result = Me.Solicita_numnero_unidades_permitidas_entrepano(id_entre_pano, _
                                                                       numero_unidades)
            If Result <> "YES" Then
                Agigna_datos_interface_entepano = Result
                Exit Function
            End If
            For i As Integer = 0 To DropDownList_reg_edit_entrepano_numero_unidades.Items.Count - 1
                If Val(DropDownList_reg_edit_entrepano_numero_unidades.Items(i).Text) = numero_unidades Then
                    DropDownList_reg_edit_entrepano_numero_unidades.Text = numero_unidades
                    Exit For
                End If
            Next
            DropDownList_reg_edit_entrepano_numero.Visible = False
            Label_reg_edit_entrepano_numero.Visible = False
            Label_reg_edit_entrepano_numero_unidades.Visible = True
            DropDownList_reg_edit_entrepano_numero_unidades.Visible = True
            Label_title_reg_edit_entrepano.Text = "Edita entrepaño"
            UpdatePanel_reg_edit_entrepano.Update()
            Agigna_datos_interface_entepano = "YES"
        Catch ex As Exception
            Agigna_datos_interface_entepano = "Inconsistencia general función Agigna_datos_interface_entepano " & ex.Message
        End Try
    End Function
    Function Solicita_numnero_unidades_permitidas_entrepano(ByVal Id_entrepano As Integer, _
                                                            ByRef numero_unidades As String) As String
        '***************************************************************************
        'Funcion : Reotorna numero unidades permitidas entre apaño
        'Fecha : 2019-02-25
        'Ing :Miguel Angel Urueta Miranda
        '***************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select NUMERO_UNIDADES_PERMITIDAS from ENTRE_PAÑO where ID_ENTREPAÑO=" & Id_entrepano
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ENTRE_PANO")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numnero_unidades_permitidas_entrepano = "Función Solicita_numnero_unidades_permitidas_entrepano dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_numnero_unidades_permitidas_entrepano = "Imposible encontrar el numero de unidades permitidas entrepano " & Id_entrepano
                Exit Function
            Else
                numero_unidades = Datset.Tables(0).Rows(0).Item(0)
                Solicita_numnero_unidades_permitidas_entrepano = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numnero_unidades_permitidas_entrepano = "Inconsistencia general funcion Verifica_Existencia_Entrepaño_Estante " & ex.Message
        End Try
    End Function
    Function Actualiza_numero_unidades_permitidas(ByVal id_entrepano As Integer, _
                                                  ByVal numero_unidades As Integer) As String
        Try
            Dim Parametro_delete As String = "update ENTRE_PAÑO set NUMERO_UNIDADES_PERMITIDAS=" & numero_unidades & " where ID_ENTREPAÑO=" & id_entrepano
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref.SELECTION_INSERT_COMMAND(Parametro_delete)
            If Result <> "YES" Then
                Actualiza_numero_unidades_permitidas = "Función Actualiza_numero_unidades_permitidas dice " & Result
                Exit Function
            Else
                Actualiza_numero_unidades_permitidas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_numero_unidades_permitidas = "Inconistencia general función Actualiza_numero_unidades_permitidas " & ex.Message
        End Try
    End Function
    Function Determina_Existencia_Unidadconservacion_Entrepaño(ByVal Id_entrepaño As Integer) As String
        '************************************************************
        'Funcion : Determina la existencia de unidades de 
        'de conservacion dentro del estante
        'Fecha : 2014-07-07
        'Ing : Miguel Angel Urueta
        'Función : Modificada para web 2019-02-25
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from UNIDAD_CONSERVACION where ENTRE_PAÑO_ID_ENTREPAÑO=" & Id_entrepaño
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("UNIDAD_CONSERVACION")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Determina_Existencia_Unidadconservacion_Entrepaño = "Función Verifica_Existencia_Entrepaño_Estante dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Determina_Existencia_Unidadconservacion_Entrepaño = "El entrepaño que desea eliminar tiene unidades de conservación relacionadas"
                Exit Function
            Else
                Determina_Existencia_Unidadconservacion_Entrepaño = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_Existencia_Unidadconservacion_Entrepaño = "Inconsistencia general Funcion Determina_Existencia_Unidadconservacion_Entrepaño " & ex.Message
        End Try
    End Function
    Function Elimina_entrepano_archivo(ByVal id_entrepano As Integer, _
                                       ByVal trenode As TreeNode, _
                                       ByRef treview As TreeView, _
                                       ByRef update As UpdatePanel) As String
        Dim Result As String = ""
        Result = Me.Determina_Existencia_Unidadconservacion_Entrepaño(id_entrepano)
        If Result <> "YES" Then
            Elimina_entrepano_archivo = Result
            Exit Function
        End If
        Dim id_estante As Integer = 0
        Result = Me.Retorna_Id_Estante_por_entrepaño(id_entrepano, _
                                                     id_estante)
        If Result <> "YES" Then
            Elimina_entrepano_archivo = Result
            Exit Function
        End If
        Dim consecutivo_entrepano_estante As Integer = 0
        Dim consecutivo_entrepano As Integer = 0
        Result = Me.Solicita_consecutivo_estrepano_estante(id_estante, _
                                                           consecutivo_entrepano_estante)
        If Result <> "YES" Then
            Elimina_entrepano_archivo = Result
            Exit Function
        End If
        Result = Me.Solicita_consecutivo_estrepano(id_entrepano, _
                                                   consecutivo_entrepano)
        If Result <> "YES" Then
            Elimina_entrepano_archivo = Result
            Exit Function
        End If
        If consecutivo_entrepano < consecutivo_entrepano_estante Then
            Elimina_entrepano_archivo = "No se puede eliminar un estrepano intermedio e inicial, empiece por eliminar desde el último"
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_ENTREPAÑO  from ESTANTE_ARCHIVO where ID_ESTANTE=" & id_estante & _
                " for update"
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Elimina_entrepano_archivo = "Imposible Encontrar consecutivo ESTANTE error de conexion "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Elimina_entrepano_archivo = "Imposible Encontrar consecutivo del entrepaño en el estatante (" & id_estante & ")"
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_entrepano_ As Integer = -2
            If mySqldatReader.HasRows = True Then
                mySqldatReader.Read()
                consecutivo_entrepano_ = mySqldatReader.Item(0)
                mySqldatReader.Close()
            End If
            '----Decrementa consecutivo
            consecutivo_entrepano_ = consecutivo_entrepano_ - 1

            '----Elimina estante
            Dim sqlregistra As String = "delete from entre_paño where ID_ENTREPAÑO=" & id_entrepano
            myCommand.CommandText = sqlregistra
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_entrepano_archivo = "Imposible eliminar entrepaño  : " & sqlregistra
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------Actualiza consecutivo estante en area
            Dim updatconsecutivo As String = "UPDATE ESTANTE_ARCHIVO SET CONSECUTIVO_ENTREPAÑO=" & consecutivo_entrepano_ & "  where ID_ESTANTE=" & id_estante
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_entrepano_archivo = "Imposible actualiza consecutivo estante  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            treview.Nodes.Remove(treview.SelectedNode)
            Dim sNodo As TreeNode = treview.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            Elimina_entrepano_archivo = "YES"
            Exit Function
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Elimina_entrepano_archivo = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_entrepano_archivo = errorM
        End Try
    End Function
End Class
