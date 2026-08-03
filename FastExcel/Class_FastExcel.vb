Imports System.IO
Imports FastExcel
Imports Newtonsoft.Json
Public Class Class_FastExcel_interfaz
    Property colums As New List(Of class_FastExcel_colums)
    Property row As New List(Of class_FastExcel_rows)
End Class
Public Class class_FastExcel_colums
    Property name_colums As String
    Property aleas_colum_plantilla As String
    Property type_colums As String
    Property visible As Boolean
End Class
Public Class class_FastExcel_rows
    Property cells As New List(Of class_FastExcel_cells)
End Class
Public Class class_FastExcel_cells
    Property value As String
    Property type As String
    Property colunm_name As String
    Property aleas_colum As String
    Property visible As Boolean
End Class
Public Class Class_FastExcel
    Function Formato_campo_fax_exel(ByVal nombre As String,
                                    ByRef salida_formato_nombre As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Elimina caracteres no alamacenables en base de datos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombre               : Representa el valor del archivo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'salida_formato_nombre  : Retorna campo sin cracteres no validos
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-03
        'Elabora               : Miguel Angel Urueta Miranda 
        '------------------------------------------------------------------------------------------------
        Try
            If nombre = "" Then
                salida_formato_nombre = ""
                Formato_campo_fax_exel = "YES"
                Exit Function
            Else
                salida_formato_nombre = nombre
                salida_formato_nombre = salida_formato_nombre.Replace("'", "")
                salida_formato_nombre = salida_formato_nombre.Replace("/", "")
                salida_formato_nombre = salida_formato_nombre.Replace("&", "")
                salida_formato_nombre = salida_formato_nombre.Replace(";", "")
                salida_formato_nombre = salida_formato_nombre.Replace("%", "")
                salida_formato_nombre = salida_formato_nombre.Replace("\", "")
                salida_formato_nombre = salida_formato_nombre.Replace("#", "")
                Formato_campo_fax_exel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Formato_campo_fax_exel = "Inconsitencia general funcion Formato_campo_fax_exel " & ex.Message
        End Try
    End Function
    Function Read_file_fast_Excell(ByVal file_excel As String,
                                   ByVal name_index As Object,
                                   ByRef row As Class_FastExcel_interfaz) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Lee archivo excel y retorna la estructura del componente fax excel al leer archivo
        'excel
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'file_excel               : Representa la ruta del archivo
        'name_index               : Representa el nombre o el index de la hoja de excel a leer
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'row  : Retorna la estructura del archivo excell
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-14
        'Elabora               : Miguel Angel Urueta Miranda 
        '------------------------------------------------------------------------------------------------
        Try
            Dim inputFile As New FileInfo(file_excel)
            Dim worksheet As Worksheet = Nothing
            row.colums = New List(Of class_FastExcel_colums)
            Using FastExcel As New FastExcel.FastExcel(inputFile, False)
                worksheet = FastExcel.Read(name_index)
                Dim rows = worksheet.Rows.ToList()
                For Each item As Object In rows(0).Cells
                    Dim item_colum As New class_FastExcel_colums
                    item_colum.name_colums = item.Value
                    item_colum.visible = False
                    row.colums.Add(item_colum)
                Next
                Dim item_row As New class_FastExcel_rows
                For i As Integer = 1 To rows.Count - 1
                    item_row = New class_FastExcel_rows
                    For Each item_ As Object In rows(i).Cells
                        Dim item As New class_FastExcel_cells
                        Dim salida As String = ""
                        Formato_campo_fax_exel(item_.value, salida)
                        item.value = salida
                        If Not rows(0).Cells((item_.ColumnNumber - 1)) Is Nothing Then
                            item.colunm_name = rows(0).Cells((item_.ColumnNumber - 1)).Value
                        End If
                        item_row.cells.Add(item)
                    Next
                    row.row.Add(item_row)
                Next
            End Using
            Read_file_fast_Excell = "YES"
        Catch ex As Exception
            Read_file_fast_Excell = "Inconsistencia general funcion Read_file_fast_Excell " & ex.Message
        End Try
    End Function
    Function Valida_campos_plantilla_fast_excell(ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                 ByRef fast_excel_colums As List(Of class_FastExcel_colums),
                                                 ByRef mat_campos As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Valida los campos de la plnatilla que no estan en archivos excel
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Representa la estructura de los campos de la plantilla
        'fast_excel_row    : Representa la estructura de los registros excel
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'mat_campos  : Retorna los campos que no estan en el archivo de excel
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-14
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Columnas As String = ""
            If Not fast_excel_colums Is Nothing Then
                For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                    If class_campos_table_bostra_table.Item(i).viisble_sql = 1 Then
                        Dim name_columna As String = class_campos_table_bostra_table.Item(i).field_destino
                        Dim estado_seleccion As String = ""
                        For k = 0 To fast_excel_colums.Count - 1
                            If UCase(name_columna) = UCase(fast_excel_colums(k).name_colums) Then
                                estado_seleccion = "YES"
                                fast_excel_colums(k).visible = True
                                fast_excel_colums(k).aleas_colum_plantilla = class_campos_table_bostra_table.Item(i).field
                                fast_excel_colums(k).name_colums = class_campos_table_bostra_table.Item(i).field_destino
                            End If
                        Next
                        If estado_seleccion <> "YES" Then
                            If mat_campos = "" Then
                                mat_campos = name_columna
                            Else
                                mat_campos = mat_campos & "-" & name_columna
                            End If
                        End If
                    End If
                Next
                Valida_campos_plantilla_fast_excell = "YES"
                Exit Function
            Else
                Valida_campos_plantilla_fast_excell = "Archivo excell sin registros"
                Exit Function
            End If
        Catch ex As Exception
            Valida_campos_plantilla_fast_excell = "Inconsistencia general funcion Valida_campos_plantilla_fast_excell " & ex.Message
        End Try
    End Function
End Class
