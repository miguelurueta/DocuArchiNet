Public Structure ra_gabexp_relacion_campos_gabinete_expediente
    Dim id_relacion_campos_tramite_gabinete_expediente As Integer
    Dim id_campo_detalle_expediente As Integer
    Dim id_detalle_gabinete As Integer
    Dim id_relacion_gab_expe As Integer
    Dim estado_relacion As Integer
End Structure
Public Structure stru_rel_exp_gabinete
    Dim id_campo_detalle_expediente As Integer
    Dim nombre_campo As String
    Dim tipo_campo As String
    Dim longitud_campo As Integer
    Dim valor_campo_expediente As String
    Dim id_detalle_gabinete As Integer
    Dim CAMPO As String
    Dim TIPO As String
    Dim valor_campo_gabinete As String
End Structure
Public Class Class_ra_gabexp_relacion_campos_gabinete_expediente
    Function SolicitaEstructuraRelacionGabineteExpediente(ByVal IdRelacionExpedienteGabinete As Integer,
                                                          ByRef StruRelExpGabinete() As stru_rel_exp_gabinete) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Solicita esctructura relación campos expediente gabinete
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_relacion_expediente_gabinete   : Representa la identificacion de el expediente 
        '                                  : y gabinetes
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'stru_rel_exp_gabinete      : Retorna la estructura de la relación de campos y
        '                             gabinetes 
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select rgc.id_campo_detalle_expediente,rgc.nombre_campo,rgc.tipo_campo,rgc.longitud_campo," &
            "dg.id_detalle_gabinete,dg.CAMPO,dg.TIPO " &
            " from  ra_gabexp_relacion_campos_gabinete_expediente As rgr " &
            "inner join   detalle_gabienete as dg on (dg.id_detalle_gabinete=rgr.detalle_gabienete_id_detalle_gabinete) " &
            "inner join  ra_gabexp_campos_detalle_expediente as rgc on " &
            "(rgc.id_campo_detalle_expediente=rgr.Ra_gabexp_campos_detalle_expediente_id_campo_detalle_expediente) " &
            " WHERE rgr.relacion_index_gabinete_expediente_id_relacion_gab_expe =" & IdRelacionExpedienteGabinete
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_gabexp_relacion_campos_gabinete_expediente")
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraRelacionGabineteExpediente = "Error funcion Solicita_estructura_relacion_gabinete_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstructuraRelacionGabineteExpediente = "Imposible encontrar relación campos expdediente gabinete para la construcción del indice del documento"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve StruRelExpGabinete(i)
                    StruRelExpGabinete(i).id_campo_detalle_expediente = Datset.Tables(0).Rows(i).Item(0)
                    StruRelExpGabinete(i).nombre_campo = Datset.Tables(0).Rows(i).Item(1)
                    StruRelExpGabinete(i).tipo_campo = Datset.Tables(0).Rows(i).Item(2)
                    StruRelExpGabinete(i).longitud_campo = Datset.Tables(0).Rows(i).Item(3)
                    StruRelExpGabinete(i).id_detalle_gabinete = Datset.Tables(0).Rows(i).Item(4)
                    StruRelExpGabinete(i).CAMPO = Datset.Tables(0).Rows(i).Item(5)
                    StruRelExpGabinete(i).TIPO = Datset.Tables(0).Rows(i).Item(6)
                Next
                SolicitaEstructuraRelacionGabineteExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraRelacionGabineteExpediente = "Inconsistencia general fucion SolicitaEstructuraRelacionGabineteExpediente " & ex.Message
        End Try
    End Function
End Class
