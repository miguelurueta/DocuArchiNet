Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http

Public Class ProductsController
    Inherits ApiController

    'Dim products As Product() = New Product() {New Product() With {
    '              .Id = 1,
    '              .Name = "Tomato Soup",
    '              .Category = "Groceries",
    '              .Price = 1
    '    }, New Product() With {
    '              .Id = 2,
    '              .Name = "Yo-yo",
    '              .Category = "Toys",
    '              .Price = 3.75D
    '    }, New Product() With {
    '              .Id = 3,
    '              .Name = "Hammer",
    '              .Category = "Hardware",
    '              .Price = 16.99D
    '    }}
    Dim products As Product() = New Product() {}

    Public Function GetAllProducts(id_im As Integer, nombre As String) As IEnumerable(Of Product)

        Dim k = products.ToList
        For i As Integer = 0 To 10
            Dim pr As New Product
            pr.Id = 10
            pr.Name = "ojo"
            pr.Category = "goseria"
            pr.Price = 200
            k.Add(pr)
        Next

        Return k
    End Function

    Public Function GetProductById(id As Integer) As Product
            Dim product = products.FirstOrDefault(Function(p) p.Id = id)
            If product Is Nothing Then
                Throw New HttpResponseException(HttpStatusCode.NotFound)
            End If
            Return product
        End Function

    Public Function GetProductsByCategory(category As String) As IEnumerable(Of Product)
        Return products.Where(Function(p) String.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
    End Function
End Class

Public Class Product
    Public Property Id() As Integer
        Get
            Return m_Id
        End Get
        Set(value As Integer)
            m_Id = value
        End Set
    End Property
    Private m_Id As Integer
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property Price() As Decimal
        Get
            Return m_Price
        End Get
        Set(value As Decimal)
            m_Price = value
        End Set
    End Property
    Private m_Price As Decimal
    Public Property Category() As String
        Get
            Return m_Category
        End Get
        Set(value As String)
            m_Category = value
        End Set
    End Property
    Private m_Category As String
End Class


