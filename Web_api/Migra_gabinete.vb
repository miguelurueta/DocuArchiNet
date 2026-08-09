Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http

Public Class Migra_gabinete
    Inherits ApiController
    Private acreditacion_ As acreditacion() = New acreditacion() {New acreditacion() With {
                  .ID = 1,
                  .PROGRAMAAC = "Tomato Soup"}}
    Public Function GetAllProducts() As IEnumerable(Of acreditacion)
        Return acreditacion_
    End Function
    '' GET api/<controller>
    'Public Function GetValues() As IEnumerable(Of String)
    '    Return New String() {"value1", "value2"}
    'End Function

    '' GET api/<controller>/5
    'Public Function GetValue(ByVal id As Integer) As String
    '    Return "value"
    'End Function

    '' POST api/<controller>
    'Public Sub PostValue(<FromBody()> ByVal value As String)

    'End Sub

    '' PUT api/<controller>/5
    'Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

    'End Sub

    '' DELETE api/<controller>/5
    'Public Sub DeleteValue(ByVal id As Integer)

    'End Sub
End Class

