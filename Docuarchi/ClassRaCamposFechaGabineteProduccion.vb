Public Class CDCamposFechaGabinetePro
    Property IdCamposFechaGabinete As Integer = 0
    Property System1IdGabinete As Integer
    Property DetalleGabieneteIdDetalleGabinete As Integer
    Property Campo As String
    Property Tipo As String
End Class
Public Class ClassRaCamposFechaGabineteProduccion
    Function SolicitaCamposFechaGabineteProduccion(ByVal IdGabinete As Integer,
                                                   ByRef CDCamposFechaGabinetePro As List(Of CDCamposFechaGabinetePro)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de campos fecha para el almacenamiento del campo fecha desde
        '          la carga de archivos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabinete          : Representa la identificación del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDCamposFechaGabinetePro  : Retorna la estructura con los campos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SQLConsulta As String = "SELECT rcfg.IdCamposFechaGabinete, rcfg.system1_id_gabinete,rcfg.detalle_gabienete_id_detalle_gabinete
                                        ,dg.CAMPO,dg.TIPO
                                        FROM ra_campos_fecha_gabinete_produccion As rcfg
                                        inner Join  detalle_gabienete as dg on (dg.id_detalle_gabinete=rcfg.detalle_gabienete_id_detalle_gabinete) 
                                        where system1_id_gabinete =" & IdGabinete
            Dim ConectDbase As New conect.Dbase_Conction_Mysql_RA
            Dim DataSet As DataSet = New DataSet("ra_campos_fecha_gabinete_produccion")
            Dim Result = ConectDbase.SELECTION_SELECT_FIELD(SQLConsulta, DataSet)
            If Result <> "YES" Then
                SolicitaCamposFechaGabineteProduccion = "Incosnistencia en la Funcion SolicitaCamposFechaGabineteProduccion " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                CDCamposFechaGabinetePro = Nothing
                SolicitaCamposFechaGabineteProduccion = "YES"
                Exit Function
            Else
                Dim IlistCDCamposFechaGabinetePro As New CDCamposFechaGabinetePro
                For i As Integer = 0 To DataSet.Tables(0).Rows.Count - 1
                    IlistCDCamposFechaGabinetePro = New CDCamposFechaGabinetePro
                    IlistCDCamposFechaGabinetePro.IdCamposFechaGabinete = DataSet.Tables(0).Rows(i).Item(0)
                    IlistCDCamposFechaGabinetePro.System1IdGabinete = DataSet.Tables(0).Rows(i).Item(1)
                    IlistCDCamposFechaGabinetePro.DetalleGabieneteIdDetalleGabinete = DataSet.Tables(0).Rows(i).Item(2)
                    IlistCDCamposFechaGabinetePro.Campo = DataSet.Tables(0).Rows(i).Item(3)
                    IlistCDCamposFechaGabinetePro.Tipo = DataSet.Tables(0).Rows(i).Item(4)
                    CDCamposFechaGabinetePro.Add(IlistCDCamposFechaGabinetePro)
                Next
                SolicitaCamposFechaGabineteProduccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCamposFechaGabineteProduccion = "Inconsistencia general funcion SolicitaCamposFechaGabineteProduccion " & ex.Message
        End Try
    End Function
End Class
