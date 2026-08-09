Imports System.Reflection

Public Class ClassGestonClase
    Public Function StructToClass(Of TStruct, TClass)(s As TStruct) As TClass
        Dim c As TClass = Activator.CreateInstance(Of TClass)()

        Dim structFields = GetType(TStruct).GetFields(BindingFlags.Public Or BindingFlags.Instance)
        Dim classProps = GetType(TClass).GetProperties(BindingFlags.Public Or BindingFlags.Instance)

        For Each field In structFields
            Dim prop = classProps.FirstOrDefault(Function(p) p.Name = field.Name)
            If prop IsNot Nothing AndAlso prop.CanWrite Then
                prop.SetValue(c, field.GetValue(s))
            End If
        Next

        Return c
    End Function
End Class
