Public Structure ra_auto_rel_campos_gabinete_expediente
    Dim id_auto_rel_campos_gabinete_expediente As Integer
    Dim ra_auto_detalle_campos_expediente_id_campos_expediente As Integer
    Dim detalle_gabienete_id_detalle_gabinete As Integer
    Dim ra_auto_registro_expediente_id_auto_registro As Integer
    '---Detalle gabinete
    Dim CAMPO As String
    Dim TIPO As String
    Dim value_campo_gabinete As String
    '---Detalle expediente
    Dim nombre_campo As String
    Dim tipo_campo As String
    Dim longitud_campo As Integer
    Dim value_campo_expediente As String
End Structure
Public Class Class_ra_auto_rel_campos_gabinete_expediente
    Function Solicita_estructura_relacion_auto_registro_gabinete_expediente(ByVal id_auto_registro As Integer,
                                                                            ByRef Ra_auto_rel_campos_gabinete_expediente() As ra_auto_rel_campos_gabinete_expediente) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita relación campos gabinete expediente para el auto
        '          registro de expedientes (En esta función no se tiene en cuenta
        '          la identificación del auto registro)
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_auto_registro      : Representa la identificación del auto registro
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ra_auto_rel_campos_gabinete_expediente  : Retorna la estructura de relacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim SQLconsulta As String = "Select id_auto_rel_campos_gabinete_expediente,CAMPO,TIPO,nombre_campo,tipo_campo,longitud_campo " &
                " from ra_auto_rel_campos_gabinete_expediente as rarc " &
                " inner join ra_auto_detalle_campos_expediente as raud on (raud.id_auto_detalle_campos_expediente=rarc.ra_auto_detalle_campos_expediente_id_campos_expediente) " &
                " inner join detalle_gabienete as dg on (dg.id_detalle_gabinete=detalle_gabienete_id_detalle_gabinete)"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_auto_rel_campos_gabinete_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_relacion_auto_registro_gabinete_expediente = " Función Solicita_estructura_relacion_auto_registro_gabinete_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_relacion_auto_registro_gabinete_expediente = "Imposible encontrar la relación de campos gabinete expediente de la auto relación (" & id_auto_registro & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Ra_auto_rel_campos_gabinete_expediente(i)
                    Ra_auto_rel_campos_gabinete_expediente(i).id_auto_rel_campos_gabinete_expediente = Datset.Tables(0).Rows(i).Item("id_auto_rel_campos_gabinete_expediente")
                    Ra_auto_rel_campos_gabinete_expediente(i).CAMPO = Datset.Tables(0).Rows(i).Item("CAMPO")
                    Ra_auto_rel_campos_gabinete_expediente(i).TIPO = Datset.Tables(0).Rows(i).Item("TIPO")
                    Ra_auto_rel_campos_gabinete_expediente(i).nombre_campo = Datset.Tables(0).Rows(i).Item("nombre_campo")
                    Ra_auto_rel_campos_gabinete_expediente(i).tipo_campo = Datset.Tables(0).Rows(i).Item("tipo_campo")
                    Ra_auto_rel_campos_gabinete_expediente(i).longitud_campo = Datset.Tables(0).Rows(i).Item("longitud_campo")
                Next
                Solicita_estructura_relacion_auto_registro_gabinete_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_relacion_auto_registro_gabinete_expediente = "Inconsistencia general funcion Solicita_estructura_relacion_auto_registro_gabinete_expediente " & ex.Message
        End Try
    End Function
End Class
