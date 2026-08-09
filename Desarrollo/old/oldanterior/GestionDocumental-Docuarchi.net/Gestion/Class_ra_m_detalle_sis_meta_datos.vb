Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Public Structure stru_detalle_sis_meta_dato
    Dim id_detalle_sistema_meta_datos As Integer
    Dim nombre_meta_dato As String
    Dim tipo_sistema_meta_datos As Integer
    Dim tipo_meta_dato As String
    Dim estru_xml_meta_dato As String
    Dim estado_obliga_torio As String
    Dim descripcion_meta_dato As String
    Dim tipo_datos_meta_datos As String
    Dim longitud_datos_meta_datos As Integer
    Dim valores_meta_dato As String
    Dim Comentario_meta_datos As String
    Dim emula_legislacion_meta_dato As String
    Dim NUMERACION As Integer
    Dim VALOR_AUTO_POBLADO As String
    Dim EQUIVALENCIA_AUTO_POBLADO As String
    Dim ESTANDAR As String
    Dim AUTO_PLOBLADO As Integer
    Dim value As String
    Dim ERROR_SERVICE As String
    Dim ESTADO_VISIBLE_METADATO As Integer
    Dim ESTADO_FIRMA_DIGITAL As String
    Dim TIPO_METADATO As Integer
End Structure
Public Class Class_ra_m_detalle_sis_meta_datos_
    Private m_id_detalle_sistema_meta_datos As String
    Public Property id_detalle_sistema_meta_datos() As String
        Get
            Return m_id_detalle_sistema_meta_datos
        End Get
        Set(value As String)
            m_id_detalle_sistema_meta_datos = value
        End Set
    End Property
    Private m_nombre_meta_dato As String
    Public Property nombre_meta_dato() As String
        Get
            Return m_nombre_meta_dato
        End Get
        Set(value As String)
            m_nombre_meta_dato = value
        End Set
    End Property
    Private m_nombre_campo_retorno_meta_dato As String
    Public Property nombre_campo_retorno_meta_dato() As String
        Get
            Return m_nombre_campo_retorno_meta_dato
        End Get
        Set(value As String)
            m_nombre_campo_retorno_meta_dato = value
        End Set
    End Property
    Private m_descripcion_error_campo_meta_dato As String
    Public Property descripcion_error_campo_meta_dato() As String
        Get
            Return m_descripcion_error_campo_meta_dato
        End Get
        Set(value As String)
            m_descripcion_error_campo_meta_dato = value
        End Set
    End Property
    Private m_tipo_sistema_meta_datos As String
    Public Property tipo_sistema_meta_datos() As String
        Get
            Return m_tipo_sistema_meta_datos
        End Get
        Set(value As String)
            m_tipo_sistema_meta_datos = value
        End Set
    End Property
    Private m_tipo_meta_dato As String
    Public Property tipo_meta_dato() As String
        Get
            Return m_tipo_meta_dato
        End Get
        Set(value As String)
            m_tipo_meta_dato = value
        End Set
    End Property
    Private m_estru_xml_meta_dato As String
    Public Property estru_xml_meta_dato() As String
        Get
            Return m_estru_xml_meta_dato
        End Get
        Set(value As String)
            m_estru_xml_meta_dato = value
        End Set
    End Property
    Private m_estado_obliga_torio As String
    Public Property estado_obliga_torio() As String
        Get
            Return m_estado_obliga_torio
        End Get
        Set(value As String)
            m_estado_obliga_torio = value
        End Set
    End Property
    Private m_descripcion_meta_dato As String
    Public Property descripcion_meta_dato() As String
        Get
            Return m_descripcion_meta_dato
        End Get
        Set(value As String)
            m_descripcion_meta_dato = value
        End Set
    End Property
    Private m_tipo_datos_meta_datos As String
    Public Property tipo_datos_meta_datos() As String
        Get
            Return m_tipo_datos_meta_datos
        End Get
        Set(value As String)
            m_tipo_datos_meta_datos = value
        End Set
    End Property
    Private m_longitud_datos_meta_datos As String
    Public Property longitud_datos_meta_datos() As String
        Get
            Return m_longitud_datos_meta_datos
        End Get
        Set(value As String)
            m_longitud_datos_meta_datos = value
        End Set
    End Property
    Private m_valores_meta_dato As String
    Public Property valores_meta_dato() As String
        Get
            Return m_valores_meta_dato
        End Get
        Set(value As String)
            m_valores_meta_dato = value
        End Set
    End Property
    Private m_Comentario_meta_datos As String
    Public Property Comentario_meta_datos() As String
        Get
            Return m_Comentario_meta_datos
        End Get
        Set(value As String)
            m_Comentario_meta_datos = value
        End Set
    End Property
    Private m_emula_legislacion_meta_dato As String
    Public Property emula_legislacion_meta_dato() As String
        Get
            Return m_emula_legislacion_meta_dato
        End Get
        Set(value As String)
            m_emula_legislacion_meta_dato = value
        End Set
    End Property
    Private m_NUMERACION As String
    Public Property NUMERACION() As String
        Get
            Return m_NUMERACION
        End Get
        Set(value As String)
            m_NUMERACION = value
        End Set
    End Property
    Private m_VALOR_AUTO_POBLADO As String
    Public Property VALOR_AUTO_POBLADO() As String
        Get
            Return m_VALOR_AUTO_POBLADO
        End Get
        Set(value As String)
            m_VALOR_AUTO_POBLADO = value
        End Set
    End Property
    Private m_EQUIVALENCIA_AUTO_POBLADO As String
    Public Property EQUIVALENCIA_AUTO_POBLADO() As String
        Get
            Return m_EQUIVALENCIA_AUTO_POBLADO
        End Get
        Set(value As String)
            m_EQUIVALENCIA_AUTO_POBLADO = value
        End Set
    End Property
    Private m_ESTANDAR As String
    Public Property ESTANDAR() As String
        Get
            Return m_ESTANDAR
        End Get
        Set(value As String)
            m_ESTANDAR = value
        End Set
    End Property
    Private m_AUTO_PLOBLADO As String
    Public Property AUTO_PLOBLADO() As String
        Get
            Return m_AUTO_PLOBLADO
        End Get
        Set(value As String)
            m_AUTO_PLOBLADO = value
        End Set
    End Property
    Private m_value As String
    Public Property value() As String
        Get
            Return m_value
        End Get
        Set(value As String)
            m_value = value
        End Set
    End Property
    Private m_ERROR_SERVICE As String
    Public Property ERROR_SERVICE() As String
        Get
            Return m_ERROR_SERVICE
        End Get
        Set(value As String)
            m_ERROR_SERVICE = value
        End Set
    End Property
    Private m_ESTADO_VISIBLE_METADATO As String
    Public Property ESTADO_VISIBLE_METADATO() As String
        Get
            Return m_ESTADO_VISIBLE_METADATO
        End Get
        Set(value As String)
            m_ESTADO_VISIBLE_METADATO = value
        End Set
    End Property
    Private m_ESTADO_FIRMA_DIGITAL As String
    Public Property ESTADO_FIRMA_DIGITAL() As String
        Get
            Return m_ESTADO_FIRMA_DIGITAL
        End Get
        Set(value As String)
            m_ESTADO_FIRMA_DIGITAL = value
        End Set
    End Property
End Class
Public Class Class_ra_m_detalle_sis_meta_datos

    Function Solicita_estructura_meta_dato_sistema(ByVal id_sistema_meta_datos As Integer,
                                                   ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        Try
            Dim Parametro_Consulta As String = "SELECT id_detalle_sistema_meta_datos," &
            "nombre_meta_dato,tipo_sistema_meta_datos," &
            "tipo_meta_dato,estru_xml_meta_dato,descripcion_meta_dato,tipo_datos_meta_datos," &
            "longitud_datos_meta_datos,valores_meta_dato,Comentario_meta_datos,emula_legislacion_meta_dato," &
            "NUMERACION,VALOR_AUTO_POBLADO,EQUIVALENCIA_AUTO_POBLADO,ESTANDAR,AUTO_PLOBLADO,estado_obliga_torio,ESTADO_VISIBLE_METADATO" &
            " FROM ra_m_detalle_sis_meta_datos  " &
            " where ra_m_sistema_meta_datos_id_sistema_meta_datos=" & id_sistema_meta_datos & " and ESTADO_VISIBLE_METADATO=1" &
            " ORDER BY NUMERACION, ESTANDAR"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_detalle_sis_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_meta_dato_sistema = "Función Solicita_estructura_meta_dato_sistema dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_detalle_sis_meta_dato(i)
                    Dim ref_clas_detalle_meta As New Class_ra_m_detalle_sis_meta_datos_
                    ref_clas_detalle_meta.id_detalle_sistema_meta_datos = Datset.Tables(0).Rows(i).Item(0)
                    ref_clas_detalle_meta.nombre_meta_dato = Datset.Tables(0).Rows(i).Item(1)
                    ref_clas_detalle_meta.tipo_sistema_meta_datos = Datset.Tables(0).Rows(i).Item(2)
                    ref_clas_detalle_meta.tipo_meta_dato = Datset.Tables(0).Rows(i).Item(3)
                    'ref_clas_detalle_meta.estru_xml_meta_dato = Datset.Tables(0).Rows(i).Item(4)
                    ref_clas_detalle_meta.estru_xml_meta_dato = ""
                    ref_clas_detalle_meta.descripcion_meta_dato = Datset.Tables(0).Rows(i).Item(5)
                    ref_clas_detalle_meta.tipo_datos_meta_datos = Datset.Tables(0).Rows(i).Item(6)
                    ref_clas_detalle_meta.longitud_datos_meta_datos = Datset.Tables(0).Rows(i).Item(7)
                    ref_clas_detalle_meta.valores_meta_dato = Datset.Tables(0).Rows(i).Item(8)
                    ref_clas_detalle_meta.Comentario_meta_datos = Datset.Tables(0).Rows(i).Item(9)
                    ref_clas_detalle_meta.emula_legislacion_meta_dato = Datset.Tables(0).Rows(i).Item(10)
                    ref_clas_detalle_meta.NUMERACION = Datset.Tables(0).Rows(i).Item(11)
                    ref_clas_detalle_meta.VALOR_AUTO_POBLADO = Datset.Tables(0).Rows(i).Item(12)
                    ref_clas_detalle_meta.EQUIVALENCIA_AUTO_POBLADO = Datset.Tables(0).Rows(i).Item(13)
                    ref_clas_detalle_meta.ESTANDAR = Datset.Tables(0).Rows(i).Item(14)
                    ref_clas_detalle_meta.AUTO_PLOBLADO = Datset.Tables(0).Rows(i).Item(15)
                    ref_clas_detalle_meta.estado_obliga_torio = Datset.Tables(0).Rows(i).Item(16)
                    ref_clas_detalle_meta.value = ""
                    ref_clas_detalle_meta.ERROR_SERVICE = "YES"
                    ref_clas_detalle_meta.ESTADO_VISIBLE_METADATO = Datset.Tables(0).Rows(i).Item(17)
                    stru_detalle_sis_meta_dato(i) = ref_clas_detalle_meta
                Next
                Solicita_estructura_meta_dato_sistema = "YES"
                Exit Function
            Else
                Solicita_estructura_meta_dato_sistema = "El sistema no tiene registrado el detalle sistema de meta datos ( " & id_sistema_meta_datos & ")  para expedientes contacte a su administrador"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_meta_dato_sistema = "Inconsistencia general función Solicita_estructura_meta_dato_sistema " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_meta_dato_sistema_stru(ByVal id_sistema_meta_datos As Integer,
                                                        ByRef stru_detalle_sis_meta_dato() As stru_detalle_sis_meta_dato) As String
        Try
            Dim Parametro_Consulta As String = "SELECT id_detalle_sistema_meta_datos," &
            "nombre_meta_dato,tipo_sistema_meta_datos," &
            "tipo_meta_dato,estru_xml_meta_dato,descripcion_meta_dato,tipo_datos_meta_datos," &
            "longitud_datos_meta_datos,valores_meta_dato,Comentario_meta_datos,emula_legislacion_meta_dato," &
            "NUMERACION,VALOR_AUTO_POBLADO,EQUIVALENCIA_AUTO_POBLADO,ESTANDAR,AUTO_PLOBLADO,estado_obliga_torio" &
            " FROM ra_m_detalle_sis_meta_datos  " &
            " where ra_m_sistema_meta_datos_id_sistema_meta_datos=" & id_sistema_meta_datos & " and ESTADO_VISIBLE_METADATO=1" &
            " ORDER BY NUMERACION, ESTANDAR"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_detalle_sis_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_meta_dato_sistema_stru = "Función Solicita_estructura_meta_dato_sistema_stru dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_detalle_sis_meta_dato(i)
                    stru_detalle_sis_meta_dato(i).id_detalle_sistema_meta_datos = Datset.Tables(0).Rows(i).Item(0)
                    stru_detalle_sis_meta_dato(i).nombre_meta_dato = Datset.Tables(0).Rows(i).Item(1)
                    stru_detalle_sis_meta_dato(i).tipo_sistema_meta_datos = Datset.Tables(0).Rows(i).Item(2)
                    stru_detalle_sis_meta_dato(i).tipo_meta_dato = Datset.Tables(0).Rows(i).Item(3)
                    stru_detalle_sis_meta_dato(i).estru_xml_meta_dato = Datset.Tables(0).Rows(i).Item(4)
                    stru_detalle_sis_meta_dato(i).descripcion_meta_dato = Datset.Tables(0).Rows(i).Item(5)
                    stru_detalle_sis_meta_dato(i).tipo_datos_meta_datos = Datset.Tables(0).Rows(i).Item(6)
                    stru_detalle_sis_meta_dato(i).longitud_datos_meta_datos = Datset.Tables(0).Rows(i).Item(7)
                    stru_detalle_sis_meta_dato(i).valores_meta_dato = Datset.Tables(0).Rows(i).Item(8)
                    stru_detalle_sis_meta_dato(i).Comentario_meta_datos = Datset.Tables(0).Rows(i).Item(9)
                    stru_detalle_sis_meta_dato(i).emula_legislacion_meta_dato = Datset.Tables(0).Rows(i).Item(10)
                    stru_detalle_sis_meta_dato(i).NUMERACION = Datset.Tables(0).Rows(i).Item(11)
                    stru_detalle_sis_meta_dato(i).VALOR_AUTO_POBLADO = Datset.Tables(0).Rows(i).Item(12)
                    stru_detalle_sis_meta_dato(i).EQUIVALENCIA_AUTO_POBLADO = Datset.Tables(0).Rows(i).Item(13)
                    stru_detalle_sis_meta_dato(i).ESTANDAR = Datset.Tables(0).Rows(i).Item(14)
                    stru_detalle_sis_meta_dato(i).AUTO_PLOBLADO = Datset.Tables(0).Rows(i).Item(15)
                    stru_detalle_sis_meta_dato(i).estado_obliga_torio = Datset.Tables(0).Rows(i).Item(16)
                    stru_detalle_sis_meta_dato(i).value = ""
                    stru_detalle_sis_meta_dato(i).ERROR_SERVICE = "YES"
                Next
                Solicita_estructura_meta_dato_sistema_stru = "YES"
                Exit Function
            Else
                Solicita_estructura_meta_dato_sistema_stru = "El sistema no tiene registrado el detalle sistema de meta datos ( " & id_sistema_meta_datos & ")  para expedientes contacte a su administrador"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_meta_dato_sistema_stru = "Inconsistencia general función Solicita_estructura_meta_dato_sistema " & ex.Message
        End Try
    End Function
    Function Asigna_contenido_estructura_meta_dato_de_archivo_xml(ByVal nombre_archivo_xml As String,
                                                                  ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        Try
            Dim xmlArchivo As New XmlDocument
            Dim xmlNodoList As XmlNodeList
            xmlArchivo.Load(nombre_archivo_xml)
            For k As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                xmlNodoList = xmlArchivo.GetElementsByTagName(stru_detalle_sis_meta_dato(k).nombre_meta_dato)
                If xmlNodoList.Count > 0 Then
                    stru_detalle_sis_meta_dato(k).value = xmlNodoList.Item(0).InnerText
                End If
            Next
            Asigna_contenido_estructura_meta_dato_de_archivo_xml = "YES"
        Catch ex As Exception
            Asigna_contenido_estructura_meta_dato_de_archivo_xml = "Inconsistencia general funcion Asigna_contenido_estructura_meta_dato_de_archivo_xml " & ex.Message
        End Try
    End Function
    Function Solicita_id_sistema_meta_dato_en_detalle_meta_dato(ByVal id_detalle_meta_dato As Integer,
                                                                ByRef id_sistema_meta_dato As Integer) As String
        '----------------------------------------------------------------
        'Funcion : Solicita la identificacion del sistema meta datos
        'por medio del detalle del meta dato
        'Fecha : 2022-02-17
        'Ing . Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT ra_m_sistema_meta_datos_id_sistema_meta_datos " &
          " FROM ra_m_detalle_sis_meta_datos  " &
          " where id_detalle_sistema_meta_datos=" & id_detalle_meta_dato
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_detalle_sis_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_sistema_meta_dato_en_detalle_meta_dato = "Función Solicita_id_sistema_meta_dato_en_detalle_meta_dato dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_sistema_meta_dato = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_sistema_meta_dato_en_detalle_meta_dato = "YES"
                Exit Function
            Else
                Solicita_id_sistema_meta_dato_en_detalle_meta_dato = "Imposible enconrar la identificación del sistema meta dato con el identificador de detalle ( " & id_detalle_meta_dato & ") en la tabla detalle"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_sistema_meta_dato_en_detalle_meta_dato = "Inconsisencia general funcion Solicita_id_sistema_meta_dato_en_detalle_meta_dato " & ex.Message
        End Try
    End Function
End Class
