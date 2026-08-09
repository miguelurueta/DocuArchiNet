Public Class class_campos_table_bostra_table
    Public field As String
    Public title As String
    Public field_destino As String
    Public visible As Boolean  'Determina si el campo es visible o no (true, false)
    Public titleTooltip As String
    Public sortable As String 'Campo que permite ordenar tabla
    Public sortName As String 'Campo que ordena la tabla cuando se listan
    Public sortOrder As String ' Tipo de ordenación del campo (desc, asc)
    Public radio As String ' Aciva si muestra el radio
    Public checkbox As Boolean ' Activa el chek (true,false)  <th data-field="state" data-checkbox="true"></th>
    Public Class_ As String ' Agrega la clase para las celdas
    Public cellStyle As String 'Agrega función java que da clase las celdas  https://examples.bootstrap-table.com/#column-options/cell-style.html#view-source
    Public halign As String 'Agrega tipo de alineación del alto de celda (right,center,left)
    Public align As String 'Agrega tipo de alineación del ancho de celda (right,center,left)
    Public formatter As String
    Public events As String
    Public clickToSelect As Boolean
    Public viisble_sql As Integer
    Public visible_like_sql As Integer
    Public data_sortable As Boolean
End Class
Public Class class_item_row_bot
    Property name_campo As String
    Property velue_campo As String
End Class
Public Class Class_bostra_table_row
    Property row As Object

End Class
Public Class class_boot_table
    Public Error_result As String
    Public row_table_boot As Object     'Seralizado DATA-SET
    Public field_table_boot As List(Of class_campos_table_bostra_table)
    Public ReciboSII As Object
    Public CodigoBarras As Object
    Public Gabinete As Object
End Class
