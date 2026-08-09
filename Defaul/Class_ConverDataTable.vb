Imports System.Data
Imports System.Runtime.CompilerServices
Imports System.Dynamic
Imports System.Reflection

Public Class Class_ConverDataTable
    Function ConvertDataTable(Of T)(ByVal dt As DataTable, ByRef data As List(Of T)) As String
        Try
            data = New List(Of T)()
            For Each row As DataRow In dt.Rows
                Dim item As T = GetItem(Of T)(row)
                data.Add(item)
            Next
            ConvertDataTable = "YES"
        Catch ex As Exception
            ConvertDataTable = "Error ConvertDataTable " & ex.Message
        End Try
    End Function
    Function GetItem(Of T)(ByVal dr As DataRow) As T
        Dim temp As Type = GetType(T)
        Dim obj As T = Activator.CreateInstance(Of T)()
        For Each column As DataColumn In dr.Table.Columns
            For Each pro As Reflection.PropertyInfo In temp.GetProperties()
                If pro.Name = column.ColumnName Then
                    pro.SetValue(obj, dr(column.ColumnName), Nothing)

                Else
                    Continue For
                End If
            Next
        Next
        Return obj
    End Function
    Private Sub SurroundingSub()
        Dim studentDetails As List(Of Object) = New List(Of Object)()
        'studentDetails = ConvertDataTable(Of Object)(dt)
    End Sub

    'Function ToList(Of T As New)(ByVal dt As DataTable, ByVal Optional isFirstRowColumnsHeader As Boolean = False) As IList(Of T)
    '    Dim results = New List(Of T)()

    '    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
    '        Dim columns = dt.Columns.Cast(Of DataColumn)().ToList()
    '        Dim rows = dt.Rows.Cast(Of DataRow)().ToList()
    '        Dim headerNames = columns.[Select](Function(col) col.ColumnName).ToList()

    '        If isFirstRowColumnsHeader Then

    '            For i = 0 To headerNames.Count - 1
    '                If rows(0)(i) <> DBNull.Value AndAlso Not String.IsNullOrEmpty(rows(0)(i).ToString()) Then headerNames(i) = rows(0)(i).ToString()
    '            Next

    '            rows.RemoveAt(0)
    '        End If

    '        If GetType(T) = GetType(System.Dynamic.ExpandoObject) OrElse GetType(T) = GetType(System.Dynamic.DynamicObject) OrElse GetType(T) = GetType(System.Object) Then
    '            Dim dynamicDt = New List(Of System.Dynamic.DynamicObject)()

    '            For Each row In rows
    '                Dim dyn As Object = New ExpandoObject()
    '                dynamicDt.Add(dyn)
    '                For i = 0 To columns.Count - 1
    '                    Dim dic = CType(dyn, IDictionary(Of String, Object))
    '                    dic(headerNames(i)) = row(columns(i))
    '                Next
    '            Next

    '            Return CType(dynamicDt, System.Dynamic.DynamicObject)
    '        Else
    '            Dim properties = GetType(T).GetProperties()

    '            If columns.Any() AndAlso properties.Any() Then

    '                For Each row In rows
    '                    Dim entity = New T()

    '                    For i = 0 To columns.Count - 1

    '                        If Not row.IsNull(columns(i)) Then
    '                            GetType(T).GetProperty(headerNames(i))?.SetValue(entity, If(row(columns(i)) = DBNull.Value, Nothing, row(columns(i))))
    '                        End If
    '                    Next

    '                    results.Add(entity)
    '                Next
    '            End If
    '        End If
    '    End If

    '    Return results
    'End Function
    Function SetObjectProperties(ByVal dataTable As DataTable, ByRef objClass As Object) As String
        Try
            Dim _dataRow As DataRow = dataTable.Rows(0)
            Dim objType As Type = objClass.GetType()
            Dim propertyList As List(Of Reflection.PropertyInfo) = New List(Of Reflection.PropertyInfo)(objType.GetProperties())
            For Each dc As DataColumn In _dataRow.Table.Columns
                Dim _prop = propertyList.Where(Function(a) a.Name = dc.ColumnName).[Select](Function(a) a).FirstOrDefault()
                If _prop Is Nothing Then
                    Continue For
                End If
                _prop.SetValue(objClass, Convert.ChangeType(_dataRow(dc), If(Nullable.GetUnderlyingType(_prop.PropertyType), _prop.PropertyType)), Nothing)
            Next
            SetObjectProperties = "YES"
        Catch ex As Exception
            SetObjectProperties = "Inconsistencia general SetObjectProperties " & ex.Message
        End Try

    End Function

End Class
