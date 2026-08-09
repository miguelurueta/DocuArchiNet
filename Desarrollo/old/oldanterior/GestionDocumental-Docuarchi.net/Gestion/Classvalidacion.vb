Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports AjaxControlToolkit
Public Class Classvalidacion
    Function Lista_Campos_Plantilla_Validacion(ByVal id_script As Integer, _
        ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION) As String
        '--------------------------------------------------------------
        'Funcion : Lista los campos y el detalle plantilla validacion
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-07-24
        'Ing : Miguel Angel Urueta Miranda
        'Modificado para la version web 2016-07-06
        'Por ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("campos_plantilla_validacion")
            Dim Parametro_Consulta As String = "select  cpv.Nombre_Campo,cpv.Tipo_Campo,cpv.Unico_campo,cpv.Aloja_null_campo,cpv.Visible_Campo" & _
             " ,cpv.Obligatorio_Campo,cpv.Orden_Campos,cpv.Campo_Primari_Key from relacion_script_plantilla as rsp inner join campos_plantilla_validacion as cpv on " & _
             " ( cpv.Plantilla_Validacion_Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion)" & _
            " where script_actividades_id_script = " & id_script & " order by cpv.Orden_Campos"
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_Campos_Plantilla_Validacion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Plantilla_Validacion = "Función Lista_Campos_Plantilla_Validación dice Error de conexión o de consulta "
                Exit Function
            Else
                For Iconta As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(Iconta)
                    If Datset.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        Matri_Datos(Iconta).Nombre_Campo = Datset.Tables(0).Rows(Iconta).Item(0).ToString
                    Else
                        Matri_Datos(Iconta).Nombre_Campo = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(1) = False Then
                        Matri_Datos(Iconta).Tipo_Campo = Datset.Tables(0).Rows(Iconta).Item(1).ToString
                    Else
                        Matri_Datos(Iconta).Tipo_Campo = ""
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(2) = False Then
                        Matri_Datos(Iconta).Unico_campo = Datset.Tables(0).Rows(Iconta).Item(2)
                    Else
                        Matri_Datos(Iconta).Unico_campo = 0
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(3) = False Then
                        Matri_Datos(Iconta).Aloja_null_campo = Datset.Tables(0).Rows(Iconta).Item(3)
                    Else
                        Matri_Datos(Iconta).Aloja_null_campo = 0
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(4) = False Then
                        Matri_Datos(Iconta).Visible_Campo = Datset.Tables(0).Rows(Iconta).Item(4)
                    Else
                        Matri_Datos(Iconta).Visible_Campo = 0
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(5) = False Then
                        Matri_Datos(Iconta).Obligatorio_Campo = Datset.Tables(0).Rows(Iconta).Item(5)
                    Else
                        Matri_Datos(Iconta).Obligatorio_Campo = 0
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(6) = False Then
                        Matri_Datos(Iconta).Orden_Campos = Datset.Tables(0).Rows(Iconta).Item(6)
                    Else
                        Matri_Datos(Iconta).Orden_Campos = 0
                    End If
                    If Datset.Tables(0).Rows(Iconta).IsNull(7) = False Then
                        Matri_Datos(Iconta).IDENTI_CAMPO = Datset.Tables(0).Rows(Iconta).Item(7)
                    Else
                        Matri_Datos(Iconta).IDENTI_CAMPO = 0
                    End If
                Next
                Lista_Campos_Plantilla_Validacion = "YES"
                Exit Function
            End If
            
        Catch ex As Exception
            Lista_Campos_Plantilla_Validacion = "Incosist funcion Lista_Campos_Plantilla_Validacion " & ex.Message
        End Try
    End Function
End Class
