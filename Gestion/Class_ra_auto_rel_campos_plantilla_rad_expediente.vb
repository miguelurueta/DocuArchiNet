Public Structure ra_auto_rel_campos_plantilla_rad_expediente
    Dim idra_auto_rel_campos_plantilla_rad_expediente As Integer
    Dim detalle_plantilla_radicado_Plantilla_Radicado_id_Plantilla As Integer
    Dim ra_auto_detalle_campos_epediente_id_detalle_campos_expediente As Integer
    Dim ra_auto_registro_expediente_id_auto_registro As Integer
    '---Detalle plantilla radicado
    Dim Campo_Plantilla As String
    Dim Tipo_Campo_plantilla As String
    Dim value_campo_plaantilla As String
    Dim Nombre_Plantilla_Radicado As String
    '---Detalle expediente
    Dim nombre_campo As String
    Dim tipo_campo As String
    Dim longitud_campo As Integer
    Dim value_campo_expediente As String
End Structure
Public Class Class_ra_auto_rel_campos_plantilla_rad_expediente
    Function Solicita_estructura_relacion_campos_plantilla_radicado_expediente(ByVal id_auto_registro As Integer,
                                                                               ByRef ra_auto_rel_campos_plantilla_rad_expediente() As ra_auto_rel_campos_plantilla_rad_expediente) As String
        '---------------------------------------------------------------------------------
        'Funcion : Solicita relación campos plantilla radicado expediente para el auto
        '          registro de expedientes desde datos del radicado
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'id_auto_registro      : Representa la identificación del auto registro
        '
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------
        'ra_auto_rel_campos_plantilla_rad_expediente  : Retorna la estructura de relacion
        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2023-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------

        Try
            Dim SQLconsulta As String = "Select idra_auto_rel_campos_plantilla_rad_expediente,Campo_Plantilla,dg.Tipo_Campo,raud.nombre_campo,raud.tipo_campo,raud.longitud_campo,spr.Nombre_Plantilla_Radicado " &
                " from ra_auto_rel_campos_plantilla_rad_expediente as rarc " &
                " inner join ra_auto_detalle_campos_expediente as raud on (raud.id_auto_detalle_campos_expediente=rarc.ra_auto_detalle_campos_epediente_id_detalle_campos_expediente) " &
                " inner join detalle_plantilla_radicado as dg on (dg.id_detalle_plantilla_radicado=detalle_plantilla_radicado_id_detalle_plantilla_radicado) " &
                " inner join system_plantilla_radicado as spr on (spr.id_Plantilla=rarc.detalle_plantilla_radicado_Plantilla_Radicado_id_Plantilla) " &
                " where ra_auto_registro_expediente_id_auto_registro=" & id_auto_registro
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_auto_rel_campos_plantilla_rad_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_relacion_campos_plantilla_radicado_expediente = " Función Solicita_estructura_relacion_campos_plantilla_radicado_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_relacion_campos_plantilla_radicado_expediente = "Imposible encontrar la relación de campos plantilla radicación expediente de la auto relación (" & id_auto_registro & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve ra_auto_rel_campos_plantilla_rad_expediente(i)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).idra_auto_rel_campos_plantilla_rad_expediente = Datset.Tables(0).Rows(i).Item("idra_auto_rel_campos_plantilla_rad_expediente")
                    ra_auto_rel_campos_plantilla_rad_expediente(i).Campo_Plantilla = Datset.Tables(0).Rows(i).Item(1)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).Tipo_Campo_plantilla = Datset.Tables(0).Rows(i).Item(2)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).nombre_campo = Datset.Tables(0).Rows(i).Item(3)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).tipo_campo = Datset.Tables(0).Rows(i).Item(4)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).longitud_campo = Datset.Tables(0).Rows(i).Item(5)
                    ra_auto_rel_campos_plantilla_rad_expediente(i).Nombre_Plantilla_Radicado = Datset.Tables(0).Rows(i).Item(6)
                Next
                Solicita_estructura_relacion_campos_plantilla_radicado_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_relacion_campos_plantilla_radicado_expediente = "Inconsistencia general funcion Solicita_estructura_relacion_campos_plantilla_radicado_expediente " & ex.Message
        End Try
    End Function
End Class
