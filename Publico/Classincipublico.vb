Public Class Classincipublico
    Public Function Listar_Treeview_Publico(ByRef Tre_v2 As TreeView) As String
        '*******************************************************
        'Funcion : Crea treeview con las opciones de workflow
        'Fecha : 2012-10-04
        'Ingeniero: Miguel Angel Urueta Miranda
        '*******************************************************
        Try
            Dim Tre_v As New TreeNode
            Tre_v.ChildNodes.Clear()
            Tre_v.Text = "OPCIONES PUBLICAS"
            Dim attrNode1Gru As New TreeNode
            attrNode1Gru.Text = "Consulta Estado Radicado"
            attrNode1Gru.Value = "OP-CR-01"
            Tre_v.ChildNodes.Add(attrNode1Gru)
            Dim attrNode1Gru1 As New TreeNode
            attrNode1Gru1.SelectAction = TreeNodeSelectAction.SelectExpand
            attrNode1Gru1.Text = "Consulta publica de documentos"
            attrNode1Gru1.Value = "OP-CP-02"
            attrNode1Gru1.ToolTip = "Consulta de documentos publicados"
            Tre_v.ChildNodes.Add(attrNode1Gru1)
            Dim attrNode1Gru2 As New TreeNode
            attrNode1Gru2.SelectAction = TreeNodeSelectAction.SelectExpand
            attrNode1Gru2.Text = "Consulta entidades oficiales"
            attrNode1Gru2.Value = "OP-CEO-02"
            Tre_v.ChildNodes.Add(attrNode1Gru2)
            Tre_v2.EnableViewState = True
            Tre_v2.Nodes.Add(Tre_v)
            Listar_Treeview_Publico = "YES"
        Catch ex As Exception
            Listar_Treeview_Publico = "Inconsistencia generando treview " & ex.Message
        End Try
    End Function
End Class
